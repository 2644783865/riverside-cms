FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["src/Applications/Application.Mvc/Application.Mvc.csproj", "src/Applications/Application.Mvc/"]
RUN dotnet restore "src/Applications/Application.Mvc/Application.Mvc.csproj"
COPY . .
WORKDIR "/src/src/Applications/Application.Mvc"
RUN dotnet build "Application.Mvc.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Application.Mvc.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Application.Mvc.dll"]