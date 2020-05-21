FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build
WORKDIR /bld
COPY ./SensorNet.Server.csproj .

RUN dotnet restore

COPY . .

RUN dotnet build
RUN dotnet publish -o /out --no-restore

FROM mcr.microsoft.com/dotnet/core/runtime:3.0 AS runtime
WORKDIR /app
COPY --from=build /out .

CMD ["dotnet", "SensorNet.Server.dll"]