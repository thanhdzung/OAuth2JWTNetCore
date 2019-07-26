FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["OAuth2NetCore/OAuth2NetCore.csproj", "OAuth2NetCore/"]
RUN dotnet restore "OAuth2NetCore/OAuth2NetCore.csproj"
COPY . .
WORKDIR "/src/OAuth2NetCore"
RUN dotnet build "OAuth2NetCore.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "OAuth2NetCore.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "OAuth2NetCore.dll"]