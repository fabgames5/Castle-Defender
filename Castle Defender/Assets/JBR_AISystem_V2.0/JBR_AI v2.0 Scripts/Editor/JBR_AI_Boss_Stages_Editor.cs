using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(JBR_AI_Boss_Stages), true)]
public class JBR_AI_Boss_Stages_Editor : JBR_Behavior_Editor
{
  //  private bool showDisplay = false;
    JBR_AI_Boss_Stages abs;
   // SerializedObject GetTarget;
   // SerializedProperty ThisList;
   // int listCount = 0;


    private void OnEnable()
    {
        abs = (JBR_AI_Boss_Stages)target;
     //   GetTarget = new SerializedObject(abs);
      //   ThisList = GetTarget.FindProperty("stages"); // Find the List in our script and create a refrence of it
    }

    public override void OnInspectorGUI()
    { 
        base.OnInspectorGUI();
      //  showDisplay = EditorGUILayout.Toggle(new GUIContent("SHow Boss AI Stages Setup", "Uncheck this box to hide extra parameters"), showDisplay);

        if (base.showDisplay)
        {
          //  if(abs.stages.Count >= abs.healthStages.Length)
          //  {
          //      abs.healthStages = new float[ abs.stages.Count];
          //  }

            for (int i = 0; i < abs.stages.Count; i++)
            {
                for (int b = 0; b < abs.stages[i].behaviourSetup.Count; b++)
                {
                    //    if (b == 0)
                    //    {
                    if (abs.stages[i].behaviourSetup[b].behavior != null)
                    {
                        abs.stages[i].behaviourSetup[b].behaviorName = abs.stages[i].behaviourSetup[b].behavior.componentName;
                    }

                   

                    //     }
                    //    if (b >= 1 )
                    //    {                  
                    //       if (abs.stages[i].behaviourSetup[b].behaviorName == "" && abs.stages[i].behaviourSetup[b].behavior != null && abs.stages[i].behaviourSetup[b].behavior != abs.stages[i].behaviourSetup[b - 1].behavior)
                    //       {
                    //          abs.stages[i].behaviourSetup[b].behaviorName = abs.stages[i].behaviourSetup[b].behavior.componentName;
                    //      }
                    //     else
                    //     {
                    //         if (abs.stages[i].behaviourSetup[b].behaviorName != "" && abs.stages[i].behaviourSetup[b].behavior != abs.stages[i].behaviourSetup[b - 1].behavior)
                    //        {
                    //            abs.stages[i].behaviourSetup[b].behaviorName = abs.stages[i].behaviourSetup[b].behavior.componentName;
                    //         }
                    //       }

                    //       }
                }
            }

            //Apply the changes to our list
          //  GetTarget.ApplyModifiedProperties();
          //  DrawDefaultInspector();
        }
    }
}