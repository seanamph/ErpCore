# ErpCore API Dockerfile
# 用途: 建置 ErpCore API 的 Docker 映像

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# 複製專案檔
COPY ["src/ErpCore.Api/ErpCore.Api.csproj", "src/ErpCore.Api/"]
COPY ["src/ErpCore.Application/ErpCore.Application.csproj", "src/ErpCore.Application/"]
COPY ["src/ErpCore.Domain/ErpCore.Domain.csproj", "src/ErpCore.Domain/"]
COPY ["src/ErpCore.Infrastructure/ErpCore.Infrastructure.csproj", "src/ErpCore.Infrastructure/"]
COPY ["src/ErpCore.Shared/ErpCore.Shared.csproj", "src/ErpCore.Shared/"]

# 還原 NuGet 套件
RUN dotnet restore "src/ErpCore.Api/ErpCore.Api.csproj"

# 複製所有原始碼
COPY . .

# 建置專案
WORKDIR "/src/src/ErpCore.Api"
RUN dotnet build "ErpCore.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ErpCore.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# 設定環境變數
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "ErpCore.Api.dll"]

