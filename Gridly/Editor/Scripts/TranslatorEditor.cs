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

            if (Project.singleton.grids.Count == 0)
                return;

            //Db
            GUILayout.Space(10);


            //Grid
            if (popupData.indexGrid == -1)
                return;
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Grid");
            EditorGUI.BeginChangeCheck();
            try
            {
                translator.grid = popupData.gridArr[EditorGUILayout.Popup(popupData.indexGrid, popupData.gridArr)];
            }
            catch
            {
                popupData.indexGrid = 0;
            }
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(translator);
                Refesh();
            }
            GUILayout.EndHorizontal();
            

            GUILayout.Space(10);


            //Key
            if (popupData.keyArr == null)
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
                Languages main = UserData.singleton.mainLangEditor;
                GUILayout.Label(main.ToString() + ": " + popupData.chosenRecord.columns.Find(x => x.columnID.Contains(main.ToString())).text);
            }
            catch { }


        }

        void Refesh()
        {
            Translator translator = (Translator)target;
            popupData.RefeshAll(translator.grid, translator.key);
        }

    }
}