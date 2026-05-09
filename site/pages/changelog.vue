<script setup>
useHead({ title: 'Changelog · OzaLog' })
</script>

<template>
  <div class="max-w-3xl mx-auto px-4 py-12 sm:py-16">
    <h1 class="text-4xl font-bold mb-4">{{ $t('changelog.title') }}</h1>
    <p class="text-slate-600 mb-6">{{ $t('changelog.intro') }}</p>
    <a
      href="https://github.com/ozakboy/OzaLog/blob/main/CHANGELOG.md"
      target="_blank"
      rel="noopener"
      class="inline-block bg-brand-700 text-white px-5 py-2.5 rounded font-medium hover:bg-brand-900 transition"
    >
      CHANGELOG.md →
    </a>
  </div>
</template>
