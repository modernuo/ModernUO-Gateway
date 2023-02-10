import fs from 'node:fs';
import { defineConfig, loadEnv } from 'vite';
import react from '@vitejs/plugin-react';

// https://vitejs.dev/config/
export default defineConfig(({ mode }) => {
  const env = { ...process.env, ...loadEnv(mode, __dirname) };

  const backendPort = env.VITE_ASPNETCORE_HTTPS_PORT;

  const target = backendPort ? `https://localhost:${backendPort}` :
  env.VITE_ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'http://localhost:44784';

  const backendRelativePaths = [
    "/weatherforecast",
    "/_configuration",
    "/.well-known",
    "/Identity",
    "/connect",
    "/ApplyDatabaseMigrations",
    "/_framework"
  ];

  const onError = (err, req, resp, target) => {
    console.error(err.message.toString);
  }

  const https = env.VITE_HTTPS?.toLowerCase() === 'true' ? {
    key: env.VITE_SSL_KEY_FILE ? fs.readFileSync(env.VITE_SSL_KEY_FILE) : undefined,
    cert: env.VITE_SSL_CRT_FILE ? fs.readFileSync(env.VITE_SSL_CRT_FILE) : undefined,
  } : false;

  const port = env.VITE_PORT ? parseInt(env.VITE_PORT, 10) : 5173;

  const proxy = backendRelativePaths.reduce((proxy, path) => ({
    ...proxy,
    [path]: {
      target,
      secure: false,
      headers: {
        Connection: 'Keep-Alive'
      },
      onError
    }
  }), {});

  return {
    server: {
      https,
      port,
      proxy,
    },
    plugins: [react()],
  };
});