# Build from solution directory:
# docker build -f MinimalAsyncApi/Dockerfile -t minimal-async-api .

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

COPY . ./
WORKDIR MinimalAsyncApi
RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/MinimalAsyncApi/out .
ENTRYPOINT ["dotnet", "MinimalAsyncApi.dll"]