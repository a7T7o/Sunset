using System.Collections.Generic;
using FarmGame.Data;
using UnityEngine;

namespace Sunset.Story
{
    [DisallowMultipleComponent]
    public class CraftingStationInteractable : MonoBehaviour, IInteractable
    {
        private const float Day1WorkbenchInteractionDistance = 1.58f;
        private const float Day1WorkbenchOverlayAutoHideDistance = 3.4f;
        private const float Day1WorkbenchHintRevealDistance = 2.85f;

        [Header("Crafting Station")]
        [SerializeField] private CraftingStation station = CraftingStation.Workbench;
        [SerializeField] private string interactionHint = "使用工作台";
        [SerializeField] private float interactionDistance = 0.5f;
        [SerializeField] private int interactionPriority = 28;
        [SerializeField] private bool createCraftingServiceIfMissing = true;
        [SerializeField] private bool notifySpringDay1Director = true;
        [SerializeField] private CraftingPanel craftingPanel;
        [SerializeField] private bool preferStoryWorkbenchOverlay = true;
        [SerializeField] private SpringDay1WorkbenchCraftingOverlay workbenchOverlay;
        [SerializeField] private float overlayAutoCloseDistance = 1.5f;

        [Header("Test Interaction")]
        [SerializeField] private bool enableProximityKeyInteraction = true;
        [SerializeField] private KeyCode proximityInteractionKey = KeyCode.E;
        [SerializeField] private float keyInteractionCooldown = 0.15f;

        [Header("Workbench Hint Bubble")]
        [SerializeField] private bool showFirstUseBubble = true;
        [SerializeField] private float bubbleRevealDistance = 0.95f;
        [SerializeField] private string bubbleCaption = "工作台";
        [SerializeField] private float overlayDirectionDeadZone = 0.18f;

        private Collider2D[] _cachedColliders;
        private SpriteRenderer[] _cachedSpriteRenderers;
        private bool _bubbleAlreadyAppeared;
        private PlayerMovement _cachedPlayerMovement;

        public int InteractionPriority => interactionPriority;
        public float InteractionDistance => interactionDistance;

        public bool CanInteract(InteractionContext context)
        {
            SpringDay1WorkbenchCraftingOverlay overlay = station == CraftingStation.Workbench ? ResolveWorkbenchOverlay() : null;
            if (overlay != null && overlay.IsVisible)
            {
                return true;
            }

            if (!ShouldExposeWorkbenchInteraction())
            {
                return false;
            }

            if (SpringDay1UiLayerUtility.IsBlockingPageUiOpen())
            {
                return false;
            }

            DialogueManager dialogueManager = DialogueManager.Instance;
            if (dialogueManager != null && dialogueManager.IsDialogueActive)
            {
                return false;
            }

            return ResolveCraftingPanel() != null
                || FindFirstObjectByType<CraftingService>(FindObjectsInactive.Include) != null
                || notifySpringDay1Director;
        }

        public void OnInteract(InteractionContext context)
        {
            SpringDay1WorkbenchCraftingOverlay existingOverlay = station == CraftingStation.Workbench ? ResolveWorkbenchOverlay() : null;
            if (existingOverlay != null && existingOverlay.IsVisible)
            {
                existingOverlay.Toggle(transform, context?.PlayerTransform, ResolveCraftingService(createCraftingServiceIfMissing), station, overlayAutoCloseDistance);
                return;
            }

            if (!ShouldExposeWorkbenchInteraction())
            {
                SpringDay1WorldHintBubble.HideIfExists(transform);
                return;
            }

            DialogueManager dialogueManager = DialogueManager.Instance;
            if (dialogueManager != null && dialogueManager.IsDialogueActive)
            {
                return;
            }

            ConsumeBubbleIfNeeded();

            CraftingService craftingService = ResolveCraftingService(createCraftingServiceIfMissing);
            CraftingPanel panel = ResolveCraftingPanel();

            if (craftingService == null && panel == null && !notifySpringDay1Director)
            {
                Debug.LogWarning($"[CraftingStationInteractable] {name} 找不到可用的制作服务或制作面板。");
                return;
            }

            if (craftingService != null)
            {
                craftingService.RefreshRuntimeContextFromScene();
                craftingService.SetStation(station);
            }

            bool openedRealCraftingUi = false;
            bool handledByWorkbenchOverlay = false;
            if (station == CraftingStation.Workbench && preferStoryWorkbenchOverlay)
            {
                SpringDay1WorkbenchCraftingOverlay overlay = ResolveWorkbenchOverlay();
                if (overlay != null)
                {
                    bool wasVisible = overlay.IsVisible;
                    bool isVisible = overlay.Toggle(transform, context?.PlayerTransform, craftingService, station, overlayAutoCloseDistance);
                    if (wasVisible && !isVisible)
                    {
                        return;
                    }

                    handledByWorkbenchOverlay = isVisible;
                }
            }

            if (!handledByWorkbenchOverlay && panel != null)
            {
                panel.Open(station);
                openedRealCraftingUi = true;
            }

            if (notifySpringDay1Director)
            {
                SpringDay1Director.EnsureRuntime();
                SpringDay1Director.Instance?.NotifyCraftingStationOpened(station);

                if (!openedRealCraftingUi && !handledByWorkbenchOverlay)
                {
                    string fallbackMessage = SpringDay1Director.Instance?.TryHandleWorkbenchTestInteraction(station);
                    if (!string.IsNullOrWhiteSpace(fallbackMessage))
                    {
                        SpringDay1PromptOverlay.EnsureRuntime();
                        SpringDay1PromptOverlay.Instance?.Show(fallbackMessage);
                    }
                }
            }
        }

        public string GetInteractionHint(InteractionContext context)
        {
            if (!CanInteract(context))
            {
                return string.Empty;
            }

            SpringDay1WorkbenchCraftingOverlay overlay = station == CraftingStation.Workbench ? ResolveWorkbenchOverlay() : null;
            if (overlay != null && overlay.IsVisible)
            {
                return "关闭工作台";
            }

            return interactionHint;
        }

        public void ConfigureRuntimeDefaults(CraftingStation targetStation, string hint, float distance, int priority)
        {
            station = targetStation;
            interactionHint = hint;
            interactionDistance = distance;
            interactionPriority = priority;
            ApplyDay1WorkbenchTuningIfNeeded();
        }

        public Bounds GetCombinedBounds()
        {
            Collider2D[] colliders = GetRelevantColliders();
            if (colliders.Length == 0)
            {
                return new Bounds(transform.position, Vector3.one);
            }

            Bounds combinedBounds = colliders[0].bounds;
            for (int index = 1; index < colliders.Length; index++)
            {
                combinedBounds.Encapsulate(colliders[index].bounds);
            }

            return combinedBounds;
        }

        public Bounds GetVisualBounds()
        {
            SpriteRenderer[] renderers = GetRelevantSpriteRenderers();
            bool hasRenderer = false;
            Bounds visualBounds = default;

            for (int index = 0; index < renderers.Length; index++)
            {
                SpriteRenderer spriteRenderer = renderers[index];
                if (spriteRenderer == null || !spriteRenderer.enabled || spriteRenderer.sprite == null)
                {
                    continue;
                }

                if (!hasRenderer)
                {
                    visualBounds = spriteRenderer.bounds;
                    hasRenderer = true;
                }
                else
                {
                    visualBounds.Encapsulate(spriteRenderer.bounds);
                }
            }

            if (hasRenderer)
            {
                return visualBounds;
            }

            return GetCombinedBounds();
        }

        public Vector2 GetClosestInteractionPoint(Vector2 playerPosition)
        {
            float bestDistance = float.MaxValue;
            Vector2 bestPoint = transform.position;
            bool found = false;

            void ConsiderCandidate(Vector2 candidate)
            {
                float distance = (candidate - playerPosition).sqrMagnitude;
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestPoint = candidate;
                    found = true;
                }
            }

            if (TryGetClosestColliderEnvelopePoint(playerPosition, out Vector2 envelopePoint))
            {
                ConsiderCandidate(envelopePoint);
            }

            if (TryGetClosestVisualPoint(playerPosition, out Vector2 visualPoint))
            {
                ConsiderCandidate(visualPoint);
            }

            Collider2D[] colliders = GetRelevantColliders();
            for (int index = 0; index < colliders.Length; index++)
            {
                Collider2D collider2D = colliders[index];
                if (collider2D == null)
                {
                    continue;
                }

                ConsiderCandidate(collider2D.ClosestPoint(playerPosition));
            }

            return found ? bestPoint : (Vector2)transform.position;
        }

        public float GetBoundaryDistance(Vector2 playerPosition)
        {
            return Vector2.Distance(playerPosition, GetClosestInteractionPoint(playerPosition));
        }

        public bool ShouldDisplayOverlayBelow(Vector2 playerPosition)
        {
            Vector2 closestPoint = GetClosestInteractionPoint(playerPosition);
            float closestDelta = playerPosition.y - closestPoint.y;
            if (Mathf.Abs(closestDelta) > overlayDirectionDeadZone)
            {
                return closestDelta > 0f;
            }

            Bounds bounds = GetVisualBounds();
            float visualDelta = playerPosition.y - bounds.center.y;
            if (Mathf.Abs(visualDelta) > Mathf.Max(0.04f, overlayDirectionDeadZone * 0.45f))
            {
                return visualDelta > 0f;
            }

            return closestDelta > 0f;
        }

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(interactionHint))
            {
                interactionHint = station == CraftingStation.AnvilForge ? "使用铁砧" : "使用工作台";
            }

            ApplyDay1WorkbenchTuningIfNeeded();
            interactionDistance = Mathf.Max(0.1f, interactionDistance);
            overlayAutoCloseDistance = Mathf.Max(interactionDistance, overlayAutoCloseDistance);
            bubbleRevealDistance = Mathf.Max(interactionDistance, bubbleRevealDistance);
            CacheColliders();
            CacheSpriteRenderers();
        }

        private void Awake()
        {
            ApplyDay1WorkbenchTuningIfNeeded();
            CacheColliders();
            CacheSpriteRenderers();
        }

        private void Update()
        {
            InteractionContext context = BuildProximityInteractionContext();
            if (context?.PlayerTransform == null)
            {
                return;
            }

            ReportWorkbenchProximityInteraction(context);
        }

        private void ReportWorkbenchProximityInteraction(InteractionContext context)
        {
            if (station != CraftingStation.Workbench)
            {
                return;
            }

            if (!ShouldExposeWorkbenchInteraction())
            {
                SpringDay1WorldHintBubble.HideIfExists(transform);
                return;
            }

            SpringDay1WorkbenchCraftingOverlay overlay = ResolveWorkbenchOverlay();
            bool overlayVisible = overlay != null && overlay.IsVisible;
            if (SpringDay1UiLayerUtility.IsBlockingPageUiOpen() && !overlayVisible)
            {
                SpringDay1WorldHintBubble.HideIfExists(transform);
                return;
            }

            if (!ShouldShowWorkbenchHint())
            {
                _bubbleAlreadyAppeared = HasConsumedWorkbenchHint();
            }

            float distance = GetBoundaryDistance(context.PlayerPosition);
            if (distance > Mathf.Max(bubbleRevealDistance, interactionDistance))
            {
                SpringDay1WorldHintBubble.HideIfExists(transform);
                return;
            }

            bool keepTutorialVisible = _bubbleAlreadyAppeared
                && !HasConsumedWorkbenchHint();
            bool shouldShowTutorial = !overlayVisible && showFirstUseBubble && (keepTutorialVisible || (!_bubbleAlreadyAppeared && ShouldShowWorkbenchHint()));
            bool canUseWorkbench = CanInteract(context);
            bool canInteractNow = canUseWorkbench && (overlayVisible || distance <= interactionDistance);
            if (shouldShowTutorial)
            {
                if (!canInteractNow)
                {
                    InteractionHintOverlay.HideIfExists();
                    SpringDay1WorldHintBubble.HideIfExists(transform);
                    return;
                }

                SpringDay1ProximityInteractionService.ReportCandidate(
                    transform,
                    proximityInteractionKey,
                    proximityInteractionKey.ToString(),
                    BuildWorkbenchPromptCaption(isTeaser: false),
                    BuildWorkbenchTutorialDetail(),
                    distance,
                    interactionPriority,
                    keyInteractionCooldown,
                    canInteractNow,
                    () => OnInteract(context),
                    SpringDay1WorldHintBubble.HintVisualKind.Tutorial,
                    showWorldIndicator: false);
                _bubbleAlreadyAppeared = true;
                return;
            }

            if (!canUseWorkbench)
            {
                InteractionHintOverlay.HideIfExists();
                SpringDay1WorldHintBubble.HideIfExists(transform);
                return;
            }

            if (!enableProximityKeyInteraction)
            {
                return;
            }

            if (!overlayVisible && !canInteractNow)
            {
                InteractionHintOverlay.HideIfExists();
                SpringDay1WorldHintBubble.HideIfExists(transform);
                return;
            }

            SpringDay1ProximityInteractionService.ReportCandidate(
                transform,
                proximityInteractionKey,
                proximityInteractionKey.ToString(),
                overlayVisible ? "关闭工作台" : BuildWorkbenchPromptCaption(isTeaser: !canInteractNow),
                overlayVisible ? "按 E 收起工作台，继续看悬浮制作状态。" : canInteractNow ? BuildWorkbenchReadyDetail() : BuildWorkbenchTeaserDetail(),
                distance,
                interactionPriority,
                keyInteractionCooldown,
                canInteractNow,
                () => OnInteract(context),
                showWorldIndicator: false);
        }

        private bool ShouldShowWorkbenchHint()
        {
            if (HasConsumedWorkbenchHint())
            {
                return false;
            }

            if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
            {
                return false;
            }

            SpringDay1Director director = SpringDay1Director.Instance;
            return director != null && director.ShouldShowWorkbenchEntryHint();
        }

        private void ConsumeBubbleIfNeeded()
        {
            if (station != CraftingStation.Workbench)
            {
                return;
            }

            SpringDay1WorldHintBubble.HideIfExists(transform);
            _bubbleAlreadyAppeared = true;
            PersistWorkbenchHintConsumed();
        }

        private void CacheColliders()
        {
            _cachedColliders = GetComponentsInChildren<Collider2D>(includeInactive: true);
            if (_cachedColliders == null || _cachedColliders.Length == 0)
            {
                Collider2D collider2D = GetComponent<Collider2D>();
                _cachedColliders = collider2D != null ? new[] { collider2D } : System.Array.Empty<Collider2D>();
            }
        }

        private void CacheSpriteRenderers()
        {
            _cachedSpriteRenderers = GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
            if (_cachedSpriteRenderers == null)
            {
                _cachedSpriteRenderers = System.Array.Empty<SpriteRenderer>();
            }
        }

        private Collider2D[] GetRelevantColliders()
        {
            if (_cachedColliders == null || _cachedColliders.Length == 0)
            {
                CacheColliders();
            }

            return _cachedColliders ?? System.Array.Empty<Collider2D>();
        }

        private SpriteRenderer[] GetRelevantSpriteRenderers()
        {
            if (_cachedSpriteRenderers == null || _cachedSpriteRenderers.Length == 0)
            {
                CacheSpriteRenderers();
            }

            return _cachedSpriteRenderers ?? System.Array.Empty<SpriteRenderer>();
        }

        private bool TryGetClosestColliderEnvelopePoint(Vector2 playerPosition, out Vector2 closestPoint)
        {
            Collider2D[] colliders = GetRelevantColliders();
            float bestDistance = float.MaxValue;
            closestPoint = Vector2.zero;
            bool found = false;

            for (int index = 0; index < colliders.Length; index++)
            {
                Collider2D collider2D = colliders[index];
                if (collider2D == null || !collider2D.enabled)
                {
                    continue;
                }

                Vector2 candidate = Vector2.zero;
                bool hasOutline = collider2D switch
                {
                    PolygonCollider2D polygonCollider => TryGetPolygonOutlineClosestPoint(polygonCollider, playerPosition, out candidate),
                    EdgeCollider2D edgeCollider => TryGetEdgeOutlineClosestPoint(edgeCollider, playerPosition, out candidate),
                    CompositeCollider2D compositeCollider => TryGetCompositeOutlineClosestPoint(compositeCollider, playerPosition, out candidate),
                    _ => false
                };

                if (!hasOutline)
                {
                    continue;
                }

                float distance = (candidate - playerPosition).sqrMagnitude;
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    closestPoint = candidate;
                    found = true;
                }
            }

            return found;
        }

        private bool TryGetClosestVisualPoint(Vector2 playerPosition, out Vector2 closestPoint)
        {
            SpriteRenderer[] renderers = GetRelevantSpriteRenderers();
            float bestDistance = float.MaxValue;
            closestPoint = Vector2.zero;
            bool found = false;

            for (int index = 0; index < renderers.Length; index++)
            {
                SpriteRenderer spriteRenderer = renderers[index];
                if (spriteRenderer == null || !spriteRenderer.enabled || spriteRenderer.sprite == null)
                {
                    continue;
                }

                Vector2 candidate = TryGetSpriteOutlineClosestPoint(spriteRenderer, playerPosition, out Vector2 outlinePoint)
                    ? outlinePoint
                    : (Vector2)spriteRenderer.bounds.ClosestPoint(playerPosition);

                float distance = (candidate - playerPosition).sqrMagnitude;
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    closestPoint = candidate;
                    found = true;
                }
            }

            return found;
        }

        private static bool TryGetPolygonOutlineClosestPoint(PolygonCollider2D polygonCollider, Vector2 playerPosition, out Vector2 closestPoint)
        {
            closestPoint = Vector2.zero;
            if (polygonCollider == null || polygonCollider.pathCount <= 0)
            {
                return false;
            }

            float bestDistance = float.MaxValue;
            bool found = false;

            for (int pathIndex = 0; pathIndex < polygonCollider.pathCount; pathIndex++)
            {
                Vector2[] points = polygonCollider.GetPath(pathIndex);
                int pointCount = points != null ? points.Length : 0;
                if (pointCount < 2)
                {
                    continue;
                }
                for (int pointIndex = 0; pointIndex < pointCount; pointIndex++)
                {
                    Vector2 worldA = polygonCollider.transform.TransformPoint(points[pointIndex]);
                    Vector2 worldB = polygonCollider.transform.TransformPoint(points[(pointIndex + 1) % pointCount]);
                    Vector2 candidate = GetClosestPointOnSegment(playerPosition, worldA, worldB);
                    float distance = (candidate - playerPosition).sqrMagnitude;
                    if (distance < bestDistance)
                    {
                        bestDistance = distance;
                        closestPoint = candidate;
                        found = true;
                    }
                }
            }

            return found;
        }

        private static bool TryGetEdgeOutlineClosestPoint(EdgeCollider2D edgeCollider, Vector2 playerPosition, out Vector2 closestPoint)
        {
            closestPoint = Vector2.zero;
            if (edgeCollider == null || edgeCollider.points == null || edgeCollider.points.Length < 2)
            {
                return false;
            }

            float bestDistance = float.MaxValue;
            bool found = false;
            Vector2[] points = edgeCollider.points;
            for (int pointIndex = 0; pointIndex < points.Length - 1; pointIndex++)
            {
                Vector2 worldA = edgeCollider.transform.TransformPoint(points[pointIndex]);
                Vector2 worldB = edgeCollider.transform.TransformPoint(points[pointIndex + 1]);
                Vector2 candidate = GetClosestPointOnSegment(playerPosition, worldA, worldB);
                float distance = (candidate - playerPosition).sqrMagnitude;
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    closestPoint = candidate;
                    found = true;
                }
            }

            return found;
        }

        private static bool TryGetCompositeOutlineClosestPoint(CompositeCollider2D compositeCollider, Vector2 playerPosition, out Vector2 closestPoint)
        {
            closestPoint = Vector2.zero;
            if (compositeCollider == null || compositeCollider.pathCount <= 0)
            {
                return false;
            }

            float bestDistance = float.MaxValue;
            bool found = false;
            Vector2[] points = System.Array.Empty<Vector2>();

            for (int pathIndex = 0; pathIndex < compositeCollider.pathCount; pathIndex++)
            {
                int pointCount = compositeCollider.GetPathPointCount(pathIndex);
                if (pointCount < 2)
                {
                    continue;
                }

                if (points.Length != pointCount)
                {
                    points = new Vector2[pointCount];
                }

                compositeCollider.GetPath(pathIndex, points);
                for (int pointIndex = 0; pointIndex < pointCount - 1; pointIndex++)
                {
                    Vector2 worldA = compositeCollider.transform.TransformPoint(points[pointIndex]);
                    Vector2 worldB = compositeCollider.transform.TransformPoint(points[pointIndex + 1]);
                    Vector2 candidate = GetClosestPointOnSegment(playerPosition, worldA, worldB);
                    float distance = (candidate - playerPosition).sqrMagnitude;
                    if (distance < bestDistance)
                    {
                        bestDistance = distance;
                        closestPoint = candidate;
                        found = true;
                    }
                }
            }

            return found;
        }

        private static bool TryGetSpriteOutlineClosestPoint(SpriteRenderer spriteRenderer, Vector2 playerPosition, out Vector2 closestPoint)
        {
            closestPoint = Vector2.zero;
            Sprite sprite = spriteRenderer.sprite;
            if (sprite == null)
            {
                return false;
            }

            int shapeCount = sprite.GetPhysicsShapeCount();
            if (shapeCount <= 0)
            {
                return false;
            }

            List<Vector2> points = new();
            float bestDistance = float.MaxValue;
            bool found = false;

            for (int shapeIndex = 0; shapeIndex < shapeCount; shapeIndex++)
            {
                points.Clear();
                sprite.GetPhysicsShape(shapeIndex, points);
                if (points.Count < 2)
                {
                    continue;
                }

                for (int pointIndex = 0; pointIndex < points.Count; pointIndex++)
                {
                    Vector2 worldA = spriteRenderer.transform.TransformPoint(points[pointIndex]);
                    Vector2 worldB = spriteRenderer.transform.TransformPoint(points[(pointIndex + 1) % points.Count]);
                    Vector2 candidate = GetClosestPointOnSegment(playerPosition, worldA, worldB);
                    float distance = (candidate - playerPosition).sqrMagnitude;
                    if (distance < bestDistance)
                    {
                        bestDistance = distance;
                        closestPoint = candidate;
                        found = true;
                    }
                }
            }

            return found;
        }

        private static Vector2 GetClosestPointOnSegment(Vector2 point, Vector2 segmentStart, Vector2 segmentEnd)
        {
            Vector2 delta = segmentEnd - segmentStart;
            float length = delta.sqrMagnitude;
            if (length <= Mathf.Epsilon)
            {
                return segmentStart;
            }

            float t = Mathf.Clamp01(Vector2.Dot(point - segmentStart, delta) / length);
            return segmentStart + delta * t;
        }

        private static bool HasConsumedWorkbenchHint()
        {
            return StoryProgressPersistenceService.IsWorkbenchHintConsumed();
        }

        private static void PersistWorkbenchHintConsumed()
        {
            StoryProgressPersistenceService.MarkWorkbenchHintConsumed();
        }

        private CraftingPanel ResolveCraftingPanel()
        {
            if (craftingPanel == null)
            {
                craftingPanel = FindFirstObjectByType<CraftingPanel>(FindObjectsInactive.Include);
            }

            return craftingPanel;
        }

        private SpringDay1WorkbenchCraftingOverlay ResolveWorkbenchOverlay()
        {
            if (workbenchOverlay == null)
            {
                SpringDay1WorkbenchCraftingOverlay.EnsureRuntime();
                workbenchOverlay = FindFirstObjectByType<SpringDay1WorkbenchCraftingOverlay>(FindObjectsInactive.Include);
            }

            return workbenchOverlay;
        }

        private static CraftingService ResolveCraftingService(bool createIfMissing)
        {
            CraftingService craftingService = FindFirstObjectByType<CraftingService>(FindObjectsInactive.Include);
            if (craftingService != null || !createIfMissing)
            {
                return craftingService;
            }

            GameObject runtimeObject = new GameObject(nameof(CraftingService));
            return runtimeObject.AddComponent<CraftingService>();
        }

        private InteractionContext BuildProximityInteractionContext()
        {
            if (_cachedPlayerMovement == null)
            {
                _cachedPlayerMovement = FindFirstObjectByType<PlayerMovement>(FindObjectsInactive.Include);
            }

            Transform playerTransform = _cachedPlayerMovement != null ? _cachedPlayerMovement.transform : null;
            if (playerTransform == null)
            {
                _cachedPlayerMovement = null;
                return null;
            }

            return new InteractionContext
            {
                PlayerTransform = playerTransform,
                PlayerPosition = SpringDay1UiLayerUtility.GetInteractionSamplePoint(playerTransform)
            };
        }

        private void ApplyDay1WorkbenchTuningIfNeeded()
        {
            if (station != CraftingStation.Workbench || !notifySpringDay1Director)
            {
                return;
            }

            interactionDistance = Mathf.Max(interactionDistance, Day1WorkbenchInteractionDistance);
            overlayAutoCloseDistance = Mathf.Max(overlayAutoCloseDistance, Day1WorkbenchOverlayAutoHideDistance);
            bubbleRevealDistance = Mathf.Max(bubbleRevealDistance, Day1WorkbenchHintRevealDistance);
            interactionPriority = Mathf.Max(interactionPriority, 28);
            if (string.IsNullOrWhiteSpace(interactionHint))
            {
                interactionHint = "使用工作台";
            }
        }

        private string BuildWorkbenchReadyDetail()
        {
            SpringDay1WorkbenchCraftingOverlay overlay = ResolveWorkbenchOverlay();

            if (overlay != null && overlay.IsVisible)
            {
                if (overlay.HasReadyWorkbenchOutputs)
                {
                    return "按 E 收起工作台；进度条可直接领取已完成产物。";
                }

                return overlay.HasActiveCraftQueue
                    ? "按 E 收起工作台；左下角会继续显示制作进度。"
                    : "按 E 收起工作台。";
            }

            if (overlay != null && overlay.HasReadyWorkbenchOutputs)
            {
                return overlay.HasActiveCraftQueue
                    ? "按 E 打开工作台，查看单件进度、点击进度条领取产物，或继续追加制作。"
                    : "按 E 打开工作台，点击进度条领取已完成产物，或继续安排制作。";
            }

            if (overlay != null && overlay.HasActiveCraftQueue)
            {
                return "按 E 打开工作台，查看单件进度、剩余数量和当前队列。";
            }

            return "按 E 打开工作台，查看配方、材料和制作进度。";
        }

        private string BuildWorkbenchPromptCaption(bool isTeaser)
        {
            SpringDay1WorkbenchCraftingOverlay overlay = ResolveWorkbenchOverlay();

            if (overlay != null && overlay.IsVisible)
            {
                return "工作台已打开";
            }

            if (overlay != null && overlay.HasReadyWorkbenchOutputs)
            {
                return overlay.HasActiveCraftQueue ? "工作台可领取" : "工作台已完成";
            }

            if (overlay != null && overlay.HasActiveCraftQueue)
            {
                return "工作台制作中";
            }

            return isTeaser ? "靠近工作台" : (string.IsNullOrWhiteSpace(bubbleCaption) ? "工作台" : bubbleCaption);
        }

        private static string BuildWorkbenchTutorialDetail()
        {
            return "按 E 打开工作台，正式进入制作教学。";
        }

        private static string BuildWorkbenchTeaserDetail()
        {
            return "再靠近一些，进入工作台交互范围。";
        }

        private bool ShouldExposeWorkbenchInteraction()
        {
            if (station != CraftingStation.Workbench || !notifySpringDay1Director)
            {
                return true;
            }

            SpringDay1Director director = SpringDay1Director.Instance;
            if (director != null)
            {
                return director.ShouldExposeWorkbenchInteraction();
            }

            StoryManager storyManager = StoryManager.Instance;
            if (storyManager == null)
            {
                return true;
            }

            return storyManager.CurrentPhase switch
            {
                StoryPhase.CrashAndMeet => false,
                StoryPhase.EnterVillage => false,
                StoryPhase.HealingAndHP => false,
                _ => true
            };
        }
    }
}
