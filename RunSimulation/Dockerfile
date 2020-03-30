FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["RunSimulation/RunSimulation.csproj", "RunSimulation/"]
RUN dotnet restore "RunSimulation/RunSimulation.csproj"
COPY . .
WORKDIR "/src/RunSimulation"
RUN dotnet build "RunSimulation.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "RunSimulation.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "RunSimulation.dll"]