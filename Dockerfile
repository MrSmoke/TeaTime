FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

# Copy in csproj so we can restore
WORKDIR /build
COPY Directory.Build.props Directory.Build.props
COPY ["src/TeaTime/TeaTime.csproj", "src/TeaTime/"]
COPY ["src/TeaTime.Data.MySql/TeaTime.Data.MySql.csproj", "src/TeaTime.Data.MySql/"]
COPY ["src/TeaTime.Common/TeaTime.Common.csproj", "src/TeaTime.Common/"]
COPY ["src/TeaTime.Slack/TeaTime.Slack.csproj", "src/TeaTime.Slack/"]
RUN dotnet restore src/TeaTime/TeaTime.csproj

# Copy in everything else
COPY sql/ sql/
COPY src/ src/

# Build
# VERSION overrides the whole version for release builds
# VERSION_SUFFIX is appended to the props VersionPrefix for dev builds
ARG VERSION
ARG VERSION_SUFFIX
WORKDIR /build/
RUN dotnet build src/TeaTime/TeaTime.csproj --no-restore -c Release \
    ${VERSION:+-p:Version=$VERSION} \
    ${VERSION_SUFFIX:+-p:VersionSuffix=$VERSION_SUFFIX}

# Publish
FROM build AS publish
WORKDIR /build/src/TeaTime
RUN dotnet publish "TeaTime.csproj" --no-build -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TeaTime.dll"]
