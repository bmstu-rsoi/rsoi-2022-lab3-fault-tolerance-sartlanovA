# FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
# WORKDIR /app
# EXPOSE 8050

# FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
# WORKDIR /app
# COPY ["Payments/Payments.csproj", "Payments/"]
# RUN dotnet restore "Payments/Payments.csproj"
# COPY . .
# WORKDIR "/src/Payments"
# RUN dotnet build "Payments.csproj" -c Release -o /app/build

# FROM build AS publish
# RUN dotnet publish "Payments.csproj" -c Release -o /app/publish

# FROM base AS final
# WORKDIR /app
# COPY --from=publish /app/publish .
# ENTRYPOINT ["dotnet", "Payments.dll"]

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine

WORKDIR /app

COPY publish/Payments/ .

EXPOSE 8050
ENTRYPOINT ["dotnet", "Payments.dll"]