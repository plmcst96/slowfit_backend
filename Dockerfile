# Usa l'immagine ufficiale dell'SDK per compilare il progetto
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App

# Copia tutto il codice e ripristina le dipendenze
COPY . ./
RUN dotnet restore slowfit.csproj
# Compila il progetto per il rilascio
RUN dotnet publish slowfit.csproj -c Release -o out

# Crea l'immagine finale usando solo il runtime (più leggera)
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /App
COPY --from=build-env /App/out .
ENTRYPOINT ["dotnet", "slowfit.dll"]
