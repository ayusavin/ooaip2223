FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine as build
LABEL maintainer="Savin Arseniy ayusavin@st.omgtu.ru"

COPY . /app/
WORKDIR /app/

RUN dotnet publish -c Release -r linux-x64 --no-self-contained ./SpaceBattle

FROM mcr.microsoft.com/dotnet/runtime:6.0-alpine

WORKDIR /app/

COPY --from=build /app/SpaceBattle/bin/Release/net6.0/linux-x64/ .

ENTRYPOINT "./space-battle"
