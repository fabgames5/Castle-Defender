using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(JBR_Base_Behavior_State),true)]
public  class JBR_Behavior_Editor : Editor
{
    public bool showDisplay = false;
    private bool pRAC;
    private bool PAAC;
    public JBR_Base_Behavior_State bbs;


    private void OnEnable()
    {
      
    }

    public override void OnInspectorGUI()
    {
        bbs = (JBR_Base_Behavior_State)target;
        pRAC = bbs.playRandomAnimationClip;
        PAAC = bbs.playAllAnimationClips;

        serializedObject.Update();
        EditorGUILayout.BeginVertical();
        bbs.componentName = EditorGUILayout.TextField("Add a Name for this Behavior to better reference it", bbs.componentName);
        showDisplay = EditorGUILayout.Toggle(new GUIContent("SHow Base Behavior Settings", "Uncheck this box to hide extra parameters"), showDisplay);

        EditorGUI.BeginChangeCheck();
        if (bbs.playRandomAnimationClip != pRAC && bbs.playRandomAnimationClip == true)
        {
            bbs.playAllAnimationClips = false;
            
        }
        if (bbs.playAllAnimationClips != PAAC && bbs.playAllAnimationClips == true)
        {
            bbs.playRandomAnimationClip = false;
            
        }
        pRAC = bbs.playRandomAnimationClip;
        PAAC = bbs.playAllAnimationClips;

        //bbs.events[0] = 

        EditorGUI.EndChangeCheck();

        //  EditorGUILayout.InspectorTitlebar(false, bbs);
        if (bbs.componentName == "")
        {
            bbs.componentName = bbs.GetType().Name;
        }
        EditorGUILayout.EndVertical();
        if (showDisplay)
        {
            DrawDefaultInspector();
        }
        else
        {

        }
    }
}


