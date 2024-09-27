FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ENV ASPNETCORE_HTTP_PORTS=5000
WORKDIR /app

COPY . /app
RUN dotnet publish  Projects/xe.data.service/xe.data.service.csproj --nologo --configuration Release --no-self-contained --output /app/publish/xe.data.service
RUN dotnet test Tests/xe.data.service.Tests/xe.data.service.Tests.csproj --nologo --no-restore --no-build --configuration Release --logger:html --results-directory /app/TestsResults

FROM python:3 AS download_config
RUN  pip install awscli
RUN --mount=type=secret,id=aws_cred,target=/root/.aws/credentials \
    --mount=type=secret,id=aws_conf,target=/root/.aws/config \
    aws s3 cp s3://data-service-config/test.txt . --profile staging

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS runner

ENV ASPNETCORE_HTTP_PORTS=80
WORKDIR /app/publish/xe.data.service

COPY --from=build /app/publish/xe.data.service .
COPY --from=download_config test.txt /app/publish/xe.data.service/
RUN cat /app/publish/xe.data.service/test.txt
ENTRYPOINT ["dotnet","xe.data.service.dll"]
EXPOSE 80