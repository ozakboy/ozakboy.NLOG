#!/usr/bin/env node
// 修 @nuxtjs/sitemap v7 + i18n + baseURL 已知 bug:
// 預設 sitemap.xml 變成 HTML meta-refresh 重導到 /sitemap_index.xml,
// 但 redirect URL 缺 baseURL 前綴 → 爬蟲 follow 後 404。
//
// 此腳本在 nuxt generate 之後執行(postgenerate hook):
// 1. 刪掉 .output/public/sitemap.xml/ 目錄 (含內部的 redirect index.html)
// 2. 把 .output/public/sitemap_index.xml 的內容複製成 .output/public/sitemap.xml
//
// Google/Bing 都接受 sitemap.xml 本身為 sitemap index 檔案,不需另外指向。

import fs from 'node:fs'
import path from 'node:path'
import { fileURLToPath } from 'node:url'

const __dirname = path.dirname(fileURLToPath(import.meta.url))
const SITE_DIR = path.resolve(__dirname, '..')
const PUBLIC_DIR = path.join(SITE_DIR, '.output', 'public')

const sitemapXmlDir = path.join(PUBLIC_DIR, 'sitemap.xml')
const sitemapIndexFile = path.join(PUBLIC_DIR, 'sitemap_index.xml')
const sitemapXmlFile = sitemapXmlDir // will become a file after fix

if (!fs.existsSync(PUBLIC_DIR)) {
  console.error(`[fix-sitemap] .output/public not found — run nuxt generate first`)
  process.exit(1)
}

if (!fs.existsSync(sitemapIndexFile)) {
  console.error(`[fix-sitemap] sitemap_index.xml not found — sitemap module did not generate it`)
  process.exit(1)
}

// 1. 若 sitemap.xml 是目錄,移除它
if (fs.existsSync(sitemapXmlDir)) {
  const stat = fs.statSync(sitemapXmlDir)
  if (stat.isDirectory()) {
    fs.rmSync(sitemapXmlDir, { recursive: true, force: true })
    console.log('[fix-sitemap] removed buggy sitemap.xml/ redirect directory')
  } else if (stat.isFile()) {
    // 已經是檔案(可能其他工具修過了),跳過
    console.log('[fix-sitemap] sitemap.xml already a file — skipping')
    process.exit(0)
  }
}

// 2. 把 sitemap_index.xml 內容複製成 sitemap.xml
fs.copyFileSync(sitemapIndexFile, sitemapXmlFile)
const size = fs.statSync(sitemapXmlFile).size
console.log(`[fix-sitemap] sitemap.xml now contains sitemap index (${size} bytes)`)
