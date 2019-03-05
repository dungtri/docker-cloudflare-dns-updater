FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch-arm32v7 AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY src/*.csproj ./src/
WORKDIR /app/src
RUN dotnet restore

# copy and build app and libraries
WORKDIR /app/
COPY src/. ./src/
WORKDIR /app/src
# add IL Linker package
RUN dotnet add package ILLink.Tasks -v 0.1.5-preview-1841731 -s https://dotnet.myget.org/F/dotnet-core/api/v3/index.json
RUN dotnet publish -c Release -r linux-arm -o out /p:ShowLinkerSizeComparison=true


# test application -- see: dotnet-docker-unit-testing.md
FROM build AS testrunner
WORKDIR /app/tests
COPY tests/. .
ENTRYPOINT ["dotnet", "test", "--logger:trx"]


FROM mcr.microsoft.com/dotnet/core/runtime-deps:2.2-stretch-slim-arm32v7 AS runtime
WORKDIR /app
COPY --from=build /app/src/out ./
ENTRYPOINT ["./dns-updater"]

