FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/TeaTime/TeaTime.csproj", "TeaTime/"]
COPY ["src/TeaTime.Data.MySql/TeaTime.Data.MySql.csproj", "TeaTime.Data.MySql/"]
COPY ["src/TeaTime.Common/TeaTime.Common.csproj", "TeaTime.Common/"]
COPY ["src/TeaTime.Slack/TeaTime.Slack.csproj", "TeaTime.Slack/"]
RUN dotnet restore "src/TeaTime/TeaTime.csproj"
COPY . .
WORKDIR "/src/TeaTime"
RUN dotnet build "TeaTime.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TeaTime.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TeaTime.dll"]
