#!/usr/bin/env node
// 將 repo 根目錄的 docs/ 同步到 site/content/,讓 @nuxt/content 讀取。
// Sync repo-root docs/ to site/content/ so @nuxt/content can read them.
//
// 觸發時機 / Triggered by:
//   - npm run dev      (predev hook)
//   - npm run generate (pregenerate hook)
//   - npm run build    (prebuild hook)
//   - npm run sync-docs (manual)
//
// 注意: 不用 fs.cpSync —— Node 22.22.x 在 Windows + 含 CJK 字元的路徑下會
// 觸發 STATUS_ACCESS_VIOLATION (0xC0000005) 直接 crash process。
// NOTE: We avoid fs.cpSync because Node 22.22.x crashes (STATUS_ACCESS_VIOLATION)
// when copying recursively across paths containing CJK characters on Windows.
// See https://github.com/nodejs/node/issues (search "cpSync access violation").

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

// 1. Wipe target so deletions in docs/ propagate.
fs.rmSync(DST, { recursive: true, force: true })

// 2. Manual recursive copy (avoids fs.cpSync Windows + CJK bug).
let count = 0
function copyDir(srcDir, dstDir) {
  fs.mkdirSync(dstDir, { recursive: true })
  for (const entry of fs.readdirSync(srcDir, { withFileTypes: true })) {
    const srcPath = path.join(srcDir, entry.name)
    const dstPath = path.join(dstDir, entry.name)
    if (entry.isDirectory()) {
      copyDir(srcPath, dstPath)
    } else if (entry.isFile()) {
      fs.copyFileSync(srcPath, dstPath)
      count++
    }
    // 忽略 symlinks / 其他類型(本專案 docs/ 內只有 .md 檔)
  }
}

copyDir(SRC, DST)

console.log(
  `[sync-docs] ${count} files copied: ` +
  `${path.relative(REPO_ROOT, SRC)} -> ${path.relative(REPO_ROOT, DST)}`
)
