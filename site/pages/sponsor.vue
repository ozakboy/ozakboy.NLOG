<script setup>
useHead({ title: 'Sponsor · OzaLog' })

const wallets = [
  { key: 'btc', icon: '₿', color: 'text-orange-500' },
  { key: 'eth', icon: 'Ξ', color: 'text-indigo-500' },
  { key: 'usdt', icon: '₮', color: 'text-emerald-500' },
]
</script>

<template>
  <div class="max-w-3xl mx-auto px-4 py-12 sm:py-16">
    <h1 class="text-4xl font-bold mb-4">{{ $t('sponsor.title') }}</h1>
    <p class="text-slate-600 mb-10 leading-relaxed">{{ $t('sponsor.intro') }}</p>

    <section>
      <h2 class="text-2xl font-bold mb-6">{{ $t('sponsor.crypto.title') }}</h2>
      <div class="space-y-4">
        <div
          v-for="w in wallets"
          :key="w.key"
          class="bg-white border border-slate-200 rounded-lg p-6 flex items-center gap-5"
        >
          <div
            class="text-4xl font-bold w-14 text-center"
            :class="w.color"
          >
            {{ w.icon }}
          </div>
          <div class="flex-1">
            <h3 class="font-semibold text-lg">{{ $t(`sponsor.crypto.${w.key}`) }}</h3>
            <p class="text-sm text-slate-500 font-mono mt-1">
              {{ $t('sponsor.crypto.placeholder') }}
            </p>
          </div>
        </div>
      </div>
    </section>
  </div>
</template>
