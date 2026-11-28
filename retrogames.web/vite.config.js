
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],

  // Caminho do projeto no GitHub Pages
  base: '/RetroGames/',

  server: {
    port: 5173,

    proxy: {
      '/api': {
        target: 'http://localhost:5267',
        changeOrigin: true,
      },
    },
  },
})
