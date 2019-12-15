FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Tests/ProductControllerUnitTests/ProductControllerUnitTests.csproj", "Tests/ProductControllerUnitTests/"]
COPY ["ProductAPI/ProductAPI.csproj", "ProductAPI/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
RUN dotnet restore "Tests/ProductControllerUnitTests/ProductControllerUnitTests.csproj"
COPY . .
WORKDIR "/src/Tests/ProductControllerUnitTests"
RUN dotnet build "ProductControllerUnitTests.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProductControllerUnitTests.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductControllerUnitTests.dll"]