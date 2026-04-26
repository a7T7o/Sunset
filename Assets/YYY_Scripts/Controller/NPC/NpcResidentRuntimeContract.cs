using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class NpcResidentRuntimeContract
{
    public static List<NpcResidentRuntimeSnapshot> CaptureSceneSnapshots(Scene scene, bool includeInactive = true)
    {
        List<NpcResidentRuntimeSnapshot> snapshots = new List<NpcResidentRuntimeSnapshot>();
        if (!scene.IsValid())
        {
            return snapshots;
        }

        GameObject[] roots = scene.GetRootGameObjects();
        for (int rootIndex = 0; rootIndex < roots.Length; rootIndex++)
        {
            GameObject root = roots[rootIndex];
            if (root == null)
            {
                continue;
            }

            NPCAutoRoamController[] controllers = root.GetComponentsInChildren<NPCAutoRoamController>(includeInactive);
            for (int index = 0; index < controllers.Length; index++)
            {
                NPCAutoRoamController controller = controllers[index];
                if (controller == null || !controller.IsNativeResidentRuntimeCandidate)
                {
                    continue;
                }

                snapshots.Add(controller.CaptureResidentRuntimeSnapshot());
            }
        }

        return snapshots;
    }

    public static bool TryApplySnapshot(Scene scene, NpcResidentRuntimeSnapshot snapshot, bool resumeResidentLogic = false)
    {
        if (!scene.IsValid() || snapshot == null || string.IsNullOrWhiteSpace(snapshot.stableKey))
        {
            return false;
        }

        if (!TryFindResident(scene, snapshot.stableKey, out NPCAutoRoamController controller))
        {
            return false;
        }

        controller.ApplyResidentRuntimeSnapshot(snapshot, resumeResidentLogic);
        return true;
    }

    public static bool TryFindResident(Scene scene, string stableKey, out NPCAutoRoamController controller)
    {
        controller = null;
        if (!scene.IsValid() || string.IsNullOrWhiteSpace(stableKey))
        {
            return false;
        }

        string normalizedStableKey = NPCDialogueContentProfile.NormalizeNpcId(stableKey);
        GameObject[] roots = scene.GetRootGameObjects();
        for (int rootIndex = 0; rootIndex < roots.Length; rootIndex++)
        {
            GameObject root = roots[rootIndex];
            if (root == null)
            {
                continue;
            }

            NPCAutoRoamController[] controllers = root.GetComponentsInChildren<NPCAutoRoamController>(includeInactive: true);
            for (int index = 0; index < controllers.Length; index++)
            {
                NPCAutoRoamController candidate = controllers[index];
                if (candidate == null)
                {
                    continue;
                }

                if (string.Equals(candidate.ResidentStableKey, normalizedStableKey, StringComparison.OrdinalIgnoreCase))
                {
                    controller = candidate;
                    return true;
                }
            }
        }

        return false;
    }

    public static Transform ResolveSceneTransform(Scene scene, string hierarchyPath, string fallbackName = "")
    {
        if (!scene.IsValid())
        {
            return null;
        }

        if (TryResolveSceneTransformByPath(scene, hierarchyPath, out Transform exactTransform))
        {
            return exactTransform;
        }

        return FindTransformByName(scene, fallbackName);
    }

    public static string BuildHierarchyPath(Transform target)
    {
        if (target == null)
        {
            return string.Empty;
        }

        Stack<string> segments = new Stack<string>();
        Transform current = target;
        while (current != null)
        {
            segments.Push(current.name);
            current = current.parent;
        }

        return string.Join("/", segments);
    }

    private static bool TryResolveSceneTransformByPath(Scene scene, string hierarchyPath, out Transform resolvedTransform)
    {
        resolvedTransform = null;
        if (!scene.IsValid() || string.IsNullOrWhiteSpace(hierarchyPath))
        {
            return false;
        }

        string[] segments = hierarchyPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length == 0)
        {
            return false;
        }

        GameObject[] roots = scene.GetRootGameObjects();
        for (int rootIndex = 0; rootIndex < roots.Length; rootIndex++)
        {
            GameObject root = roots[rootIndex];
            if (root == null || !string.Equals(root.name, segments[0], StringComparison.Ordinal))
            {
                continue;
            }

            Transform current = root.transform;
            bool failed = false;
            for (int segmentIndex = 1; segmentIndex < segments.Length; segmentIndex++)
            {
                current = current.Find(segments[segmentIndex]);
                if (current == null)
                {
                    failed = true;
                    break;
                }
            }

            if (!failed)
            {
                resolvedTransform = current;
                return true;
            }
        }

        return false;
    }

    private static Transform FindTransformByName(Scene scene, string targetName)
    {
        if (!scene.IsValid() || string.IsNullOrWhiteSpace(targetName))
        {
            return null;
        }

        GameObject[] roots = scene.GetRootGameObjects();
        for (int rootIndex = 0; rootIndex < roots.Length; rootIndex++)
        {
            Transform resolved = FindNamedChildRecursive(roots[rootIndex].transform, targetName);
            if (resolved != null)
            {
                return resolved;
            }
        }

        return null;
    }

    private static Transform FindNamedChildRecursive(Transform root, string targetName)
    {
        if (root == null)
        {
            return null;
        }

        if (string.Equals(root.name, targetName, StringComparison.Ordinal))
        {
            return root;
        }

        for (int index = 0; index < root.childCount; index++)
        {
            Transform resolved = FindNamedChildRecursive(root.GetChild(index), targetName);
            if (resolved != null)
            {
                return resolved;
            }
        }

        return null;
    }
}
