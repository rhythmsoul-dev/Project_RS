using UnityEngine;
using UnityEditor;

public class TimeScaleEditor : EditorWindow
{
    [MenuItem("Tools/Time Scale Controller")]
    public static void ShowWindow()
    {
        GetWindow<TimeScaleEditor>("Time Scale Controller");
    }

    private void OnGUI()
    {
        GUILayout.Label("Time Scale Controller", EditorStyles.boldLabel);

        Time.timeScale = EditorGUILayout.Slider("Time Scale", Time.timeScale, 0f, 20f);

        if (GUILayout.Button("초기화"))
        {
            Time.timeScale = 1f;
        }

        GUILayout.Space(10);
        GUILayout.Label($"현재 TimeScale: {Time.timeScale}", EditorStyles.helpBox);
    }

    private void OnInspectorUpdate()
    {
        Repaint();
    }
}
