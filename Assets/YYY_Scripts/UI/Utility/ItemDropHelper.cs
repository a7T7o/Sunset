using FarmGame.Data;
using FarmGame.Data.Core;
using UnityEngine;

namespace FarmGame.UI
{
    /// <summary>
    /// 统一处理玩家丢弃到世界中的物品生成逻辑。
    /// </summary>
    public static class ItemDropHelper
    {
        private static PlayerController _cachedPlayer;

        public static void DropAtPlayer(ItemStack item, float cooldown = 5f)
        {
            if (item.IsEmpty)
            {
                return;
            }

            Vector3 dropPos = GetPlayerDropPosition();
            if (dropPos == Vector3.zero)
            {
                Debug.LogError("[ItemDropHelper] 无法获取玩家位置，物品将丢失");
                return;
            }

            SpawnWorldItem(item, dropPos, cooldown);
        }

        public static void DropAtPlayer(InventoryItem item, float cooldown = 5f)
        {
            if (item == null || item.IsEmpty)
            {
                return;
            }

            Vector3 dropPos = GetPlayerDropPosition();
            if (dropPos == Vector3.zero)
            {
                Debug.LogError("[ItemDropHelper] 无法获取玩家位置，物品将丢失");
                return;
            }

            SpawnWorldItem(item, dropPos, cooldown);
        }

        public static void DropAtPosition(ItemStack item, Vector3 position, float cooldown = 5f)
        {
            if (item.IsEmpty)
            {
                return;
            }

            SpawnWorldItem(item, position, cooldown);
        }

        public static void DropAtPosition(InventoryItem item, Vector3 position, float cooldown = 5f)
        {
            if (item == null || item.IsEmpty)
            {
                return;
            }

            SpawnWorldItem(item, position, cooldown);
        }

        private static Vector3 GetPlayerDropPosition()
        {
            if (_cachedPlayer == null)
            {
                _cachedPlayer = Object.FindFirstObjectByType<PlayerController>();
            }

            if (_cachedPlayer == null)
            {
                Debug.LogError("[ItemDropHelper] 找不到 PlayerController");
                return Vector3.zero;
            }

            var collider = _cachedPlayer.GetComponent<Collider2D>();
            if (collider != null)
            {
                return collider.bounds.center;
            }

            return _cachedPlayer.transform.position;
        }

        private static void SpawnWorldItem(ItemStack item, Vector3 position, float cooldown)
        {
            if (WorldItemPool.Instance == null)
            {
                Debug.LogError("[ItemDropHelper] WorldItemPool.Instance 为 null，物品将丢失");
                return;
            }

            var pickup = WorldItemPool.Instance.SpawnById(
                item.itemId,
                item.quality,
                item.amount,
                position,
                playAnimation: true,
                setSpawnCooldown: false);

            if (pickup != null)
            {
                pickup.SetDropCooldown(cooldown);
            }
            else
            {
                Debug.LogError($"[ItemDropHelper] 生成物品失败: itemId={item.itemId}");
            }
        }

        private static void SpawnWorldItem(InventoryItem item, Vector3 position, float cooldown)
        {
            if (WorldItemPool.Instance == null)
            {
                Debug.LogError("[ItemDropHelper] WorldItemPool.Instance 为 null，物品将丢失");
                return;
            }

            var pickup = WorldItemPool.Instance.SpawnById(
                item.ItemId,
                item.Quality,
                item.Amount,
                position,
                playAnimation: true,
                setSpawnCooldown: false);

            if (pickup != null)
            {
                pickup.SetRuntimeItem(item, WorldItemPool.Instance.Database);
                pickup.SetDropCooldown(cooldown);
            }
            else
            {
                Debug.LogError($"[ItemDropHelper] 生成实例态物品失败: itemId={item.ItemId}");
            }
        }

        public static void ClearCache()
        {
            _cachedPlayer = null;
        }
    }
}
