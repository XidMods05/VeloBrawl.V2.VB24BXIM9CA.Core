FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["VeloBrawl.V2.VB24BXIM9CA.Core/VeloBrawl.V2.VB24BXIM9CA.Core.csproj", "VeloBrawl.V2.VB24BXIM9CA.Core/"]
RUN dotnet restore "VeloBrawl.V2.VB24BXIM9CA.Core/VeloBrawl.V2.VB24BXIM9CA.Core.csproj"
COPY . .
WORKDIR "/src/VeloBrawl.V2.VB24BXIM9CA.Core"
RUN dotnet build "VeloBrawl.V2.VB24BXIM9CA.Core.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "VeloBrawl.V2.VB24BXIM9CA.Core.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VeloBrawl.V2.VB24BXIM9CA.Core.dll"]
