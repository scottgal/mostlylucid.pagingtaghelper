# Base runtime - Smaller Alpine image
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build Stage - SDK is only needed here
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
ARG BUILD_CONFIGURATION=Release

# Install Node.js (only for build, not in final image)
RUN apk add --no-cache nodejs npm \
    && npm install -g npm-run-all 

WORKDIR /src
COPY ["mostlylucid.pagingtaghelper.sample/mostlylucid.pagingtaghelper.sample.csproj", "mostlylucid.pagingtaghelper.sample/"]
COPY ["mostlylucid.pagingtaghelper/mostlylucid.pagingtaghelper.csproj", "mostlylucid.pagingtaghelper/"]
COPY ["mostlylucid.pagingtaghelper/ReleaseNotes.txt", "mostlylucid.pagingtaghelper/"]

# Restore dependencies (only once)
RUN dotnet restore "mostlylucid.pagingtaghelper.sample/mostlylucid.pagingtaghelper.sample.csproj"

COPY . .
WORKDIR "/src/mostlylucid.pagingtaghelper.sample"

# Install npm dependencies before publishing
RUN npm install

# Publish without trimming but with ReadyToRun (better performance)
RUN dotnet publish "mostlylucid.pagingtaghelper.sample.csproj" -c $BUILD_CONFIGURATION -o /app/publish \
    --no-restore -p:PublishReadyToRun=true -p:PublishSingleFile=false

# Final runtime image - Alpine-based
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "mostlylucid.pagingtaghelper.sample.dll"]