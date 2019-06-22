FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["Actio.Services.Activities/Actio.Services.Activities.csproj", "Actio.Services.Activities/"]
RUN dotnet restore "Actio.Services.Activities/Actio.Services.Activities.csproj"
COPY . .
WORKDIR "/src/Actio.Services.Activities"
RUN dotnet build "Actio.Services.Activities.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Actio.Services.Activities.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Actio.Services.Activities.dll"]