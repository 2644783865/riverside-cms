FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY ["src/Services/Cms.Api/Cms.Api.csproj", "src/Services/Cms.Api/"]
RUN dotnet restore "src/Services/Cms.Api/Cms.Api.csproj"
COPY . .
WORKDIR "/src/src/Services/Cms.Api"
RUN dotnet build "Cms.Api.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Cms.Api.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Cms.Api.dll"]