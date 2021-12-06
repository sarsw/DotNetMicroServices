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
ENTRYPOINT [ "dotnet", "CommandsService.dll" ]

# To build the image use "docker build -t <your Docker id>/platformservice ."
# To run the container use 'docker run -p 8080:80 -d <your Docker id>/platformservice'
# to send the image to DockerHub use 'docker push <your Docker id>/platformservice'
# Other useful commands:
# "docker ps"               get a list of running containers
# "docker stop <id>"        stop a container by id (from the ps cmd)
# "docker start <id>"       restart a container by id (from the ps cmd)