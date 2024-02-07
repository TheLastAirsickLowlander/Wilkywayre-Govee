FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Govee.Service/Govee.Service.csproj", "Govee.Service/"]
RUN dotnet restore "Govee.Service/Govee.Service.csproj"
COPY . .
WORKDIR "/src/Govee.Service"
RUN dotnet build "Govee.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Govee.Service.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Govee.Service.dll"]
