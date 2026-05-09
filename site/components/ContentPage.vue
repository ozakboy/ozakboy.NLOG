<script setup lang="ts">
// 共用內容頁元件:依 i18n 當前語系從 @nuxt/content 撈對應 markdown 渲染。
// 路徑規則:content/<locale>/<slug>.md -> path='/<locale>/<slug>'
const props = defineProps<{
  slug: string
  fallbackTitle?: string
}>()

const { locale } = useI18n()

const targetPath = computed(() => `/${locale.value}/${props.slug}`)

const { data: result } = await useAsyncData(
  () => `content-${locale.value}-${props.slug}`,
  async () => {
    const target = `/${locale.value}/${props.slug}`

    // 嘗試 1:直接用 .path() 撈
    const byPath = await queryCollection('content').path(target).first()
    if (byPath) {
      return { doc: byPath, debug: { matched: 'path()', target, all: null } }
    }

    // 嘗試 2:抓全部找符合的(用於 debug + fallback)
    const all = await queryCollection('content').all()
    const allPaths = all.map((d: any) => ({
      path: d.path,
      id: d.id,
      stem: d.stem,
      title: d.title,
    }))

    // 嘗試 path / id / stem 多種比對
    const matched = all.find((d: any) =>
      d.path === target
      || d.id === target
      || d.path?.endsWith(`/${locale.value}/${props.slug}`)
      || d.id?.endsWith(`/${locale.value}/${props.slug}.md`)
      || (d.stem === props.slug && (d.path || d.id || '').includes(`/${locale.value}/`))
    )

    return {
      doc: matched ?? null,
      debug: {
        matched: matched ? 'fallback (.all() + manual match)' : 'NONE',
        target,
        all: allPaths,
      },
    }
  },
  { watch: [locale] }
)

const doc = computed(() => result.value?.doc)
const debug = computed(() => result.value?.debug)

// dev mode 時印 debug 到 console
if (import.meta.dev && import.meta.client) {
  watchEffect(() => {
    if (debug.value) {
      // eslint-disable-next-line no-console
      console.log('[ContentPage]', {
        slug: props.slug,
        locale: locale.value,
        target: debug.value.target,
        matched: debug.value.matched,
        availablePaths: debug.value.all,
      })
    }
  })
}

useHead({
  title: () =>
    doc.value?.title
      ? `${doc.value.title} · OzaLog`
      : props.fallbackTitle ?? 'OzaLog',
})
</script>

<template>
  <article class="max-w-3xl mx-auto px-4 py-12 sm:py-16 prose prose-slate prose-pre:bg-slate-900 prose-pre:text-slate-100 prose-headings:scroll-mt-20">
    <ContentRenderer v-if="doc" :value="doc" />
    <div v-else class="not-prose bg-amber-50 border border-amber-200 rounded p-4 text-sm text-slate-700 space-y-2">
      <p class="font-semibold">Content not found</p>
      <p>
        Locale: <code class="font-mono text-xs bg-slate-100 px-1 rounded">{{ locale }}</code>
        · Slug: <code class="font-mono text-xs bg-slate-100 px-1 rounded">{{ slug }}</code>
      </p>
      <p>Expected path: <code class="font-mono text-xs bg-slate-100 px-1 rounded">{{ targetPath }}</code></p>
      <details v-if="debug && debug.all" class="mt-3">
        <summary class="cursor-pointer text-xs text-slate-500">
          Debug: 已載入 {{ debug.all.length }} 個內容(點擊展開)
        </summary>
        <pre class="mt-2 text-xs bg-slate-50 border border-slate-200 rounded p-2 overflow-x-auto">{{ JSON.stringify(debug.all, null, 2) }}</pre>
      </details>
      <p class="mt-2 text-xs text-slate-500">
        編輯 <code class="font-mono">../docs/</code> 後請執行 <code class="font-mono">pnpm sync-docs</code>(或重啟 dev server)。
      </p>
    </div>
  </article>
</template>
