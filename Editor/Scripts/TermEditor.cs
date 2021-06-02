using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
namespace Gridly.Internal
{
    public class TermEditor : GridlyEditor
    {
        Vector3 m_Scroll = new Vector3();
        static TermEditor window;

        static int databaseSelectIndex;
        static Database database
        {

            get
            {
                try
                {
                    return Project.singleton.databases[popupData.indexDb];
                }
                catch
                {
                    databaseSelectIndex = 0;
                    Refesh();
                    return Project.singleton.databases[databaseSelectIndex];
                }
            }
        }

        static int gridSelectIndex;
        static Grid grid
        {
            get
            {
                try
                {
                    return database.grids[gridSelectIndex];
                }
                catch
                {
                    gridSelectIndex = 0;
                    Refesh();
                    return database.grids[gridSelectIndex];
                }
            }
        }


        [MenuItem("Tools/Gridly/Term", false, 3)]
        private static void Init()
        {
            if(Project.singleton.databases.Count == 0)
            {
                "you need to setup data before using this feature".Print();
                return;
            }

            window = (TermEditor)GetWindow(typeof(TermEditor), false, "Term Editor - " + GridlyInfo.ver);
            Vector2 vector2 = new Vector2(400, 400);
            window.minSize = vector2;
            Refesh();
            window.Show();
        }


        static string databaseSelect = "";
        static string gridSelect = "";
        static List<Record> records;
        static GridlyArrData popupData = new GridlyArrData();
        static void Refesh()
        {
            popupData.RefeshAll(databaseSelect, gridSelect, null, null);
            RefeshList();
        }
        static void RefeshList()
        {
            if (popupData.indexGrid == -1)
                return;
            records = getRecord(grid);
            TermListLegth = records.Count;
            
        }

        static float mRowSize = 40;
        static int TermListLegth = 0;
        float scrollHeight;
        string selectRecordID;
        float YPos;
        float ScrolSizeY => 280 + position.height - 400;
        static string search;
        static List<Record> getRecord(Grid grid)
        {
            if(!string.IsNullOrEmpty(search))
                return grid.records.FindAll(x => x.recordID.Contains(search));
            return grid.records;
        }
        private void OnGUI()
        {
            
            GUILayout.Space(20);

            #region select data
            EditorGUI.BeginChangeCheck();
            databaseSelectIndex = EditorGUILayout.Popup("Database",popupData.indexDb, popupData.databaseArr);
            databaseSelect = popupData.databaseArr[databaseSelectIndex];

            if (EditorGUI.EndChangeCheck())
            {
                m_Scroll = Vector3.zero;
                Refesh();
                return;
            }

            
            if (popupData.indexGrid == -1)
                return;
            
            EditorGUI.BeginChangeCheck();
            gridSelectIndex = EditorGUILayout.Popup("Grid",popupData.indexGrid, popupData.gridArr);  
            gridSelect = popupData.gridArr[gridSelectIndex];
            if (EditorGUI.EndChangeCheck())
            {
                m_Scroll = Vector3.zero;
                UserData.singleton.Save();
                Refesh();
                return;
            }


            #endregion
            GUILayout.Space(10);
            #region Search
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();
            search = GUILayout.TextField(search, GUI.skin.GetStyle("ToolbarSeachTextField"));
            if (EditorGUI.EndChangeCheck())
            {
                RefeshList();
                //selectRecordID = "";
                m_Scroll = Vector3.zero;
                //showCreateKey = false;
            }
            GUILayout.Label("Search", GUILayout.Width(60));
            GUILayout.EndHorizontal();
            #endregion
            GUILayout.Space(5);


            m_Scroll = GUILayout.BeginScrollView(m_Scroll, TextStyle, GUILayout.MinHeight(ScrolSizeY), GUILayout.ExpandHeight(false));
            if (Event.current != null && Event.current.type == EventType.Layout)
                scrollHeight = m_Scroll.y;

            #region draw record

            YPos = 0;

            int nDraw = 0;
            float nSkipStart = 0;
            bool spaceStart = false;
  
            for (int i = 0; i < TermListLegth; i++)
            {


                if (YPos < scrollHeight - mRowSize && !spaceStart)
                {
                    spaceStart = true;
                    nSkipStart = ((scrollHeight - mRowSize) / mRowSize);
                    YPos = scrollHeight - mRowSize;
                    i = (int)nSkipStart;
                    GUILayout.Space(nSkipStart * mRowSize);
                }

                GUILayout.Space(mRowSize);
                try
                {
                    DrawRecord(i + 1, records[i]);
                }
                catch
                {
                    break;
                }
                YPos += mRowSize;
                

                nDraw++;
                if (YPos > scrollHeight + ScrolSizeY)
                {
                    break;
                }

                if (i == TermListLegth - 1)
                {

                }


            }
            GUILayout.Space((TermListLegth+1 - (nDraw + nSkipStart)) * (mRowSize));
            DrawAddRecord();
            GUILayout.EndScrollView();
            #endregion

        }
        bool showCreateKey;
        string nameNewKey;
        void DrawAddRecord()
        {
            
            GUI.backgroundColor = new Color(1, 1, 1, 0.4f);
            GUILayout.BeginHorizontal();
            if(GUILayout.Button("+", EditorStyles.toolbarButton, GUILayout.Width(30)))
            {
                showCreateKey = !showCreateKey;
            }
            GUI.color = Color.white;
            if (showCreateKey) ShowCreateKey();
            GUILayout.EndHorizontal();

            void ShowCreateKey()
            {
                GUILayout.BeginHorizontal(EditorStyles.toolbar);
                nameNewKey = EditorGUILayout.TextField(nameNewKey, EditorStyles.toolbarTextField, GUILayout.ExpandWidth(true));
                GUILayout.EndHorizontal();

                if(GUILayout.Button("Create Record", "toolbarbutton", GUILayout.ExpandWidth(false)))
                {
                    

                    Record record = new Record();
                    record.recordID = nameNewKey;


                    foreach (var i in Project.singleton.langSupports)
                    {
                        record.columns.Add(new Column(i.languagesSuport.ToString(), ""));
                    }

                    grid.records.Add(record);
                    Project.singleton.setDirty();

                    showCreateKey = false;
                    selectRecordID = nameNewKey;
                    nameNewKey = "";
                    RefeshList();
                }
            }
            
        }

        void DrawRecord(int i,Record record)
        {
            bool selected = selectRecordID == record.recordID;
            if (selected)
            {
                Color color = Color.Lerp(Color.cyan, Color.white, 0.3f);
                color.a = 0.4f;
                GUI.backgroundColor = color;
                SelectStyle.normal.textColor = Color.white;
            }
            else { 
                GUI.backgroundColor = darkLightColor;
                SelectStyle.normal.textColor = new Color(1, 1, 1, 0.2f);
            }
            
            
            Rect rect = new Rect(2, YPos, position.width, mRowSize);
            
            if (GUI.Button(rect, new GUIContent(i + ". " +record.recordID), SelectStyle))
            {
                
                if (selectRecordID == record.recordID)
                    selectRecordID = "";
                else selectRecordID = record.recordID;

                //-turn off rename feature
                isRename = false;
                theNameToRename = "";
                ///

                GUI.FocusControl(null);
            }

           
            if (selected)
                DrawDetailRecord(record);

        }

        bool isRename;
        string theNameToRename = "";
        void DrawDetailRecord(Record record)
        {
            foreach (var i in Project.singleton.langSupports)
            {

                GUILayout.BeginHorizontal(TextStyle);
                GUILayout.Label(i.name, EditorStyles.boldLabel,GUILayout.Width(70 + (position.width - 400)*0.1f));

                Column col = record.columns.Find(x => x.columnID == i.languagesSuport.ToString());
                string text = "";
                if (col != null)
                    text = col.text;
                EditorGUI.BeginChangeCheck();
                text = GUILayout.TextField(text);
                if (EditorGUI.EndChangeCheck())
                {
                    if (col != null)
                    {
                        col.text = text;
                        Project.singleton.setDirty();
                    }
                    else
                    {
                        Debug.Log("you cannot edit this field because there is no column " +i.languagesSuport + " on Gridly. To fix this please login to Gridly and add column with columnID as \""+i.languagesSuport+"\"");
                    }
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.Space(2);

            if (isRename)
            {
                ShowRename();
                void ShowRename()
                {
                    GUILayout.BeginHorizontal(TextStyle);
                    GUILayout.Label(record.recordID);


                    theNameToRename = GUILayout.TextField(theNameToRename, GUILayout.Width(position.width*0.7f));
                    if (GUILayout.Button("Save", GUILayout.Width(50)))
                    {
                        selectRecordID = theNameToRename;
                        GridlyFunctionEditor.editor.DeleteRecord(record.recordID, grid.choesenViewID, null); //delete old record before rename
                        record.recordID = theNameToRename;
                        GridlyFunctionEditor.editor.AddRecord(record, grid.choesenViewID);
                        Project.singleton.setDirty();
                    }
                    
                    GUILayout.EndHorizontal();
                }
            }
            #region delete, rename
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Delete"))
            {
                if(EditorUtility.DisplayDialog("Confirm delete", "Are you sure you want to delete this record", "Yes", "Cancel"))
                {
                    EditorUtility.SetDirty(UserData.singleton);
                    GridlyFunctionEditor.editor.DeleteRecord(record.recordID, grid.choesenViewID, null);
                    grid.records.Remove(record);
                    Refesh();
                    Project.singleton.setDirty();
                }

            }

            
            if (GUILayout.Button("Rename"))
            {
                isRename = !isRename;
            }

            if (GUILayout.Button("Update"))
            {
                GridlyFunctionEditor.editor.AddRecord(record, grid.choesenViewID);
            }

            GUILayout.EndHorizontal();
            #endregion

            YPos = GUILayoutUtility.GetLastRect().y;
            GUILayout.Space(mRowSize); 
        }
    }


}
