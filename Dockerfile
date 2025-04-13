FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY ["https/", "/https/"]

ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=yourpassword

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS with-node
RUN apt-get update
RUN apt-get install curl
RUN curl -sL https://deb.nodesource.com/setup_20.x | bash
RUN apt-get -y install nodejs

FROM with-node AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Gallery.Server", "Gallery.Server/"]
COPY ["Gallery.client", "Gallery.client/"]
RUN dotnet restore "./Gallery.Server/Gallery.Server.csproj"
COPY . .
WORKDIR "/src/Gallery.Server"
RUN dotnet build "./Gallery.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Gallery.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

COPY ["Gallery.Server/Data/", "Data/"]
USER root
RUN chmod -R 777 Data/
USER $APP_UID

ENTRYPOINT ["dotnet", "Gallery.Server.dll"]
