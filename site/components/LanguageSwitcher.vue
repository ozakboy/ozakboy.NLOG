<script setup>
const { locale, locales } = useI18n()
const switchLocalePath = useSwitchLocalePath()

const otherLocale = computed(() => {
  const others = locales.value.filter((l) => l.code !== locale.value)
  return others[0]
})
</script>

<template>
  <NuxtLink
    v-if="otherLocale"
    :to="switchLocalePath(otherLocale.code)"
    class="text-xs hover:text-brand-700 border border-slate-300 px-2 py-1 rounded"
  >
    {{ otherLocale.code === 'zh-TW' ? '繁中' : 'EN' }}
  </NuxtLink>
</template>
