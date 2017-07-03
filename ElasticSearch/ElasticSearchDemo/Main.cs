using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

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


    }
}
