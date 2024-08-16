FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Curus.API/Curus.API.csproj", "Curus.API/"]
COPY ["Curus.Repository/Curus.Repository.csproj", "Curus.Repository/"]
COPY ["Curus.Service/Curus.Service.csproj", "Curus.Service/"]
RUN dotnet restore "Curus.API/Curus.API.csproj"
COPY . .
WORKDIR "/src/Curus.API"
RUN dotnet build "Curus.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Curus.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Curus.API.dll"]
