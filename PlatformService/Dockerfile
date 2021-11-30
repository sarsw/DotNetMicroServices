# Build
# Pull image
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS bulid-env
WORKDIR /app

# Copy our project over
COPY *.csproj ./
# Pull the packages
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

# Prepare the runtime (smaller) image and copy build-env files to it
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=bulid-env /app/out .

# Define the thing to run
ENTRYPOINT [ "dotnet", "PlatformService.dll" ]

# To build the image use "docker build -t <your Docker id>/platformservice ."