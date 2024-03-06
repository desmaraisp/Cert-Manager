FROM node:20-alpine as deps
WORKDIR /app

COPY ./src/react-app/package*.json /app/src/react-app/
RUN cd /app/src/react-app && npm ci

FROM deps as build
COPY ./src/react-app/ /app/src/react-app
RUN cd /app/src/react-app && npm run build

FROM nginx:1.21.6-alpine
COPY --from=build /app/src/react-app/dist /usr/share/nginx/html
COPY ./src/react-app/nginx.conf /etc/nginx/conf.d/default.conf

COPY ./src/react-app/env-subst.sh /docker-entrypoint.d/env.sh
RUN chmod +x /docker-entrypoint.d/env.sh