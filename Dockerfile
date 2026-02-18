# Giai đoạn 1: Build & Publish
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy file csproj và restore dependencies
# Lưu ý đường dẫn dựa trên cấu trúc folder src/
COPY ["Garage_Management.API/Garage_Management.API.csproj", "Garage_Management.API/"]
COPY ["Garage_Management.Application/Garage_Management.Application.csproj", "Garage_Management.Application/"]
COPY ["Garage_Management.Base/Garage_Management.Base.csproj", "Garage_Management.Base/"]
COPY ["Garage_Management.Infrastructure/Garage_Management.Infrastructure.csproj", "Garage_Management.Infrastructure/"]

RUN dotnet restore "Garage_Management.API/Garage_Management.API.csproj"

# Copy toàn bộ source code còn lại
COPY . .

# Build và Publish
WORKDIR "/app/Garage_Management.API"
RUN dotnet build "Garage_Management.API.csproj" -c Release -o /app/build
RUN dotnet publish "Garage_Management.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Giai đoạn 2: Runtime (Chạy ứng dụng)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Garage_Management.API.dll"]