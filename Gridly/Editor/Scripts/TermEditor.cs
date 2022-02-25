using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Gridly.Internal
{
    public class TermEditor : GridlyEditor
    {
        Vector3 m_Scroll = new Vector3();
        public static TermEditor window;
        static int gridSelectIndex;
        static Grid grid
        {
            get
            {
                try
                {
                    return Project.singleton.grids[gridSelectIndex];
                }
                catch
                {
                    gridSelectIndex = 0;
                    Refesh();
                    return Project.singleton.grids[gridSelectIndex];
                }
            }
        }
        [MenuItem("Tools/Gridly/String Editor", false, 0)]
        private static void Init()
        {
            if (Project.singleton.grids.Count == 0)
            {
                "you need to setup data before using this feature".Print();
                return;
            }

            window = (TermEditor)GetWindow(typeof(TermEditor), false, "String Editor - " + GridlyInfo.ver);
            Vector2 vector2 = new Vector2(400, 400);
            window.minSize = vector2;
            Refesh();
            window.Show();
        }
        public static void RepaintThis()
        {
            if (window != null)
            {
                window.Repaint();

            }
        }
        static string gridSelect = "";
        static List<Record> records;
        static GridlyArrData popupData = new GridlyArrData();
        public static void Refesh()
        {

            OnEnableEditor();
            init = true;
            popupData.RefeshAll(gridSelect, null);
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
        float ScrolSizeY => 280 + position.height - 415;
        static string search;
        float res = 0;
        int countReset = 0;
        static List<Record> getRecord(Grid grid)
        {
            if (!string.IsNullOrEmpty(search))
            {
                return grid.records.FindAll(x => x.recordID.ToLower().Contains(search.ToLower()) |
                    x.columns.Find(x => x.columnID == UserData.singleton.mainLangEditor.ToString() & x.text.ToLower().Contains(search.ToLower())) != null
                );
            }
            return grid.records;
        }
        static bool init;
        public void OnGUI()
        {

            if (!init)
            {
                Refesh();
            }

            GUILayout.Space(20);

            #region select data



            EditorGUI.BeginChangeCheck();
            gridSelectIndex = EditorGUILayout.Popup("Grid", popupData.indexGrid, popupData.gridArr);
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

            if (GUILayout.Button(new GUIContent() { text = "▲", tooltip = "Scroll to the top" }, GUILayout.Width(35)))
            {
                m_Scroll.y = 0;
            }

            if (GUILayout.Button(new GUIContent() { text = "▼", tooltip = "Scroll to the bottom" }, GUILayout.Width(35)))
            {
                m_Scroll.y = TermListLegth * mRowSize;
                showCreateKey = true;
                OnGUI();
            }

            GUILayout.EndHorizontal();

            #endregion
            GUILayout.Space(5);
            m_Scroll = GUILayout.BeginScrollView(m_Scroll, TextStyle, GUILayout.MinHeight(ScrolSizeY), GUILayout.ExpandHeight(true));


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
                bool selected = false;
                float befforY = YPos;
                try
                {
                    DrawRecord(i + 1, records[i], ref selected);
                }
                catch
                {
                    break;
                }
                YPos += mRowSize;


                if (selected)
                {
                    if (countReset > 0)
                    {
                        res = GUILayoutUtility.GetLastRect().yMin - befforY;
                        countReset -= 1;
                    }

                    YPos += res;


                }

                nDraw++;

                //cut
                if (YPos > scrollHeight + ScrolSizeY)
                {
                    break;
                }

            }
            GUILayout.Space((TermListLegth + 1 - (nDraw + nSkipStart)) * (mRowSize));
            DrawAddRecord();

            #endregion
            GUILayout.EndScrollView();
            #region Button

            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent() { text = "Import grid", tooltip = "Import this grid data from from Gridly" }))
            {
                if (EditorUtility.DisplayDialog("Confirm Import grid", "Are you sure you want to import grid from Gridly?. It will overwrite the old data including translations.", "Yes", "Cancel"))
                {
                    popupData.grid.records.Clear();
                    Refesh();
                    RepaintThis();

                    GridlyFunctionEditor.editor.doneOneProcess += Refesh;
                    GridlyFunctionEditor.editor.doneOneProcess += RepaintThis;

                    GridlyFunctionEditor.editor.RefeshDowloadTotal();
                    GridlyFunctionEditor.editor.SetupRecords(popupData.grid, 0);
                }
            }

            if (GUILayout.Button(new GUIContent() { text = "Import all", tooltip = "Import all data from Gridly" }))
            {
                if (EditorUtility.DisplayDialog("Confirm Export", "Are you sure you want to import all data from Gridly?. It will overwrite the old data including translations.", "Yes", "Cancel"))
                {
                    GridlyFunctionEditor.editor.doneOneProcess += Refesh;
                    GridlyFunctionEditor.editor.doneOneProcess += RepaintThis;
                    GridlyFunctionEditor.editor.SetupDatabases();
                }
            }
            if (GUILayout.Button(new GUIContent() { text = "Export source languages", tooltip = "Export the source language of the selected grid" }))
            {
                GridlyFunctionEditor.editor.AddUpdateRecordAll(popupData.grid.records, popupData.grid.choesenViewID, false, true);
            }
            GUILayout.EndHorizontal();
            #endregion
        }
        bool showCreateKey = true;
        string nameNewKey;
        void DrawAddRecord()
        {
            GUI.backgroundColor = new Color(1, 1, 1, 0.4f);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+", EditorStyles.toolbarButton, GUILayout.Width(30)))
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

                if (GUILayout.Button("Create Record", "toolbarbutton", GUILayout.ExpandWidth(false)))
                {
                    Record record = new Record();
                    record.recordID = nameNewKey;
                    GridlyFunctionEditor.editor.AddRecord(record, grid.choesenViewID);
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
        void DrawRecord(int i, Record record, ref bool selected)
        {
            selected = selectRecordID == record.recordID;
            if (selected)
            {
                Color color = Color.Lerp(Color.cyan, Color.white, 0.3f);
                color.a = 0.3f;
                GUI.backgroundColor = color;
                SelectStyle.normal.textColor = Color.white;
            }
            else
            {
                GUI.backgroundColor = darkLightColor;
                SelectStyle.normal.textColor = new Color(1, 1, 1, 0.2f);
            }
            Rect rect = new Rect(2, YPos, position.width, mRowSize);
            //draw record
            if (GUI.Button(rect, new GUIContent(i + ". " + record.recordID + " |  " + GetShowText(ref record).Replace('\n', ' ')), SelectStyle))
            {
                countReset = 2;
                if (selectRecordID == record.recordID)
                    selectRecordID = "";
                else selectRecordID = record.recordID;
                isRename = false;
                theNameToRename = "";
            }
            if (selected)
            {
                DrawDetailRecord(record);
            }
        }
        string GetShowText(ref Record record)
        {

            foreach (var i in record.columns)
            {
                if (i.columnID == UserData.singleton.mainLangEditor.ToString())
                    return LimitText(ref i.text);
            }
            return string.Empty;
        }
        string LimitText(ref string input)
        {
            string final = "";
            const int length = 50;
            for (int i = 0; i < input.Length; i++)
            {
                if (i >= length)
                {
                    final += "...";
                    break;
                }
                final += input[i];
            }
            return final;
        }
        bool isRename;
        string theNameToRename = "";
        bool isBeginFoldOutFileField;
        void DrawDetailRecord(Record record)
        {
            //return;
            float widthNameLang = 0;
            foreach (var i in Project.singleton.langSupports)
            {
                GUILayout.BeginHorizontal(TextStyle);
                string name = i.name;
                GUIContent contenLabel;
                if (i.languagesSuport == UserData.singleton.mainLangEditor)
                {
                    name = "*" + name;
                    contenLabel = new GUIContent() { text = name, tooltip = "This is the source language" };
                }
                else contenLabel = new GUIContent() { text = name };
                widthNameLang = 70 + (position.width - 400) * 0.1f;
                GUILayout.Label(contenLabel, EditorStyles.boldLabel, GUILayout.Width(widthNameLang));
                Column col = record.columns.Find(x => x.columnID == i.languagesSuport.ToString());
                string text = "";
                if (col != null)
                    text = col.text;
                EditorGUI.BeginChangeCheck();
                text = GUILayout.TextArea(text, GUILayout.MaxWidth(position.width - 80 - widthNameLang));
                if (EditorGUI.EndChangeCheck())
                {
                    if (col != null)
                    {
                        col.text = text;
                        Project.singleton.setDirty();
                    }
                    else
                    {
                        Debug.Log("you cannot edit this field because there is no column " + i.languagesSuport + " on Gridly. To fix this please login to Gridly and add column with columnID as \"" + i.languagesSuport + "\"");
                    }
                }
                if (GUILayout.Button(new GUIContent() { text = "Export", tooltip = "Export this field to Gridly" }, GUILayout.Width(60)))
                {
                    GridlyFunctionEditor.editor.UpdateRecordLang(record, grid.choesenViewID, i.languagesSuport);
                }
                GUILayout.EndHorizontal();
            }
            #region Object Field
            GUILayout.Space(10);
            DrawObjectField(record);
            #endregion
            GUILayout.Space(2);
            if (isRename)
            {
                ShowRename();
                void ShowRename()
                {
                    GUILayout.BeginHorizontal(TextStyle);
                    GUILayout.Label(record.recordID);
                    theNameToRename = GUILayout.TextField(theNameToRename, GUILayout.Width(position.width * 0.7f));
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
                if (EditorUtility.DisplayDialog("Confirm delete", "Are you sure you want to delete this record", "Yes", "Cancel"))
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
            GUILayout.EndHorizontal();
            #endregion
            GUILayout.Space(mRowSize);
        }
        void DrawObjectField(Record record)
        {
            var normalColor = GUI.backgroundColor;
            bool exitFileCol = record.columns.Find(x => x.columnID == ObjectField.FILE_COLUMN_ID) != null;
            GUI.backgroundColor = Color.Lerp(Color.yellow, Color.white, 0.4f);
            if (GUILayout.Button(new GUIContent("file")))
            {
                if (!exitFileCol)
                {
                    Debug.LogError("please create more column with id \"" + ObjectField.FILE_COLUMN_ID + "\" on Gridly");
                    return;
                }
                isBeginFoldOutFileField = !isBeginFoldOutFileField;
            }
            if (isBeginFoldOutFileField && exitFileCol)
            {
                int removeAt = -1;
                for (int i = 0; i < record.objects.Count; i++)
                {
                    GUILayout.BeginHorizontal(TextStyle);

                    EditorGUI.BeginChangeCheck();
                    record.objects[i].obj = EditorGUILayout.ObjectField(record.objects[i].obj, typeof(Object), allowSceneObjects: false);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Project.singleton.setDirty();
                    }
                    if (GUILayout.Button(new GUIContent() { text = "Export", tooltip = "Export this field to Gridly" }, GUILayout.Width(60)))
                    {
                        string path = AssetDatabase.GetAssetPath(record.objects[i].obj);
                        string fullPath = Application.dataPath.Replace("Assets", "") + path;
                        GridlyFunctionEditor.editor.UploadFile(grid.choesenViewID, record.recordID, fullPath);
                    }
                    if (GUILayout.Button("x", GUILayout.Width(30)))
                    {
                        removeAt = i;
                    }
                    GUILayout.EndHorizontal();
                }
                if (removeAt != -1)
                {
                    record.objects.RemoveAt(removeAt);
                    Project.singleton.setDirty();
                }
                if (GUILayout.Button("+", GUILayout.Width(30)))
                {
                    record.objects.Add(new ObjectField());
                    Project.singleton.setDirty();
                }
            }
            GUI.backgroundColor = normalColor;
        }
    }
}


