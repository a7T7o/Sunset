using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
public class TraversalBlockManager2D : MonoBehaviour
{
    private static readonly FieldInfo NavGridObstacleTagsField = typeof(NavGrid2D).GetField("obstacleTags", BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly FieldInfo NavGridObstacleMaskField = typeof(NavGrid2D).GetField("obstacleMask", BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly string[] RequiredSceneBlockingIncludeKeywords = { "wall", "props", "fence", "rock", "tree", "border", "building", "house", "structure" };
    private static readonly string[] RequiredSceneBlockingExcludeKeywords = { "base", "grass", "ground", "background", "bridge", "farmland" };

    [Header("目标引用")]
    [SerializeField] private NavGrid2D navGrid;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private bool autoFindReferencesIfMissing = true;

    [Header("阻挡来源")]
    [SerializeField] private Tilemap[] blockingTilemaps = new Tilemap[0];
    [SerializeField] private Collider2D[] blockingColliders = new Collider2D[0];
    [SerializeField] private bool useTilemapOccupancyFallback = false;
    [SerializeField] private bool includeColliderlessBlockingTilemapsAsFallback = true;
    [SerializeField] private string[] traversalSoftPassNameKeywords = new[] { "water" };

    [Header("自动场景静态阻挡补全")]
    [SerializeField] private bool autoCollectSceneBlockingTilemaps = true;
    [SerializeField] private bool autoCollectSceneBlockingColliders = false;
    [SerializeField] private string[] sceneBlockingIncludeKeywords = new[] { "wall", "props", "fence", "rock", "tree", "border", "building", "house", "structure" };
    [SerializeField] private string[] sceneBlockingExcludeKeywords = new[] { "base", "grass", "ground", "background", "bridge", "farmland" };

    [Header("可走覆盖来源")]
    [SerializeField] private Tilemap[] walkableOverrideTilemaps = new Tilemap[0];
    [SerializeField] private Collider2D[] walkableOverrideColliders = new Collider2D[0];
    [SerializeField] private bool useWalkableOverrideTilemapOccupancyFallback = true;
    [SerializeField] private bool includeColliderlessWalkableOverrideTilemapsAsFallback = true;

    [Header("世界边界")]
    [SerializeField] private bool overrideNavGridWorldBounds = true;
    [SerializeField] private Tilemap[] boundsTilemaps = new Tilemap[0];
    [SerializeField] private Collider2D[] boundsColliders = new Collider2D[0];
    [SerializeField, Min(0f)] private float boundsPadding = 0f;

    [Header("玩家约束")]
    [SerializeField] private bool bindPlayerMovement = true;
    [SerializeField] private bool enforcePlayerNavGridBounds = true;

    [Header("NPC 约束")]
    [SerializeField] private bool bindNpcAutoRoamControllers = true;
    [SerializeField] private bool enforceNpcNavGridBounds = true;

    [Header("自动应用")]
    [SerializeField] private bool applyOnAwake = true;
    [SerializeField] private bool refreshOnEnable = false;
    [SerializeField] private bool logBindings = true;

    [System.NonSerialized] private string[] effectiveSceneBlockingIncludeKeywords = new string[0];
    [System.NonSerialized] private string[] effectiveSceneBlockingExcludeKeywords = new string[0];

    public bool AppliesConfigurationOnAwake => applyOnAwake;

    private void Awake()
    {
        if (applyOnAwake)
        {
            ApplyConfiguration(rebuildNavGrid: true);
        }
    }

    private void OnEnable()
    {
        if (refreshOnEnable && Application.isPlaying)
        {
            ApplyConfiguration(rebuildNavGrid: true);
        }
    }

    private void OnValidate()
    {
        boundsPadding = Mathf.Max(0f, boundsPadding);
        traversalSoftPassNameKeywords = NormalizeKeywords(traversalSoftPassNameKeywords);
        sceneBlockingIncludeKeywords = NormalizeKeywords(sceneBlockingIncludeKeywords);
        sceneBlockingExcludeKeywords = NormalizeKeywords(sceneBlockingExcludeKeywords);
        RefreshEffectiveSceneBlockingKeywords();
    }

    [ContextMenu("Apply Traversal Setup")]
    public void ApplyConfiguration()
    {
        ApplyConfiguration(rebuildNavGrid: true);
    }

    public void BindRuntimeSceneReferences(NavGrid2D runtimeNavGrid, PlayerMovement runtimePlayerMovement)
    {
        if (runtimeNavGrid != null)
        {
            navGrid = runtimeNavGrid;
        }

        if (runtimePlayerMovement != null)
        {
            playerMovement = runtimePlayerMovement;
        }
    }

    public void ApplyConfiguration(bool rebuildNavGrid)
    {
        traversalSoftPassNameKeywords = NormalizeKeywords(traversalSoftPassNameKeywords);
        sceneBlockingIncludeKeywords = NormalizeKeywords(sceneBlockingIncludeKeywords);
        sceneBlockingExcludeKeywords = NormalizeKeywords(sceneBlockingExcludeKeywords);
        RefreshEffectiveSceneBlockingKeywords();
        ResolveReferences();
        if (navGrid == null)
        {
            Debug.LogWarning("[TraversalBlockManager2D] 未找到 NavGrid2D，当前不会应用 traversal blocking。", this);
            return;
        }

        CollectBlockingSources(
            out Collider2D[] resolvedBlockingColliders,
            out Tilemap[] fallbackTilemaps,
            out Collider2D[] resolvedSoftPassColliders,
            out Tilemap[] softPassFallbackTilemaps);
        CollectWalkableOverrideSources(
            out Collider2D[] resolvedWalkableOverrideColliders,
            out Tilemap[] walkableOverrideFallbackTilemaps);

        navGrid.SetObstacleTilemapAutoDetection(false, new string[0], rebuildImmediately: false);
        navGrid.ConfigureExplicitObstacleSources(
            resolvedBlockingColliders,
            fallbackTilemaps,
            rebuildImmediately: false);
        navGrid.ConfigureTraversalSoftPassSources(
            resolvedSoftPassColliders,
            softPassFallbackTilemaps,
            rebuildImmediately: false);
        navGrid.ConfigureExplicitWalkableOverrideSources(
            resolvedWalkableOverrideColliders,
            walkableOverrideFallbackTilemaps,
            rebuildImmediately: false);
        navGrid.SetBoundsPadding(boundsPadding, rebuildImmediately: false);

        bool usingManualBounds = false;
        if (overrideNavGridWorldBounds)
        {
            if (HasAnyReference(boundsTilemaps) || HasAnyReference(boundsColliders))
            {
                if (TryCalculateConfiguredWorldBounds(out Bounds worldBounds))
                {
                    navGrid.SetWorldBounds(worldBounds, rebuildImmediately: false);
                    usingManualBounds = true;
                }
                else
                {
                    navGrid.SetAutoDetectWorldBounds(true, rebuildImmediately: false);
                }
            }
            else if (logBindings)
            {
                Debug.Log(
                    "[TraversalBlockManager2D] 未显式配置 boundsTilemaps / boundsColliders，保留 NavGrid2D 当前 world bounds，不再用 traversal 阻挡源反推整张地图边界。",
                    this);
            }
        }

        if (playerMovement != null)
        {
            if (bindPlayerMovement)
            {
                playerMovement.SetNavGrid(navGrid);
                playerMovement.SetTraversalSoftPassBlockers(
                    resolvedSoftPassColliders,
                    enabled: resolvedSoftPassColliders.Length > 0);
            }
            else
            {
                playerMovement.SetTraversalSoftPassBlockers(new Collider2D[0], enabled: false);
            }

            playerMovement.SetNavGridBoundsEnforcement(
                bindPlayerMovement && enforcePlayerNavGridBounds);
        }

        int boundNpcCount = ApplyNpcTraversalBindings(resolvedSoftPassColliders);

        if (rebuildNavGrid)
        {
            navGrid.RefreshGrid();
        }

        if (logBindings)
        {
            Debug.Log(
                $"[TraversalBlockManager2D] 已应用：阻挡碰撞体={resolvedBlockingColliders.Length}，阻挡 Tile fallback={fallbackTilemaps.Length}，" +
                $"软穿越碰撞体={resolvedSoftPassColliders.Length}，软穿越 Tile fallback={softPassFallbackTilemaps.Length}，" +
                $"可走覆盖碰撞体={resolvedWalkableOverrideColliders.Length}，可走覆盖 Tile fallback={walkableOverrideFallbackTilemaps.Length}，" +
                $"manualBounds={(usingManualBounds ? "on" : "off")}，playerBounds={(playerMovement != null && bindPlayerMovement && enforcePlayerNavGridBounds ? "on" : "off")}，" +
                $"npcBindings={boundNpcCount}，npcBounds={(bindNpcAutoRoamControllers && enforceNpcNavGridBounds ? "on" : "off")}",
                this);
        }
    }

    private void ResolveReferences()
    {
        if (!autoFindReferencesIfMissing)
        {
            return;
        }

        if (navGrid == null)
        {
            navGrid = FindFirstObjectByType<NavGrid2D>();
        }

        if (playerMovement == null)
        {
            playerMovement = FindFirstObjectByType<PlayerMovement>();
        }
    }

    private int ApplyNpcTraversalBindings(Collider2D[] resolvedSoftPassColliders)
    {
        NPCAutoRoamController[] npcControllers = FindObjectsByType<NPCAutoRoamController>(FindObjectsSortMode.None);
        if (npcControllers == null || npcControllers.Length == 0)
        {
            return 0;
        }

        int boundCount = 0;
        for (int index = 0; index < npcControllers.Length; index++)
        {
            NPCAutoRoamController npcController = npcControllers[index];
            if (npcController == null)
            {
                continue;
            }

            npcController.SetNavGrid(navGrid);
            if (bindNpcAutoRoamControllers)
            {
                npcController.SetTraversalSoftPassBlockers(
                    resolvedSoftPassColliders,
                    enabled: resolvedSoftPassColliders.Length > 0);
            }
            else
            {
                npcController.SetTraversalSoftPassBlockers(new Collider2D[0], enabled: false);
            }

            npcController.SetNavGridBoundsEnforcement(
                bindNpcAutoRoamControllers && enforceNpcNavGridBounds);
            boundCount++;
        }

        return boundCount;
    }

    private void CollectBlockingSources(
        out Collider2D[] hardColliders,
        out Tilemap[] hardFallbackTilemaps,
        out Collider2D[] softPassColliders,
        out Tilemap[] softPassFallbackTilemaps)
    {
        List<Collider2D> hardColliderResults = new List<Collider2D>();
        List<Tilemap> hardFallbackResults = new List<Tilemap>();
        List<Collider2D> softPassColliderResults = new List<Collider2D>();
        List<Tilemap> softPassFallbackResults = new List<Tilemap>();

        Collider2D[] normalizedColliders = GetResolvedBlockingColliders();
        for (int index = 0; index < normalizedColliders.Length; index++)
        {
            Collider2D collider = normalizedColliders[index];
            if (ShouldTreatAsSoftPassSource(collider))
            {
                AddUnique(softPassColliderResults, collider);
            }
            else
            {
                AddUnique(hardColliderResults, collider);
            }
        }

        Tilemap[] normalizedTilemaps = GetResolvedBlockingTilemaps();
        for (int index = 0; index < normalizedTilemaps.Length; index++)
        {
            Tilemap tilemap = normalizedTilemaps[index];
            if (tilemap == null)
            {
                continue;
            }

            Collider2D tilemapCollider = tilemap.GetComponent<Collider2D>();
            bool useFallback = tilemapCollider == null
                ? includeColliderlessBlockingTilemapsAsFallback
                : useTilemapOccupancyFallback;
            bool isSoftPassSource = ShouldTreatAsSoftPassSource(tilemap);
            if (tilemapCollider != null)
            {
                if (isSoftPassSource)
                {
                    AddUnique(softPassColliderResults, tilemapCollider);
                }
                else
                {
                    AddUnique(hardColliderResults, tilemapCollider);
                }
            }
            else if (!useFallback)
            {
                Debug.LogWarning(
                    $"[TraversalBlockManager2D] {tilemap.name} 没有 Collider2D，且当前关闭了整格 tile fallback；这张 Tilemap 不会阻挡 traversal。",
                    tilemap);
            }

            if (useFallback)
            {
                if (isSoftPassSource)
                {
                    AddUnique(softPassFallbackResults, tilemap);
                }
                else
                {
                    AddUnique(hardFallbackResults, tilemap);
                }
            }
        }

        hardColliders = hardColliderResults.ToArray();
        hardFallbackTilemaps = hardFallbackResults.ToArray();
        softPassColliders = softPassColliderResults.ToArray();
        softPassFallbackTilemaps = softPassFallbackResults.ToArray();
    }

    private void CollectWalkableOverrideSources(
        out Collider2D[] colliders,
        out Tilemap[] fallbackTilemaps)
    {
        List<Collider2D> colliderResults = new List<Collider2D>();
        List<Tilemap> fallbackResults = new List<Tilemap>();

        Collider2D[] normalizedColliders = NormalizeColliders(walkableOverrideColliders);
        for (int index = 0; index < normalizedColliders.Length; index++)
        {
            AddUnique(colliderResults, normalizedColliders[index]);
        }

        Tilemap[] normalizedTilemaps = NormalizeTilemaps(walkableOverrideTilemaps);
        for (int index = 0; index < normalizedTilemaps.Length; index++)
        {
            Tilemap tilemap = normalizedTilemaps[index];
            if (tilemap == null)
            {
                continue;
            }

            Collider2D tilemapCollider = tilemap.GetComponent<Collider2D>();
            bool useFallback = tilemapCollider == null
                ? includeColliderlessWalkableOverrideTilemapsAsFallback
                : useWalkableOverrideTilemapOccupancyFallback;
            if (tilemapCollider != null)
            {
                AddUnique(colliderResults, tilemapCollider);
            }
            else if (!useFallback)
            {
                Debug.LogWarning(
                    $"[TraversalBlockManager2D] {tilemap.name} 没有 Collider2D，且当前关闭了可走覆盖 tile fallback；这张 Tilemap 不会把桥面判成可走。",
                    tilemap);
            }

            if (useFallback)
            {
                AddUnique(fallbackResults, tilemap);
            }
        }

        colliders = colliderResults.ToArray();
        fallbackTilemaps = fallbackResults.ToArray();
    }

    private bool TryCalculateConfiguredWorldBounds(out Bounds bounds)
    {
        bounds = default;
        bool hasBounds = false;

        Tilemap[] tilemapSources = HasAnyReference(boundsTilemaps)
            ? NormalizeTilemaps(boundsTilemaps)
            : GetResolvedBlockingTilemaps();
        Collider2D[] colliderSources = HasAnyReference(boundsColliders)
            ? NormalizeColliders(boundsColliders)
            : BuildBoundsColliderFallback();

        for (int index = 0; index < tilemapSources.Length; index++)
        {
            if (TryGetTilemapWorldBounds(tilemapSources[index], out Bounds tilemapBounds))
            {
                Encapsulate(ref bounds, ref hasBounds, tilemapBounds);
            }
        }

        for (int index = 0; index < colliderSources.Length; index++)
        {
            Collider2D collider = colliderSources[index];
            if (collider == null || !collider.enabled || !collider.gameObject.activeInHierarchy)
            {
                continue;
            }

            Encapsulate(ref bounds, ref hasBounds, collider.bounds);
        }

        if (!hasBounds)
        {
            return false;
        }

        if (boundsPadding > 0f)
        {
            bounds.Expand(new Vector3(boundsPadding * 2f, boundsPadding * 2f, 0f));
        }

        return true;
    }

    private static bool TryGetTilemapWorldBounds(Tilemap tilemap, out Bounds bounds)
    {
        bounds = default;
        if (tilemap == null || !tilemap.gameObject.activeInHierarchy)
        {
            return false;
        }

        Bounds localBounds = tilemap.localBounds;
        if (localBounds.size.x <= 0f || localBounds.size.y <= 0f)
        {
            return false;
        }

        Vector3 worldMin = tilemap.transform.TransformPoint(localBounds.min);
        Vector3 worldMax = tilemap.transform.TransformPoint(localBounds.max);
        bounds.SetMinMax(Vector3.Min(worldMin, worldMax), Vector3.Max(worldMin, worldMax));
        return true;
    }

    private static void Encapsulate(ref Bounds bounds, ref bool hasBounds, Bounds candidate)
    {
        if (!hasBounds)
        {
            bounds = candidate;
            hasBounds = true;
            return;
        }

        bounds.Encapsulate(candidate.min);
        bounds.Encapsulate(candidate.max);
    }

    private static bool HasAnyReference<T>(T[] source) where T : Object
    {
        if (source == null)
        {
            return false;
        }

        for (int index = 0; index < source.Length; index++)
        {
            if (source[index] != null)
            {
                return true;
            }
        }

        return false;
    }

    private bool HasAnyExplicitTraversalSourceConfigured()
    {
        return HasAnyReference(blockingTilemaps) ||
               HasAnyReference(blockingColliders) ||
               HasAnyReference(walkableOverrideTilemaps) ||
               HasAnyReference(walkableOverrideColliders);
    }

    private Collider2D[] BuildBoundsColliderFallback()
    {
        var results = new List<Collider2D>();
        Tilemap[] normalizedBlockingTilemaps = GetResolvedBlockingTilemaps();
        for (int index = 0; index < normalizedBlockingTilemaps.Length; index++)
        {
            Collider2D tilemapCollider = normalizedBlockingTilemaps[index] != null
                ? normalizedBlockingTilemaps[index].GetComponent<Collider2D>()
                : null;
            AddUnique(results, tilemapCollider);
        }

        Collider2D[] normalizedBlockingColliders = GetResolvedBlockingColliders();
        for (int index = 0; index < normalizedBlockingColliders.Length; index++)
        {
            AddUnique(results, normalizedBlockingColliders[index]);
        }

        return results.ToArray();
    }

    private Tilemap[] GetResolvedBlockingTilemaps()
    {
        List<Tilemap> results = new List<Tilemap>();
        Tilemap[] manualTilemaps = NormalizeTilemaps(blockingTilemaps);
        for (int index = 0; index < manualTilemaps.Length; index++)
        {
            AddUnique(results, manualTilemaps[index]);
        }

        if (!autoCollectSceneBlockingTilemaps)
        {
            return results.ToArray();
        }

        Tilemap[] sceneTilemaps = Object.FindObjectsByType<Tilemap>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        for (int index = 0; index < sceneTilemaps.Length; index++)
        {
            Tilemap tilemap = sceneTilemaps[index];
            if (!ShouldAutoCollectBlockingSource(tilemap))
            {
                continue;
            }

            AddUnique(results, tilemap);
        }

        return results.ToArray();
    }

    private Collider2D[] GetResolvedBlockingColliders()
    {
        List<Collider2D> results = new List<Collider2D>();
        Collider2D[] manualColliders = NormalizeColliders(blockingColliders);
        for (int index = 0; index < manualColliders.Length; index++)
        {
            AddUnique(results, manualColliders[index]);
        }

        if (!autoCollectSceneBlockingColliders)
        {
            return results.ToArray();
        }

        Collider2D[] sceneColliders = Object.FindObjectsByType<Collider2D>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        for (int index = 0; index < sceneColliders.Length; index++)
        {
            Collider2D collider = sceneColliders[index];
            if (!ShouldAutoCollectBlockingSource(collider))
            {
                continue;
            }

            AddUnique(results, collider);
        }

        return results.ToArray();
    }

    private static Tilemap[] NormalizeTilemaps(Tilemap[] source)
    {
        List<Tilemap> results = new List<Tilemap>();
        if (source == null)
        {
            return results.ToArray();
        }

        for (int index = 0; index < source.Length; index++)
        {
            AddUnique(results, source[index]);
        }

        return results.ToArray();
    }

    private static Collider2D[] NormalizeColliders(Collider2D[] source)
    {
        List<Collider2D> results = new List<Collider2D>();
        if (source == null)
        {
            return results.ToArray();
        }

        for (int index = 0; index < source.Length; index++)
        {
            AddUnique(results, source[index]);
        }

        return results.ToArray();
    }

    private static void AddUnique<T>(List<T> list, T item) where T : Object
    {
        if (item == null)
        {
            return;
        }

        for (int index = 0; index < list.Count; index++)
        {
            if (list[index] == item)
            {
                return;
            }
        }

        list.Add(item);
    }

    private static string[] NormalizeKeywords(string[] source)
    {
        var results = new List<string>();
        if (source == null)
        {
            return results.ToArray();
        }

        for (int index = 0; index < source.Length; index++)
        {
            string keyword = source[index];
            if (string.IsNullOrWhiteSpace(keyword))
            {
                continue;
            }

            string normalizedKeyword = keyword.Trim().ToLowerInvariant();
            if (!results.Contains(normalizedKeyword))
            {
                results.Add(normalizedKeyword);
            }
        }

        return results.ToArray();
    }

    private bool ShouldTreatAsSoftPassSource(Object source)
    {
        if (source == null || traversalSoftPassNameKeywords == null || traversalSoftPassNameKeywords.Length == 0)
        {
            return false;
        }

        string loweredName = source.name.ToLowerInvariant();
        for (int index = 0; index < traversalSoftPassNameKeywords.Length; index++)
        {
            string keyword = traversalSoftPassNameKeywords[index];
            if (!string.IsNullOrWhiteSpace(keyword) && loweredName.Contains(keyword))
            {
                return true;
            }
        }

        return false;
    }

    private bool ShouldAutoCollectBlockingSource(Object source)
    {
        if (source == null)
        {
            return false;
        }

        EnsureEffectiveSceneBlockingKeywordsReady();

        if (IsExplicitWalkableOverrideSource(source))
        {
            return false;
        }

        if (IsAutoWalkableFarmlandSurfaceSource(source))
        {
            return false;
        }

        if (source is Component component)
        {
            if (component.gameObject.scene != gameObject.scene || !component.gameObject.activeInHierarchy)
            {
                return false;
            }

            if (component is Collider2D collider)
            {
                if (!collider.enabled || collider.isTrigger)
                {
                    return false;
                }

                if (collider.attachedRigidbody != null &&
                    collider.attachedRigidbody.bodyType == RigidbodyType2D.Dynamic)
                {
                    return false;
                }

                if (TryGetNavigationUnitInParents(component, out INavigationUnit navigationUnit) &&
                    navigationUnit.GetUnitType() != NavigationUnitType.StaticObstacle)
                {
                    return false;
                }
            }
        }

        if (MatchesSourceKeyword(source, effectiveSceneBlockingExcludeKeywords))
        {
            return false;
        }

        return ShouldTreatAsSoftPassSource(source) ||
               MatchesConfiguredNavGridObstacleContract(source) ||
               MatchesSourceKeyword(source, effectiveSceneBlockingIncludeKeywords);
    }

    private void EnsureEffectiveSceneBlockingKeywordsReady()
    {
        if ((effectiveSceneBlockingIncludeKeywords == null || effectiveSceneBlockingIncludeKeywords.Length == 0) &&
            (effectiveSceneBlockingExcludeKeywords == null || effectiveSceneBlockingExcludeKeywords.Length == 0))
        {
            RefreshEffectiveSceneBlockingKeywords();
        }
    }

    private void RefreshEffectiveSceneBlockingKeywords()
    {
        effectiveSceneBlockingIncludeKeywords = MergeRequiredKeywords(
            NormalizeKeywords(sceneBlockingIncludeKeywords),
            RequiredSceneBlockingIncludeKeywords);
        effectiveSceneBlockingExcludeKeywords = MergeRequiredKeywords(
            NormalizeKeywords(sceneBlockingExcludeKeywords),
            RequiredSceneBlockingExcludeKeywords);
    }

    private static string[] MergeRequiredKeywords(string[] serializedKeywords, string[] requiredKeywords)
    {
        List<string> mergedKeywords = new List<string>();
        AppendUniqueKeywords(mergedKeywords, serializedKeywords);
        AppendUniqueKeywords(mergedKeywords, requiredKeywords);
        return mergedKeywords.ToArray();
    }

    private static void AppendUniqueKeywords(List<string> target, string[] source)
    {
        if (target == null || source == null)
        {
            return;
        }

        for (int index = 0; index < source.Length; index++)
        {
            string keyword = source[index];
            if (string.IsNullOrWhiteSpace(keyword))
            {
                continue;
            }

            string normalizedKeyword = keyword.Trim().ToLowerInvariant();
            if (!target.Contains(normalizedKeyword))
            {
                target.Add(normalizedKeyword);
            }
        }
    }

    private static bool IsAutoWalkableFarmlandSurfaceSource(Object source)
    {
        if (source == null)
        {
            return false;
        }

        if (NameLooksLikeWalkableFarmlandSurface(source.name))
        {
            return true;
        }

        if (source is Component component)
        {
            Transform current = component.transform.parent;
            int depth = 0;
            while (current != null && depth < 4)
            {
                if (NameLooksLikeWalkableFarmlandSurface(current.name))
                {
                    return true;
                }

                current = current.parent;
                depth++;
            }
        }

        return false;
    }

    private static bool NameLooksLikeWalkableFarmlandSurface(string sourceName)
    {
        if (string.IsNullOrWhiteSpace(sourceName))
        {
            return false;
        }

        string loweredName = sourceName
            .Trim()
            .ToLowerInvariant()
            .Replace(" ", string.Empty);

        if (!loweredName.Contains("farmland") || loweredName.Contains("water"))
        {
            return false;
        }

        return loweredName.Contains("border") || loweredName.Contains("center");
    }

    private bool MatchesConfiguredNavGridObstacleContract(Object source)
    {
        if (navGrid == null || !(source is Component component))
        {
            return false;
        }

        return MatchesConfiguredNavGridObstacleTags(component.transform) ||
               MatchesConfiguredNavGridObstacleLayers(component.transform);
    }

    private bool MatchesConfiguredNavGridObstacleTags(Transform sourceTransform)
    {
        string[] configuredTags = GetConfiguredNavGridObstacleTags();
        if (configuredTags == null || configuredTags.Length == 0 || sourceTransform == null)
        {
            return false;
        }

        Transform current = sourceTransform;
        int depth = 0;
        while (current != null && depth < 6)
        {
            string currentTag = current.tag;
            for (int index = 0; index < configuredTags.Length; index++)
            {
                string configuredTag = configuredTags[index];
                if (!string.IsNullOrWhiteSpace(configuredTag) &&
                    string.Equals(currentTag, configuredTag, System.StringComparison.Ordinal))
                {
                    return true;
                }
            }

            current = current.parent;
            depth++;
        }

        return false;
    }

    private bool MatchesConfiguredNavGridObstacleLayers(Transform sourceTransform)
    {
        LayerMask configuredMask = GetConfiguredNavGridObstacleMask();
        if (configuredMask.value == 0 || sourceTransform == null)
        {
            return false;
        }

        Transform current = sourceTransform;
        int depth = 0;
        while (current != null && depth < 6)
        {
            if (((1 << current.gameObject.layer) & configuredMask.value) != 0)
            {
                return true;
            }

            current = current.parent;
            depth++;
        }

        return false;
    }

    private string[] GetConfiguredNavGridObstacleTags()
    {
        if (navGrid == null || NavGridObstacleTagsField == null)
        {
            return null;
        }

        return NavGridObstacleTagsField.GetValue(navGrid) as string[];
    }

    private LayerMask GetConfiguredNavGridObstacleMask()
    {
        if (navGrid == null || NavGridObstacleMaskField == null)
        {
            return default;
        }

        object rawValue = NavGridObstacleMaskField.GetValue(navGrid);
        return rawValue is LayerMask mask ? mask : default;
    }

    private bool IsExplicitWalkableOverrideSource(Object source)
    {
        if (source == null)
        {
            return false;
        }

        if (source is Tilemap tilemap)
        {
            Tilemap[] explicitTilemaps = NormalizeTilemaps(walkableOverrideTilemaps);
            for (int index = 0; index < explicitTilemaps.Length; index++)
            {
                if (explicitTilemaps[index] == tilemap)
                {
                    return true;
                }
            }
        }

        if (source is Collider2D collider)
        {
            Collider2D[] explicitColliders = NormalizeColliders(walkableOverrideColliders);
            for (int index = 0; index < explicitColliders.Length; index++)
            {
                if (explicitColliders[index] == collider)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool MatchesSourceKeyword(Object source, string[] keywords)
    {
        if (source == null || keywords == null || keywords.Length == 0)
        {
            return false;
        }

        if (SourceNameMatches(source.name, keywords))
        {
            return true;
        }

        if (source is Component component)
        {
            Transform current = component.transform.parent;
            int depth = 0;
            while (current != null && depth < 4)
            {
                if (SourceNameMatches(current.name, keywords))
                {
                    return true;
                }

                current = current.parent;
                depth++;
            }
        }

        return false;
    }

    private static bool SourceNameMatches(string sourceName, string[] keywords)
    {
        if (string.IsNullOrWhiteSpace(sourceName) || keywords == null || keywords.Length == 0)
        {
            return false;
        }

        string loweredName = sourceName.ToLowerInvariant();
        for (int index = 0; index < keywords.Length; index++)
        {
            string keyword = keywords[index];
            if (!string.IsNullOrWhiteSpace(keyword) && loweredName.Contains(keyword))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryGetNavigationUnitInParents(Component component, out INavigationUnit navigationUnit)
    {
        navigationUnit = null;
        if (component == null)
        {
            return false;
        }

        Transform current = component.transform;
        List<MonoBehaviour> behaviours = new List<MonoBehaviour>(8);
        while (current != null)
        {
            behaviours.Clear();
            current.GetComponents(behaviours);
            for (int index = 0; index < behaviours.Count; index++)
            {
                if (behaviours[index] is INavigationUnit foundUnit)
                {
                    navigationUnit = foundUnit;
                    return true;
                }
            }

            current = current.parent;
        }

        return false;
    }
}
