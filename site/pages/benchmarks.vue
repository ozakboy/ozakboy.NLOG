<script setup>
useHead({ title: 'Benchmarks · OzaLog' })
</script>

<template>
  <div class="max-w-3xl mx-auto px-4 py-12 sm:py-16">
    <h1 class="text-4xl font-bold mb-4">{{ $t('benchmarks.title') }}</h1>
    <p class="text-slate-600 mb-6">{{ $t('benchmarks.intro') }}</p>
    <div class="bg-amber-50 border border-amber-200 rounded p-4 text-sm text-slate-700">
      {{ $t('benchmarks.wip') }}
    </div>
  </div>
</template>
