FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY CommentApp.csproj ./
RUN dotnet restore CommentApp.csproj

COPY . ./

RUN dotnet publish CommentApp.csproj -c Release -o out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/out .

RUN mkdir -p wwwroot/uploads/images wwwroot/uploads/docs
EXPOSE 8080
ENTRYPOINT ["dotnet", "CommentApp.dll"]