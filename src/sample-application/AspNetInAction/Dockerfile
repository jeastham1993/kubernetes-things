FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build-env
WORKDIR /app
    
# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore
    
# Copy everything else and build
COPY ./ ./
RUN dotnet publish --runtime alpine-x64 -c Release --self-contained true -o out

# Build runtime image
FROM alpine:3.16
# Add some libs required by .NET runtime 
# https://github.com/dotnet/core/blob/master/Documentation/build-and-install-rhel6-prerequisites.md#troubleshooting
RUN apk add --no-cache \ 
    openssh libunwind \
    nghttp2-libs libidn krb5-libs libuuid lttng-ust zlib \
    libstdc++ libintl \
    icu 
    
EXPOSE 80
EXPOSE 443
    
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["./AspNetInAction", "--urls", "http://0.0.0.0:80"]