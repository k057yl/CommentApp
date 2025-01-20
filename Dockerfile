# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER root
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Устанавливаем необходимые утилиты для работы с шрифтами
RUN apt-get update && apt-get install -y \
    fontconfig

# Этот этап используется для сборки проекта службы
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CommentApp.csproj", "."]
RUN dotnet restore "./CommentApp.csproj"
COPY . . 
WORKDIR "/src/."
RUN dotnet build "./CommentApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CommentApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
WORKDIR /app

# Копируем шрифт в контейнер (из папки wwwroot/fonts)
COPY wwwroot/fonts/ArialBlack.ttf /usr/share/fonts/truetype/msttcorefonts/ArialBlack.ttf

# Перезапускаем кэш шрифтов
RUN fc-cache -fv

# Копируем из опубликованного каталога
COPY --from=publish /app/publish .
COPY wwwroot /app/wwwroot
ENTRYPOINT ["dotnet", "CommentApp.dll"]