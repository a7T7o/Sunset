#if UNITY_EDITOR
using System;
using System.Reflection;
using Sunset.Story;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sunset.Story.Editor
{
    internal static class SpringDay1LatePhaseValidationMenu
    {
        private const string ForceDinnerMenuPath = "Sunset/Story/Validation/Force Spring Day1 Dinner Validation Jump";
        private const string ForceFreeTimeMenuPath = "Sunset/Story/Validation/Force Spring Day1 FreeTime Validation Jump";
        private const string AdvanceOneHourMenuPath = "Sunset/Story/Validation/Advance Spring Day1 Validation Clock +1h";
        private const string ForceMorningTownMenuPath = "Sunset/Story/Validation/Force Spring Day1 Morning Town Validation Jump";
        private const string TownSceneName = "Town";
        private const int MorningReleaseHour = 7;
        private static readonly BindingFlags InstanceMemberFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        [MenuItem(ForceDinnerMenuPath)]
        private static void ForceDinnerValidationJump()
        {
            if (!TryGetRuntime(out SpringDay1Director director, out StoryManager storyManager))
            {
                return;
            }

            PreparePostTutorialExploreWindow(director, storyManager);
            if (!TryLoadValidationScene(ref director, ref storyManager, TownSceneName))
            {
                Debug.LogWarning("[SpringDay1LatePhaseValidation] 无法同步切到 Town，晚饭验收入口未推进。");
                return;
            }

            InvokePrivate(director, "ActivateDinnerGatheringOnTownScene");
            Debug.Log("[SpringDay1LatePhaseValidation] 已强制推进到晚饭验收入口。");
        }

        [MenuItem(ForceFreeTimeMenuPath)]
        private static void ForceFreeTimeValidationJump()
        {
            if (!TryGetRuntime(out SpringDay1Director director, out StoryManager storyManager))
            {
                return;
            }

            PreparePostTutorialExploreWindow(director, storyManager);
            InvokePrivate(director, "EnterFreeTime");
            Debug.Log("[SpringDay1LatePhaseValidation] 已强制推进到自由时段验收入口。");
        }

        [MenuItem(AdvanceOneHourMenuPath)]
        private static void AdvanceOneHourValidationClock()
        {
            if (!TryGetRuntime(out SpringDay1Director director, out _))
            {
                return;
            }

            TimeManager timeManager = TimeManager.Instance;
            if (timeManager == null)
            {
                Debug.LogWarning("[SpringDay1LatePhaseValidation] 当前缺少 TimeManager。");
                return;
            }

            int targetHour = timeManager.GetHour() + 1;
            if (targetHour > 26)
            {
                timeManager.Sleep();
                Debug.Log($"[SpringDay1LatePhaseValidation] 已跨天推进到 {timeManager.GetFormattedTime()}。");
                return;
            }

            if (director.TryNormalizeDebugTimeTarget(
                    timeManager.GetYear(),
                    timeManager.GetSeason(),
                    timeManager.GetDay(),
                    targetHour,
                    0,
                    out int normalizedYear,
                    out SeasonManager.Season normalizedSeason,
                    out int normalizedDay,
                    out int normalizedHour,
                    out int normalizedMinute))
            {
                timeManager.SetTime(
                    normalizedYear,
                    normalizedSeason,
                    normalizedDay,
                    normalizedHour,
                    normalizedMinute);
            }
            else
            {
                timeManager.SetTime(
                    timeManager.GetYear(),
                    timeManager.GetSeason(),
                    timeManager.GetDay(),
                    targetHour,
                    0);
            }

            Debug.Log($"[SpringDay1LatePhaseValidation] 已推进验证时钟到 {timeManager.GetFormattedTime()}。");
        }

        [MenuItem(ForceMorningTownMenuPath)]
        private static void ForceMorningTownValidationJump()
        {
            if (!TryGetRuntime(out SpringDay1Director director, out StoryManager storyManager))
            {
                return;
            }

            if (storyManager.CurrentPhase != StoryPhase.DayEnd)
            {
                Debug.LogWarning("[SpringDay1LatePhaseValidation] 请先把 Day1 收束到 DayEnd，再触发次晨 Town 验证入口。");
                return;
            }

            TimeManager timeManager = TimeManager.Instance;
            if (timeManager == null)
            {
                Debug.LogWarning("[SpringDay1LatePhaseValidation] 当前缺少 TimeManager。");
                return;
            }

            timeManager.SetTime(
                timeManager.GetYear(),
                timeManager.GetSeason(),
                timeManager.GetDay(),
                MorningReleaseHour,
                0);

            if (!TryLoadValidationScene(ref director, ref storyManager, TownSceneName))
            {
                Debug.LogWarning("[SpringDay1LatePhaseValidation] 无法同步切到 Town，次晨验证入口未推进。");
                return;
            }

            InvokePrivate(director, "UpdateSceneStoryNpcVisibility");
            SpringDay1NpcCrowdDirector.ForceImmediateSync();
            Physics2D.SyncTransforms();

            Debug.Log("[SpringDay1LatePhaseValidation] 已强制推进到次晨 07:00 Town 验证入口。");
        }

        [MenuItem(ForceDinnerMenuPath, true)]
        [MenuItem(ForceFreeTimeMenuPath, true)]
        [MenuItem(AdvanceOneHourMenuPath, true)]
        [MenuItem(ForceMorningTownMenuPath, true)]
        private static bool ValidateMenus()
        {
            return !EditorApplication.isCompiling && !EditorApplication.isUpdating;
        }

        private static bool TryGetRuntime(out SpringDay1Director director, out StoryManager storyManager)
        {
            director = SpringDay1Director.Instance ?? UnityEngine.Object.FindFirstObjectByType<SpringDay1Director>(FindObjectsInactive.Include);
            storyManager = StoryManager.Instance ?? UnityEngine.Object.FindFirstObjectByType<StoryManager>(FindObjectsInactive.Include);

            if (!Application.isPlaying)
            {
                Debug.LogWarning("[SpringDay1LatePhaseValidation] 请先进入 PlayMode 再触发晚段验收跳转。");
                return false;
            }

            if (director == null || storyManager == null)
            {
                Debug.LogWarning("[SpringDay1LatePhaseValidation] 当前缺少 Day1Director 或 StoryManager。");
                return false;
            }

            return true;
        }

        private static void PreparePostTutorialExploreWindow(SpringDay1Director director, StoryManager storyManager)
        {
            SetPrivateField(director, "_tillObjectiveCompleted", true);
            SetPrivateField(director, "_plantObjectiveCompleted", true);
            SetPrivateField(director, "_waterObjectiveCompleted", true);
            SetPrivateField(director, "_woodObjectiveCompleted", true);
            SetPrivateField(director, "_craftedCount", Math.Max(1, GetPrivateField<int>(director, "requiredCraftedCount")));
            SetPrivateField(director, "_postTutorialWrapSequenceQueued", true);
            SetPrivateField(director, "_postTutorialExploreWindowPending", false);
            SetPrivateField(director, "_postTutorialExploreWindowEarliestEnterAt", -1f);

            storyManager.SetPhase(StoryPhase.FarmingTutorial);
            InvokePrivate(director, "EnterPostTutorialExploreWindow");
        }

        private static bool TryLoadValidationScene(
            ref SpringDay1Director director,
            ref StoryManager storyManager,
            string targetSceneName)
        {
            Scene activeScene = SceneManager.GetActiveScene();
            if (string.Equals(activeScene.name, targetSceneName, StringComparison.Ordinal))
            {
                return true;
            }

            if (director == null)
            {
                return false;
            }

            InvokePrivateStatic(typeof(SpringDay1Director), "LoadSceneThroughPersistentBridge", targetSceneName);

            director = SpringDay1Director.Instance ?? UnityEngine.Object.FindFirstObjectByType<SpringDay1Director>(FindObjectsInactive.Include);
            storyManager = StoryManager.Instance ?? UnityEngine.Object.FindFirstObjectByType<StoryManager>(FindObjectsInactive.Include);
            activeScene = SceneManager.GetActiveScene();

            return director != null
                && storyManager != null
                && string.Equals(activeScene.name, targetSceneName, StringComparison.Ordinal);
        }

        private static void InvokePrivate(object target, string methodName, params object[] args)
        {
            MethodInfo method = target.GetType().GetMethod(methodName, InstanceMemberFlags);
            if (method == null)
            {
                throw new MissingMethodException(target.GetType().FullName, methodName);
            }

            method.Invoke(target, args);
        }

        private static void InvokePrivateStatic(Type targetType, string methodName, params object[] args)
        {
            MethodInfo method = targetType.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (method == null)
            {
                throw new MissingMethodException(targetType.FullName, methodName);
            }

            method.Invoke(null, args);
        }

        private static T InvokePrivate<T>(object target, string methodName, params object[] args)
        {
            MethodInfo method = target.GetType().GetMethod(methodName, InstanceMemberFlags);
            if (method == null)
            {
                throw new MissingMethodException(target.GetType().FullName, methodName);
            }

            object result = method.Invoke(target, args);
            return result is T typed ? typed : default;
        }

        private static T GetPrivateField<T>(object target, string fieldName)
        {
            FieldInfo field = target.GetType().GetField(fieldName, InstanceMemberFlags);
            if (field == null)
            {
                return default;
            }

            object value = field.GetValue(target);
            return value is T typed ? typed : default;
        }

        private static void SetPrivateField(object target, string fieldName, object value)
        {
            FieldInfo field = target.GetType().GetField(fieldName, InstanceMemberFlags);
            if (field == null)
            {
                throw new MissingFieldException(target.GetType().FullName, fieldName);
            }

            field.SetValue(target, value);
        }
    }
}
#endif
