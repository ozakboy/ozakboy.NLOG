<script setup>
useHead({ title: 'Docs · OzaLog' })
const localePath = useLocalePath()

const sections = [
  { path: '/docs/getting-started', key: 'gettingStarted', icon: '🚀' },
  { path: '/docs/configuration', key: 'configuration', icon: '⚙️' },
  { path: '/docs/api', key: 'api', icon: '📘' },
  { path: '/docs/async-pipeline', key: 'asyncPipeline', icon: '⚡' },
]
</script>

<template>
  <div class="max-w-4xl mx-auto px-4 py-12 sm:py-16">
    <h1 class="text-4xl font-bold mb-4">{{ $t('docs.title') }}</h1>
    <p class="text-slate-600 mb-10">{{ $t('docs.intro') }}</p>
    <div class="grid sm:grid-cols-2 gap-4">
      <NuxtLink
        v-for="s in sections"
        :key="s.path"
        :to="localePath(s.path)"
        class="border border-slate-200 rounded-lg p-6 bg-white hover:shadow-md hover:border-brand-500 transition flex items-start gap-4"
      >
        <div class="text-2xl">{{ s.icon }}</div>
        <h2 class="font-semibold text-lg">{{ $t(`docs.sections.${s.key}`) }}</h2>
      </NuxtLink>
    </div>
  </div>
</template>
