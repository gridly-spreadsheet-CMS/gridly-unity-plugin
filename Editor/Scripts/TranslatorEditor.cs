using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Gridly;
namespace Gridly.Internal
{
    
    [CustomEditor(typeof(Translator))]
    public class TranslatorEditor : Editor
    {
        GridlyArrData popupData = new GridlyArrData();

        static string search = "";
        private void OnEnable()
        {
            search = "";
        }
        public override void OnInspectorGUI()
        {
            
            base.OnInspectorGUI();
            Translator translator = (Translator)target;

            if (!popupData.init)
            {
                Refesh();
                
            }

            //Db
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Database");
            EditorGUI.BeginChangeCheck();

            translator.database = popupData.databaseArr[EditorGUILayout.Popup(popupData.indexDb,popupData.databaseArr)];
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(translator);
                Refesh();
            }
            GUILayout.EndHorizontal();

            //Grid
            if (popupData.indexGrid == -1)
                return;
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Grid");
            EditorGUI.BeginChangeCheck();
            translator.grid = popupData.gridArr[EditorGUILayout.Popup(popupData.indexGrid, popupData.gridArr)];
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(translator);
                Refesh();
            }
            GUILayout.EndHorizontal();
            

            GUILayout.Space(10);


            //Key
            if (popupData.indexKey == -1)
                return;

            GUILayout.Space(5);
            EditorGUI.BeginChangeCheck();
            search = GUILayout.TextField(search, GUI.skin.GetStyle("ToolbarSeachTextField"));
            if (EditorGUI.EndChangeCheck())
            {
                popupData.searchKey = search;
                Refesh();
            }

            
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Key");
            EditorGUI.BeginChangeCheck();
            translator.key = popupData.keyArr[EditorGUILayout.Popup(popupData.indexKey, popupData.keyArr)];
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(translator);
                Refesh();
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            try
            {
                foreach (var i in Project.singleton.langSupports)
                {
                    GUILayout.Label(i.name + ": " + popupData.chosenRecord.columns.Find(x => x.columnID.Contains(i.languagesSuport.ToString())).text);
                }
            }
            catch { }


        }

        void Refesh()
        {
            Translator translator = (Translator)target;
            popupData.RefeshKey(translator.database, translator.grid, translator.key);
        }

    }
}