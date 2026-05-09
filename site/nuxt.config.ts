// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: '2025-01-01',
  devtools: { enabled: true },

  // Nuxt site config (used by sitemap module to prepend absolute URL)
  // 注意:url 只能放 domain,baseURL 在 app.baseURL 處理(GitHub Pages project site 規則)
  site: {
    url: 'https://ozakboy.github.io',
    name: 'OzaLog',
    description:
      'Lean .NET local file logging library with the simplest possible static API. HFT-grade async pipeline, zero NuGet dependencies on net8+. Designed for high-throughput multi-threaded scenarios like cryptocurrency tick streams.',
  },

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
            'OzaLog — a lean, lightweight .NET local file logging library with HFT-grade async pipeline. Zero NuGet dependencies on net8+. NOT related to NLog (formerly Ozakboy.NLOG, renamed v3.0).',
        },
        {
          name: 'keywords',
          content:
            'OzaLog, .NET logger, csharp logging, HFT logging, local file logger, async logger, zero dependency, NuGet, dotnet, crypto tick stream',
        },
        { name: 'author', content: 'ozakboy' },

        // Search engine verification
        {
          name: 'google-site-verification',
          content: '7B6Z2O-JFfFD6I0jeayWg1SFeDWKZmf4RwSVGbQHmVk',
        },
        { name: 'msvalidate.01', content: '4928B4223346F74DB53D9754C37164AB' },

        // Open Graph (Facebook / LinkedIn / general social)
        { property: 'og:type', content: 'website' },
        { property: 'og:site_name', content: 'OzaLog' },
        { property: 'og:title', content: 'OzaLog — Lean .NET Local File Logger' },
        {
          property: 'og:description',
          content:
            'Simplest possible static API. HFT-grade async pipeline. Zero NuGet deps on net8+. Designed for crypto tick stream scenarios.',
        },
        { property: 'og:url', content: 'https://ozakboy.github.io/OzaLog/' },
        { property: 'og:image', content: 'https://ozakboy.github.io/OzaLog/logo.png' },
        { property: 'og:image:alt', content: 'OzaLog logo' },
        { property: 'og:locale', content: 'zh_TW' },
        { property: 'og:locale:alternate', content: 'en_US' },

        // Twitter Card
        { name: 'twitter:card', content: 'summary' },
        { name: 'twitter:title', content: 'OzaLog — Lean .NET Local File Logger' },
        {
          name: 'twitter:description',
          content:
            'Simplest possible static API. HFT-grade async pipeline. Zero NuGet deps on net8+.',
        },
        { name: 'twitter:image', content: 'https://ozakboy.github.io/OzaLog/logo.png' },
      ],
      link: [
        { rel: 'icon', type: 'image/png', href: '/OzaLog/logo.png' },
        { rel: 'apple-touch-icon', href: '/OzaLog/logo.png' },
        { rel: 'canonical', href: 'https://ozakboy.github.io/OzaLog/' },
      ],
    },
  },

  modules: [
    '@nuxtjs/i18n',
    '@nuxtjs/tailwindcss',
    '@nuxt/content',
    '@nuxtjs/sitemap',
    // @nuxtjs/robots 不適用(Project Pages baseURL 與 robots.txt root 衝突),
    // 改用 site/public/robots.txt 靜態檔
  ],

  // @nuxt/content 預設讀 site/content/。內容由 scripts/sync-docs.mjs 自動從 ../docs/ 同步進來。
  content: {
    build: {
      markdown: {
        toc: { depth: 3 },
        highlight: {
          theme: 'github-light',
        },
      },
    },
  },

  i18n: {
    baseUrl: 'https://ozakboy.github.io',
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

  // Sitemap: 自動產 sitemap.xml,部署後位於 https://ozakboy.github.io/OzaLog/sitemap.xml
  // 自動掃描 pages/ 內路由 + i18n locale 變體
  sitemap: {
    autoLastmod: true,
  },

  // GitHub Pages 部署:使用內建 preset,自動產生 404.html / .nojekyll
  nitro: {
    preset: 'github_pages',
  },

  ssr: true,
})
