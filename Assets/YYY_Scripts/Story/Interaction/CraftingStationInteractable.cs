using System.Collections.Generic;
using FarmGame.Data;
using UnityEngine;

namespace Sunset.Story
{
    [DisallowMultipleComponent]
    public class CraftingStationInteractable : MonoBehaviour, IInteractable
    {
        private const string WorkbenchHintConsumedKey = "spring-day1.workbench-entry-hint-consumed";
        private const float Day1WorkbenchInteractionDistance = 0.5f;
        private const float Day1WorkbenchOverlayAutoHideDistance = 1.5f;
        private const float Day1WorkbenchHintRevealDistance = 0.95f;

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

        private float _lastKeyInteractionAt = -999f;
        private Collider2D[] _cachedColliders;
        private SpriteRenderer[] _cachedSpriteRenderers;
        private bool _bubbleAlreadyAppeared;

        public int InteractionPriority => interactionPriority;
        public float InteractionDistance => interactionDistance;

        public bool CanInteract(InteractionContext context)
        {
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
            return CanInteract(context) ? interactionHint : string.Empty;
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
            if (TryGetClosestColliderEnvelopePoint(playerPosition, out Vector2 envelopePoint))
            {
                return envelopePoint;
            }

            if (TryGetClosestVisualPoint(playerPosition, out Vector2 visualPoint))
            {
                return visualPoint;
            }

            Collider2D[] colliders = GetRelevantColliders();
            if (colliders.Length == 0)
            {
                return transform.position;
            }

            float bestDistance = float.MaxValue;
            Vector2 bestPoint = transform.position;
            for (int index = 0; index < colliders.Length; index++)
            {
                Collider2D collider2D = colliders[index];
                if (collider2D == null)
                {
                    continue;
                }

                Vector2 candidate = collider2D.ClosestPoint(playerPosition);
                float distance = (candidate - playerPosition).sqrMagnitude;
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestPoint = candidate;
                }
            }

            return bestPoint;
        }

        public float GetBoundaryDistance(Vector2 playerPosition)
        {
            return Vector2.Distance(playerPosition, GetClosestInteractionPoint(playerPosition));
        }

        public bool ShouldDisplayOverlayBelow(Vector2 playerPosition)
        {
            Bounds bounds = GetVisualBounds();
            float verticalDelta = playerPosition.y - bounds.center.y;
            if (Mathf.Abs(verticalDelta) > 0.04f)
            {
                return verticalDelta > 0f;
            }

            Vector2 closestPoint = GetClosestInteractionPoint(playerPosition);
            return playerPosition.y > closestPoint.y;
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

            UpdateWorkbenchHintBubble(context);

            if (SpringDay1UiLayerUtility.IsBlockingPageUiOpen())
            {
                return;
            }

            if (!enableProximityKeyInteraction || !Input.GetKeyDown(proximityInteractionKey))
            {
                return;
            }

            if (Time.unscaledTime - _lastKeyInteractionAt < keyInteractionCooldown)
            {
                return;
            }

            float distance = GetBoundaryDistance(context.PlayerPosition);
            if (distance > interactionDistance)
            {
                return;
            }

            if (!CanInteract(context))
            {
                return;
            }

            _lastKeyInteractionAt = Time.unscaledTime;
            OnInteract(context);
        }

        private void UpdateWorkbenchHintBubble(InteractionContext context)
        {
            if (station != CraftingStation.Workbench)
            {
                return;
            }

            if (SpringDay1UiLayerUtility.IsBlockingPageUiOpen())
            {
                SpringDay1WorldHintBubble.HideIfExists(transform);
                return;
            }

            if (!ShouldShowWorkbenchHint())
            {
                _bubbleAlreadyAppeared = HasConsumedWorkbenchHint();
            }

            SpringDay1WorkbenchCraftingOverlay overlay = workbenchOverlay != null
                ? workbenchOverlay
                : FindFirstObjectByType<SpringDay1WorkbenchCraftingOverlay>(FindObjectsInactive.Include);
            if (overlay != null && overlay.IsVisible)
            {
                SpringDay1WorldHintBubble.HideIfExists(transform);
                return;
            }

            float distance = GetBoundaryDistance(context.PlayerPosition);
            if (distance > bubbleRevealDistance)
            {
                SpringDay1WorldHintBubble.HideIfExists(transform);
                return;
            }

            SpringDay1WorldHintBubble.EnsureRuntime();
            bool keepTutorialVisible = _bubbleAlreadyAppeared
                && !HasConsumedWorkbenchHint()
                && SpringDay1WorldHintBubble.Instance.CurrentAnchorTarget == transform;
            bool shouldShowTutorial = showFirstUseBubble && (keepTutorialVisible || (!_bubbleAlreadyAppeared && ShouldShowWorkbenchHint()));
            if (shouldShowTutorial)
            {
                SpringDay1WorldHintBubble.Instance.Show(
                    transform,
                    proximityInteractionKey.ToString(),
                    "工作台",
                    "按 E 打开",
                    SpringDay1WorldHintBubble.HintVisualKind.Tutorial);
                _bubbleAlreadyAppeared = true;
                return;
            }

            if (!CanInteract(context))
            {
                SpringDay1WorldHintBubble.HideIfExists(transform);
                return;
            }

            SpringDay1WorldHintBubble.Instance.Show(
                transform,
                proximityInteractionKey.ToString(),
                bubbleCaption,
                string.Empty,
                SpringDay1WorldHintBubble.HintVisualKind.Interaction);
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
            return PlayerPrefs.GetInt(WorkbenchHintConsumedKey, 0) == 1;
        }

        private static void PersistWorkbenchHintConsumed()
        {
            if (PlayerPrefs.GetInt(WorkbenchHintConsumedKey, 0) == 1)
            {
                return;
            }

            PlayerPrefs.SetInt(WorkbenchHintConsumedKey, 1);
            PlayerPrefs.Save();
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
            PlayerMovement playerMovement = FindFirstObjectByType<PlayerMovement>(FindObjectsInactive.Include);
            Transform playerTransform = playerMovement != null ? playerMovement.transform : null;
            if (playerTransform == null)
            {
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

            interactionDistance = Day1WorkbenchInteractionDistance;
            overlayAutoCloseDistance = Day1WorkbenchOverlayAutoHideDistance;
            bubbleRevealDistance = Day1WorkbenchHintRevealDistance;
            interactionPriority = Mathf.Max(interactionPriority, 28);
            if (string.IsNullOrWhiteSpace(interactionHint))
            {
                interactionHint = "使用工作台";
            }
        }
    }
}
