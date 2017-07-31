FROM microsoft/dotnet:1.0-sdk-projectjson

ADD ./ /usr/local/src
WORKDIR /usr/local/src/

RUN cd /usr/local/src/
RUN dotnet restore -s https://nuget.cnblogs.com/v3/index.json -s http://sinonuget.chinacloudsites.cn/nuget
RUN dotnet build -c Release

ENV TZ=Asia/Shanghai
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezoness

CMD ["dotnet","run","-c","Release"]