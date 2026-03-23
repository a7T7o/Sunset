using System;
using System.Threading;
using UnityEngine;
using FarmGame.Data;

/// <summary>
/// 放置事务对象。
/// 统一记录一次放置的关键提交阶段，便于在半提交时做最小回滚。
/// </summary>
public sealed class PlacementExecutionTransaction
{
    private static int nextTransactionId;
    private readonly Action<string> logger;

    public int TransactionId { get; }
    public ItemData ItemData { get; }
    public int ItemId { get; }
    public int Quality { get; }
    public Vector3 LockedPosition { get; }
    public int SlotIndex { get; }
    public GameObject SpawnedObject { get; private set; }
    public bool VisualReady { get; private set; }
    public bool InventoryCommitted { get; private set; }
    public bool OccupancyCommitted { get; private set; }
    public string LastRollbackReason { get; private set; }

    public bool IsFullyCommitted =>
        SpawnedObject != null &&
        VisualReady &&
        InventoryCommitted &&
        OccupancyCommitted;

    public PlacementExecutionTransaction(ItemData itemData, int quality, Vector3 lockedPosition, int slotIndex, Action<string> logger = null)
    {
        ItemData = itemData;
        ItemId = itemData != null ? itemData.itemID : -1;
        Quality = quality;
        LockedPosition = lockedPosition;
        SlotIndex = slotIndex;
        this.logger = logger;
        TransactionId = Interlocked.Increment(ref nextTransactionId);

        Log($"tx#{TransactionId} begin item={itemData?.itemName ?? "null"} slot={slotIndex} pos={lockedPosition}");
    }

    public void MarkSpawned(GameObject spawnedObject)
    {
        SpawnedObject = spawnedObject;
        Log($"tx#{TransactionId} spawned={spawnedObject?.name ?? "null"}");
    }

    public void MarkVisualReady()
    {
        VisualReady = true;
        Log($"tx#{TransactionId} visual-ready");
    }

    public void MarkInventoryCommitted()
    {
        InventoryCommitted = true;
        Log($"tx#{TransactionId} inventory-committed");
    }

    public void MarkOccupancyCommitted()
    {
        OccupancyCommitted = true;
        Log($"tx#{TransactionId} occupancy-committed");
    }

    public void Rollback(string reason, Action restoreInventory = null, Action rollbackOccupancy = null)
    {
        LastRollbackReason = reason;
        Log($"tx#{TransactionId} rollback reason={reason}");

        rollbackOccupancy?.Invoke();

        if (SpawnedObject != null)
        {
            DestroyUnityObject(SpawnedObject);
            SpawnedObject = null;
        }

        if (InventoryCommitted)
        {
            restoreInventory?.Invoke();
            InventoryCommitted = false;
        }

        VisualReady = false;
        OccupancyCommitted = false;
    }

    private void Log(string message)
    {
        logger?.Invoke(message);
    }

    private static void DestroyUnityObject(GameObject target)
    {
        if (target == null)
        {
            return;
        }

        if (Application.isPlaying)
        {
            UnityEngine.Object.Destroy(target);
            return;
        }

        UnityEngine.Object.DestroyImmediate(target);
    }
}
