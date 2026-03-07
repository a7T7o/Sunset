# pre-bash-block 实测报告（2026-03-06）

## 1. 目标

验证 `.claude/hooks/pre-bash-block.sh` 在当前仓库里的**实际能力边界**，重点回答三件事：

1. 脚本单独执行时，到底会输出什么、返回什么。
2. 通过 Claude Code 当前 `.claude/settings.json` wiring 后，它在实际 Bash 调用前是否形成了“可观测阻断”。
3. 当前实现是否真的配得上“block”这个名字。

---

## 2. 被测对象

### 2.1 Hook 脚本

文件：`.claude/hooks/pre-bash-block.sh`

当前关键逻辑：
- 从 stdin 读取内容。
- 归一化空白。
- 用正则匹配少量危险模式：
  - `rm -rf /`
  - `git reset --hard`
  - `git push -f`
  - `git push --force`
- **无论是否命中，最终都返回 `exit 0`**。

### 2.2 Hook wiring

文件：`.claude/settings.json`

当前 `PreToolUse` 配置：

```json
{
  "matcher": "Bash(rm*|git reset --hard*|git push -f*)",
  "hooks": [
    {
      "type": "command",
      "command": ".claude/hooks/pre-bash-block.sh"
    }
  ]
}
```

注意：wiring 里显式写的是 `git push -f*`，**没有显式写出 `git push --force*`**。

---

## 3. 测试方法

本次只做**安全实测**，不执行任何真实破坏动作。

### 3.1 单脚本实测

直接把字符串通过 stdin 喂给 `.claude/hooks/pre-bash-block.sh`，观察：
- stdout
- exit code

### 3.2 Runner 侧安全探针

通过 Claude Code 的 Bash 工具执行**无破坏**命令，观察是否出现：
- 明显阻断
- 用户可见 hook 输出
- 命令被拒绝执行

本次安全探针选用：
- `rm --version`
- `git push -f -h`
- `git push --force -h`

这些命令都不会删除文件，也不会真的 push。

### 3.3 静态对照

把：
- `.claude/settings.json` 的 matcher
- `.claude/hooks/pre-bash-block.sh` 的脚本正则

放在一起看，判断覆盖范围是否一致。

---

## 4. 单脚本实测结果

### 4.1 结果表

| 输入 | 输出 | 退出码 |
|------|------|--------|
| 空 stdin | `[hook][audit][pre-bash] no dangerous pattern matched` | `0` |
| `rm --version` | `[hook][audit][pre-bash] no dangerous pattern matched` | `0` |
| `rm -rf /` | `[hook][audit][pre-bash] matched dangerous shell pattern` | `0` |
| `git reset --hard` | `[hook][audit][pre-bash] matched dangerous shell pattern` | `0` |
| `git push -f origin main` | `[hook][audit][pre-bash] matched dangerous shell pattern` | `0` |
| `git push --force origin main` | `[hook][audit][pre-bash] matched dangerous shell pattern` | `0` |

### 4.2 直接结论

单看脚本本身，它现在是一个：
- **危险模式分类器 / 审计提示器**，不是阻断器。

原因非常直接：
- 命中危险模式后输出提示；
- 但紧接着 `exit 0`；
- 没有任何非零退出码；
- 因此从 shell 语义上看，它从不主动报错。

---

## 5. Runner 侧安全探针结果

### 5.1 `rm --version`

现象：
- 命令成功执行。
- 返回正常版本信息。
- 本次 Bash 输出中**没有看到任何可见的 hook 提示文本**。

### 5.2 `git push -f -h`

现象：
- 命令成功执行。
- 返回 `git push` 帮助信息。
- 本次 Bash 输出中**没有看到任何可见的 hook 提示文本**。

### 5.3 `git push --force -h`

现象：
- 命令成功执行。
- 返回 `git push` 帮助信息。
- 本次 Bash 输出中**没有看到任何可见的 hook 提示文本**。

### 5.4 Runner 侧可得出的最强结论

当前这组安全探针能够证明：

1. **没有观察到任何“可见阻断”**。
2. **没有观察到任何用户可见的 pre-bash hook 输出**。
3. 对 `git push -f -h` 而言：
   - 要么 hook 被触发了，但因为脚本 `exit 0` 所以放行；
   - 要么 matcher/runner 没触发。
   - 无论哪一种，结果都一样：**没有阻断效果**。
4. 对 `git push --force -h` 而言：
   - 命令同样成功执行；
   - 且 settings matcher 没显式覆盖 `--force`；
   - 因此当前 wiring 存在明显覆盖疑点。

---

## 6. 脚本与 wiring 的静态对照

### 6.1 脚本能识别的内容

脚本正则覆盖：
- `rm -rf /`
- `git reset --hard`
- `git push -f`
- `git push --force`

### 6.2 settings wiring 声明的触发条件

`PreToolUse.matcher` 当前写的是：
- `rm*`
- `git reset --hard*`
- `git push -f*`

### 6.3 发现的关键不一致

**不一致点：脚本比 wiring 更宽，wiring 比脚本更窄。**

最典型的例子：
- 脚本本身会识别 `git push --force`
- 但 settings matcher 里没有明确写 `git push --force*`

这意味着：
- 即使脚本内部有 `--force` 检测逻辑，
- **只要 runner 没有因为 matcher 命中而调用脚本，这段逻辑就根本没有机会生效。**

结论：
- 当前实现不仅“没有硬阻断”；
- 还存在**wiring 覆盖面与脚本覆盖面不一致**的问题。

---

## 7. 最终判断

### 7.1 这个 hook 现在到底是什么

更准确的命名应该是：
- `pre-bash danger audit marker`
- 或 `pre-bash best-effort detector`

而不是严格意义上的 `block`。

### 7.2 为什么它现在不能叫 blocker

因为阻断至少要满足以下一条：
- 命中后返回非零退出码；或
- 通过 runner 约定的结构化输出显式 block。

当前版本两条都没有证据成立。

### 7.3 当前最稳的现实结论

**`pre-bash-block.sh` 当前不是“可靠阻断层”，而只是“最好努力的危险模式审计层”。**

这和前面治理文档里的定位是一致的：
- 它最多只能算辅助层；
- 不能替代权限最小化；
- 不能替代人工确认；
- 不能替代 `CLAUDE.md` 的危险操作纪律。

---

## 8. 剩余不确定点

本次实测后，仍有 3 个点不能仅靠当前证据彻底钉死：

1. **Claude Code 的 hook runner 对 stdout/stderr 的展示规则**
   - 本次 Bash 输出中没有看到 hook 文本；
   - 但这不等于 hook 一定没触发，也可能只是没有透传给当前显示层。

2. **Claude Code 的 `PreToolUse.matcher` 精确语法**
   - 当前只能确认字符串写法本身存在 `--force` 覆盖缺口；
   - 但 runner 对 `Bash(...)` 的精确匹配语义，仍需要官方文档或专门的受控对照实验补锚。

3. **Claude Code 在 `PreToolUse` 中真正的 block 判定协议**
   - 本次未改脚本；
   - 所以没有做“命中后返回非零退出码”的受控实测。

---

## 9. 当前可落地结论

本轮可以正式落盘的结论有四条：

1. `.claude/hooks/pre-bash-block.sh` **单独执行时永远返回 0**。
2. 因此它在脚本层面**不具备自证的阻断能力**。
3. 当前 `.claude/settings.json` 的 matcher 与脚本内部检测范围**不一致**，尤其是 `git push --force` 存在 wiring 覆盖疑点。
4. 所以该 hook 的现实定位应继续维持为：
   - **best-effort audit / reminder**
   - **不是可靠 blocker**

---

## 10. 如需继续下一轮验证，正确方向是什么

如果后续要把“不确定”进一步收紧，正确方向不是继续脑补，而是做**受控对照实验**：

1. 在隔离环境里准备一个临时 hook 变体；
2. 命中模式后显式返回非零退出码；
3. 观察 Claude runner 是否真的拦截工具调用；
4. 再单独验证 `Bash(git push -f*)` 与 `Bash(git push --force*)` 的触发差异。

在这一步之前，任何“它已经能可靠阻断 Bash 危险命令”的表述都不严谨。

---

## 11. 补充说明（非本仓库主证据）

本次还检索了社区资料与相关 issue 线索，得到一个**倾向性补充**：
- Claude Code 社区资料普遍把 `PreToolUse` 描述为：通过 stdin 接收工具信息，并可通过特定退出码阻断。

但由于本轮主目标是**仓库内实测报告**，所以本报告的主结论仍然只以：
- 当前脚本源码
- 当前 settings wiring
- 当前安全实测输出

这三类证据为准，不把外部资料当成主锤。

---

## 12. 二次复测补记（修补后）

### 12.1 已做修补

- `.claude/settings.json` 的 `PreToolUse.matcher` 已显式补齐 `git push --force*`。
- `.claude/hooks/pre-bash-block.sh` 已改为命中危险模式时输出 block 提示并 `exit 2`。
- `.claude/settings.local.json` 已移除 `EnterPlanMode`，本仓库继续停留在 `acceptEdits` 工作方式。

### 12.2 单脚本复测

| 输入 | 输出 | 退出码 |
|------|------|--------|
| `rm --version` | 无输出 | `0` |
| `git push -f origin main` | `[hook][block][pre-bash] matched dangerous shell pattern` | `2` |
| `git push --force origin main` | `[hook][block][pre-bash] matched dangerous shell pattern` | `2` |

### 12.3 Runner 侧补充探针

- 重新执行 `git push -f -h`、`git push --force -h`、`rm --version`，结果仍然只是各自原始命令输出，没有可见 hook 文本。
- 为排除“输出被隐藏但 hook 仍执行”的可能，曾临时给 `pre-bash-block.sh` 加入 probe log 写入；安全探针结束后 `.claude/hooks/pre-bash-probe.log` 仍不存在。
- 这说明：**截至本轮，仍然没有证据证明当前 Bash runner 实际调用了仓库内的 `PreToolUse` hook。**

### 12.4 更新后的最稳结论

1. **脚本层面**：`pre-bash-block.sh` 现在已经具备“命中即非零退出”的 gate 语义。
2. **配置层面**：`git push -f*` 与 `git push --force*` 现已在 matcher 中显式并列。
3. **runner 层面**：仍未证实 repo hook 被调用，因此不能宣称 Bash 已被可靠阻断。
4. **治理结论**：`pre-bash-block.sh` 仍只能被定位为 best-effort 防呆层；真正主防线仍是权限配置、`CLAUDE.md` 与人工纪律。

---

## 13. 三次复测补记（runner 真实语义已证实）

### 13.1 外部证据补锚

在继续做 runner 实测前，补查了 Claude Code 文档与社区/issue 线索，得到两条关键结论：

1. `PreToolUse` 的可靠阻断语义是 **hook 返回 `exit 2`**。
2. `matcher` 当前**不支持** `Bash(rm*|git push -f*)` 这种“按 Bash 参数做模式匹配”的写法；更稳的做法是直接把 matcher 写成 `Bash`，再在 hook 脚本内部自行做命令过滤。

因此本轮先把 `.claude/settings.json` 的 `PreToolUse.matcher` 改为：

```json
"matcher": "Bash"
```

再继续做 runner 侧安全探针。

### 13.2 受控实测方法

为避免“hook 可能被调用但输出被隐藏”这种歧义，本轮采用了双证据法：

1. 临时给 `.claude/hooks/pre-bash-block.sh` 加入 probe log 写入；
2. 用 Claude Code Bash 工具分别执行 3 个安全命令：
   - `git push -f -h`
   - `git push --force -h`
   - `rm --version`
3. 观察：
   - Bash 工具是否直接返回 `PreToolUse:Bash hook error`
   - probe log 是否实际写入
4. 额外再执行 `git push origin HEAD`，验证“非危险 Bash 命令在 matcher=\"Bash\" 情况下是否会被脚本正常放行”。

### 13.3 实测结果

#### A. `git push -f -h`
- Bash 工具未返回 git help，而是直接返回：

```text
PreToolUse:Bash hook error: [.claude/hooks/pre-bash-block.sh]: [hook][block][pre-bash] matched dangerous shell pattern
```

#### B. `git push --force -h`
- 结果与 `git push -f -h` 一致，同样直接被 `PreToolUse` 拦截并返回 hook error。

#### C. `rm --version`
- 命令正常执行，返回 GNU rm 版本信息。
- 说明：在 `matcher: "Bash"` 条件下，runner 确实调用了 hook，但具体是否放行仍由脚本内部过滤逻辑决定。

#### D. probe log
- `.claude/hooks/pre-bash-probe.log` 成功生成，共记录 3 次调用：
  - 一次对应 `git push -f -h`
  - 一次对应 `git push --force -h`
  - 一次对应 `rm --version`
- 这证明：**当前 Claude runner 确实调用了仓库内的 `PreToolUse` hook。**

#### E. `git push origin HEAD`
- 返回：`Everything up-to-date`
- 说明：在 `matcher: "Bash"` 的配置下，普通 `git push` 会进入 hook，但由于脚本未匹配到危险模式，因此被正常放行。

### 13.4 更新后的结论

1. **runner 调用已证实**：当 `.claude/settings.json` 使用 `"matcher": "Bash"` 时，当前 Claude Code Bash runner 会实际调用仓库内的 `PreToolUse` hook。
2. **阻断语义已证实**：`pre-bash-block.sh` 返回 `exit 2` 时，Bash 工具会直接返回 `PreToolUse:Bash hook error`，原命令不会继续执行。
3. **旧 matcher 写法不可靠**：`Bash(rm*|git reset --hard*|git push -f*|git push --force*)` 这类参数模式写法不能再当成可靠 wiring；真正可靠的做法是 `matcher: "Bash"` + 脚本内过滤。
4. **脚本/runner 分层现在终于钉死**：
   - `settings.json` 负责把所有 Bash 调用送进 hook；
   - `pre-bash-block.sh` 负责判断哪些 Bash 命令应被阻断；
   - `exit 2` 负责把“脚本命中危险模式”真正转成 runner 级阻断。
5. **仍未直接证实的只剩 Stop**：`stop-update-memory.sh` 的 standalone 检测能力已确认，但 `Stop` 生命周期对 `exit 2` 的真实阻断语义，仍未做 runner 级直接实测。

