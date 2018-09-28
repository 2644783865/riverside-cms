FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["src/Applications/Web/Web.Mvc/Web.Mvc.csproj", "src/Applications/Web/Web.Mvc/"]
RUN dotnet restore "src/Applications/Web/Web.Mvc/Web.Mvc.csproj"
COPY . .
WORKDIR "/src/src/Applications/Web/Web.Mvc"
RUN dotnet build "Web.Mvc.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Web.Mvc.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Web.Mvc.dll"]