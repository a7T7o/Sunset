using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
[AddComponentMenu("Sunset/Navigation/Stair Layer Transition Zone 2D")]
public class StairLayerTransitionZone2D : MonoBehaviour
{
    [Serializable]
    private struct ExitTargetConfig
    {
        [Tooltip("离开楼梯后要切到的 Unity Layer 名称，例如 LAYER 1 / LAYER 2。留空则不改 Unity Layer。")]
        public string unityLayerName;

        [Tooltip("离开楼梯后要切到的 Sorting Layer 名称，例如 Layer 1 / Layer 2。留空则不改 Sorting Layer。")]
        public string sortingLayerName;
    }

    private enum ExitEdge
    {
        None,
        Top,
        Bottom,
        Left,
        Right
    }

    [Header("退出目标")]
    [SerializeField] private ExitTargetConfig topExitTarget;
    [SerializeField] private ExitTargetConfig bottomExitTarget;

    [Header("同步范围")]
    [SerializeField] private bool syncUnityLayerToAllChildren = true;
    [SerializeField] private bool syncSortingLayerToAllRenderers = true;
    [SerializeField] private bool syncSortingLayerToSortingGroups = true;

    [Header("调试")]
    [SerializeField] private bool showDebugLogs = false;

    private readonly Dictionary<int, int> activeOverlapCounts = new Dictionary<int, int>();
    private Collider2D zoneCollider;

    private void Awake()
    {
        zoneCollider = GetComponent<Collider2D>();
        if (zoneCollider != null && !zoneCollider.isTrigger)
        {
            zoneCollider.isTrigger = true;
        }
    }

    private void Reset()
    {
        zoneCollider = GetComponent<Collider2D>();
        if (zoneCollider != null)
        {
            zoneCollider.isTrigger = true;
        }
    }

    private void OnValidate()
    {
        zoneCollider = GetComponent<Collider2D>();
        if (zoneCollider != null && !zoneCollider.isTrigger)
        {
            zoneCollider.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!TryResolvePlayerRoot(other, out GameObject playerRoot))
        {
            return;
        }

        int playerId = playerRoot.GetInstanceID();
        activeOverlapCounts.TryGetValue(playerId, out int currentCount);
        activeOverlapCounts[playerId] = currentCount + 1;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!TryResolvePlayerRoot(other, out GameObject playerRoot))
        {
            return;
        }

        int playerId = playerRoot.GetInstanceID();
        activeOverlapCounts.TryGetValue(playerId, out int currentCount);
        currentCount = Mathf.Max(0, currentCount - 1);

        if (currentCount > 0)
        {
            activeOverlapCounts[playerId] = currentCount;
            return;
        }

        activeOverlapCounts.Remove(playerId);
        ApplyTransitionForExit(other, playerRoot);
    }

    private void OnDisable()
    {
        activeOverlapCounts.Clear();
    }

    private void ApplyTransitionForExit(Collider2D exitingCollider, GameObject playerRoot)
    {
        Collider2D effectiveZoneCollider = zoneCollider != null ? zoneCollider : GetComponent<Collider2D>();
        if (effectiveZoneCollider == null)
        {
            return;
        }

        Vector2 footPoint = GetPlayerFootPoint(playerRoot, exitingCollider);
        ExitEdge exitEdge = DetermineExitEdge(effectiveZoneCollider.bounds, footPoint);

        switch (exitEdge)
        {
            case ExitEdge.Top:
                ApplyTargetConfig(playerRoot, topExitTarget, "Top");
                break;

            case ExitEdge.Bottom:
                ApplyTargetConfig(playerRoot, bottomExitTarget, "Bottom");
                break;

            default:
                if (showDebugLogs)
                {
                    Debug.Log($"[StairLayerTransitionZone2D] 忽略侧边离开：edge={exitEdge}, player={playerRoot.name}", this);
                }
                break;
        }
    }

    private void ApplyTargetConfig(GameObject playerRoot, ExitTargetConfig target, string edgeLabel)
    {
        if (playerRoot == null)
        {
            return;
        }

        bool changedAnyState = false;

        if (!string.IsNullOrWhiteSpace(target.unityLayerName) &&
            TryGetUnityLayer(target.unityLayerName, out int unityLayer))
        {
            ApplyUnityLayer(playerRoot, unityLayer);
            changedAnyState = true;
        }

        if (!string.IsNullOrWhiteSpace(target.sortingLayerName) &&
            HasSortingLayer(target.sortingLayerName))
        {
            ApplySortingLayer(playerRoot, target.sortingLayerName);
            changedAnyState = true;
        }

        if (showDebugLogs && changedAnyState)
        {
            Debug.Log(
                $"[StairLayerTransitionZone2D] {playerRoot.name} 从 {edgeLabel} 边界离开，切换到 UnityLayer={target.unityLayerName}, SortingLayer={target.sortingLayerName}",
                this);
        }
    }

    private void ApplyUnityLayer(GameObject playerRoot, int unityLayer)
    {
        if (syncUnityLayerToAllChildren)
        {
            Transform[] transforms = playerRoot.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < transforms.Length; i++)
            {
                transforms[i].gameObject.layer = unityLayer;
            }

            return;
        }

        playerRoot.layer = unityLayer;
    }

    private void ApplySortingLayer(GameObject playerRoot, string sortingLayerName)
    {
        if (syncSortingLayerToAllRenderers)
        {
            SpriteRenderer[] spriteRenderers = playerRoot.GetComponentsInChildren<SpriteRenderer>(true);
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                spriteRenderers[i].sortingLayerName = sortingLayerName;
            }
        }

        if (syncSortingLayerToSortingGroups)
        {
            SortingGroup[] sortingGroups = playerRoot.GetComponentsInChildren<SortingGroup>(true);
            for (int i = 0; i < sortingGroups.Length; i++)
            {
                sortingGroups[i].sortingLayerName = sortingLayerName;
            }
        }
    }

    private static Vector2 GetPlayerFootPoint(GameObject playerRoot, Collider2D fallbackCollider)
    {
        Collider2D rootCollider = playerRoot != null ? playerRoot.GetComponent<Collider2D>() : null;
        if (rootCollider != null)
        {
            return GetFootPoint(rootCollider);
        }

        if (playerRoot != null)
        {
            Collider2D[] childColliders = playerRoot.GetComponentsInChildren<Collider2D>(true);
            for (int i = 0; i < childColliders.Length; i++)
            {
                Collider2D childCollider = childColliders[i];
                if (childCollider == null || childCollider.isTrigger)
                {
                    continue;
                }

                return GetFootPoint(childCollider);
            }
        }

        return GetFootPoint(fallbackCollider);
    }

    private static Vector2 GetFootPoint(Collider2D playerCollider)
    {
        if (playerCollider == null)
        {
            return Vector2.zero;
        }

        Bounds bounds = playerCollider.bounds;
        return new Vector2(bounds.center.x, bounds.min.y);
    }

    private static ExitEdge DetermineExitEdge(Bounds zoneBounds, Vector2 footPoint)
    {
        if (footPoint.y >= zoneBounds.max.y)
        {
            return ExitEdge.Top;
        }

        if (footPoint.y <= zoneBounds.min.y)
        {
            return ExitEdge.Bottom;
        }

        if (footPoint.x <= zoneBounds.min.x)
        {
            return ExitEdge.Left;
        }

        if (footPoint.x >= zoneBounds.max.x)
        {
            return ExitEdge.Right;
        }

        float topDistance = Mathf.Abs(zoneBounds.max.y - footPoint.y);
        float bottomDistance = Mathf.Abs(footPoint.y - zoneBounds.min.y);
        float leftDistance = Mathf.Abs(footPoint.x - zoneBounds.min.x);
        float rightDistance = Mathf.Abs(zoneBounds.max.x - footPoint.x);

        float smallestDistance = Mathf.Min(topDistance, bottomDistance, leftDistance, rightDistance);
        if (Mathf.Approximately(smallestDistance, topDistance))
        {
            return ExitEdge.Top;
        }

        if (Mathf.Approximately(smallestDistance, bottomDistance))
        {
            return ExitEdge.Bottom;
        }

        if (Mathf.Approximately(smallestDistance, leftDistance))
        {
            return ExitEdge.Left;
        }

        return ExitEdge.Right;
    }

    private static bool TryGetUnityLayer(string layerName, out int unityLayer)
    {
        unityLayer = LayerMask.NameToLayer(layerName);
        return unityLayer >= 0;
    }

    private static bool HasSortingLayer(string sortingLayerName)
    {
        SortingLayer[] layers = SortingLayer.layers;
        for (int i = 0; i < layers.Length; i++)
        {
            if (string.Equals(layers[i].name, sortingLayerName, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryResolvePlayerRoot(Collider2D other, out GameObject playerRoot)
    {
        playerRoot = null;
        if (other == null)
        {
            return false;
        }

        PlayerMovement movement = other.GetComponentInParent<PlayerMovement>();
        if (movement != null)
        {
            playerRoot = movement.gameObject;
            return true;
        }

        PlayerController playerController = other.GetComponentInParent<PlayerController>();
        if (playerController != null)
        {
            playerRoot = playerController.gameObject;
            return true;
        }

        PlayerAnimController playerAnim = other.GetComponentInParent<PlayerAnimController>();
        if (playerAnim != null)
        {
            playerRoot = playerAnim.gameObject;
            return true;
        }

        Transform taggedPlayer = other.transform.root;
        if (taggedPlayer != null && taggedPlayer.CompareTag("Player"))
        {
            playerRoot = taggedPlayer.gameObject;
            return true;
        }

        return false;
    }
}
