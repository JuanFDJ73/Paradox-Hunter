using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DoorKeyRequired))]
public class DoorKeyRequiredEditor : Editor
{
    private SerializedProperty requiredKeyIDProp;
    private SerializedProperty timeRequirementProp;
    private SerializedProperty spawnPointIDProp;
    private SerializedProperty isUnlockedProp;
    private SerializedProperty lockedVisualProp;
    private SerializedProperty unlockedVisualProp;

    private void OnEnable()
    {
        requiredKeyIDProp = serializedObject.FindProperty("requiredKeyID");
        timeRequirementProp = serializedObject.FindProperty("timeRequirement");
        spawnPointIDProp = serializedObject.FindProperty("spawnPointID");
        isUnlockedProp = serializedObject.FindProperty("isUnlocked");
        lockedVisualProp = serializedObject.FindProperty("lockedVisual");
        unlockedVisualProp = serializedObject.FindProperty("unlockedVisual");
    }

    public override void OnInspectorGUI()
    {
        DoorKeyRequired door = (DoorKeyRequired)target;
        serializedObject.Update();

        // === Configuración de Llave ===
        EditorGUILayout.LabelField("Configuración de Llave", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(requiredKeyIDProp, new GUIContent("ID de Llave Requerida"));
        
        EditorGUILayout.Space(10);

        // === Configuración de Tiempo ===
        EditorGUILayout.LabelField("Configuración de Tiempo", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(timeRequirementProp, new GUIContent("Requisito de Tiempo"));
        
        EditorGUILayout.Space(10);

        // === Configuración de Escena (Drag & Drop) ===
        EditorGUILayout.LabelField("Destino", EditorStyles.boldLabel);
        
        EditorGUI.BeginChangeCheck();
        SceneAsset newScene = (SceneAsset)EditorGUILayout.ObjectField(
            "Escena Destino", 
            door.sceneAsset, 
            typeof(SceneAsset), 
            false
        );
        
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(door, "Changed DoorKeyRequired Scene");
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
        
        // Advertencia si no hay escena asignada
        if (string.IsNullOrEmpty(door.SceneName))
        {
            EditorGUILayout.HelpBox("¡Arrastra una escena al campo de arriba!", MessageType.Warning);
        }
        
        EditorGUILayout.Space(10);

        // === Spawn Point ===
        EditorGUILayout.LabelField("Spawn Point", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(spawnPointIDProp, new GUIContent("Spawn Point ID", "ID del SpawnPoint en la escena destino donde aparecerá el jugador"));
        
        if (string.IsNullOrEmpty(spawnPointIDProp.stringValue))
        {
            EditorGUILayout.HelpBox("Escribe el ID del SpawnPoint de la escena destino (ej: 'Start', '3', 'Castle')", MessageType.Info);
        }
        
        EditorGUILayout.Space(10);

        // === Estado ===
        EditorGUILayout.LabelField("Estado", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(isUnlockedProp, new GUIContent("Desbloqueada"));
        
        EditorGUILayout.Space(10);

        // === Visual (Opcional) ===
        EditorGUILayout.LabelField("Visual (Opcional)", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(lockedVisualProp, new GUIContent("Visual Cerrada"));
        EditorGUILayout.PropertyField(unlockedVisualProp, new GUIContent("Visual Abierta"));

        serializedObject.ApplyModifiedProperties();
    }
}
