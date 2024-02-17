using UnityEngine;
using UnityEditor;
using System;
using InfinityPBR;

[CustomEditor(typeof(TriggerToggle))]
[CanEditMultipleObjects]
[Serializable]
public class TriggerToggleEditor : Editor
{
    private static bool showHelpBoxes = true;
    
    public override void OnInspectorGUI()
    {
        TriggerToggle triggerToggle = (TriggerToggle)target;

        Undo.RecordObject (triggerToggle, "Toggle show help boxes");
        showHelpBoxes = EditorGUILayout.Toggle("Show help boxes", showHelpBoxes);
        if (showHelpBoxes)
        {
            EditorGUILayout.HelpBox("TRIGGER TOGGLE\nUse this script to toggle (on/off) triggers, and cascade " +
                                    "results through other toggles. Toggle off \"Show help boxes\" to hide this and " +
                                    "other help box messages.", MessageType.None);
        }
        
        // ***** SETUP *****
        EditorGUILayout.Space();
        if (showHelpBoxes)
        {
            EditorGUILayout.HelpBox("SETUP\n\n" +
                                    "INTERACTION LAYERS: Often the player, this can only be interacted with when an object " +
                                    "of these types has entered the trigger area. Disables OnTriggerExit.\n" +
                                    "PLAY AUDIO: Toggle to true if audio is meant to play.\n\n" +
                                    "PLAY ANIMATION: Toggle true if animation is meant to play.\n\n" +
                                    "CAN INTERACT: Default state. Set false if this trigger should not interact by default.\n\n" +
                                    "CAN BE LOCKED: If true, this object can be locked, often by another trigger or event.\n\n" +
                                    "UNLOCKED: Default state. True if starts unlocked.\n\nIS OPEN: Default state. True if this" +
                                    " starts in the \"open\" position.\n\n" +
                                    "LOCK DELAY: Delay after unlock/lock that any lock actions are triggered. Only affects objects" +
                                    " which become locked or unlocked after this object is triggered.\n\n" +
                                    "TRIGGER DELAY: Delay after trigger that any open/close actions are triggered. Set to 0 for " +
                                    "instant reaction.", MessageType.Info);
        }
        else
        {
            EditorGUILayout.LabelField("Setup", EditorStyles.boldLabel);
        }

        Undo.RecordObject(triggerToggle, "Update layermask");
        triggerToggle.interactionLayers = EditorTools.LayerMaskField("Interaction Layers", triggerToggle.interactionLayers);
        Undo.RecordObject(triggerToggle, "Update Play Audio");
        triggerToggle.playAudio = EditorGUILayout.Toggle("Play audio", triggerToggle.playAudio);
        Undo.RecordObject(triggerToggle, "Update Play Animation");
        triggerToggle.playAnimation = EditorGUILayout.Toggle("Play animation", triggerToggle.playAnimation);
        
        EditorGUILayout.Space();
        Undo.RecordObject(triggerToggle, "Update Can Interact");
        triggerToggle.canInteract = EditorGUILayout.Toggle("Can interact", triggerToggle.canInteract);
        Undo.RecordObject(triggerToggle, "Update Is Open");
        triggerToggle.isOpen = EditorGUILayout.Toggle("Is Open", triggerToggle.isOpen);
        Undo.RecordObject(triggerToggle, "Update Trigger Delay");
        triggerToggle.toggleDelay = EditorGUILayout.FloatField("Trigger Delay", triggerToggle.toggleDelay);

        EditorGUILayout.Space();
        Undo.RecordObject(triggerToggle, "Update Can Be Locked");
        triggerToggle.canBeLocked = EditorGUILayout.Toggle("Can be locked", triggerToggle.canBeLocked);
        Undo.RecordObject(triggerToggle, "Update Unlocked");
        triggerToggle.unlocked = EditorGUILayout.Toggle("Unlocked", triggerToggle.unlocked);
        Undo.RecordObject(triggerToggle, "Update Lock Delay");
        triggerToggle.lockDelay = EditorGUILayout.FloatField("Lock Delay", triggerToggle.lockDelay);
        
        
        
        // ***** END SETUP *****
        
        
        
        
        // ***** COMPONENT LINKS *****
        EditorGUILayout.Space();
        if (showHelpBoxes)
        {
            EditorGUILayout.HelpBox("COMPONENT LINKS\n\n" +
                                    "Assign links to other components here. They do not have to be on the same object as " +
                                    "this script. Most often, they will not be. Leave empty if you won't be using those " +
                                    "features of this script.\n\n" +
                                    "ANIMATION: This is the \"Animation\" " +
                                    "component (not \"Animator\") attached to the object that will move, if any.\n" +
                                    "AUDIO SOURCE: If there will be an audioclip played during this toggle, assign the " +
                                    "Audio Source component here.", MessageType.Info);
        }
        else
        {
            EditorGUILayout.LabelField("Component Links", EditorStyles.boldLabel);
        }

        Undo.RecordObject (triggerToggle, "Set Animation Component");
        triggerToggle.animation = EditorGUILayout.ObjectField("Animation", triggerToggle.animation, typeof(Animation), true) as Animation;
        Undo.RecordObject (triggerToggle, "Set AudioSource Component");
        triggerToggle.audioSource = EditorGUILayout.ObjectField("Audio Source", triggerToggle.audioSource, typeof(AudioSource), true) as AudioSource;

        // ***** END COMPONENT LINKS *****
        
        
        
        // ***** OBJECT LINKS *****
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Object Links", EditorStyles.boldLabel);

        if (showHelpBoxes)
        {
            EditorGUILayout.HelpBox("ANIMATION CLIPS: These are the Animation Clips which will play on open and close.", MessageType.Info);
        }
        Undo.RecordObject (triggerToggle, "Set Animation Open");
        triggerToggle.openAnimation = EditorGUILayout.ObjectField("Open Animation", triggerToggle.openAnimation, typeof(AnimationClip), true) as AnimationClip;
        Undo.RecordObject (triggerToggle, "Set Animation Close");
        triggerToggle.closeAnimation = EditorGUILayout.ObjectField("Close Animation", triggerToggle.closeAnimation, typeof(AnimationClip), true) as AnimationClip;

        var serializedObject = new SerializedObject(triggerToggle);
        var openAudioClipsProperty = serializedObject.FindProperty("openAudioClips");
        var closeAudioClipsProperty = serializedObject.FindProperty("closeAudioClips");
        var unlockAudioClipsProperty = serializedObject.FindProperty("unlockAudioClips");
        var lockAudioClipsProperty = serializedObject.FindProperty("lockAudioClips");
        var triggerObjectsProperty = serializedObject.FindProperty("triggerObjects");
        var unlockObjectsProperty = serializedObject.FindProperty("unlockObjects");
        serializedObject.Update();
        if (showHelpBoxes)
        {
            EditorGUILayout.HelpBox("OPEN/CLOSE AUDIO CLIPS: These are the Audio Clips that may play on open and close. One will be " +
                                    "selected randomly from the array.", MessageType.Info);
        }
        EditorGUILayout.PropertyField(openAudioClipsProperty, true);
        EditorGUILayout.PropertyField(closeAudioClipsProperty, true);
        
        if (showHelpBoxes)
        {
            EditorGUILayout.HelpBox("LOCK AUDIO CLIPS: These are the Audio Clips that this object will play when it is either locked" +
                                    " or unlocked, by any method.", MessageType.Info);
        }
        EditorGUILayout.PropertyField(unlockAudioClipsProperty, true);
        EditorGUILayout.PropertyField(lockAudioClipsProperty, true);
        
        if (showHelpBoxes)
        {
            EditorGUILayout.HelpBox("TRIGGER/UNLOCK OBJECTS: When this object is toggled, it will also toggle the objects listed " +
                                    "here. Trigger objects will have their own trigger activated, and unlock objects will have their " +
                                    "lock toggled.", MessageType.Info);
        }
        EditorGUILayout.PropertyField(triggerObjectsProperty, true);
        EditorGUILayout.PropertyField(unlockObjectsProperty, true);
        serializedObject.ApplyModifiedProperties();
        
        // ***** END OBJECT LINKS *****
        
        /*
         
         if (showHelpBoxes)
        {
            EditorGUILayout.HelpBox("OBJECT LINKS\n\n" +
                                    "ANIMATION CLIPS: These are the Animation Clips which will play on open and close.\n" +
                                    "OPEN/CLOSE AUDIO CLIPS: These are the Audio Clips that may play on open and close. One will be " +
                                    "selected randomly from the array.\n" +
                                    "LOCK AUDIO CLIPS: These are the Audio Clips that this object will play when it is either locked" +
                                    " or unlocked, by any method.\n" +
                                    "TRIGGER/UNLOCK OBJECTS: When this object is toggled, it will also toggle the objects listed " +
                                    "here. Trigger objects will have their own trigger activated, and unlock objects will have their " +
                                    "lock toggled.", MessageType.Info);
        }
        
        
         * public AnimationClip openAnimation;
        public AnimationClip closeAnimation;

        public AudioClip[] openAudioClips;
        public AudioClip[] closeAudioClips;
        public AudioClip[] unlockAudioClips;
        public AudioClip[] lockAudioClips;

        public GameObject[] triggerObjects;
        public GameObject[] unlockObjects;
        
         */
    }

}
