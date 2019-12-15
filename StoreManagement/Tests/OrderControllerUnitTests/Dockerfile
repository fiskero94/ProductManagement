FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Tests/OrderControllerUnitTests/OrderControllerUnitTests.csproj", "Tests/OrderControllerUnitTests/"]
COPY ["OrderAPI/OrderAPI.csproj", "OrderAPI/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
RUN dotnet restore "Tests/OrderControllerUnitTests/OrderControllerUnitTests.csproj"
COPY . .
WORKDIR "/src/Tests/OrderControllerUnitTests"
RUN dotnet build "OrderControllerUnitTests.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OrderControllerUnitTests.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderControllerUnitTests.dll"]