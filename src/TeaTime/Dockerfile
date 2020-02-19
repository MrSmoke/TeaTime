#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

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
RUN dotnet build "TeaTime.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TeaTime.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TeaTime.dll"]