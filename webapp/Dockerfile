FROM mcr.microsoft.com/dotnet/aspnet:5.0-focal AS base
WORKDIR /app
EXPOSE 5000

ENV ASPNETCORE_URLS=http://+:5000

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM node:current-alpine3.14 AS node_with_ts
RUN npm install -g typescriptйя

FROM node_with_ts AS ts_build
WORKDIR /src
COPY webapp . 
RUN tsc --project tsconfig.json

FROM mcr.microsoft.com/dotnet/sdk:5.0-focal AS build
WORKDIR /src
COPY ["webapp/FriendsAppNoORM.csproj", "./"]
RUN dotnet restore "FriendsAppNoORM.csproj"
COPY --from=ts_build ./src .
RUN dotnet build "FriendsAppNoORM.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FriendsAppNoORM.csproj" -c Release -o /app/publish
    
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FriendsAppNoORM.dll"]
