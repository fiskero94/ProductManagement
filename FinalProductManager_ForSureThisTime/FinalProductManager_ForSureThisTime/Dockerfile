FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["FinalProductManager_ForSureThisTime/FinalProductManager_ForSureThisTime.csproj", "FinalProductManager_ForSureThisTime/"]
RUN dotnet restore "FinalProductManager_ForSureThisTime/FinalProductManager_ForSureThisTime.csproj"
COPY . .
WORKDIR "/src/FinalProductManager_ForSureThisTime"
RUN dotnet build "FinalProductManager_ForSureThisTime.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FinalProductManager_ForSureThisTime.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FinalProductManager_ForSureThisTime.dll"]