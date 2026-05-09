#!/usr/bin/env node
// 將 repo 根目錄的 docs/ 同步到 site/content/,讓 @nuxt/content 讀取。
// Sync repo-root docs/ to site/content/ so @nuxt/content can read them.
//
// 觸發時機 / Triggered by:
//   - pnpm run dev      (predev hook)
//   - pnpm run generate (pregenerate hook)
//   - pnpm run build    (prebuild hook)
//   - pnpm run sync-docs (manual)

import fs from 'node:fs'
import path from 'node:path'
import { fileURLToPath } from 'node:url'

const __dirname = path.dirname(fileURLToPath(import.meta.url))
const SITE_DIR = path.resolve(__dirname, '..')
const REPO_ROOT = path.resolve(SITE_DIR, '..')
const SRC = path.join(REPO_ROOT, 'docs')
const DST = path.join(SITE_DIR, 'content')

if (!fs.existsSync(SRC)) {
  console.error(`[sync-docs] source not found: ${SRC}`)
  process.exit(1)
}

// Wipe and recreate target so deleted files in docs/ are reflected.
fs.rmSync(DST, { recursive: true, force: true })
fs.mkdirSync(DST, { recursive: true })

// Recursive copy (Node 16.7+ has fs.cpSync).
fs.cpSync(SRC, DST, { recursive: true, force: true })

// Count files for log.
let count = 0
function walk(dir) {
  for (const entry of fs.readdirSync(dir, { withFileTypes: true })) {
    const p = path.join(dir, entry.name)
    if (entry.isDirectory()) walk(p)
    else if (entry.isFile()) count++
  }
}
walk(DST)

console.log(`[sync-docs] ${count} files copied: ${path.relative(REPO_ROOT, SRC)} -> ${path.relative(REPO_ROOT, DST)}`)
