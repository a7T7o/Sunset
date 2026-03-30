using UnityEngine;

namespace Sunset.Story
{
    [CreateAssetMenu(fileName = "SpringDay1UiPrefabRegistry", menuName = "Sunset/Story/Spring Day1 UI Prefab Registry")]
    public sealed class SpringDay1UiPrefabRegistry : ScriptableObject
    {
        private const string ResourcePath = "Story/SpringDay1UiPrefabRegistry";
#if UNITY_EDITOR
        private const string AssetPath = "Assets/Resources/Story/SpringDay1UiPrefabRegistry.asset";
#endif

        [SerializeField] private GameObject promptOverlayPrefab;
        [SerializeField] private GameObject workbenchOverlayPrefab;

        private static SpringDay1UiPrefabRegistry _cachedRegistry;

        public static GameObject LoadPromptOverlayPrefab()
        {
            SpringDay1UiPrefabRegistry registry = Load();
            return registry != null ? registry.promptOverlayPrefab : null;
        }

        public static GameObject LoadWorkbenchOverlayPrefab()
        {
            SpringDay1UiPrefabRegistry registry = Load();
            return registry != null ? registry.workbenchOverlayPrefab : null;
        }

        private static SpringDay1UiPrefabRegistry Load()
        {
            if (_cachedRegistry != null)
            {
                return _cachedRegistry;
            }

            _cachedRegistry = Resources.Load<SpringDay1UiPrefabRegistry>(ResourcePath);
#if UNITY_EDITOR
            if (_cachedRegistry == null)
            {
                _cachedRegistry = UnityEditor.AssetDatabase.LoadAssetAtPath<SpringDay1UiPrefabRegistry>(AssetPath);
            }
#endif
            return _cachedRegistry;
        }
    }
}
