using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class DayTimeEditorWindow : EditorWindow
{
    private DayTimeManager script;
    private SerializedObject serialized;
    
    
    [MenuItem("Custom/Day Time Editor")]
    public static void ShowWindow()
    {
        GetWindow<DayTimeEditorWindow>("Configure say time settings");
    }

    private void OnGUI()
    {
        if (!script || script != FindObjectOfType<DayTimeManager>())
            if (!Initialize())
                return;
        
        serialized = new SerializedObject(script);
        GUILayout.Label("Day settings");
        script.skyboxDay = (Material)
            EditorGUILayout.ObjectField("Skybox material", script.skyboxDay, typeof(Material), false);
        script.skyboxIntensityDay = EditorGUILayout.FloatField("Skybox day light", script.skyboxIntensityDay);
        EditorGUILayout.PropertyField(serialized.FindProperty("enabledAtDay"), true);
        if (GUILayout.Button("Select directional Day light"))
            SelectDirectionalLight(true);
        if (GUILayout.Button("Change to Day"))
            script.StartDay();
        
        GUILayout.Space(12);
        GUILayout.Label("Night settings");
        script.skyboxNight = (Material)
            EditorGUILayout.ObjectField("Skybox material", script.skyboxNight, typeof(Material), false);
        script.skyboxIntensityNight = EditorGUILayout.FloatField("Skybox night light", script.skyboxIntensityNight);
        EditorGUILayout.PropertyField(serialized.FindProperty("enabledAtNight"), true);
        if (GUILayout.Button("Select directional Night light"))
            SelectDirectionalLight(false);
        if (GUILayout.Button("Change to Night"))
            script.StartNight();


        if (GUI.changed)
        {
            serialized.ApplyModifiedProperties();
            EditorUtility.SetDirty(script);
        }
    }

    private bool Initialize()
    {
        script = FindObjectOfType<DayTimeManager>();
        if (!script)
        {
            EditorGUILayout.HelpBox("Add Day Time Manager to scene", MessageType.Error);
            return false;
        }
        return true;
    }

    private void SelectDirectionalLight(bool day)
    {
        Object[] selection = new Object[1];
        if (day)
            selection[0] = script.directionalLightDay.gameObject;
        else
            selection[0] = script.directionalLightNight.gameObject;

        Selection.objects = selection;
    }
}
