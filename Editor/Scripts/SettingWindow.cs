using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Gridly.Internal
{
    public class SettingWindow : EditorWindow
    {
        Texture2D m_logo;

        [MenuItem("Window/Gridly/Setting", false, 1)]
        private static void Init()
        {
            SettingWindow window = (SettingWindow)GetWindow(typeof(SettingWindow), true, "Gridly Setting Window -V1");

            Vector2 vector2 = new Vector2(400,400);
            window.minSize = vector2;
            window.maxSize = vector2;
            window.Show();
        }


        [MenuItem("Window/Gridly/Get Data From Gridly", false, 2)]
        private static void GetDataFromGridly()
        {
            UserData.singleton.isGeneratedData = true;
            Save();
            GridlyEditor.SetupDatabases();
        }

        [MenuItem("Window/Gridly/Test", false, 3)]
        private static void Test()
        {
            Grid grid = Project.singleton.databases.Find(x => x.databaseName == "customer").grids.Find(x => x.nameGrid == "gametexts");
            GridlyEditor.SetupRecords(grid, "", 0);
        }

        private void OnEnable()
        {
            m_logo = (Texture2D)Resources.Load("Gridly_Icon", typeof(Texture2D));
            
        }

        private void OnGUI()
        {
            GUILayout.Label(m_logo);
            GUILayout.Space(10);
            GUILayout.Label("Enter your API key here:", EditorStyles.boldLabel);
            UserData.singleton.keyAPI = EditorGUILayout.TextField(UserData.singleton.keyAPI);

            GUILayout.Space(10);
            GUILayout.Label("Enter your Project ID here:", EditorStyles.boldLabel);
            Project.singleton.ProjectID = EditorGUILayout.TextField(Project.singleton.ProjectID);

            GUILayout.Space(10);
            UserData.singleton.useSeparateData = EditorGUILayout.Toggle("Use Separate Data", UserData.singleton.useSeparateData);
            if(UserData.singleton.previousUseSepData != UserData.singleton.useSeparateData)
            {
                Save();
            }

            GUILayout.Space(10);
            //save
            if (GUILayout.Button("Save Setting"))
            {
                Save();
            }

            GUILayout.Space(10);
            //list project
            EditorGUILayout.HelpBox("Click here to see the project IDs in cosole", MessageType.Info);
            if (GUILayout.Button("List project IDs"))
            {
                GridlyEditor.ShowProjectIDs();
            }

            GUILayout.Space(10);
            //setup
            if (UserData.singleton.isGeneratedData)
                EditorGUILayout.HelpBox("New data settings will overwrite the old data", MessageType.Info);
            if (GUILayout.Button("Get data from Gridly"))
            {
                
                UserData.singleton.isGeneratedData = true;
                Save();
                GridlyEditor.SetupDatabases();
            }
            
        }

        static void Save()
        {

            if (!UserData.singleton.useSeparateData)
            {
                if (AssetDatabase.IsValidFolder("Assets/Gridly/Resources/Databases"))
                {
                    AssetDatabase.DeleteAsset("Assets/Gridly/Resources/Databases");
                }
            }

            UserData.singleton.previousUseSepData = UserData.singleton.useSeparateData;
            AddDefine("Gridly_UseSeparateData", UserData.singleton.useSeparateData);
            

            
            EditorUtility.SetDirty(UserData.singleton);
            EditorUtility.SetDirty(Project.singleton);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

        }


        private static void AddDefine(string directive, bool isOn)
        {
            string textToWrite = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

            if (!isOn)
            {
                if (textToWrite.Contains(directive))
                {
                    textToWrite = textToWrite.Replace(directive, "");
                }
            }
            else
            {
                if (!textToWrite.Contains(directive))
                {
                    if (textToWrite == "")
                    {
                        textToWrite += directive;
                    }
                    else
                    {
                        textToWrite += "," + directive;
                    }
                }
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, textToWrite);
        }


    }
}