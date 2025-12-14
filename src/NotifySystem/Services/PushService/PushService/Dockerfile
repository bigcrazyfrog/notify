FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/PushService/PushService/PushService.csproj", "Services/PushService/PushService/"]
COPY ["NotifySystem.Application/NotifySystem.Application.csproj", "NotifySystem.Application/"]
COPY ["NotifySystem.Core/NotifySystem.Core.csproj", "NotifySystem.Core/"]
COPY ["NotifySystem.Infrastructure/NotifySystem.Infrastructure.csproj", "NotifySystem.Infrastructure/"]
RUN dotnet restore "Services/PushService/PushService/PushService.csproj"
COPY . .
WORKDIR "/src/Services/PushService/PushService"
RUN dotnet build "PushService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "PushService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PushService.dll"]
