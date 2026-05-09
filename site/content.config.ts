import { defineContentConfig, defineCollection } from '@nuxt/content'

// 單一 collection,涵蓋所有從 ../docs/ 同步進來的 markdown。
// 路徑會是 /en/<slug> 或 /zh-TW/<slug>。
export default defineContentConfig({
  collections: {
    content: defineCollection({
      type: 'page',
      source: '**/*.md',
    }),
  },
})
