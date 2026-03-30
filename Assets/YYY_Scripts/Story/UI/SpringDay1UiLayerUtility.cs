using FarmGame.UI;
using UnityEngine;

namespace Sunset.Story
{
    public static class SpringDay1UiLayerUtility
    {
        public static bool IsBlockingPageUiOpen()
        {
            PackagePanelTabsUI packageTabs = Object.FindFirstObjectByType<PackagePanelTabsUI>(FindObjectsInactive.Include);
            bool packageOpen = packageTabs != null && packageTabs.IsPanelOpen();
            bool boxOpen = BoxPanelUI.ActiveInstance != null && BoxPanelUI.ActiveInstance.IsOpen;
            return packageOpen || boxOpen;
        }

        public static Transform ResolveUiParent()
        {
            GameObject uiRoot = GameObject.Find("UI");
            if (uiRoot != null)
            {
                Canvas canvas = uiRoot.GetComponent<Canvas>() ?? uiRoot.GetComponentInChildren<Canvas>(true);
                return canvas != null ? canvas.transform : uiRoot.transform;
            }

            Canvas fallbackCanvas = Object.FindFirstObjectByType<Canvas>(FindObjectsInactive.Include);
            return fallbackCanvas != null ? fallbackCanvas.transform : null;
        }

        public static Camera GetWorldProjectionCamera(Canvas canvas = null)
        {
            if (canvas != null)
            {
                Canvas rootCanvas = canvas.rootCanvas != null ? canvas.rootCanvas : canvas;
                if (rootCanvas.renderMode == RenderMode.ScreenSpaceCamera && rootCanvas.worldCamera != null)
                {
                    return rootCanvas.worldCamera;
                }

                if (rootCanvas.renderMode == RenderMode.WorldSpace && rootCanvas.worldCamera != null)
                {
                    return rootCanvas.worldCamera;
                }
            }

            return Camera.main ?? Object.FindFirstObjectByType<Camera>(FindObjectsInactive.Include);
        }

        public static Camera GetUiEventCamera(Canvas canvas)
        {
            if (canvas == null)
            {
                return null;
            }

            Canvas rootCanvas = canvas.rootCanvas != null ? canvas.rootCanvas : canvas;
            return rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : rootCanvas.worldCamera;
        }

        public static Vector2 SnapToCanvasPixel(Canvas canvas, Vector2 localPoint)
        {
            Canvas rootCanvas = canvas != null && canvas.rootCanvas != null ? canvas.rootCanvas : canvas;
            float scaleFactor = rootCanvas != null && rootCanvas.scaleFactor > 0.001f
                ? rootCanvas.scaleFactor
                : 1f;

            return new Vector2(
                Mathf.Round(localPoint.x * scaleFactor) / scaleFactor,
                Mathf.Round(localPoint.y * scaleFactor) / scaleFactor);
        }

        public static T EnsureComponent<T>(GameObject gameObject) where T : Component
        {
            if (gameObject == null)
            {
                return null;
            }

            T component = gameObject.GetComponent<T>();
            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }

            return component;
        }

        public static bool TryGetPresentationBounds(Transform target, out Bounds bounds)
        {
            bounds = default;
            if (target == null)
            {
                return false;
            }

            if (target.TryGetComponent(out CraftingStationInteractable craftingInteractable))
            {
                bounds = craftingInteractable.GetVisualBounds();
                return true;
            }

            SpriteRenderer[] renderers = target.GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
            bool hasRenderer = false;
            for (int index = 0; index < renderers.Length; index++)
            {
                SpriteRenderer renderer = renderers[index];
                if (renderer == null || !renderer.enabled || renderer.sprite == null)
                {
                    continue;
                }

                if (!hasRenderer)
                {
                    bounds = renderer.bounds;
                    hasRenderer = true;
                }
                else
                {
                    bounds.Encapsulate(renderer.bounds);
                }
            }

            if (hasRenderer)
            {
                return true;
            }

            Collider2D[] colliders = target.GetComponentsInChildren<Collider2D>(includeInactive: true);
            bool hasCollider = false;
            for (int index = 0; index < colliders.Length; index++)
            {
                Collider2D collider2D = colliders[index];
                if (collider2D == null)
                {
                    continue;
                }

                if (!hasCollider)
                {
                    bounds = collider2D.bounds;
                    hasCollider = true;
                }
                else
                {
                    bounds.Encapsulate(collider2D.bounds);
                }
            }

            if (hasCollider)
            {
                return true;
            }

            bounds = new Bounds(target.position, Vector3.one);
            return true;
        }

        public static Vector2 GetInteractionSamplePoint(Transform target)
        {
            return TryGetInteractionSamplePoint(target, out Vector2 samplePoint)
                ? samplePoint
                : target != null ? (Vector2)target.position : Vector2.zero;
        }

        public static bool TryGetInteractionSamplePoint(Transform target, out Vector2 samplePoint)
        {
            samplePoint = Vector2.zero;
            if (target == null)
            {
                return false;
            }

            if (TryGetColliderBounds(target, out Bounds colliderBounds))
            {
                samplePoint = new Vector2(colliderBounds.center.x, colliderBounds.min.y + 0.02f);
                return true;
            }

            if (TryGetPresentationBounds(target, out Bounds presentationBounds))
            {
                samplePoint = new Vector2(presentationBounds.center.x, presentationBounds.min.y + 0.02f);
                return true;
            }

            samplePoint = target.position;
            return true;
        }

        private static bool TryGetColliderBounds(Transform target, out Bounds bounds)
        {
            bounds = default;
            if (target == null)
            {
                return false;
            }

            Collider2D[] colliders = target.GetComponentsInChildren<Collider2D>(includeInactive: true);
            bool hasCollider = false;
            for (int index = 0; index < colliders.Length; index++)
            {
                Collider2D collider2D = colliders[index];
                if (collider2D == null || !collider2D.enabled)
                {
                    continue;
                }

                if (!hasCollider)
                {
                    bounds = collider2D.bounds;
                    hasCollider = true;
                }
                else
                {
                    bounds.Encapsulate(collider2D.bounds);
                }
            }

            return hasCollider;
        }
    }
}
