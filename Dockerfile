FROM node:20-alpine

WORKDIR /app

# Install native build tools for better-sqlite3
RUN apk add --no-cache python3 make g++

COPY package*.json ./
RUN npm ci

COPY . .
RUN npm run build

RUN npm prune --production

RUN mkdir -p /data

ENV DATABASE_PATH=/data/planner.db
ENV PORT=3000
ENV HOST=0.0.0.0
ENV NODE_ENV=production

EXPOSE 3000

VOLUME ["/data"]

CMD ["node", "build"]
