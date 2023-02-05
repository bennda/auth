#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
#COPY ["DBN.Auth.WebApi/DBN.Auth.WebApi.csproj", "DBN.Auth.WebApi/"]
COPY ["DBN.Auth.WebApi.csproj", "DBN.Auth.WebApi/"]
##RUN dotnet restore "DBN.Auth.WebApi/DBN.Auth.WebApi.csproj" -r linux-x64
RUN dotnet restore "./DBN.Auth.WebApi.csproj" -r linux-x64
COPY . .
WORKDIR "/src/DBN.Auth.WebApi"
RUN dotnet build "DBN.Auth.WebApi.csproj" -c Release -o /app/build --no-restore -r linux-x64

FROM build AS publish
RUN dotnet publish "DBN.Auth.WebApi.csproj" -c Release -r linux-x64 -o /app/publish --self-contained false --no-restore
#RUN dotnet publish "DBN.Auth.WebApi.csproj" -c Release -r linux-x64 -o /app/publish /p:UseAppHost=false --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DBN.Auth.WebApi.dll"]