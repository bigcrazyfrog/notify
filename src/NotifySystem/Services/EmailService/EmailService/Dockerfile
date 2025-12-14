FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["NotifySystem/NotifySystem.Core/NotifySystem.Core.csproj", "NotifySystem/NotifySystem.Core/"]
COPY ["NotifySystem/NotifySystem.Infrastructure/NotifySystem.Infrastructure.csproj", "NotifySystem/NotifySystem.Infrastructure/"]
COPY ["NotifySystem/NotifySystem.Application/NotifySystem.Application.csproj", "NotifySystem/NotifySystem.Application/"]
COPY ["NotifySystem/Services/EmailService/EmailService/EmailService.csproj", "NotifySystem/Services/EmailService/EmailService/"]
COPY ["NotifySystem/Services/EmailService/EmailService.Application/EmailService.Application.csproj", "NotifySystem/Services/EmailService/EmailService.Application/"]
COPY ["NotifySystem/Services/EmailService/EmailService.Infrastructure/EmailService.Infrastructure.csproj", "NotifySystem/Services/EmailService/EmailService.Infrastructure/"]

RUN dotnet restore "NotifySystem/Services/EmailService/EmailService/EmailService.csproj"

COPY . .

WORKDIR "/src/NotifySystem/Services/EmailService/EmailService"
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/runtime:9.0
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "EmailService.dll"]
