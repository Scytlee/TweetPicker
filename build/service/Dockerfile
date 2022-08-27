FROM mcr.microsoft.com/dotnet/runtime:6.0-alpine AS runtime
WORKDIR /app
COPY out ./
RUN apk add icu-libs
ENTRYPOINT ["dotnet", "TweetPicker.Service.dll"]