// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: '2025-01-01',
  devtools: { enabled: true },

  app: {
    baseURL: '/OzaLog/',
    head: {
      title: 'OzaLog — Lean .NET Local File Logger',
      htmlAttrs: { lang: 'zh-TW' },
      meta: [
        { charset: 'utf-8' },
        { name: 'viewport', content: 'width=device-width, initial-scale=1' },
        {
          name: 'description',
          content:
            'OzaLog — a lean, lightweight .NET local file logging library with HFT-grade async pipeline. Zero NuGet dependencies on net8+.',
        },
        { property: 'og:title', content: 'OzaLog' },
        { property: 'og:type', content: 'website' },
        { property: 'og:url', content: 'https://ozakboy.github.io/OzaLog/' },
      ],
      link: [{ rel: 'icon', type: 'image/x-icon', href: '/OzaLog/favicon.ico' }],
    },
  },

  modules: ['@nuxtjs/i18n', '@nuxtjs/tailwindcss'],

  i18n: {
    locales: [
      { code: 'zh-TW', name: '繁體中文', file: 'zh-TW.json' },
      { code: 'en', name: 'English', file: 'en.json' },
    ],
    defaultLocale: 'zh-TW',
    strategy: 'prefix_except_default',
    langDir: 'locales/',
    detectBrowserLanguage: {
      useCookie: true,
      cookieKey: 'i18n_redirected',
      redirectOn: 'root',
    },
  },

  // GitHub Pages 部署：使用內建 preset，自動產生 404.html / .nojekyll
  nitro: {
    preset: 'github_pages',
  },

  ssr: true,
})
