FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5051

ENV ASPNETCORE_URLS=http://+:5051

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["My_SocNet_Win.csproj", "./"]
RUN dotnet restore "My_SocNet_Win.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "My_SocNet_Win.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "My_SocNet_Win.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "My_SocNet_Win.dll"]
