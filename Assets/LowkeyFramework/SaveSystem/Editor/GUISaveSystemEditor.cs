using System;
using UnityEditor;
using UnityEngine;

public class GUISaveSystemEditor : EditorWindow
{
    string myString = "Hello World";
    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;
    int mode = 0;
    Vector2 hierarchyScroll = new Vector2(0, 0);

    [MenuItem("Window/GUI Save System")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(GUISaveSystemEditor));
    }

    void OnGUI()
    {

        mode = GUILayout.Toolbar(mode, new string[] { "Behaviours", "Prefabs", "GameObjects" });

        hierarchyScroll =  EditorGUILayout.BeginScrollView(hierarchyScroll, EditorStyles.helpBox);

        switch (mode)
        {
            case 0:
                DrawBasedOnBehaviours();
                break;
            case 1:
                DrawBasedOnPrefabs();
                break;
            case 2:
                DrawBasedOnGameObjects();
                break;
        }
        EditorGUILayout.EndScrollView();

    }

    private void DrawBasedOnGameObjects()
    {
        throw new NotImplementedException();
    }

    private void DrawBasedOnPrefabs()
    {
        throw new NotImplementedException();
    }

    private void DrawBasedOnBehaviours()
    {
        throw new NotImplementedException();
    }
}