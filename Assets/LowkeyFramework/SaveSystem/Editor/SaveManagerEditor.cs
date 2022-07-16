using System.Text;
using System.Text.RegularExpressions;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using LowkeyFramework.AttributeSaveSystem;

[CustomEditor(typeof(SaveManager))]
public class SaveManagerEditor : Editor
{
    private string testSaveName = "test1234";

    private int saveChosen = 0;
    private int lastSaveChosen = 0;

    private Vector2 scrollSaveFiles = new Vector2(0, 0);
    private Vector2 scrollSave = new Vector2(0, 0);
    private string renameField = "";

    private bool saveChanged => saveChosen != lastSaveChosen;

    private SaveManager Target => (SaveManager)target;

    private JSONEditor jsonEditor;

        private void OnEnable()
        {
            jsonEditor = CreateInstance<JSONEditor>();
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if(GUILayout.Button("Save"))
            {
                Target.Save(testSaveName);
            }

            if(GUILayout.Button("Save Encoded"))
            {
                Target.Save(testSaveName, true);
            }

        if(GUILayout.Button("Load"))
        {
            Target.Load(testSaveName);
        }

        if(GUILayout.Button("Generate random key")){
            Target.key = RandomString(16);
            EditorUtility.SetDirty(Target);
        }
        if (GUILayout.Button("Delete all Savefile"))
        {

            string[] saveFilesPaths = FileManager.ListFiles().Where(saveFileName => isJsonFileName(saveFileName)).ToArray();

            foreach (var saveFile in saveFilesPaths)
            {
                FileManager.DeleteFile(saveFile);
            }
        }

        GUILayout.Space(20);

        DrawSaveFiles();

        GUILayout.Space(20);

        DrawSaveFileEditor();

    }


    private static string RandomString(int length)
    {
        const string pool = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
        var builder = new StringBuilder();
        var random = new System.Random();

            for(int i = 0; i < length; i++)
            {
                char c = pool[random.Next(0, pool.Length)];
                builder.Append(c);
            }

        return builder.ToString();
    }


    private bool isJsonFileName(string saveFileName)
    {
        return new Regex("\\.json\\.bak$").IsMatch(saveFileName) || new Regex("\\.json$").IsMatch(saveFileName);
    }

    private void DrawSaveFiles()
    {
        scrollSaveFiles = GUILayout.BeginScrollView(scrollSaveFiles, EditorStyles.helpBox);
        EditorGUI.indentLevel++;

        GUILayout.Label("Savefiles", new GUIStyle { fontSize = 16, alignment = TextAnchor.MiddleCenter, normal = new GUIStyleState { textColor = Color.white } });

        string[] saveFilesPaths = FileManager.ListFiles().Where(saveFileName => isJsonFileName(saveFileName)).ToArray();

        saveChosen = GUILayout.SelectionGrid(saveChosen, saveFilesPaths.Select(savePath => Path.GetFileName(savePath)).ToArray(), 1);


        EditorGUI.indentLevel--;
        GUILayout.EndScrollView();
    }

    private void DrawSaveFileEditor()
    {
        scrollSave = GUILayout.BeginScrollView(scrollSave, EditorStyles.helpBox);
        EditorGUI.indentLevel++;


        string[] saveFilesPaths = FileManager.ListFiles().Where(saveFileName => isJsonFileName(saveFileName)).ToArray();

        GUILayout.Label("Savefile Options", new GUIStyle { fontSize = 16, alignment = TextAnchor.MiddleCenter, normal = new GUIStyleState { textColor = Color.white } });

        if (GUILayout.Button("Delete Savefile"))
        {
            FileManager.DeleteFile(saveFilesPaths[saveChosen]);
        }

        EditorGUILayout.BeginHorizontal();
        renameField = GUILayout.TextField(renameField, GUILayout.Width(400));

        if (GUILayout.Button("Rename Savefile"))
        {
            if(FileManager.CopyFile(saveFilesPaths[saveChosen], renameField))
            {
                FileManager.DeleteFile(saveFilesPaths[saveChosen]);
            }
        }

        EditorGUILayout.EndHorizontal();


        GUI.enabled = new Regex("\\.json\\.bak$").IsMatch(saveFilesPaths[saveChosen]);
        if (GUILayout.Button("Swap current Savefile for backup"))
        {
            FileManager.MoveFile(saveFilesPaths[saveChosen], saveFilesPaths[saveChosen].Replace(".bak", ""));
            saveChosen--;
        }
        GUI.enabled = true;

        GUILayout.Space(10);

        GUILayout.Label("Savefile Editor", new GUIStyle { fontSize = 16, alignment = TextAnchor.MiddleCenter, normal = new GUIStyleState { textColor = Color.white } });
        if (GUILayout.Button("Save chages to Savefile"))
        {
            jsonEditor.WriteToJson(saveFilesPaths[saveChosen]);
        }

        if ((saveChanged || jsonEditor.isEmpty()) && Target.LoadDecodeSaveFile(saveFilesPaths[saveChosen], out string jsonFile))
        {
            renameField = Path.GetFileName(saveFilesPaths[saveChosen]);
            lastSaveChosen = saveChosen;
            jsonEditor.LoadJson(jsonFile);
        }

        jsonEditor.JsonInspectorGUI();



        EditorGUI.indentLevel--;
        GUILayout.EndScrollView();
    }

}
