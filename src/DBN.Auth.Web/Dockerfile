FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["DBN.Auth.Web/DBN.Auth.Web.csproj", "DBN.Auth.Web/"]
RUN dotnet restore "DBN.Auth.Web/DBN.Auth.Web.csproj"
COPY . .
WORKDIR "/src/DBN.Auth.Web"
RUN dotnet build "DBN.Auth.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DBN.Auth.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DBN.Auth.Web.dll"]