# OzaLog Site

OzaLog 介紹網站（**Nuxt 4** + Vite + Vue 3 + Tailwind CSS + i18n + @nuxt/content）。
部署至 GitHub Pages：<https://ozakboy.github.io/OzaLog/>

## 套件管理器

本專案使用 **npm**（Node 20 LTS）。

> 為何不用 pnpm：Nuxt 4 內部用了多個 Rust-native 套件（oxc-parser、oxc-transform、unrs-resolver、rolldown 等），每個都有平台專屬的 optional native binding。pnpm 在 Windows + `node-linker=hoisted` 模式下對這類 optionalDependencies 處理有 bug，會反覆漏裝 binding。npm 則無此問題。

## Node 版本

- **Node 22 LTS**（與 GitHub Actions workflow 一致；見 `.nvmrc`）
- 不要低於 Node 20.19,Nuxt 4 / oxc-* 系列要求 `^20.19.0 || >=22.12.0`
- 推薦 Node 22.22.1+ (本專案開發用)

## 開發

```bash
cd site
npm install
npm run dev   # http://localhost:3000
```

## 建置（產生靜態檔案）

```bash
npm run generate   # 輸出至 .output/public/
```

## 部署

push 到 `main` 並改動 `site/**` 或 `docs/**` 會自動觸發 [.github/workflows/deploy-site.yml](../.github/workflows/deploy-site.yml)，由 GitHub Actions 建置並部署到 GitHub Pages。

## 結構

```
site/
├── nuxt.config.ts         # baseURL: '/OzaLog/'、i18n、Tailwind、@nuxt/content 設定
├── content.config.ts      # @nuxt/content collection 定義
├── app.vue                # 根元件
├── layouts/default.vue    # 共用 header / footer
├── pages/                 # 自動路由
├── components/            # 共用元件 (含 ContentPage.vue / LanguageSwitcher.vue)
├── i18n/locales/          # i18n 字串 (zh-TW.json / en.json)
├── content/               # 由 scripts/sync-docs.mjs 從 ../docs/ 自動產生 (gitignored)
├── scripts/sync-docs.mjs  # docs/ → site/content/ 同步腳本
├── public/binance-pay-qr.png  # Binance Pay 贊助 QR (使用者手動放)
└── tailwind.config.js
```

## 雙語

- 預設語系：`zh-TW`（URL 為 `/`）
- 英文：`/en/`
- 切換：layout header 右側 LanguageSwitcher
- 內容檔案：`docs/{en,zh-TW}/*.md` ←(同步)→ `site/content/{en,zh-TW}/*.md`

## GitHub Pages 設定提醒

首次部署前需在 repo Settings → Pages → Source 選 **"GitHub Actions"**。
