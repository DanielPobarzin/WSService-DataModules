
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
USER root

RUN chmod -R 777 /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Communications/Communications.csproj", "Communications/"]
COPY ["Entities/Entities.csproj", "Entities/"]
COPY ["Interactors/Interactors.csproj", "Interactors/"]
COPY ["Repositories/Repositories.csproj", "Repositories/"]
COPY ["Shared/Shared.csproj", "Shared/"]

RUN dotnet restore "Communications/Communications.csproj"

COPY . .

WORKDIR "/src/Communications"
RUN dotnet build "Communications.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "Communications.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY ./Container/localhost.pfx /https/localhost.pfx
COPY ./Container/schema.json /app/
COPY ./Container/configureDefault.json /app/
COPY --from=publish /app/publish .
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/localhost.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=1234
ENTRYPOINT ["dotnet", "Communications.dll"]