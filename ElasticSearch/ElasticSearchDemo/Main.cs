using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using System.Linq.Expressions;

namespace ElasticSearchDemo
{
    public class Main
    {
        private ElasticClient _client;

        public Main()
        {
            var node = new Uri("http://127.0.0.1:9200");
            var settings = new ConnectionSettings(node).DefaultIndex("people");
            _client = new ElasticClient(settings);
        }

        [Fact]
        public async Task IndexAndGetDocument()
        {
            var persion = new Person
            {
                Id = 1,
                FirstName = "Martijn",
                LastName = "Laarman"
            };

            var indexResponse = await _client.IndexAsync(persion);

            //search path is /people/person/_search
            var searchResponse = await _client.SearchAsync<Person>(s => s
            .From(0)
            .Size(10)
            .Query(q => q
                .Match(m => m
                    .Field(f => f.FirstName).Query("Martijn"))));

            var people = searchResponse.Documents;

            //search path is /people/_search
            searchResponse = await _client.SearchAsync<Person>(s => s
                .AllTypes()
                .From(0)
                .Size(10)
                .Query(q => q
                .Match(m => m
                    .Field(f => f.FirstName).Query("Martijn"))));

            //search path is /_all/person/_search
            searchResponse = await _client.SearchAsync<Person>(s => s
               .AllIndices()
               .From(0)
               .Size(10)
               .Query(q => q
               .Match(m => m
                    .Field(f => f.FirstName).Query("Martijn"))));

            //search path is /_search
            searchResponse = await _client.SearchAsync<Person>(s => s
                .AllIndices()
                .AllTypes()
                .From(0)
               .Size(10)
               .Query(q => q
               .Match(m => m
                    .Field(f => f.FirstName).Query("Martijn"))));

            searchResponse = await _client.SearchAsync<Person>(s => s
                .Size(0)
                .Query(q => q
                  .Match(m => m
                    .Field(f => f.FirstName)
                    .Query("Martijn")))
               .Aggregations(a => a
                 .Terms("last_names", ta => ta
                   .Field(f => f.LastName))));

            var termsAgregation = searchResponse.Aggs.Terms("last_names");
        }

        [Fact]
        public async Task IndexWithCustomeIndexTypeAndId()
        {
            var person = new Person
            {
                Id = 2,
                FirstName = "yao",
                LastName = "zhengfa"
            };

            var index = await _client.IndexAsync(person, i => i
              .Index("another-index")
              .Type("another-type")
              .Id("1-should-not-be-the-id")
              .Refresh(Elasticsearch.Net.Refresh.True)
              .Ttl("1m"));

            var searchResults = _client.Search<Person>(s => s
              .Index("another-index")
              .Type("another-type")
              .From(0)
              .Size(10)
              .Query(q => q
                 .Match(m => m.Field(f => f.FirstName).Query("yao"))));

            Assert.NotNull(searchResults);
        }

        [Fact]
        public async Task SearchWithKey()
        {
            var searchResults = await _client.SearchAsync<Person>(s => s
              .Index("another-index")
              .Type("another-type")
              .MatchAll());

            searchResults = await _client.SearchAsync<Person>(s => s
              .Index("another-index")
              .Type("another-type")
              .Query(q => q.QueryString(qs => qs.Query("*y*"))));

            Assert.NotNull(searchResults);
        }

        [Fact]
        public async Task SearchWithDate()
        {
            var project = new Project
            {
                Id = 1,
                Name = "测试项目",
                Current = DateTime.Now
            };

            var index = await _client.IndexAsync(project, i => i
              .Index("test")
              .Type("projects")
              .Id("1"));

            var searchResults = await _client.SearchAsync<Project>(s => s
              .Index("test")
              .Type("projects")
              .Query(q => q
                .DateRange(r => r
                  .Field(f => f.Current)
                  .GreaterThanOrEquals(new DateTime(2017, 1, 1))
                  .LessThan(DateTime.Now))));

            Assert.NotNull(searchResults);
        }

        [Fact]
        public async Task SearchWithDateNoScore()
        {
            var searchResults = await _client.SearchAsync<Project>(s => s
              .Index("test")
              .Type("projects")
              .Query(q => q
                .Bool(b => b
                  .Filter(bf => bf
                    .DateRange(r => r
                      .Field(f => f.Current)
                      .GreaterThanOrEquals(new DateTime(2017, 1, 1))
                      .LessThanOrEquals(DateTime.Now))))));

            Assert.NotNull(searchResults);
        }

        [Fact]
        public async Task SearchWithCombiningQueryies()
        {
            var searchResponse = await _client.SearchAsync<Project>(s => s
              .Index("test")
              .Type("projects")
              .Query(q => q
                .Bool(b => b
                  .Must(mu => mu
                    .Match(m => m
                      .Field(f => f.Name).Query("测试项目")), mu => mu
                    .Match(m => m
                      .Field(f => f.Id).Query("1"))).Filter(fi => fi
                 .DateRange(r => r
                   .Field(f => f.Current)
                   .GreaterThanOrEquals(new DateTime(2017, 1, 1))
                   .LessThan(DateTime.Now))))));

            Assert.NotNull(searchResponse);
        }

        [Fact]
        public async Task SearchWithOr()
        {
            var firstSearchResponse = await _client.SearchAsync<Project>(s => s
              .Index("test")
              .Type("projects")
              .Query(q => q
                .Term(x => x.Name, "测试项目") || q
                .Term(x => x.Id, "1")));

            Assert.NotNull(firstSearchResponse);
            
            var secondSearchResponse = _client.Search<Project>(new SearchRequest<Project>
            {
                Query = new TermQuery { Field = "Name", Value = "测试项目" } ||
                        new TermQuery { Field = "id", Value = "1" }
            });

            Assert.NotNull(secondSearchResponse);
        }

        [Fact]
        public async Task SearchWithAnd()
        {
            var firstSearchResponse = await _client.SearchAsync<Project>(s => s
              .Index("test")
              .Type("projects")
              .Query(q => q
                .Term(p => p.Id, "1") && q
                .Term(p => p.Name, "项")));

            Assert.NotNull(firstSearchResponse);
        }
    }
}
