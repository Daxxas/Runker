using System.Collections.Generic;
using MovementTesting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CharacterPlayback))]
public class CharacterPlaybackEditor : Editor
{
    public Object source;
    
    SerializedProperty recordedPositions;
    
    public override void OnInspectorGUI()
    {
        CharacterPlayback characterPlayback = (CharacterPlayback)target;
         
        EditorGUI.BeginChangeCheck();

        SaveDuringPlay.SaveDuringPlay.Enabled = EditorGUILayout.Toggle("Save During Play", SaveDuringPlay.SaveDuringPlay.Enabled);
        
        string buttonText = characterPlayback.recording ? "Stop Recording" : "Start Recording";
        
        if (GUILayout.Button(buttonText))
        {
            characterPlayback.Record(!characterPlayback.recording);

        }

        if(GUILayout.Button("Reload Line Renderer"))
        {
            var lineRenderer = characterPlayback.GetComponent<LineRenderer>();
            
            lineRenderer.positionCount = characterPlayback.recordedPositions.Count;
            for (int i = 0; i < characterPlayback.recordedPositions.Count; i++)
            {
                lineRenderer.SetPosition(i, characterPlayback.recordedPositions[i].position);
            }

            lineRenderer.useWorldSpace = false;
        }
        
        base.OnInspectorGUI();
    }
}
