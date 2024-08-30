FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base
WORKDIR /app

ENV HUSKY=0
ENV ASPNETCORE_HTTP_PORTS=5000
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ENV HUSKY=0
ENV ASPNETCORE_HTTP_PORTS=5000
WORKDIR /app

COPY . /app
RUN dotnet publish  Projects/xe.data.service/xe.data.service.csproj --nologo --configuration Release --no-self-contained --output /app/publish/xe.data.service
RUN dotnet test Tests/xe.data.service.Tests/xe.data.service.Tests.csproj --nologo --no-restore --no-build --configuration Release --logger:html --results-directory /app/TestsResults

FROM base AS final
COPY entrypoint.sh /entrypoint.sh
USER root
RUN chmod +x /entrypoint.sh
RUN apt-get update
RUN apt-get install unzip
RUN curl "https://awscli.amazonaws.com/awscli-exe-linux-x86_64.zip" -o "awscliv2.zip"
RUN unzip awscliv2.zip
RUN ./aws/install

ENV ASPNETCORE_HTTP_PORTS=80
WORKDIR /app/publish/xe.data.service
COPY --from=build /app/publish/xe.data.service .
ENTRYPOINT ["/entrypoint.sh"]