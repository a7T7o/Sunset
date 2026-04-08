using FarmGame.Data;
using FarmGame.Data.Core;
using UnityEngine;

public static class AssetLocator
{
    private const string DefaultItemDatabaseResourcesPath = "Data/Database/MasterItemDatabase";
    private const string DefaultPrefabDatabaseResourcesPath = "Data/Database/PrefabDatabase";
    private const string DefaultDayNightConfigResourcesPath = "DayNightConfig";

    // 运行时优先走 Resources；找不到时再从当前已加载资产里回收，避免 build 只能靠 Editor fallback。
    public static ItemDatabase LoadItemDatabase(string resourcesPath = DefaultItemDatabaseResourcesPath)
    {
        ItemDatabase database = LoadFromResourcesOrLoadedAssets<ItemDatabase>(resourcesPath, "MasterItemDatabase");
#if UNITY_EDITOR
        if (database == null)
        {
            database = LoadFirstEditorAsset<ItemDatabase>("t:ItemDatabase MasterItemDatabase", "t:ItemDatabase");
        }
#endif
        return database;
    }

    public static PrefabDatabase LoadPrefabDatabase(string resourcesPath = DefaultPrefabDatabaseResourcesPath)
    {
        PrefabDatabase database = LoadFromResourcesOrLoadedAssets<PrefabDatabase>(resourcesPath, "PrefabDatabase");
#if UNITY_EDITOR
        if (database == null)
        {
            database = LoadFirstEditorAsset<PrefabDatabase>("t:PrefabDatabase");
        }
#endif
        return database;
    }

    public static DayNightConfig LoadDayNightConfig(string resourcesPath = DefaultDayNightConfigResourcesPath)
    {
        DayNightConfig config = LoadFromResourcesOrLoadedAssets<DayNightConfig>(resourcesPath, "DayNightConfig");
#if UNITY_EDITOR
        if (config == null)
        {
            config = LoadFirstEditorAsset<DayNightConfig>("t:DayNightConfig");
        }
#endif
        return config;
    }

    private static T LoadFromResourcesOrLoadedAssets<T>(string resourcesPath, string preferredName)
        where T : Object
    {
        if (!string.IsNullOrWhiteSpace(resourcesPath))
        {
            T fromResources = Resources.Load<T>(resourcesPath);
            if (fromResources != null)
            {
                return fromResources;
            }
        }

        T preferred = null;
        T[] loadedAssets = Resources.FindObjectsOfTypeAll<T>();
        for (int index = 0; index < loadedAssets.Length; index++)
        {
            T candidate = loadedAssets[index];
            if (candidate == null)
            {
                continue;
            }

            if (preferred == null)
            {
                preferred = candidate;
            }

            if (!string.IsNullOrWhiteSpace(preferredName) &&
                string.Equals(candidate.name, preferredName, System.StringComparison.Ordinal))
            {
                return candidate;
            }
        }

        return preferred;
    }

#if UNITY_EDITOR
    private static T LoadFirstEditorAsset<T>(params string[] findAssetQueries)
        where T : Object
    {
        for (int queryIndex = 0; queryIndex < findAssetQueries.Length; queryIndex++)
        {
            string query = findAssetQueries[queryIndex];
            if (string.IsNullOrWhiteSpace(query))
            {
                continue;
            }

            string[] guids = UnityEditor.AssetDatabase.FindAssets(query);
            if (guids == null || guids.Length == 0)
            {
                continue;
            }

            for (int guidIndex = 0; guidIndex < guids.Length; guidIndex++)
            {
                string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[guidIndex]);
                T asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null)
                {
                    return asset;
                }
            }
        }

        return null;
    }
#endif
}
