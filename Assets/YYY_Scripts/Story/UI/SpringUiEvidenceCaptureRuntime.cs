using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Sunset.Story
{
    [DisallowMultipleComponent]
    public class SpringUiEvidenceCaptureRuntime : MonoBehaviour
    {
        private const string ArtifactRootRelative = ".codex/artifacts/ui-captures/spring-ui";
        private const string PendingDirectoryName = "pending";
        private const string AcceptedDirectoryName = "accepted";
        private const string LatestFileName = "latest.json";
        private const string ManifestFileName = "manifest.jsonl";
        private const string CaptureSource = "ScreenCapture.CaptureScreenshotAsTexture@WaitForEndOfFrame";

        private static SpringUiEvidenceCaptureRuntime _instance;
        private bool _captureInProgress;

        public static SpringUiEvidenceCaptureRuntime EnsureRuntime()
        {
            if (_instance != null)
            {
                return _instance;
            }

            _instance = FindFirstObjectByType<SpringUiEvidenceCaptureRuntime>(FindObjectsInactive.Include);
            if (_instance != null)
            {
                return _instance;
            }

            GameObject runtimeObject = new GameObject(nameof(SpringUiEvidenceCaptureRuntime));
            runtimeObject.hideFlags = HideFlags.HideAndDontSave;
            _instance = runtimeObject.AddComponent<SpringUiEvidenceCaptureRuntime>();
            return _instance;
        }

        public static string CaptureCurrentSpringUiEvidence(string reason = null)
        {
            if (!Application.isPlaying)
            {
                return "[SpringUiEvidence] 请先进入 PlayMode，再抓取最终合成屏。";
            }

            SpringUiEvidenceCaptureRuntime runtime = EnsureRuntime();
            if (runtime._captureInProgress)
            {
                return "[SpringUiEvidence] 当前已有一轮抓取在进行中，请稍后再试。";
            }

            runtime.StartCoroutine(runtime.CaptureAtEndOfFrame(string.IsNullOrWhiteSpace(reason) ? "manual" : reason));
            return "[SpringUiEvidence] 已排队到当前帧结束后抓取。";
        }

        public static string PromoteLatestCaptureToAccepted(string actor = "menu")
        {
            EnsureArtifactDirectories();

            LatestPointer pointer = ReadLatestPointer();
            if (pointer == null)
            {
                return "[SpringUiEvidence] 未找到 latest.json，暂无可提升证据。";
            }

            if (string.Equals(pointer.lifecycle, "accepted", StringComparison.OrdinalIgnoreCase))
            {
                return $"[SpringUiEvidence] 最新证据已经在 accepted：{pointer.imagePath}";
            }

            string repoRoot = GetRepositoryRoot();
            string sourcePng = ToAbsolutePath(repoRoot, pointer.imagePath);
            string sourceJson = ToAbsolutePath(repoRoot, pointer.jsonPath);
            if (!File.Exists(sourcePng) || !File.Exists(sourceJson))
            {
                return "[SpringUiEvidence] latest 指向的文件不存在，无法提升。";
            }

            string acceptedDirectory = Path.Combine(GetArtifactRootPath(repoRoot), AcceptedDirectoryName);
            Directory.CreateDirectory(acceptedDirectory);

            string targetPng = Path.Combine(acceptedDirectory, Path.GetFileName(sourcePng));
            string targetJson = Path.Combine(acceptedDirectory, Path.GetFileName(sourceJson));

            if (File.Exists(targetPng))
            {
                File.Delete(targetPng);
            }

            if (File.Exists(targetJson))
            {
                File.Delete(targetJson);
            }

            File.Move(sourcePng, targetPng);
            File.Move(sourceJson, targetJson);

            CaptureMetadata metadata = ReadJson<CaptureMetadata>(targetJson);
            if (metadata != null)
            {
                metadata.lifecycle = "accepted";
                metadata.promotedAtUtc = DateTime.UtcNow.ToString("O");
                metadata.imagePath = ToRepoRelativePath(repoRoot, targetPng);
                metadata.jsonPath = ToRepoRelativePath(repoRoot, targetJson);
                WriteJson(targetJson, metadata);
            }

            pointer.lifecycle = "accepted";
            pointer.imagePath = ToRepoRelativePath(repoRoot, targetPng);
            pointer.jsonPath = ToRepoRelativePath(repoRoot, targetJson);
            pointer.promotedAtUtc = DateTime.UtcNow.ToString("O");
            WriteJson(Path.Combine(GetArtifactRootPath(repoRoot), LatestFileName), pointer);

            AppendManifest(new ManifestEntry
            {
                timestampUtc = DateTime.UtcNow.ToString("O"),
                action = "promote",
                actor = actor,
                captureId = pointer.captureId,
                lifecycle = pointer.lifecycle,
                imagePath = pointer.imagePath,
                jsonPath = pointer.jsonPath
            });

            return $"[SpringUiEvidence] 已提升到 accepted：{pointer.imagePath}";
        }

        public static string PrunePendingCaptures(int retentionDays = 14, bool dryRun = false, string actor = "menu")
        {
            EnsureArtifactDirectories();

            string repoRoot = GetRepositoryRoot();
            string pendingDirectory = Path.Combine(GetArtifactRootPath(repoRoot), PendingDirectoryName);
            Directory.CreateDirectory(pendingDirectory);

            DateTime cutoffUtc = DateTime.UtcNow.AddDays(-Mathf.Max(1, retentionDays));
            int deletedCount = 0;
            List<string> deletedFiles = new List<string>();

            foreach (string filePath in Directory.GetFiles(pendingDirectory))
            {
                DateTime lastWriteUtc = File.GetLastWriteTimeUtc(filePath);
                if (lastWriteUtc >= cutoffUtc)
                {
                    continue;
                }

                deletedCount++;
                deletedFiles.Add(ToRepoRelativePath(repoRoot, filePath));
                if (!dryRun)
                {
                    File.Delete(filePath);
                }
            }

            AppendManifest(new ManifestEntry
            {
                timestampUtc = DateTime.UtcNow.ToString("O"),
                action = "prune",
                actor = actor,
                lifecycle = "pending",
                deletedCount = deletedCount,
                dryRun = dryRun,
                retentionDays = retentionDays,
                deletedFiles = deletedFiles.ToArray()
            });

            return dryRun
                ? $"[SpringUiEvidence] dry-run 完成，预计清理 {deletedCount} 个 pending 文件。"
                : $"[SpringUiEvidence] 已清理 {deletedCount} 个过期 pending 文件。";
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private IEnumerator CaptureAtEndOfFrame(string reason)
        {
            _captureInProgress = true;

            try
            {
                yield return new WaitForEndOfFrame();

                string repoRoot = GetRepositoryRoot();
                EnsureArtifactDirectories();

                Texture2D screenshot = ScreenCapture.CaptureScreenshotAsTexture();
                if (screenshot == null)
                {
                    Debug.LogError("[SpringUiEvidence] ScreenCapture 返回空纹理，抓取失败。");
                    yield break;
                }

                string captureId = $"{DateTime.Now:yyyyMMdd-HHmmss-fff}_{SanitizeForFileName(reason)}";
                string pendingDirectory = Path.Combine(GetArtifactRootPath(repoRoot), PendingDirectoryName);
                string pngPath = Path.Combine(pendingDirectory, captureId + ".png");
                string jsonPath = Path.Combine(pendingDirectory, captureId + ".json");

                byte[] pngBytes = screenshot.EncodeToPNG();
                Destroy(screenshot);

                File.WriteAllBytes(pngPath, pngBytes);

                CaptureMetadata metadata = BuildMetadata(repoRoot, captureId, reason, pngPath, jsonPath);
                WriteJson(jsonPath, metadata);

                LatestPointer pointer = new LatestPointer
                {
                    captureId = metadata.captureId,
                    reason = metadata.reason,
                    lifecycle = metadata.lifecycle,
                    timestampUtc = metadata.timestampUtc,
                    imagePath = metadata.imagePath,
                    jsonPath = metadata.jsonPath
                };
                WriteJson(Path.Combine(GetArtifactRootPath(repoRoot), LatestFileName), pointer);

                AppendManifest(new ManifestEntry
                {
                    timestampUtc = metadata.timestampUtc,
                    action = "capture",
                    actor = "runtime",
                    captureId = metadata.captureId,
                    reason = metadata.reason,
                    lifecycle = metadata.lifecycle,
                    imagePath = metadata.imagePath,
                    jsonPath = metadata.jsonPath
                });

                Debug.Log($"[SpringUiEvidence] 已生成 capture：{metadata.imagePath}");
            }
            finally
            {
                _captureInProgress = false;
            }
        }

        private static CaptureMetadata BuildMetadata(string repoRoot, string captureId, string reason, string pngPath, string jsonPath)
        {
            SpringDay1PromptOverlay promptOverlay = FindFirstObjectByType<SpringDay1PromptOverlay>(FindObjectsInactive.Include);
            SpringDay1WorkbenchCraftingOverlay workbenchOverlay = FindFirstObjectByType<SpringDay1WorkbenchCraftingOverlay>(FindObjectsInactive.Include);

            CaptureMetadata metadata = new CaptureMetadata
            {
                captureId = captureId,
                reason = reason,
                lifecycle = "pending",
                timestampUtc = DateTime.UtcNow.ToString("O"),
                timestampLocal = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                scene = SceneManager.GetActiveScene().name,
                frameCount = Time.frameCount,
                screenWidth = Screen.width,
                screenHeight = Screen.height,
                captureSource = CaptureSource,
                imagePath = ToRepoRelativePath(repoRoot, pngPath),
                jsonPath = ToRepoRelativePath(repoRoot, jsonPath),
                runtimeValidationSnapshot = BuildRuntimeValidationSnapshot(),
                prompt = BuildPromptSnapshot(promptOverlay),
                workbench = BuildWorkbenchSnapshot(workbenchOverlay)
            };

            return metadata;
        }

        private static string BuildRuntimeValidationSnapshot()
        {
            SpringDay1LiveValidationRunner runner = FindFirstObjectByType<SpringDay1LiveValidationRunner>(FindObjectsInactive.Include);
            if (runner == null)
            {
                return "n/a";
            }

            try
            {
                return runner.BuildSnapshot("ui-evidence");
            }
            catch (Exception exception)
            {
                return $"snapshot-failed:{exception.GetType().Name}";
            }
        }

        private static PromptSnapshot BuildPromptSnapshot(SpringDay1PromptOverlay overlay)
        {
            if (overlay == null)
            {
                return new PromptSnapshot { exists = false };
            }

            RectTransform rootRect = overlay.transform as RectTransform;
            RectTransform cardRect = FindDirectChildRect(rootRect, "TaskCardRoot");
            RectTransform frontPage = cardRect != null ? FindDirectChildRect(cardRect, "Page") : null;
            RectTransform backPage = cardRect != null ? FindDirectChildRect(cardRect, "BackPage") : null;
            Canvas canvas = overlay.GetComponent<Canvas>();
            CanvasGroup canvasGroup = overlay.GetComponent<CanvasGroup>();

            return new PromptSnapshot
            {
                exists = true,
                objectPath = BuildHierarchyPath(overlay.transform),
                activeInHierarchy = overlay.gameObject.activeInHierarchy,
                renderMode = canvas != null ? canvas.renderMode.ToString() : "n/a",
                canvasAlpha = canvasGroup != null ? canvasGroup.alpha : -1f,
                rootRect = CaptureRect(rootRect),
                cardRect = CaptureRect(cardRect),
                pageRect = CaptureRect(frontPage),
                backPageRect = CaptureRect(backPage),
                title = CaptureText(FindDescendantComponent<TextMeshProUGUI>(overlay.transform, "TitleText")),
                subtitle = CaptureText(FindDescendantComponent<TextMeshProUGUI>(overlay.transform, "SubtitleText")),
                focus = CaptureText(FindDescendantComponent<TextMeshProUGUI>(overlay.transform, "FocusText")),
                footer = CaptureText(FindDescendantComponent<TextMeshProUGUI>(overlay.transform, "FooterText")),
                taskRowCount = CountRows(frontPage),
                backPageRowCount = CountRows(backPage)
            };
        }

        private static WorkbenchSnapshot BuildWorkbenchSnapshot(SpringDay1WorkbenchCraftingOverlay overlay)
        {
            if (overlay == null)
            {
                return new WorkbenchSnapshot { exists = false };
            }

            RectTransform rootRect = overlay.transform as RectTransform;
            RectTransform panelRect = FindDirectChildRect(rootRect, "PanelRoot");
            RectTransform recipeColumn = panelRect != null ? FindDescendantRect(panelRect, "RecipeColumn") : null;
            RectTransform detailColumn = panelRect != null ? FindDescendantRect(panelRect, "DetailColumn") : null;
            RectTransform recipeViewport = recipeColumn != null ? FindDescendantRect(recipeColumn, "Viewport") : null;
            RectTransform recipeContent = recipeViewport != null ? FindDirectChildRect(recipeViewport, "Content") : null;
            RectTransform materialsViewport = detailColumn != null ? FindDescendantRect(detailColumn, "MaterialsViewport") : null;
            RectTransform materialsContent = materialsViewport != null ? FindDirectChildRect(materialsViewport, "Content") : null;
            RectTransform progressRoot = panelRect != null ? FindDescendantRect(panelRect, "ProgressBackground") : null;
            RectTransform floatingRoot = FindDirectChildRect(rootRect, "FloatingProgressRoot");
            Button craftButton = panelRect != null ? FindDescendantComponent<Button>(panelRect, "CraftButton") : null;
            Canvas canvas = overlay.GetComponent<Canvas>();
            CanvasGroup canvasGroup = overlay.GetComponent<CanvasGroup>();

            return new WorkbenchSnapshot
            {
                exists = true,
                objectPath = BuildHierarchyPath(overlay.transform),
                activeInHierarchy = overlay.gameObject.activeInHierarchy,
                renderMode = canvas != null ? canvas.renderMode.ToString() : "n/a",
                canvasAlpha = canvasGroup != null ? canvasGroup.alpha : -1f,
                rootRect = CaptureRect(rootRect),
                panelRect = CaptureRect(panelRect),
                recipeViewportRect = CaptureRect(recipeViewport),
                recipeContentRect = CaptureRect(recipeContent),
                materialsViewportRect = CaptureRect(materialsViewport),
                materialsContentRect = CaptureRect(materialsContent),
                progressRootRect = CaptureRect(progressRoot),
                floatingProgressRect = CaptureRect(floatingRoot),
                craftButtonRect = CaptureRect(craftButton != null ? craftButton.GetComponent<RectTransform>() : null),
                recipeRowCount = recipeContent != null ? recipeContent.childCount : 0,
                selectedName = CaptureText(FindDescendantComponent<TextMeshProUGUI>(panelRect, "SelectedName")),
                selectedDescription = CaptureText(FindDescendantComponent<TextMeshProUGUI>(panelRect, "SelectedDescription")),
                selectedMaterials = CaptureText(FindDescendantComponent<TextMeshProUGUI>(panelRect, "SelectedMaterials")),
                stageHint = CaptureText(FindDescendantComponent<TextMeshProUGUI>(panelRect, "StageHint")),
                progressLabel = CaptureText(FindDescendantComponent<TextMeshProUGUI>(panelRect, "ProgressLabel")),
                quantityValue = CaptureText(FindDescendantComponent<TextMeshProUGUI>(panelRect, "QuantityValue")),
                floatingLabel = CaptureText(FindDescendantComponent<TextMeshProUGUI>(floatingRoot, "Label")),
                floatingFillAmount = CaptureImageFill(FindDescendantComponent<Image>(floatingRoot, "ProgressFill")),
                progressFillAmount = CaptureImageFill(FindDescendantComponent<Image>(progressRoot, "ProgressFill")),
                craftButtonVisible = craftButton != null && craftButton.gameObject.activeInHierarchy,
                craftButtonInteractable = craftButton != null && craftButton.interactable
            };
        }

        private static float CaptureImageFill(Image image)
        {
            return image != null ? image.fillAmount : -1f;
        }

        private static TextSnapshot CaptureText(TextMeshProUGUI text)
        {
            if (text == null)
            {
                return new TextSnapshot { exists = false };
            }

            return new TextSnapshot
            {
                exists = true,
                text = text.text,
                font = text.font != null ? text.font.name : "n/a",
                fontSize = text.fontSize,
                preferredHeight = text.preferredHeight,
                rect = CaptureRect(text.rectTransform)
            };
        }

        private static RectSnapshot CaptureRect(RectTransform rect)
        {
            if (rect == null)
            {
                return new RectSnapshot { exists = false };
            }

            return new RectSnapshot
            {
                exists = true,
                path = BuildHierarchyPath(rect),
                activeSelf = rect.gameObject.activeSelf,
                activeInHierarchy = rect.gameObject.activeInHierarchy,
                anchoredPosition = rect.anchoredPosition,
                sizeDelta = rect.sizeDelta,
                rectSize = rect.rect.size,
                anchorMin = rect.anchorMin,
                anchorMax = rect.anchorMax,
                pivot = rect.pivot,
                localScale = rect.localScale,
                localEulerAngles = rect.localEulerAngles
            };
        }

        private static int CountRows(RectTransform pageRoot)
        {
            if (pageRoot == null)
            {
                return 0;
            }

            int count = 0;
            foreach (Transform child in pageRoot.GetComponentsInChildren<Transform>(true))
            {
                if (child != pageRoot && child.name.StartsWith("TaskRow_", StringComparison.Ordinal))
                {
                    count++;
                }
            }

            return count;
        }

        private static T FindDescendantComponent<T>(Transform root, string name) where T : Component
        {
            Transform transform = FindDescendant(root, name);
            return transform != null ? transform.GetComponent<T>() : null;
        }

        private static RectTransform FindDescendantRect(Transform root, string name)
        {
            Transform transform = FindDescendant(root, name);
            return transform as RectTransform;
        }

        private static RectTransform FindDirectChildRect(Transform parent, string name)
        {
            if (parent == null)
            {
                return null;
            }

            for (int index = 0; index < parent.childCount; index++)
            {
                Transform child = parent.GetChild(index);
                if (string.Equals(child.name, name, StringComparison.Ordinal))
                {
                    return child as RectTransform;
                }
            }

            return null;
        }

        private static Transform FindDescendant(Transform root, string name)
        {
            if (root == null)
            {
                return null;
            }

            Queue<Transform> queue = new Queue<Transform>();
            queue.Enqueue(root);
            while (queue.Count > 0)
            {
                Transform current = queue.Dequeue();
                if (string.Equals(current.name, name, StringComparison.Ordinal))
                {
                    return current;
                }

                for (int index = 0; index < current.childCount; index++)
                {
                    queue.Enqueue(current.GetChild(index));
                }
            }

            return null;
        }

        private static string BuildHierarchyPath(Transform transform)
        {
            if (transform == null)
            {
                return string.Empty;
            }

            Stack<string> names = new Stack<string>();
            Transform cursor = transform;
            while (cursor != null)
            {
                names.Push(cursor.name);
                cursor = cursor.parent;
            }

            return string.Join("/", names.ToArray());
        }

        private static void EnsureArtifactDirectories()
        {
            string repoRoot = GetRepositoryRoot();
            string artifactRoot = GetArtifactRootPath(repoRoot);
            Directory.CreateDirectory(artifactRoot);
            Directory.CreateDirectory(Path.Combine(artifactRoot, PendingDirectoryName));
            Directory.CreateDirectory(Path.Combine(artifactRoot, AcceptedDirectoryName));
        }

        private static string GetArtifactRootPath(string repoRoot = null)
        {
            string resolvedRoot = string.IsNullOrWhiteSpace(repoRoot) ? GetRepositoryRoot() : repoRoot;
            return Path.Combine(resolvedRoot, ArtifactRootRelative.Replace('/', Path.DirectorySeparatorChar));
        }

        private static string GetRepositoryRoot()
        {
            return Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        }

        private static string ToRepoRelativePath(string repoRoot, string absolutePath)
        {
            string relative = Path.GetRelativePath(repoRoot, absolutePath);
            return relative.Replace('\\', '/');
        }

        private static string ToAbsolutePath(string repoRoot, string relativePath)
        {
            string normalized = relativePath.Replace('/', Path.DirectorySeparatorChar);
            return Path.GetFullPath(Path.Combine(repoRoot, normalized));
        }

        private static void WriteJson<T>(string path, T data)
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(path, json);
        }

        private static T ReadJson<T>(string path) where T : class
        {
            if (!File.Exists(path))
            {
                return null;
            }

            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<T>(json);
        }

        private static LatestPointer ReadLatestPointer()
        {
            string latestPath = Path.Combine(GetArtifactRootPath(), LatestFileName);
            return ReadJson<LatestPointer>(latestPath);
        }

        private static void AppendManifest(ManifestEntry entry)
        {
            string manifestPath = Path.Combine(GetArtifactRootPath(), ManifestFileName);
            string line = JsonUtility.ToJson(entry);
            File.AppendAllText(manifestPath, line + Environment.NewLine);
        }

        private static string SanitizeForFileName(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                return "manual";
            }

            char[] invalidChars = Path.GetInvalidFileNameChars();
            char[] buffer = raw.Trim().ToCharArray();
            for (int index = 0; index < buffer.Length; index++)
            {
                char current = buffer[index];
                if (Array.IndexOf(invalidChars, current) >= 0 || char.IsWhiteSpace(current))
                {
                    buffer[index] = '-';
                }
            }

            string sanitized = new string(buffer).Trim('-');
            return string.IsNullOrWhiteSpace(sanitized) ? "manual" : sanitized;
        }

        [Serializable]
        private sealed class CaptureMetadata
        {
            public string captureId;
            public string reason;
            public string lifecycle;
            public string timestampUtc;
            public string timestampLocal;
            public string promotedAtUtc;
            public string scene;
            public int frameCount;
            public int screenWidth;
            public int screenHeight;
            public string captureSource;
            public string imagePath;
            public string jsonPath;
            public string runtimeValidationSnapshot;
            public PromptSnapshot prompt;
            public WorkbenchSnapshot workbench;
        }

        [Serializable]
        private sealed class LatestPointer
        {
            public string captureId;
            public string reason;
            public string lifecycle;
            public string timestampUtc;
            public string promotedAtUtc;
            public string imagePath;
            public string jsonPath;
        }

        [Serializable]
        private sealed class ManifestEntry
        {
            public string timestampUtc;
            public string action;
            public string actor;
            public string captureId;
            public string reason;
            public string lifecycle;
            public string imagePath;
            public string jsonPath;
            public int deletedCount;
            public bool dryRun;
            public int retentionDays;
            public string[] deletedFiles;
        }

        [Serializable]
        private sealed class PromptSnapshot
        {
            public bool exists;
            public string objectPath;
            public bool activeInHierarchy;
            public string renderMode;
            public float canvasAlpha;
            public RectSnapshot rootRect;
            public RectSnapshot cardRect;
            public RectSnapshot pageRect;
            public RectSnapshot backPageRect;
            public TextSnapshot title;
            public TextSnapshot subtitle;
            public TextSnapshot focus;
            public TextSnapshot footer;
            public int taskRowCount;
            public int backPageRowCount;
        }

        [Serializable]
        private sealed class WorkbenchSnapshot
        {
            public bool exists;
            public string objectPath;
            public bool activeInHierarchy;
            public string renderMode;
            public float canvasAlpha;
            public RectSnapshot rootRect;
            public RectSnapshot panelRect;
            public RectSnapshot recipeViewportRect;
            public RectSnapshot recipeContentRect;
            public RectSnapshot materialsViewportRect;
            public RectSnapshot materialsContentRect;
            public RectSnapshot progressRootRect;
            public RectSnapshot floatingProgressRect;
            public RectSnapshot craftButtonRect;
            public int recipeRowCount;
            public TextSnapshot selectedName;
            public TextSnapshot selectedDescription;
            public TextSnapshot selectedMaterials;
            public TextSnapshot stageHint;
            public TextSnapshot progressLabel;
            public TextSnapshot quantityValue;
            public TextSnapshot floatingLabel;
            public float progressFillAmount;
            public float floatingFillAmount;
            public bool craftButtonVisible;
            public bool craftButtonInteractable;
        }

        [Serializable]
        private sealed class TextSnapshot
        {
            public bool exists;
            public string text;
            public string font;
            public float fontSize;
            public float preferredHeight;
            public RectSnapshot rect;
        }

        [Serializable]
        private sealed class RectSnapshot
        {
            public bool exists;
            public string path;
            public bool activeSelf;
            public bool activeInHierarchy;
            public Vector2 anchoredPosition;
            public Vector2 sizeDelta;
            public Vector2 rectSize;
            public Vector2 anchorMin;
            public Vector2 anchorMax;
            public Vector2 pivot;
            public Vector3 localScale;
            public Vector3 localEulerAngles;
        }
    }
}
