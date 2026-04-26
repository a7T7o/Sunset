using System;
using UnityEngine;

[Serializable]
public sealed class NpcResidentRuntimeSnapshot
{
    public string stableKey = string.Empty;
    public string sceneName = string.Empty;
    public string residentGroupName = string.Empty;
    public string residentGroupHierarchyPath = string.Empty;
    public string homeAnchorName = string.Empty;
    public string homeAnchorHierarchyPath = string.Empty;
    public bool hasHomeAnchor;
    public bool homeAnchorSceneOwned;
    public Vector2 residentPosition = Vector2.zero;
    public Vector2 homeAnchorPosition = Vector2.zero;
    public bool wasRoaming;
    public bool scriptedControlActive;
    public string scriptedControlOwnerKey = string.Empty;
    public bool resumeRoamWhenReleased;
}
