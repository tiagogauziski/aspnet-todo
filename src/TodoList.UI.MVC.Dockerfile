#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["TodoList.UI.MVC/TodoList.UI.MVC.csproj", "TodoList.UI.MVC/"]
RUN dotnet restore "TodoList.UI.MVC/TodoList.UI.MVC.csproj"
COPY . .
WORKDIR "/src/TodoList.UI.MVC"
RUN dotnet build "TodoList.UI.MVC.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TodoList.UI.MVC.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TodoList.UI.MVC.dll"]