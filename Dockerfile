FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Копируем только HTTPS-сертификаты (папка монтируется в compose, но копия нужна "на всякий случай")
COPY ["https/", "/https/"]

ENV JWT_SECRET_KEY=YAMeteKUDa5a1F0RGALLERy5ervErJWT
ENV JWT_EXPIRES_DAYS=12
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=yourpassword

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS with-node
RUN apt-get update && \
    apt-get install -y curl && \
    curl -sL https://deb.nodesource.com/setup_20.x | bash && \
    apt-get install -y nodejs && \
    rm -rf /var/lib/apt/lists/*

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

# Создаем папку Data (но не копируем файлы, они будут монтироваться извне)
RUN mkdir -p /app/Data && \
    chmod -R 755 /app/Data

ENTRYPOINT ["dotnet", "Gallery.Server.dll"]