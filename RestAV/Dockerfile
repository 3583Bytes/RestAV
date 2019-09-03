FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["RestAV/RestAV.csproj", "RestAV/"]
RUN dotnet restore "RestAV/RestAV.csproj"
COPY . .
WORKDIR "/src/RestAV"
RUN dotnet build "RestAV.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "RestAV.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "RestAV.dll"]