#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
RUN apt-get update && apt-get -y install libgdiplus libc6-dev
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/Orneholm.CognitiveWorkbench.Web/Orneholm.CognitiveWorkbench.Web.csproj", "Orneholm.CognitiveWorkbench.Web/"]
RUN dotnet restore "Orneholm.CognitiveWorkbench.Web/Orneholm.CognitiveWorkbench.Web.csproj"
COPY /src .
WORKDIR "/src/Orneholm.CognitiveWorkbench.Web"
RUN dotnet build "Orneholm.CognitiveWorkbench.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Orneholm.CognitiveWorkbench.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Orneholm.CognitiveWorkbench.Web.dll"]
