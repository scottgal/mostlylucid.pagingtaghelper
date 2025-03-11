FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release

# Install Node.js (Updated to v18)
RUN curl -fsSL https://deb.nodesource.com/setup_18.x | bash - \
    && apt-get install -y \
        nodejs \
    && rm -rf /var/lib/apt/lists/*


WORKDIR /src
COPY ["mostlylucid.pagingtaghelper.sample/mostlylucid.pagingtaghelper.sample.csproj", "mostlylucid.pagingtaghelper.sample/"]
COPY ["mostlylucid.pagingtaghelper/mostlylucid.pagingtaghelper.csproj", "mostlylucid.pagingtaghelper/"]
RUN dotnet restore "mostlylucid.pagingtaghelper.sample/mostlylucid.pagingtaghelper.sample.csproj"
COPY . .
WORKDIR "/src/mostlylucid.pagingtaghelper.sample"
RUN dotnet build "mostlylucid.pagingtaghelper.sample.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "mostlylucid.pagingtaghelper.sample.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "mostlylucid.pagingtaghelper.sample.dll"]