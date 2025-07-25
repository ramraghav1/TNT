# Stage 1: Build
FROM node:20 AS build
WORKDIR /app

# Copy package.json and package-lock.json from the current context (which is client folder)
COPY package*.json ./

RUN npm install --legacy-peer-deps

COPY . .

RUN npm run build --prod

# Stage 2: Serve with NGINX
FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
