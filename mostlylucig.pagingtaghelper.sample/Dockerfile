FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release

# Install Node.js
RUN curl -fsSL https://deb.nodesource.com/setup_14.x | bash - \
    && apt-get install -y \
        nodejs \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /src
COPY ["mostlylucig.pagingtaghelper.sample/mostlylucig.pagingtaghelper.sample.csproj", "mostlylucig.pagingtaghelper.sample/"]
COPY ["mostlylucid.pagingtaghelper/mostlylucid.pagingtaghelper.csproj", "mostlylucid.pagingtaghelper/"]
RUN dotnet restore "mostlylucig.pagingtaghelper.sample/mostlylucig.pagingtaghelper.sample.csproj"
COPY . .
WORKDIR "/src/mostlylucig.pagingtaghelper.sample"
RUN dotnet build "mostlylucig.pagingtaghelper.sample.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "mostlylucig.pagingtaghelper.sample.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "mostlylucig.pagingtaghelper.sample.dll"]
