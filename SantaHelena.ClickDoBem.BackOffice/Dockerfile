FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 54274
EXPOSE 44385

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["SantaHelena.ClickDoBem.BackOffice/SantaHelena.ClickDoBem.BackOffice.csproj", "SantaHelena.ClickDoBem.BackOffice/"]
RUN dotnet restore "SantaHelena.ClickDoBem.BackOffice/SantaHelena.ClickDoBem.BackOffice.csproj"
COPY . .
WORKDIR "/src/SantaHelena.ClickDoBem.BackOffice"
RUN dotnet build "SantaHelena.ClickDoBem.BackOffice.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "SantaHelena.ClickDoBem.BackOffice.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "SantaHelena.ClickDoBem.BackOffice.dll"]