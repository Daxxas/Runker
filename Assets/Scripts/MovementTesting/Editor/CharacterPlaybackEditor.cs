using System.Collections.Generic;
using MovementTesting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CharacterPlayback))]
public class CharacterPlaybackEditor : Editor
{
    public Object source;
    
    public override void OnInspectorGUI()
    {
        CharacterPlayback characterPlayback = (CharacterPlayback)target;
         
        EditorGUI.BeginChangeCheck();

        
        string buttonText = characterPlayback.recording ? "Stop Recording" : "Start Recording";
        
        if (GUILayout.Button(buttonText))
        {
            characterPlayback.Record(!characterPlayback.recording);

        }
        
        if(GUILayout.Button("Save"))
        {
            // serializedObject.FindProperty("recordedPositions").arraySize = characterPlayback.recordedPositions.Count;
            // serializedObject.ApplyModifiedProperties();
        }
        
        if(GUILayout.Button("Reload Line Renderer"))
        {
            var lineRenderer = characterPlayback.GetComponent<LineRenderer>();
            
            lineRenderer.positionCount = characterPlayback.recordedPositions.Count;
            for (int i = 0; i < characterPlayback.recordedPositions.Count; i++)
            {
                lineRenderer.SetPosition(i, characterPlayback.recordedPositions[i].position);
            }
        }
        
        base.OnInspectorGUI();
    }
}
