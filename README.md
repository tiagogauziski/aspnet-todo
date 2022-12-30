# TODO application using ASP.NET Core

The goal of this application is to implement a simple RESTFul API for a TODO application. 

# Docker
- Install the application using the following command:
```
docker pull ghcr.io/tiagogauziski/aspnet-todo:main
```

## Helpful commands

### Run SQL Server as Docker container
```
# Source: https://learn.microsoft.com/en-us/sql/linux/quickstart-install-connect-docker?view=sql-server-ver16&pivots=cs1-powershell
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=<YourStrong@Passw0rd>" `
   -p 1433:1433 --name sqlserver --hostname sqlserver `
   -d `
   mcr.microsoft.com/mssql/server:2022-latest
```