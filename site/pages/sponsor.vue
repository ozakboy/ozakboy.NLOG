<script setup>
useHead({ title: 'Sponsor · OzaLog' })

const config = useRuntimeConfig()
const baseURL = config.app.baseURL || '/'

const usdtAddress = '0x971743C856a9E5E3fFB0fe1Fbeb2b8216e47c5A3'

const copied = ref(false)
async function copyAddress() {
  try {
    await navigator.clipboard.writeText(usdtAddress)
    copied.value = true
    setTimeout(() => (copied.value = false), 2000)
  } catch {
    // Fallback: select text manually
    copied.value = false
  }
}
</script>

<template>
  <div class="max-w-3xl mx-auto px-4 py-12 sm:py-16">
    <h1 class="text-4xl font-bold mb-4">{{ $t('sponsor.title') }}</h1>
    <p class="text-slate-600 mb-10 leading-relaxed">{{ $t('sponsor.intro') }}</p>

    <section>
      <h2 class="text-2xl font-bold mb-6">{{ $t('sponsor.crypto.title') }}</h2>

      <!-- USDT (BEP20) -->
      <div class="bg-white border border-slate-200 rounded-lg p-6 mb-4">
        <div class="flex items-start gap-5">
          <div class="text-4xl font-bold text-emerald-500 w-14 text-center mt-1">₮</div>
          <div class="flex-1 min-w-0">
            <h3 class="font-semibold text-lg">{{ $t('sponsor.usdt.title') }}</h3>

            <!-- Network warning -->
            <div class="mt-2 bg-red-50 border border-red-200 rounded px-3 py-2 text-sm text-red-700">
              <span class="font-semibold">⚠️ {{ $t('sponsor.usdt.warningTitle') }}</span>
              <span class="ml-1">{{ $t('sponsor.usdt.warningBody') }}</span>
            </div>

            <!-- Address + copy -->
            <div class="mt-3 flex items-center gap-2 bg-slate-50 border border-slate-200 rounded p-3">
              <code class="flex-1 text-xs sm:text-sm font-mono break-all text-slate-800">{{ usdtAddress }}</code>
              <button
                type="button"
                class="shrink-0 text-xs px-3 py-1.5 rounded bg-brand-700 text-white hover:bg-brand-900 transition disabled:opacity-50"
                :disabled="copied"
                @click="copyAddress"
              >
                {{ copied ? $t('sponsor.copied') : $t('sponsor.copy') }}
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Binance Pay -->
      <div class="bg-white border border-slate-200 rounded-lg p-6">
        <div class="flex items-start gap-5">
          <div class="text-4xl font-bold text-yellow-500 w-14 text-center mt-1">B</div>
          <div class="flex-1">
            <h3 class="font-semibold text-lg">{{ $t('sponsor.binancePay.title') }}</h3>
            <p class="text-sm text-slate-600 mt-2">{{ $t('sponsor.binancePay.desc') }}</p>
            <div class="mt-4 flex justify-center sm:justify-start">
              <img
                :src="`${baseURL}binance-pay-qr.png`"
                :alt="$t('sponsor.binancePay.title')"
                class="w-56 h-56 border border-slate-200 rounded bg-white p-2 object-contain"
                width="224"
                height="224"
              />
            </div>
          </div>
        </div>
      </div>
    </section>
  </div>
</template>
