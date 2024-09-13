FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Use the correct path for the project file
COPY ["student-managerment-api.csproj", "./"]
RUN dotnet restore "student-managerment-api.csproj"

# Copy the rest of the application files
COPY . .

WORKDIR "/src/"
RUN dotnet build "student-managerment-api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "student-managerment-api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "student-managerment-api.dll"]