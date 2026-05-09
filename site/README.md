# OzaLog Site

OzaLog 介紹網站（Nuxt 3 + Vite + Vue 3 + Tailwind CSS + i18n）。
部署至 GitHub Pages：<https://ozakboy.github.io/OzaLog/>

## 套件管理器

本專案使用 **pnpm**（搭配 `.npmrc` 中的 `node-linker=hoisted`，避免 Windows symlink 鎖檔問題）。

## 開發

```bash
cd site
pnpm install
pnpm run dev   # http://localhost:3000
```

## 建置（產生靜態檔案）

```bash
pnpm run generate   # 輸出至 .output/public/
```

## 部署

push 到 `main` 並改動 `site/**` 會自動觸發 [.github/workflows/deploy-site.yml](../.github/workflows/deploy-site.yml)，由 GitHub Actions 建置並部署到 GitHub Pages。

## 結構

```
site/
├── nuxt.config.ts         # baseURL: '/OzaLog/'、i18n、Tailwind 設定
├── app.vue                # 根元件
├── layouts/default.vue    # 共用 header / footer
├── pages/                 # 自動路由
├── components/            # 共用元件
├── locales/               # i18n（zh-TW.json / en.json）
└── tailwind.config.js
```

## 雙語

- 預設語系：`zh-TW`（URL 為 `/`）
- 英文：`/en/`
- 切換：layout header 右側 LanguageSwitcher

## GitHub Pages 設定提醒

首次部署前需在 repo Settings → Pages → Source 選 **"GitHub Actions"**。
