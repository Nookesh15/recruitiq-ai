FROM node:20-alpine AS build
WORKDIR /app

# Cache npm install layer
COPY recruitiq-frontend/package.json recruitiq-frontend/package-lock.json ./
RUN npm ci

COPY recruitiq-frontend/ .
RUN npm run build -- --configuration production

# Serve with nginx
FROM nginx:1.27-alpine AS runtime
COPY --from=build /app/dist/recruitiq-frontend/browser /usr/share/nginx/html

# Nginx config: redirect all routes to index.html for Angular routing
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
