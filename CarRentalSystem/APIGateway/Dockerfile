# FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
# WORKDIR /app
# EXPOSE 8080
# #EXPOSE 443

# FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
# WORKDIR /src
# COPY ["APIGateway/APIGateway.csproj", "APIGateway/"]
# RUN dotnet restore "APIGateway/APIGateway.csproj"
# COPY . .
# WORKDIR "/src/APIGateway"
# RUN dotnet build "APIGateway.csproj" -c Release -o /app/build

# FROM build AS publish
# RUN dotnet publish "APIGateway.csproj" -c Release -o /app/publish

# FROM base AS final
# WORKDIR /app
# COPY --from=publish /app/publish .
# ENTRYPOINT ["dotnet", "APIGateway.dll"]

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine

WORKDIR /app

COPY publish/APIGateway/ .

EXPOSE 8080
ENTRYPOINT ["dotnet", "APIGateway.dll"]