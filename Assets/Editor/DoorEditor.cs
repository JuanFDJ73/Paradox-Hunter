using UnityEngine;
using UnityEditor;

// Editor personalizado para Door
[CustomEditor(typeof(Door))]
public class DoorEditor : Editor
{
    private SerializedProperty inputThresholdProp;
    private SerializedProperty doorTimeProp;
    private SerializedProperty spawnPointIDProp;

    private void OnEnable()
    {
        inputThresholdProp = serializedObject.FindProperty("inputThreshold");
        doorTimeProp = serializedObject.FindProperty("doorTime");
        spawnPointIDProp = serializedObject.FindProperty("spawnPointID");
    }

    public override void OnInspectorGUI()
    {
        Door door = (Door)target;
        serializedObject.Update();

        EditorGUILayout.LabelField("Configuración de Escena", EditorStyles.boldLabel);
        
        // Campo drag & drop para la escena
        EditorGUI.BeginChangeCheck();
        SceneAsset newScene = (SceneAsset)EditorGUILayout.ObjectField(
            "Escena Destino", 
            door.sceneAsset, 
            typeof(SceneAsset), 
            false
        );
        
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(door, "Changed Door Scene");
            door.sceneAsset = newScene;
            
            // Guardar el nombre de la escena
            if (newScene != null)
            {
                door.SceneName = newScene.name;
            }
            else
            {
                door.SceneName = "";
            }
            
            EditorUtility.SetDirty(door);
        }

        // Mostrar el nombre de la escena (solo lectura)
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.TextField("Nombre de Escena", door.SceneName);
        EditorGUI.EndDisabledGroup();

        // Verificar si la escena está en Build Settings
        if (door.sceneAsset != null)
        {
            string scenePath = AssetDatabase.GetAssetPath(door.sceneAsset);
            bool isInBuildSettings = false;
            
            foreach (var scene in EditorBuildSettings.scenes)
            {
                if (scene.path == scenePath)
                {
                    isInBuildSettings = scene.enabled;
                    break;
                }
            }

            if (!isInBuildSettings)
            {
                EditorGUILayout.HelpBox(
                    "¡Esta escena no está en Build Settings! Agrégala en File → Build Settings.", 
                    MessageType.Warning
                );
                
                if (GUILayout.Button("Agregar a Build Settings"))
                {
                    var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
                    scenes.Add(new EditorBuildSettingsScene(scenePath, true));
                    EditorBuildSettings.scenes = scenes.ToArray();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("✓ Escena configurada correctamente", MessageType.Info);
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Spawn Point", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(spawnPointIDProp, new GUIContent("Spawn Point ID", "ID del SpawnPoint en la escena destino donde aparecerá el jugador"));
        
        if (string.IsNullOrEmpty(spawnPointIDProp.stringValue))
        {
            EditorGUILayout.HelpBox("Escribe el ID del SpawnPoint de la escena destino (ej: 'Start', '3', 'Castle')", MessageType.Info);
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Configuración de Tiempo", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(doorTimeProp, new GUIContent("Tiempo de la Puerta", "En qué tiempo funciona esta puerta"));
        
        // Mostrar ayuda según el tiempo seleccionado
        Door.DoorTime selectedTime = (Door.DoorTime)doorTimeProp.enumValueIndex;
        if (selectedTime == Door.DoorTime.Past)
        {
            EditorGUILayout.HelpBox("Esta puerta solo funciona cuando el jugador está en el PASADO", MessageType.Info);
        }
        else
        {
            EditorGUILayout.HelpBox("Esta puerta solo funciona cuando el jugador está en el FUTURO", MessageType.Info);
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Configuración de Input", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(inputThresholdProp, new GUIContent("Input Threshold"));

        serializedObject.ApplyModifiedProperties();
    }
}
