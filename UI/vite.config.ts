import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

// Dev proxy to the API so relative fetch('/applications/...') works without CORS.
export default defineConfig(() => {
  const api = process.env.API_URL || 'http://localhost:5000';
  return {
    plugins: [react()],
    server: {
      proxy: {
        '/applications': {
          target: api,
          changeOrigin: true,
        },
      },
    },
  };
});
