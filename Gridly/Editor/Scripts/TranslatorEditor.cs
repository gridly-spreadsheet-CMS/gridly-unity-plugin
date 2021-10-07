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
        Column chosenColum;
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


            GUILayout.Space(5);
            EditorGUI.BeginChangeCheck();
            search = GUILayout.TextField(search, GUI.skin.GetStyle("ToolbarSeachTextField"));
            if (EditorGUI.EndChangeCheck())
            {
                
                popupData.searchKey = search;
                Refesh();
            }

            if (popupData.keyArr == null)
                return;


            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Key");
            EditorGUI.BeginChangeCheck();
            try
            {
                translator.key = popupData.keyArr[EditorGUILayout.Popup(popupData.indexKey, popupData.keyArr)];
            }
            catch { GUILayout.Label("can't find key"); }
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
                GUILayout.BeginHorizontal();
                GUILayout.Label(main.ToString() + ": ");
                if (chosenColum != null)
                {
                    chosenColum.text = GUILayout.TextArea(chosenColum.text);
                    if(GUILayout.Button(new GUIContent() {text = "Export" , tooltip = "Export text to Girdly" }, GUILayout.MinWidth(60)))
                    {
                        GridlyFunctionEditor.editor.UpdateRecordLang(popupData.chosenRecord, popupData.grid.choesenViewID, UserData.singleton.mainLangEditor);
                    }
                }
    
                GUILayout.EndHorizontal();
            }
            catch { }


        }

        

        void Refesh()
        {
            Translator translator = (Translator)target;
            popupData.RefeshAll(translator.grid, translator.key);

            try
            {
                Languages main = UserData.singleton.mainLangEditor;
                chosenColum = popupData.chosenRecord.columns.Find(x => x.columnID == main.ToString());
            }
            catch { }
            
        }

    }
}