FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Restore — copy project files first for better layer caching
COPY RecruitIQ.sln .
COPY RecruitIQ.Domain/RecruitIQ.Domain.csproj RecruitIQ.Domain/
COPY RecruitIQ.Application/RecruitIQ.Application.csproj RecruitIQ.Application/
COPY RecruitIQ.Infrastructure/RecruitIQ.Infrastructure.csproj RecruitIQ.Infrastructure/
COPY RecruitIQ.API/RecruitIQ.API.csproj RecruitIQ.API/

RUN dotnet restore

# Build & publish
COPY . .
RUN dotnet publish RecruitIQ.API/RecruitIQ.API.csproj -c Release -o /app/publish --no-restore

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000

ENTRYPOINT ["dotnet", "RecruitIQ.API.dll"]
