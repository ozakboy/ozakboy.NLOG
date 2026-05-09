<script setup lang="ts">
// 共用內容頁元件:依 i18n 當前語系從 @nuxt/content 撈對應 markdown 渲染。
// 路徑規則:content/<locale>/<slug>.md
const props = defineProps<{
  slug: string
  fallbackTitle?: string
}>()

const { locale } = useI18n()

const contentPath = computed(() => `/${locale.value}/${props.slug}`)

const { data: doc } = await useAsyncData(
  () => `${locale.value}-${props.slug}`,
  () => queryCollection('content').path(contentPath.value).first(),
  { watch: [locale] }
)

useHead({
  title: () => (doc.value?.title ? `${doc.value.title} · OzaLog` : (props.fallbackTitle ?? 'OzaLog')),
})
</script>

<template>
  <article class="max-w-3xl mx-auto px-4 py-12 sm:py-16 prose prose-slate prose-pre:bg-slate-900 prose-pre:text-slate-100 prose-headings:scroll-mt-20">
    <ContentRenderer v-if="doc" :value="doc" />
    <div v-else class="not-prose bg-amber-50 border border-amber-200 rounded p-4 text-sm text-slate-700">
      <p class="font-semibold mb-1">Content not found</p>
      <p>
        Expected:
        <code class="font-mono text-xs">site/content/{{ locale }}/{{ slug }}.md</code>
      </p>
      <p class="mt-2 text-xs text-slate-500">
        編輯 <code class="font-mono">../docs/</code> 後請執行 <code class="font-mono">pnpm sync-docs</code> 或重啟 <code class="font-mono">pnpm dev</code>。
      </p>
    </div>
  </article>
</template>
