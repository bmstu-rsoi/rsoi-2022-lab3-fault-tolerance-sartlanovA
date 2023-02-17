# FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
# WORKDIR /app
# EXPOSE 8070
# #EXPOSE 443

# FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
# WORKDIR /src
# COPY ["Cars/Cars.csproj", "Cars/"]
# RUN dotnet restore "Cars/Cars.csproj"
# COPY . .
# WORKDIR "/src/Cars"
# RUN dotnet build "Cars.csproj" -c Release -o /app/build

# FROM build AS publish
# RUN dotnet publish "Cars.csproj" -c Release -o /app/publish

# FROM base AS final
# WORKDIR /app
# COPY --from=publish /app/publish .
# ENTRYPOINT ["dotnet", "Cars.dll"]

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine

WORKDIR /app

COPY publish/Cars/ .

EXPOSE 8070
ENTRYPOINT ["dotnet", "Cars.dll"]