FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

# Copy in csproj so we can restore
WORKDIR /build
COPY TeaTime.sln TeaTime.sln
COPY ["src/TeaTime/TeaTime.csproj", "src/TeaTime/"]
COPY ["src/TeaTime.Data.MySql/TeaTime.Data.MySql.csproj", "src/TeaTime.Data.MySql/"]
COPY ["src/TeaTime.Common/TeaTime.Common.csproj", "src/TeaTime.Common/"]
COPY ["src/TeaTime.Slack/TeaTime.Slack.csproj", "src/TeaTime.Slack/"]
COPY ["test/TeaTime.Data.MySql.Tests/TeaTime.Data.MySql.Tests.csproj", "test/TeaTime.Data.MySql.Tests/"]
COPY ["test/TeaTime.Slack.Tests/TeaTime.Slack.Tests.csproj", "test/TeaTime.Slack.Tests/"]
COPY ["test/TeaTime.Common.Tests/TeaTime.Common.Tests.csproj", "test/TeaTime.Common.Tests/"]
RUN dotnet restore

# Copy in everything else
COPY sql/ sql/
COPY src/ src/
COPY test/ test/

# Build
WORKDIR /build/
RUN dotnet build --no-restore -c Release

# Test
RUN dotnet test -c Release --no-build

# Publish
FROM build AS publish
WORKDIR /build/src/TeaTime
RUN dotnet publish "TeaTime.csproj" --no-build -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TeaTime.dll"]
