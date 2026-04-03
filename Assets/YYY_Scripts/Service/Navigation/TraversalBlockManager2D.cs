using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
public class TraversalBlockManager2D : MonoBehaviour
{
    [Header("目标引用")]
    [SerializeField] private NavGrid2D navGrid;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private bool autoFindReferencesIfMissing = true;

    [Header("阻挡来源")]
    [SerializeField] private Tilemap[] blockingTilemaps = new Tilemap[0];
    [SerializeField] private Collider2D[] blockingColliders = new Collider2D[0];
    [SerializeField] private bool useTilemapOccupancyFallback = false;

    [Header("可走覆盖来源")]
    [SerializeField] private Tilemap[] walkableOverrideTilemaps = new Tilemap[0];
    [SerializeField] private Collider2D[] walkableOverrideColliders = new Collider2D[0];
    [SerializeField] private bool useWalkableOverrideTilemapOccupancyFallback = true;

    [Header("世界边界")]
    [SerializeField] private bool overrideNavGridWorldBounds = true;
    [SerializeField] private Tilemap[] boundsTilemaps = new Tilemap[0];
    [SerializeField] private Collider2D[] boundsColliders = new Collider2D[0];
    [SerializeField, Min(0f)] private float boundsPadding = 0f;

    [Header("玩家约束")]
    [SerializeField] private bool bindPlayerMovement = true;
    [SerializeField] private bool enforcePlayerNavGridBounds = true;

    [Header("自动应用")]
    [SerializeField] private bool applyOnAwake = true;
    [SerializeField] private bool refreshOnEnable = false;
    [SerializeField] private bool logBindings = true;

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
    }

    [ContextMenu("Apply Traversal Setup")]
    public void ApplyConfiguration()
    {
        ApplyConfiguration(rebuildNavGrid: true);
    }

    public void ApplyConfiguration(bool rebuildNavGrid)
    {
        ResolveReferences();
        if (navGrid == null)
        {
            Debug.LogWarning("[TraversalBlockManager2D] 未找到 NavGrid2D，当前不会应用 traversal blocking。", this);
            return;
        }

        Collider2D[] resolvedBlockingColliders = CollectBlockingColliders();
        Tilemap[] fallbackTilemaps = useTilemapOccupancyFallback
            ? NormalizeTilemaps(blockingTilemaps)
            : new Tilemap[0];
        Collider2D[] resolvedWalkableOverrideColliders = CollectWalkableOverrideColliders();
        Tilemap[] walkableOverrideFallbackTilemaps = useWalkableOverrideTilemapOccupancyFallback
            ? NormalizeTilemaps(walkableOverrideTilemaps)
            : new Tilemap[0];

        navGrid.SetObstacleTilemapAutoDetection(false, new string[0], rebuildImmediately: false);
        navGrid.ConfigureExplicitObstacleSources(
            resolvedBlockingColliders,
            fallbackTilemaps,
            rebuildImmediately: false);
        navGrid.ConfigureExplicitWalkableOverrideSources(
            resolvedWalkableOverrideColliders,
            walkableOverrideFallbackTilemaps,
            rebuildImmediately: false);
        navGrid.SetBoundsPadding(boundsPadding, rebuildImmediately: false);

        bool usingManualBounds = false;
        if (overrideNavGridWorldBounds)
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

        if (playerMovement != null)
        {
            if (bindPlayerMovement)
            {
                playerMovement.SetNavGrid(navGrid);
                playerMovement.SetTraversalSoftPassBlockers(resolvedBlockingColliders, enabled: true);
            }
            else
            {
                playerMovement.SetTraversalSoftPassBlockers(new Collider2D[0], enabled: false);
            }

            playerMovement.SetNavGridBoundsEnforcement(
                bindPlayerMovement && enforcePlayerNavGridBounds);
        }

        if (rebuildNavGrid)
        {
            navGrid.RefreshGrid();
        }

        if (logBindings)
        {
            Debug.Log(
                $"[TraversalBlockManager2D] 已应用：阻挡碰撞体={resolvedBlockingColliders.Length}，阻挡 Tile fallback={fallbackTilemaps.Length}，" +
                $"可走覆盖碰撞体={resolvedWalkableOverrideColliders.Length}，可走覆盖 Tile fallback={walkableOverrideFallbackTilemaps.Length}，" +
                $"manualBounds={(usingManualBounds ? "on" : "off")}，playerBounds={(playerMovement != null && bindPlayerMovement && enforcePlayerNavGridBounds ? "on" : "off")}",
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

    private Collider2D[] CollectBlockingColliders()
    {
        List<Collider2D> results = new List<Collider2D>();

        Collider2D[] normalizedColliders = NormalizeColliders(blockingColliders);
        for (int index = 0; index < normalizedColliders.Length; index++)
        {
            AddUnique(results, normalizedColliders[index]);
        }

        Tilemap[] normalizedTilemaps = NormalizeTilemaps(blockingTilemaps);
        for (int index = 0; index < normalizedTilemaps.Length; index++)
        {
            Tilemap tilemap = normalizedTilemaps[index];
            if (tilemap == null)
            {
                continue;
            }

            Collider2D tilemapCollider = tilemap.GetComponent<Collider2D>();
            if (tilemapCollider != null)
            {
                AddUnique(results, tilemapCollider);
            }
            else if (!useTilemapOccupancyFallback)
            {
                Debug.LogWarning(
                    $"[TraversalBlockManager2D] {tilemap.name} 没有 Collider2D，且当前关闭了整格 tile fallback；这张 Tilemap 不会阻挡 traversal。",
                    tilemap);
            }
        }

        return results.ToArray();
    }

    private Collider2D[] CollectWalkableOverrideColliders()
    {
        List<Collider2D> results = new List<Collider2D>();

        Collider2D[] normalizedColliders = NormalizeColliders(walkableOverrideColliders);
        for (int index = 0; index < normalizedColliders.Length; index++)
        {
            AddUnique(results, normalizedColliders[index]);
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
            if (tilemapCollider != null)
            {
                AddUnique(results, tilemapCollider);
            }
            else if (!useWalkableOverrideTilemapOccupancyFallback)
            {
                Debug.LogWarning(
                    $"[TraversalBlockManager2D] {tilemap.name} 没有 Collider2D，且当前关闭了可走覆盖 tile fallback；这张 Tilemap 不会把桥面判成可走。",
                    tilemap);
            }
        }

        return results.ToArray();
    }

    private bool TryCalculateConfiguredWorldBounds(out Bounds bounds)
    {
        bounds = default;
        bool hasBounds = false;

        Tilemap[] tilemapSources = HasAnyReference(boundsTilemaps)
            ? NormalizeTilemaps(boundsTilemaps)
            : NormalizeTilemaps(blockingTilemaps);
        Collider2D[] colliderSources = HasAnyReference(boundsColliders)
            ? NormalizeColliders(boundsColliders)
            : CollectBlockingColliders();

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
}
