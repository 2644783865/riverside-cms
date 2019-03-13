FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["src/Applications/Web/Web.Api/Web.Api.csproj", "src/Applications/Web/Web.Api/"]
RUN dotnet restore "src/Applications/Web/Web.Api/Web.Api.csproj"
COPY . .
WORKDIR "/src/src/Applications/Web/Web.Api"
RUN dotnet build "Web.Api.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Web.Api.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Web.Api.dll"]