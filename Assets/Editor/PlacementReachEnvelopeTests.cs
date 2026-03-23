using FarmGame.Data;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

[TestFixture]
public class PlacementReachEnvelopeTests
{
    [Test]
    public void PreviewBounds_IncludeVisiblePrefabEnvelopeInsteadOfOnlyGridCells()
    {
        var previewRoot = new GameObject("PlacementReachEnvelopePreview");
        var preview = previewRoot.AddComponent<PlacementPreview>();

        var placementPrefab = new GameObject("PlacementReachEnvelopePrefab");
        var spriteChild = new GameObject("Sprite");
        spriteChild.transform.SetParent(placementPrefab.transform, false);
        var spriteRenderer = spriteChild.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateSolidSprite(64, 128);

        var collider = spriteChild.AddComponent<BoxCollider2D>();
        collider.size = Vector2.one;

        var data = ScriptableObject.CreateInstance<StorageData>();
        data.itemName = "ReachEnvelopeChest";
        data.storageCapacity = 4;
        data.storageRows = 1;
        data.storageCols = 4;
        data.storagePrefab = placementPrefab;

        try
        {
            preview.Show(data, Vector2Int.one);
            preview.ForceUpdatePosition(Vector3.zero);

            Bounds visualBounds = preview.GetVisualPreviewBounds();
            Bounds interactionBounds = preview.GetPreviewBounds();

            Assert.That(visualBounds.size, Is.EqualTo(Vector3.one));
            Assert.Greater(interactionBounds.size.y, visualBounds.size.y);
            Assert.GreaterOrEqual(interactionBounds.size.x, visualBounds.size.x);
            Assert.That(interactionBounds.Contains(new Vector3(0.5f, 1.25f, 0f)), Is.True);
        }
        finally
        {
            Object.DestroyImmediate(data);
            Object.DestroyImmediate(placementPrefab);
            Object.DestroyImmediate(previewRoot);
        }
    }

    private static Sprite CreateSolidSprite(int width, int height)
    {
        var texture = new Texture2D(width, height);
        var pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }

        texture.SetPixels(pixels);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 64f);
    }
}
