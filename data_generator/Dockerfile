FROM mcr.microsoft.com/dotnet/aspnet:5.0-focal  AS base
WORKDIR /app

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:5.0-focal AS build

WORKDIR /webapp
COPY ["webapp/FriendsAppNoORM.csproj", "./"]
RUN dotnet restore "FriendsAppNoORM.csproj"
COPY webapp .    

WORKDIR /src
COPY ["data_generator/FriendsAppDataGenerator.csproj", "./"]
RUN dotnet restore "FriendsAppDataGenerator.csproj"
COPY data_generator .
WORKDIR "/src/."
RUN dotnet build "FriendsAppDataGenerator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FriendsAppDataGenerator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FriendsAppDataGenerator.dll"]
