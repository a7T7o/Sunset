#if UNITY_INCLUDE_TESTS
using FarmGame.Data;
using FarmGame.Data.Core;
using FarmGame.World;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[TestFixture]
public class ChestInventoryBridgeTests
{
    private sealed class ChestHarness
    {
        public GameObject Root { get; }
        public StorageData Data { get; }
        public ChestController Controller { get; }

        public ChestHarness(int capacity, bool autoInitialize = true)
        {
            Root = new GameObject("ChestInventoryBridgeTests");
            Controller = Root.AddComponent<ChestController>();
            Data = ScriptableObject.CreateInstance<StorageData>();
            Data.storageCapacity = capacity;
            Data.storageRows = 1;
            Data.storageCols = capacity;
            Data.maxHealth = 2;
            Data.defaultLocked = false;

            if (autoInitialize)
            {
                Controller.Initialize(Data);
            }
        }

        public void Dispose()
        {
            Object.DestroyImmediate(Root);
            Object.DestroyImmediate(Data);
        }
    }

    [Test]
    public void LegacyInventoryWrite_SyncsToV2WithoutRecursiveOverflow()
    {
        var harness = new ChestHarness(8);
        try
        {
            var legacyStack = new ItemStack(1401, 0, 3);

            harness.Controller.Inventory.SetSlot(0, legacyStack);

            InventoryItem runtimeItem = harness.Controller.InventoryV2.GetItem(0);
            Assert.NotNull(runtimeItem);
            Assert.AreEqual(legacyStack.itemId, runtimeItem.ItemId);
            Assert.AreEqual(legacyStack.quality, runtimeItem.Quality);
            Assert.AreEqual(legacyStack.amount, runtimeItem.Amount);
        }
        finally
        {
            harness.Dispose();
        }
    }

    [Test]
    public void RuntimeInventoryWrite_RefreshesLegacyMirrorWithoutRecursiveOverflow()
    {
        var harness = new ChestHarness(8);
        try
        {
            var runtimeItem = new InventoryItem(1402, 1, 2);
            runtimeItem.SetDurability(20, 12);

            harness.Controller.InventoryV2.SetItem(1, runtimeItem);

            ItemStack legacyStack = harness.Controller.Inventory.GetSlot(1);
            Assert.AreEqual(runtimeItem.ItemId, legacyStack.itemId);
            Assert.AreEqual(runtimeItem.Quality, legacyStack.quality);
            Assert.AreEqual(runtimeItem.Amount, legacyStack.amount);
        }
        finally
        {
            harness.Dispose();
        }
    }

    [Test]
    public void MixedChestOperations_RemainConsistentAcrossRapidSetClearSequence()
    {
        var harness = new ChestHarness(6);
        try
        {
            for (int i = 0; i < 10; i++)
            {
                harness.Controller.Inventory.SetSlot(0, new ItemStack(1500 + i, 0, i + 1));
                harness.Controller.InventoryV2.ClearItem(0);
                harness.Controller.InventoryV2.SetItem(2, new InventoryItem(1600 + i, 0, 1));
                harness.Controller.Inventory.ClearSlot(2);
            }

            Assert.IsTrue(harness.Controller.Inventory.GetSlot(0).IsEmpty);
            Assert.IsTrue(harness.Controller.Inventory.GetSlot(2).IsEmpty);
            Assert.IsNull(harness.Controller.InventoryV2.GetItem(0));
            Assert.IsNull(harness.Controller.InventoryV2.GetItem(2));
        }
        finally
        {
            harness.Dispose();
        }
    }

    [Test]
    public void SaveLoad_RestoresAuthoritativeInventoryAndLegacyMirrorWithoutReintroducingBridgeLoop()
    {
        var source = new ChestHarness(8);
        var restored = new ChestHarness(8);
        try
        {
            var runtimeItem = new InventoryItem(1701, 2, 4);
            runtimeItem.SetDurability(40, 17);
            runtimeItem.SetProperty("seedRemaining", 9);
            runtimeItem.SetProperty("customName", "bridge-save-load");

            source.Controller.InventoryV2.SetItem(3, runtimeItem);
            source.Controller.Inventory.SetSlot(5, new ItemStack(1702, 1, 6));

            WorldObjectSaveData saveData = source.Controller.Save();
            source.Controller.Inventory.ClearSlot(3);
            source.Controller.InventoryV2.ClearItem(5);

            restored.Controller.Load(saveData);

            InventoryItem loadedRuntimeItem = restored.Controller.InventoryV2.GetItem(3);
            Assert.NotNull(loadedRuntimeItem);
            Assert.AreEqual(runtimeItem.ItemId, loadedRuntimeItem.ItemId);
            Assert.AreEqual(runtimeItem.Quality, loadedRuntimeItem.Quality);
            Assert.AreEqual(runtimeItem.Amount, loadedRuntimeItem.Amount);
            Assert.AreEqual(runtimeItem.MaxDurability, loadedRuntimeItem.MaxDurability);
            Assert.AreEqual(runtimeItem.CurrentDurability, loadedRuntimeItem.CurrentDurability);
            Assert.AreEqual("bridge-save-load", loadedRuntimeItem.GetProperty("customName"));
            Assert.AreEqual(9, loadedRuntimeItem.GetPropertyInt("seedRemaining", 0));

            ItemStack mirroredRuntimeStack = restored.Controller.Inventory.GetSlot(3);
            Assert.AreEqual(runtimeItem.ItemId, mirroredRuntimeStack.itemId);
            Assert.AreEqual(runtimeItem.Quality, mirroredRuntimeStack.quality);
            Assert.AreEqual(runtimeItem.Amount, mirroredRuntimeStack.amount);

            InventoryItem loadedLegacySyncedItem = restored.Controller.InventoryV2.GetItem(5);
            Assert.NotNull(loadedLegacySyncedItem);
            Assert.AreEqual(1702, loadedLegacySyncedItem.ItemId);
            Assert.AreEqual(1, loadedLegacySyncedItem.Quality);
            Assert.AreEqual(6, loadedLegacySyncedItem.Amount);

            ItemStack mirroredLegacyStack = restored.Controller.Inventory.GetSlot(5);
            Assert.AreEqual(1702, mirroredLegacyStack.itemId);
            Assert.AreEqual(1, mirroredLegacyStack.quality);
            Assert.AreEqual(6, mirroredLegacyStack.amount);
            Assert.IsFalse(restored.Controller.IsEmpty);
        }
        finally
        {
            source.Dispose();
            restored.Dispose();
        }
    }

    [Test]
    public void AuthoringPreset_InitializeSeedsRuntimeInventoryAndLegacyMirror()
    {
        var harness = new ChestHarness(8, autoInitialize: false);
        try
        {
            harness.Controller.SetAuthoringSlotsFromEditor(new List<InventorySlotSaveData>
            {
                new InventorySlotSaveData { slotIndex = 1, itemId = 1801, quality = 2, amount = 5 },
                new InventorySlotSaveData { slotIndex = 4, itemId = 1802, quality = 0, amount = 1 }
            });

            harness.Controller.Initialize(harness.Data);

            InventoryItem slotOne = harness.Controller.InventoryV2.GetItem(1);
            Assert.NotNull(slotOne);
            Assert.AreEqual(1801, slotOne.ItemId);
            Assert.AreEqual(2, slotOne.Quality);
            Assert.AreEqual(5, slotOne.Amount);

            ItemStack mirroredSlotOne = harness.Controller.Inventory.GetSlot(1);
            Assert.AreEqual(1801, mirroredSlotOne.itemId);
            Assert.AreEqual(2, mirroredSlotOne.quality);
            Assert.AreEqual(5, mirroredSlotOne.amount);

            InventoryItem slotFour = harness.Controller.InventoryV2.GetItem(4);
            Assert.NotNull(slotFour);
            Assert.AreEqual(1802, slotFour.ItemId);
            Assert.AreEqual(1, slotFour.Amount);
            Assert.IsFalse(harness.Controller.IsEmpty);
        }
        finally
        {
            harness.Dispose();
        }
    }

    [Test]
    public void AuthoringPreset_DoesNotOverrideLoadedEmptySave()
    {
        var harness = new ChestHarness(8, autoInitialize: false);
        try
        {
            harness.Controller.SetAuthoringSlotsFromEditor(new List<InventorySlotSaveData>
            {
                new InventorySlotSaveData { slotIndex = 2, itemId = 1901, quality = 1, amount = 3 }
            });

            var emptySave = new WorldObjectSaveData
            {
                guid = "chest-authoring-empty-save",
                objectType = "Chest",
                sceneName = "Tests",
                isActive = true,
                genericData = JsonUtility.ToJson(new ChestSaveData
                {
                    capacity = 8,
                    isLocked = false,
                    customName = "empty-save",
                    slots = new List<InventorySlotSaveData>()
                })
            };

            harness.Controller.Load(emptySave);
            harness.Controller.Initialize(harness.Data);

            Assert.IsTrue(harness.Controller.Inventory.GetSlot(2).IsEmpty);
            Assert.IsNull(harness.Controller.InventoryV2.GetItem(2));
            Assert.IsTrue(harness.Controller.IsEmpty);
        }
        finally
        {
            harness.Dispose();
        }
    }
}
#endif
