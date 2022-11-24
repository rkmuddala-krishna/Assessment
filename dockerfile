FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
# ARG BUILD_CONFIGURATION=Debug
# ENV ASPNETCORE_ENVIRONMENT=Development
# ENV DOTNET_USE_POLLING_FILE_WATCHER=true  
# ENV ASPNETCORE_URLS=http://+:80  
# EXPOSE 80
# ENV DOTNET_URLS=http://+:5001
WORKDIR /src
#COPY ["DSO.OTP.API/OTPServicedb.db", "DSO.OTP.API/"]
COPY ["DSO.OTP.API/DSO.OTP.API.csproj", "DSO.OTP.API/"]
COPY ["DSO.ServiceLibrary/DSO.ServiceLibrary.csproj", "DDSO.ServiceLibrary/"]
RUN dotnet restore "DSO.OTP.API/DSO.OTP.API.csproj"
COPY . .
WORKDIR "/src/DSO.OTP.API"
RUN dotnet build "DSO.OTP.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DSO.OTP.API.csproj" -c Release -o /app/publish
COPY ["DSO.OTP.API/OTPServicedb.db", "/app/publish/"]

LABEL kind="DSO_OTP_DEV"
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# FROM alpine:3.10
# RUN apk add --update sqlite
# RUN mkdir /db
# WORKDIR /db

# #ENTRYPOINT ["sqlite3"]
# CMD ["test.db"]

RUN  apt-get update; apt-get -qq install curl;
RUN  apt-get -qq install sqlite3

# RUN /usr/bin/sqlite3 OTPServicedb.db
# RUN  /usr/bin/sqlite3 sqlite3
#RUN  /usr/bin/sqlite3 sqlite3 .open OTPServicedb.db
#RUN /usr/bin/sqlite3  OTPServicedb.db
#RUN /usr/bin/sqlite3 CREATE TABLE OTP(ID INTEGER PRIMARY KEY AUTOINCREMENT,OTPCode TEXT NOT NULL,Email TEXT NOT NULL,CREATEDDATE REAL DEFAULT (datetime('now', 'localtime')),OTPCheckCount INTEGER NOT NULL);
#RUN .Exit

ENTRYPOINT ["dotnet", "DSO.OTP.API.dll","--urls=http://*:5001"] 
# "--urls=http://*:5002"
