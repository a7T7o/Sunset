#if UNITY_INCLUDE_TESTS
using System.Collections.Generic;
using FarmGame.Data;
using FarmGame.Data.Core;
using FarmGame.World;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[TestFixture]
public class ChestAuthoringSerializationTests
{
    private sealed class ChestHarness
    {
        public GameObject Root { get; }
        public StorageData Data { get; }
        public ChestController Controller { get; }

        public ChestHarness(int capacity)
        {
            Root = new GameObject("ChestAuthoringSerializationTests");
            Controller = Root.AddComponent<ChestController>();
            Data = ScriptableObject.CreateInstance<StorageData>();
            Data.storageCapacity = capacity;
            Data.storageRows = 1;
            Data.storageCols = capacity;
            Data.maxHealth = 1;
            Data.defaultLocked = false;
        }

        public void Dispose()
        {
            Object.DestroyImmediate(Root);
            Object.DestroyImmediate(Data);
        }
    }

    [Test]
    public void AuthoringPreset_PrefabSerializationRoundTripPreservesConfiguredSlots()
    {
        var harness = new ChestHarness(8);
        const string TempFolder = "Assets/__ChestAuthoringTempTests";
        const string TempPrefabPath = "Assets/__ChestAuthoringTempTests/ChestAuthoringRoundTrip.prefab";

        try
        {
            harness.Controller.SetAuthoringSlotsFromEditor(new List<InventorySlotSaveData>
            {
                new InventorySlotSaveData { slotIndex = 0, itemId = 2001, quality = 3, amount = 4 },
                new InventorySlotSaveData { slotIndex = 5, itemId = 2002, quality = 1, amount = 2 }
            });

            if (!AssetDatabase.IsValidFolder(TempFolder))
            {
                AssetDatabase.CreateFolder("Assets", "__ChestAuthoringTempTests");
            }

            GameObject prefabAsset = PrefabUtility.SaveAsPrefabAsset(harness.Root, TempPrefabPath);
            Assert.NotNull(prefabAsset);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            GameObject reloadedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(TempPrefabPath);
            Assert.NotNull(reloadedPrefab);

            ChestController reloadedController = reloadedPrefab.GetComponent<ChestController>();
            Assert.NotNull(reloadedController);

            IReadOnlyList<InventorySlotSaveData> snapshot = reloadedController.GetAuthoringSlotsSnapshot();
            Assert.AreEqual(2, snapshot.Count);

            Assert.AreEqual(0, snapshot[0].slotIndex);
            Assert.AreEqual(2001, snapshot[0].itemId);
            Assert.AreEqual(3, snapshot[0].quality);
            Assert.AreEqual(4, snapshot[0].amount);

            Assert.AreEqual(5, snapshot[1].slotIndex);
            Assert.AreEqual(2002, snapshot[1].itemId);
            Assert.AreEqual(1, snapshot[1].quality);
            Assert.AreEqual(2, snapshot[1].amount);
        }
        finally
        {
            harness.Dispose();
            AssetDatabase.DeleteAsset(TempPrefabPath);
            AssetDatabase.DeleteAsset(TempFolder);
            AssetDatabase.Refresh();
        }
    }
}
#endif
