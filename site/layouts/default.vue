<script setup>
const localePath = useLocalePath()
const config = useRuntimeConfig()
const baseURL = config.app.baseURL || '/'
</script>

<template>
  <div class="min-h-screen flex flex-col bg-slate-50 text-slate-900">
    <header class="border-b border-slate-200 bg-white sticky top-0 z-10">
      <div class="max-w-6xl mx-auto px-4 py-3 flex items-center justify-between gap-4">
        <NuxtLink :to="localePath('/')" class="flex items-center gap-2">
          <img
            :src="`${baseURL}logo.png`"
            alt="OzaLog logo"
            class="w-8 h-8 rounded-full"
            width="32"
            height="32"
          />
          <span class="text-xl font-bold text-brand-700">OzaLog</span>
          <span class="text-xs text-slate-500 hidden sm:inline">{{ $t('header.tagline') }}</span>
        </NuxtLink>
        <nav class="flex items-center gap-4 sm:gap-6 text-sm">
          <NuxtLink :to="localePath('/docs')" class="hover:text-brand-700">{{ $t('nav.docs') }}</NuxtLink>
          <NuxtLink :to="localePath('/benchmarks')" class="hover:text-brand-700 hidden sm:inline">{{ $t('nav.benchmarks') }}</NuxtLink>
          <NuxtLink :to="localePath('/migration')" class="hover:text-brand-700 hidden md:inline">{{ $t('nav.migration') }}</NuxtLink>
          <NuxtLink :to="localePath('/changelog')" class="hover:text-brand-700 hidden md:inline">{{ $t('nav.changelog') }}</NuxtLink>
          <NuxtLink :to="localePath('/sponsor')" class="hover:text-brand-700">{{ $t('nav.sponsor') }}</NuxtLink>
          <a href="https://github.com/ozakboy/OzaLog" target="_blank" rel="noopener" class="hover:text-brand-700">GitHub</a>
          <LanguageSwitcher />
        </nav>
      </div>
    </header>

    <main class="flex-1">
      <slot />
    </main>

    <footer class="border-t border-slate-200 bg-white mt-16">
      <div class="max-w-6xl mx-auto px-4 py-6 text-sm text-slate-500 flex flex-col sm:flex-row items-center justify-between gap-2">
        <span>&copy; {{ new Date().getFullYear() }} ozakboy. MIT License.</span>
        <div class="flex gap-4">
          <a href="https://www.nuget.org/packages/OzaLog/" target="_blank" rel="noopener" class="hover:text-brand-700">NuGet</a>
          <a href="https://github.com/ozakboy/OzaLog" target="_blank" rel="noopener" class="hover:text-brand-700">GitHub</a>
        </div>
      </div>
    </footer>
  </div>
</template>
