# Użyj odpowiedniego obrazu Node.js (v22.12.0)
FROM node:22.12.0 AS build

WORKDIR /app

# Skopiuj package.json i zainstaluj zależności
COPY package.json package-lock.json ./
RUN npm install

# Skopiuj cały projekt i zbuduj aplikację Angular
COPY . .
RUN npm run build --prod

# Użyj nginx do serwowania aplikacji
FROM nginx:alpine
COPY --from=build /app/dist/calendar.ui/browser /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
