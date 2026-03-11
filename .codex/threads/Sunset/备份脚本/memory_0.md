## 2026-03-11
- 用户目标：参考 `D:\Unity\Unity_learning\Sync_Sunset.ps1`，制作一个只针对 `Sunset` 的完整备份脚本，备份目录落在 `D:\Unity\Unity_learning\000_Copy`，命名格式为 `Sunset_YYYY.MM.DD_01`，脚本名为 `Sunset_CopyTool.ps1`。
- 当前主线目标：交付一个可直接运行的 `Sunset` 全量快照备份脚本。
- 本轮子任务或阻塞：验证完整备份时发现 Unity 正占用 `Temp\UnityLockfile` 和部分 `Temp` 临时文件，导致运行中的项目无法做到“不遗漏任何内容”。
- 已完成事项：新增 `D:\Unity\Unity_learning\Sunset_CopyTool.ps1`；实现源目录和目标根目录固定配置；实现按日期与两位编号自动生成备份目录；使用 `robocopy /E /COPY:DAT /DCOPY:DAT /MT:16` 做全量复制；失败时自动清理半成品目录；新增 `UnityLockfile` 独占检测；新增按项目路径反查 `Unity.exe` 进程并输出 PID。
- 关键决策：不沿用 `Sync_Sunset.ps1` 的筛选同步逻辑，而改成整库快照；不使用 `/COPYALL`，因为当前环境缺少审计权限；不使用“跳过锁文件继续备份”，因为用户要求“全面备份、不遗漏任何内容”，因此改为“项目占用时拒绝备份”。
- 涉及文件或路径：`D:\Unity\Unity_learning\Sunset_CopyTool.ps1`、`D:\Unity\Unity_learning\Sync_Sunset.ps1`、`D:\Unity\Unity_learning\000_Copy`、`D:\Unity\Unity_learning\Sunset\Temp\UnityLockfile`。
- 验证结果：脚本命名与编号逻辑已验证；当前环境下能稳定输出 `Sunset is currently in use by Unity.exe (PID 47256). Close that project and run the backup again.`；由于 Unity 未关闭，本轮尚未完成一次“零遗漏”的最终备份快照。
- 恢复点或下一步：关闭 PID 47256 对应的 `Sunset` Unity 项目后，再执行 `powershell -ExecutionPolicy Bypass -File D:\Unity\Unity_learning\Sunset_CopyTool.ps1`，即可生成新的完整备份目录。
