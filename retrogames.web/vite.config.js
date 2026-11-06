import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

// Front-end (5173) e API (5080) na mesma origem via proxy: sem CORS e com cookies SameSite=Strict.
export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    proxy: { '/api': { target: 'http://localhost:5080', changeOrigin: false } },
  },
});
