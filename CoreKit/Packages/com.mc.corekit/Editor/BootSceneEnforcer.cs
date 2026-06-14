using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace MC.CoreKit.Editor
{
    // [진입점 기둥 — 에디터 툴링]
    // 어느 씬에서 Play를 눌러도 '부트 씬'부터 시작되도록 강제. "Core 씬에서만 시작해야 한다"는
    // 제약을 없애 디자이너 마찰을 제거한다.
    //
    // Unity 네이티브 EditorSceneManager.playModeStartScene 사용 → 종료 시 원래 편집 씬 복구는
    // Unity가 자동 처리. (Rolice의 수동 EditorPrefs save/restore 패턴을 네이티브로 일반화·간소화.)
    //
    // 부트 씬 = Build Settings의 첫 번째(인덱스 0) 활성 씬. 메뉴로 on/off.
    [InitializeOnLoad]
    public static class BootSceneEnforcer
    {
        const string EnabledKey = "MC.CoreKit.BootSceneEnforcer.Enabled";
        const string MenuPath   = "Tools/CoreKit/Force Boot Scene On Play";

        static bool Enabled
        {
            get => EditorPrefs.GetBool(EnabledKey, true);
            set => EditorPrefs.SetBool(EnabledKey, value);
        }

        static BootSceneEnforcer()
        {
            Apply();
            EditorBuildSettings.sceneListChanged += Apply;
        }

        static void Apply()
        {
            if (!Enabled)
            {
                EditorSceneManager.playModeStartScene = null;
                return;
            }

            var bootPath = GetBootScenePath();
            EditorSceneManager.playModeStartScene = string.IsNullOrEmpty(bootPath)
                ? null
                : AssetDatabase.LoadAssetAtPath<SceneAsset>(bootPath);
        }

        // Build Settings 최상단의 활성 씬 = 부트 씬 (관례: 인덱스 0이 부트).
        static string GetBootScenePath()
        {
            foreach (var scene in EditorBuildSettings.scenes)
                if (scene.enabled)
                    return scene.path;
            return null;
        }

        [MenuItem(MenuPath)]
        static void Toggle()
        {
            Enabled = !Enabled;
            Apply();
            var boot = GetBootScenePath();
            Debug.Log($"[CoreKit] Force Boot Scene On Play = {Enabled}"
                      + (Enabled ? $" (boot: {boot})" : ""));
        }

        [MenuItem(MenuPath, validate = true)]
        static bool ToggleValidate()
        {
            Menu.SetChecked(MenuPath, Enabled);
            return true;
        }
    }
}
