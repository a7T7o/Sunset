using FarmGame.UI;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        public static bool ShouldHidePromptOverlayForParentModalUi()
        {
            if (IsBlockingPageUiOpen())
            {
                return true;
            }

            SpringDay1WorkbenchCraftingOverlay workbenchOverlay = Object.FindFirstObjectByType<SpringDay1WorkbenchCraftingOverlay>(FindObjectsInactive.Include);
            return workbenchOverlay != null && workbenchOverlay.IsVisible;
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
            if (TryGetCanvasWorldCamera(canvas, out Camera canvasWorldCamera))
            {
                return canvasWorldCamera;
            }

            Scene preferredScene = canvas != null ? canvas.gameObject.scene : default;
            return ResolveSceneWorldCamera(preferredScene);
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

        public static bool TryProjectWorldToCanvas(
            Canvas canvas,
            RectTransform rootRect,
            Vector3 worldPoint,
            Vector2 screenOffset,
            out Vector2 localPoint)
        {
            localPoint = default;
            if (rootRect == null || !TryProjectWorldToScreen(canvas, worldPoint, screenOffset, out Vector2 screenPoint))
            {
                return false;
            }

            return RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rootRect,
                screenPoint,
                GetUiEventCamera(canvas),
                out localPoint);
        }

        public static bool TryProjectWorldToScreen(
            Canvas canvas,
            Vector3 worldPoint,
            Vector2 screenOffset,
            out Vector2 screenPoint)
        {
            return TryProjectWorldToScreen(GetWorldProjectionCamera(canvas), worldPoint, screenOffset, out screenPoint);
        }

        public static bool TryProjectWorldToScreen(
            Camera worldCamera,
            Vector3 worldPoint,
            Vector2 screenOffset,
            out Vector2 screenPoint)
        {
            screenPoint = default;
            if (!IsUsableWorldCamera(worldCamera))
            {
                return false;
            }

            Vector3 viewportPoint = worldCamera.WorldToViewportPoint(worldPoint);
            if (!IsFinite(viewportPoint) ||
                viewportPoint.z <= 0f ||
                viewportPoint.x < 0f ||
                viewportPoint.x > 1f ||
                viewportPoint.y < 0f ||
                viewportPoint.y > 1f)
            {
                return false;
            }

            Vector3 rawScreenPoint = worldCamera.WorldToScreenPoint(worldPoint);
            if (!IsFinite(rawScreenPoint))
            {
                return false;
            }

            screenPoint = new Vector2(rawScreenPoint.x, rawScreenPoint.y) + screenOffset;
            return IsFinite(screenPoint);
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

        private static bool TryGetCanvasWorldCamera(Canvas canvas, out Camera worldCamera)
        {
            worldCamera = null;
            if (canvas == null)
            {
                return false;
            }

            Canvas rootCanvas = canvas.rootCanvas != null ? canvas.rootCanvas : canvas;
            if ((rootCanvas.renderMode == RenderMode.ScreenSpaceCamera || rootCanvas.renderMode == RenderMode.WorldSpace) &&
                IsUsableWorldCamera(rootCanvas.worldCamera))
            {
                worldCamera = rootCanvas.worldCamera;
                return true;
            }

            return false;
        }

        private static Camera ResolveSceneWorldCamera(Scene preferredScene)
        {
            Camera bestCamera = null;
            int bestScore = int.MinValue;
            Scene activeScene = SceneManager.GetActiveScene();
            Camera[] cameras = Object.FindObjectsByType<Camera>(FindObjectsSortMode.None);
            for (int index = 0; index < cameras.Length; index++)
            {
                Camera candidate = cameras[index];
                if (!IsUsableWorldCamera(candidate))
                {
                    continue;
                }

                int score = ScoreWorldCamera(candidate, preferredScene, activeScene);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestCamera = candidate;
                }
            }

            return bestCamera;
        }

        private static bool IsUsableWorldCamera(Camera candidate)
        {
            if (candidate == null || !candidate.enabled || candidate.targetTexture != null)
            {
                return false;
            }

            GameObject cameraObject = candidate.gameObject;
            Scene scene = cameraObject.scene;
            return cameraObject.activeInHierarchy && scene.IsValid() && scene.isLoaded;
        }

        private static int ScoreWorldCamera(Camera candidate, Scene preferredScene, Scene activeScene)
        {
            int score = 0;
            Scene candidateScene = candidate.gameObject.scene;
            if (preferredScene.IsValid() && candidateScene == preferredScene)
            {
                score += 180;
            }

            if (activeScene.IsValid() && candidateScene == activeScene)
            {
                score += 140;
            }

            if (candidate.CompareTag("MainCamera"))
            {
                score += 220;
            }

            CinemachineBrain brain = candidate.GetComponent<CinemachineBrain>();
            if (brain != null && brain.enabled)
            {
                score += 200;
            }

            score += Mathf.RoundToInt(Mathf.Clamp(candidate.depth, -20f, 20f));
            return score;
        }

        private static bool IsFinite(Vector2 value)
        {
            return float.IsFinite(value.x) && float.IsFinite(value.y);
        }

        private static bool IsFinite(Vector3 value)
        {
            return float.IsFinite(value.x) && float.IsFinite(value.y) && float.IsFinite(value.z);
        }
    }
}
