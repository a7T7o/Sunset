using FarmGame.Data;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

[TestFixture]
public class PlacementExecutionTransactionTests
{
    [Test]
    public void Rollback_AfterSpawnAndInventoryCommit_DestroysObjectAndRestoresInventoryOnce()
    {
        var item = ScriptableObject.CreateInstance<StorageData>();
        var transaction = new PlacementExecutionTransaction(item, 2, Vector3.one, 4);
        var spawned = new GameObject("PlacementTxRollback");
        int restoreCalls = 0;
        int rollbackCalls = 0;

        try
        {
            transaction.MarkSpawned(spawned);
            transaction.MarkVisualReady();
            transaction.MarkInventoryCommitted();

            transaction.Rollback("unit-test", () => restoreCalls++, () => rollbackCalls++);

            Assert.AreEqual(1, restoreCalls);
            Assert.AreEqual(1, rollbackCalls);
            Assert.IsNull(transaction.SpawnedObject);
            Assert.IsFalse(transaction.VisualReady);
            Assert.IsFalse(transaction.InventoryCommitted);
            Assert.IsFalse(transaction.OccupancyCommitted);
            Assert.AreEqual("unit-test", transaction.LastRollbackReason);
        }
        finally
        {
            Object.DestroyImmediate(item);
            if (spawned != null)
            {
                Object.DestroyImmediate(spawned);
            }
        }
    }

    [Test]
    public void MarkStages_CanReachFullyCommittedState()
    {
        var item = ScriptableObject.CreateInstance<StorageData>();
        var transaction = new PlacementExecutionTransaction(item, 0, Vector3.zero, 1);
        var spawned = new GameObject("PlacementTxCommitted");

        try
        {
            transaction.MarkSpawned(spawned);
            transaction.MarkVisualReady();
            transaction.MarkInventoryCommitted();
            transaction.MarkOccupancyCommitted();

            Assert.IsTrue(transaction.IsFullyCommitted);
        }
        finally
        {
            Object.DestroyImmediate(spawned);
            Object.DestroyImmediate(item);
        }
    }
}
