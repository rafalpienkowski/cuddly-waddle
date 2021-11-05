FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

COPY /src .

RUN dotnet restore "/src/SMS.App/SMS.App.csproj"
WORKDIR "/src/SMS.App"

RUN dotnet build "SMS.App.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SMS.App.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SMS.App.dll"]
