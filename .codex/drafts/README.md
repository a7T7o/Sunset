# Draft 沙盒说明

## 1. 用途
- 本目录是 Sunset 线程在 shared root 等待态、挂起态、或 `return-main` 后暂不适合污染 `main` 时使用的本地 Draft 沙盒。
- 这里允许存放：
  - 代码草稿
  - 伪代码
  - checkpoint 草案
  - 事后复盘草稿

## 2. Git 规则
- `.codex/drafts/` 下除本 README 外，默认全部被 `.gitignore` 忽略。
- 这意味着：
  - 线程可以在等待期大胆写草稿
  - 这些草稿不会进入 shared root 的 dirty 检查
  - 也不会误污染 `main`

## 3. 推荐目录结构
- 每条线程使用自己的子目录：
  - `.codex/drafts/<OwnerThread>/`
- 建议文件名包含时间戳和目标分支：
  - `20260319-193000-codex-farm-1.0.2-cleanroom001.md`

## 4. 使用边界
- Draft 不是正式证据，不替代：
  - tracked memory
  - 回执卡
  - requirements / design / tasks
  - 真实代码提交
- 一旦线程真正拿到 grant 并进入任务分支，只应把“本轮最小 checkpoint 所需内容”从 Draft 迁入正式落盘，而不是整包搬运。
- `return-main` 后如果队列仍有人等待，优先继续把复盘放在 Draft 或最小聊天回执中，避免再次把 `main` 写脏。

## 5. 一句话口径
- Draft 沙盒负责“先想好、先写草稿、不污染 Git”；正式证据和正式代码仍要在正确的时机、以最小白名单方式进入仓库。
