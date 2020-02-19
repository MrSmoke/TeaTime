FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/TeaTime/TeaTime.csproj", "src/TeaTime/"]
COPY ["src/TeaTime.Slack/TeaTime.Slack.csproj", "src/TeaTime.Slack/"]
COPY ["src/TeaTime.Common/TeaTime.Common.csproj", "src/TeaTime.Common/"]
COPY ["src/TeaTime.Data.MySql/TeaTime.Data.MySql.csproj", "src/TeaTime.Data.MySql/"]
RUN dotnet restore "src/TeaTime/TeaTime.csproj"
COPY . .
WORKDIR "/src/src/TeaTime"
RUN dotnet build "TeaTime.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "TeaTime.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "TeaTime.dll"]