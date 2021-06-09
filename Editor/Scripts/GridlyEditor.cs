using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
namespace Gridly.Internal
{
    public class GridlyInfo
    {
        public static string ver = "V1";
        public const int LanguagesLeng = 146;
    }
    public partial class GridlyEditor : EditorWindow
    {
        public static Texture2D m_logo;
        static GUIStyle Style_ToolBarButton_Big;
        public static GUIStyle TextStyle;
        public List<string> listLanguage;
        private void OnEnable()
        {

           
            m_logo = (Texture2D)Resources.Load("Gridly_Icon", typeof(Texture2D));
            Style_ToolBarButton_Big = new GUIStyle(EditorStyles.toolbarButton);
            Style_ToolBarButton_Big.fixedHeight = Style_ToolBarButton_Big.fixedHeight * 1.5f;

            TextStyle = new GUIStyle(EditorStyles.toolbarTextField);
            TextStyle.fixedHeight = 0;

            SelectStyle = new GUIStyle(EditorStyles.toolbarTextField);
            SelectStyle.fixedHeight = 0;
            SelectStyle.normal.textColor = new Color(1, 1, 1, 0.3f);

            listLanguage = new List<string>();
            for (int i = 0; i < GridlyInfo.LanguagesLeng; i++)
            {
                listLanguage.Add(((Languages)(i)).ToString());
            }

           

        }
        public Color darkLightColor = new Color(0, 0, 0, 0.15f);
        public Color darkColor = new Color(0, 0, 0, 0.25f);
        public static GUIStyle SelectStyle;

        public enum eViewMode
        {
            Setting,
            Languages,
            Schedule,
        }
        public static void OnGUI_ToggleEnumBig<Enum>(string text, ref Enum currentMode, Enum newMode, Texture texture, string tooltip)
        {
            OnGUI_ToggleEnum<Enum>(text, ref currentMode, newMode, texture, tooltip, Style_ToolBarButton_Big);
        }
        public static void OnGUI_ToggleEnum<Enum>(string text, ref Enum currentMode, Enum newMode, Texture texture, string tooltip, GUIStyle style)
        {
            GUI.changed = false;
            if (GUILayout.Toggle(currentMode.Equals(newMode), new GUIContent(text, texture, tooltip), style, GUILayout.ExpandWidth(true)))
            {
                currentMode = newMode;
                //if (GUI.changed)
                //  ClearErrors();
            }
        }


    }
    public partial class GridlySetting : GridlyEditor
    {
        #region Header
        public static eViewMode mCurrentViewMode = eViewMode.Setting;
        Vector3 m_Scroll = new Vector3();
        int selectLang;
        string search = "";
        [MenuItem("Tools/Gridly/Setup/Setting", false, 1)]
        private static void InitWindow()
        {
            GridlySetting window = (GridlySetting)GetWindow(typeof(GridlySetting), true, "Gridly Setting Window - " + GridlyInfo.ver);
            Vector2 vector2 = new Vector2(400, 400);
            window.minSize = vector2;
            window.maxSize = vector2;
            window.Show();
            
        }


        //[MenuItem("Tools/Gridly/Import/Gridly/Get Data From Gridly", false, 0)]
        private static void GetDataFromGridly()
        {
            GridlyFunctionEditor.editor.SetupDatabases();
        }
        #endregion


        private void OnGUI()
        {
            GUI.changed = false;
            GUILayout.Label(m_logo);
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            OnGUI_ToggleEnumBig("Gridly Setup", ref mCurrentViewMode, eViewMode.Setting, null, null);
            OnGUI_ToggleEnumBig("Languages", ref mCurrentViewMode, eViewMode.Languages, null, null);
            OnGUI_ToggleEnumBig("Sync Schedule", ref mCurrentViewMode, eViewMode.Schedule, null, null);
            if (EditorGUI.EndChangeCheck())
            {
                m_Scroll = Vector3.zero;
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            if (mCurrentViewMode == eViewMode.Setting)
                SettingWin();
            else if (mCurrentViewMode == eViewMode.Languages)
            {
            
                LanguageWin();
                
            }
            else if (mCurrentViewMode == eViewMode.Schedule)
            {
              
                if (Project.singleton.databases.Count == 0)
                {
                    "you need to setup data before using this tab".Print();
                    mCurrentViewMode = eViewMode.Setting;
                    return;
                }

                ScheduleWin();
                Refesh();
            }

            
        }

        
        void SettingWin()
        {
            GUILayout.Label("Enter your API key here:", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            string _APIkey = EditorGUILayout.TextField(UserData.singleton.keyAPI);
            if (EditorGUI.EndChangeCheck() && !string.IsNullOrEmpty(_APIkey))
            {
                UserData.singleton.keyAPI = _APIkey;
                UserData.singleton.setDirty();
                
            }


            GUILayout.Space(10);
            GUILayout.Label("Enter your Project ID here:", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            Project.singleton.ProjectID = EditorGUILayout.TextField(Project.singleton.ProjectID);
            if (EditorGUI.EndChangeCheck())
            {
                Project.singleton.setDirty();
            }

            GUILayout.Space(10);
            EditorGUI.BeginChangeCheck();
            UserData.singleton.showServerMess = GUILayout.Toggle(UserData.singleton.showServerMess, "Print server messages to the console");
            if (EditorGUI.EndChangeCheck())
                UserData.singleton.setDirty();

            GUILayout.Space(10);
            if (GridlyFunctionEditor.dowloadn > 0)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Dowloading...");
                if(GUILayout.Button("Cancel", GUILayout.Width(50)))
                {
                    GridlyFunctionEditor.dowloadn = 0;
                    EditorApplication.update = null;
                }
                GUILayout.EndHorizontal();

            }

            //setup
            GUILayout.Space(10);
            EditorGUILayout.HelpBox("Download and setup all data", MessageType.Info);
            if (GUILayout.Button("Get data from Gridly"))
            {
                GridlyFunctionEditor.editor.SetupDatabases();
            }

            GUILayout.Space(10);
            if (GUILayout.Button("Clear local data"))
            {
                if (EditorUtility.DisplayDialog("Confirm delete", "Are you sure you want to delete the local data", "Yes", "Cancel"))
                {
                    Project.singleton.databases = new List<Database>();
                    Project.singleton.Save();
                }
            }

        }
        void LanguageWin()
        {
            int deleteIndex = -1;

            #region list Lang
            m_Scroll = GUILayout.BeginScrollView(m_Scroll, TextStyle, GUILayout.MinHeight(200), GUILayout.ExpandHeight(false));
            SerializedObject serializedObject = new SerializedObject(Project.singleton);
            SerializedProperty property = serializedObject.FindProperty("langSupports");

            for (int i = 0; i < property.arraySize; i++)
            {
                GUILayout.Space(2);
                LangSupport langSupport = Project.singleton.langSupports[i];
                GUILayout.BeginHorizontal();
                

                if (GUILayout.Button("X", "toolbarbutton"))
                {
                    deleteIndex = i;
                }


                EditorGUI.BeginChangeCheck();
                string name = langSupport.name;
                name = EditorGUILayout.TextField(langSupport.name);
                langSupport.languagesSuport = (Languages)EditorGUILayout.EnumPopup(langSupport.languagesSuport);

                if (EditorGUI.EndChangeCheck() && !string.IsNullOrEmpty(name))
                {
                    langSupport.name = name;
                    Project.singleton.setDirty();
                }
                GUILayout.EndHorizontal();
                //font

                SerializedProperty font = property.GetArrayElementAtIndex(i).FindPropertyRelative("font");
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(font,true);
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                }

                SerializedProperty fontTM = property.GetArrayElementAtIndex(i).FindPropertyRelative("tmFont");
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(fontTM, true);
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                }




                GUILayout.Space(15);
            }
            GUILayout.EndScrollView();

            if (deleteIndex != -1)
            {
                if (EditorUtility.DisplayDialog("Confirm delete", "Are you sure you want to delete the selected language", "Yes", "Cancel"))
                {
                    Project.singleton.langSupports.RemoveAt(deleteIndex);
                    Project.singleton.setDirty();
                }
            }
            #endregion



            #region AddLang

            GUILayout.Space(3);
            search = GUILayout.TextField(search,GUI.skin.GetStyle( "ToolbarSeachTextField" ));

            GUILayout.BeginHorizontal();
            List<string> final = listLanguage.FindAll(x => x.Contains(search));
            selectLang = EditorGUILayout.Popup(selectLang, final.ToArray(), EditorStyles.toolbarDropDown);
            if (GUILayout.Button("Add"))
            {
                Languages language = (Languages)System.Enum.Parse(typeof(Languages), final[selectLang]);
                foreach (var i in Project.singleton.langSupports)
                    if (i.languagesSuport == language)
                        return;
                Project.singleton.langSupports.Add(new LangSupport() { name = language.ToString(), languagesSuport = language });
                Project.singleton.setDirty();
            }
            GUILayout.EndHorizontal();
            #endregion
        }

        #region schedule

        static GridlyArrData arrData = new GridlyArrData();
        static string chosenDatabase = "";
        static float YPos;
        static float mRowSize = 25;
        static float ScrolSizeY = 200;
        float scrollHeight = 0;
        void ScheduleWin()
        {
       
            if (!arrData.init)
                Refesh();

            EditorGUI.BeginChangeCheck();
            chosenDatabase = arrData.databaseArr[EditorGUILayout.Popup("Databases", arrData.indexDb, arrData.databaseArr)];
            if (EditorGUI.EndChangeCheck())
            {
                m_Scroll = Vector3.zero;
                return;
            }
            GUILayout.Space(10);

            #region Scroll
            
            m_Scroll = GUILayout.BeginScrollView(m_Scroll, TextStyle, GUILayout.MinHeight(ScrolSizeY), GUILayout.ExpandHeight(false));
            if (Event.current != null && Event.current.type == EventType.Layout)
                scrollHeight = m_Scroll.y;

            YPos = 0;
            int nDraw = 0;
            float nSkipStart = 0;
            bool spaceStart = false;

            for (int i = 0; i < gridCount; i++)
            {
                
                if (YPos < scrollHeight&& !spaceStart)
                {
                    spaceStart = true;
                    nSkipStart = ((scrollHeight - mRowSize) / mRowSize);
                    YPos = scrollHeight - mRowSize;
                    i = (int)nSkipStart;
                    GUILayout.Space(nSkipStart * mRowSize);
                }

                //GUILayout.Space(mRowSize);
                
                
                DrawGrid(arrData.chosenDb.grids[i]);

                YPos += mRowSize;

                nDraw++;
                if (YPos > scrollHeight + ScrolSizeY)
                {
                    break;
                }
            }
            GUILayout.Space((gridCount + 1 - (nDraw + nSkipStart)) * (mRowSize));
            GUILayout.EndScrollView();
            #endregion
        }
        void DrawGrid(Grid grid)
        {
            if (!grid.syncSchedule.Enable)
            {
                if (selectGrid == grid.gridID)
                {
                    GUI.backgroundColor = Color.Lerp(Color.gray, Color.cyan, 0.35f);
                    GUI.color = Color.Lerp(Color.cyan, Color.white, 0.3f);
                }
                else
                {
                    GUI.backgroundColor = Color.Lerp(Color.gray, Color.white, 0.8f);
                    GUI.color = Color.white;
                }
            }

            else
            {
                GUI.backgroundColor = Color.Lerp(Color.cyan, Color.white, 0.3f);
                GUI.color = Color.Lerp(Color.cyan, Color.white, 0.3f);
            }



            if(GUILayout.Button(grid.nameGrid,SelectStyle, GUILayout.Height(mRowSize), GUILayout.Width(position.width)))
            {
                if (selectGrid == grid.gridID)
                    selectGrid = "";
                else
                    selectGrid = grid.gridID;
                
            }

            
            if (selectGrid == grid.gridID)
                DrawDetailGrid(grid);
        }

        static string selectGrid = "";
        void DrawDetailGrid(Grid grid)
        {
            GUI.backgroundColor = Color.Lerp(Color.cyan, Color.white, 0.85f);
            GUI.color = Color.white;
            
            GUILayout.BeginHorizontal(TextStyle);
            EditorGUI.BeginChangeCheck();
            grid.syncSchedule.AutoUpdate =  (SyncSchedule.SyncType)(EditorGUILayout.Popup("Auto Update", (int)(grid.syncSchedule.AutoUpdate), Enum.GetNames(typeof(SyncSchedule.SyncType)).ToArray()));
            if (EditorGUI.EndChangeCheck())
                Project.singleton.setDirty();
            GUILayout.EndHorizontal();

            /*
            GUILayout.BeginHorizontal(TextStyle);
            EditorGUI.BeginChangeCheck();
            grid.syncSchedule.EditorAutoUpdate = (SyncSchedule.SyncType)(EditorGUILayout.Popup("In-Editor Auto Update", (int)(grid.syncSchedule.EditorAutoUpdate), Enum.GetNames(typeof(SyncSchedule.SyncType)).ToArray()));
            if (EditorGUI.EndChangeCheck())
                Project.singleton.setDirty();
            GUILayout.EndHorizontal();
            */
            GUILayout.Space(10);
        }
        static void Refesh()
        {
            arrData.RefeshAll(chosenDatabase, null, null, null);

            RefeshList();
        }

        static int gridCount;
        static void RefeshList()
        {
            gridCount = arrData.chosenDb.grids.Count;
        }
        #endregion
    }
    public class DataEditor : EditorWindow
    {

        Texture2D m_logo;
        static GridlyArrData popupData= new GridlyArrData();
        //[MenuItem("Tools/Gridly/Import/Gridly/Data Editor", false, 2)]
        private static void Init()
        {
            DataEditor window = (DataEditor)GetWindow(typeof(DataEditor), true, "Gridly Data Editor - " + GridlyInfo.ver);

            Vector2 vector2 = new Vector2(400, 400);
            window.minSize = vector2;
            window.maxSize = vector2;
            window.Show();

            
            Refesh();
        }

        int page;
        static string databaseSelect = "";
        static string gridSelect = "";
        static string viewSelect = "";
        private void OnEnable()
        {
            m_logo = (Texture2D)Resources.Load("Gridly_Icon", typeof(Texture2D));
            Refesh();
        }

        public void OnGUI()
        {

            GUILayout.Label(m_logo);
            GUILayout.Space(30);
            GUI.changed = false;
            GUILayout.Label("Select your database");
            databaseSelect = popupData.databaseArr[EditorGUILayout.Popup(popupData.indexDb, popupData.databaseArr)];

            if (popupData.indexGrid != -1)
            {
                GUILayout.Space(10);
                GUILayout.Label("Select your grid");
                gridSelect = popupData.gridArr[EditorGUILayout.Popup(popupData.indexGrid, popupData.gridArr)];



                GUILayout.Space(10);
                GUILayout.Label("Select your view");
                viewSelect = popupData.viewArr[EditorGUILayout.Popup(popupData.indexView, popupData.viewArr)];


                GUILayout.Space(10);
                EditorGUILayout.HelpBox("If range = 0 => select from record 0 -> 1000,if range = 1 record 1000 -> 2000,...", MessageType.Info);
                GUILayout.Label("Select Record Range");
                page = EditorGUILayout.IntField(page);


                GUILayout.Space(10);
                if (GUILayout.Button("Merge"))
                {
                    GridlyFunctionEditor.editor.MergeRecord(popupData.grid, page);
                    UserData.singleton.Save();
                }
            }
            if (GUI.changed)
            {
                Refesh();
            }

        }
        static void Refesh()
        {
            popupData.RefeshView(databaseSelect, gridSelect, viewSelect);
        }
    }
    public class ViewEditor : EditorWindow
    {
        #region head
        Texture2D m_logo;

        static GridlyArrData arrData = new GridlyArrData();

        #endregion
        //[MenuItem("Tools/Gridly/Import/Gridly/ViewEditor", false, 49)]
        private static void Init()
        {
            if (Project.singleton.databases.Count == 0)
            {
                Debug.LogError("You need to get Gridly data to use this feature");
                return;
            }
            ViewEditor window = (ViewEditor)GetWindow(typeof(ViewEditor), true, "ViewEditor - " + GridlyInfo.ver);

            Vector2 vector2 = new Vector2(400, 400);
            window.minSize = vector2;
            window.maxSize = vector2;
            window.Show();
            Refesh();
        }
        private void OnEnable()
        {
            m_logo = (Texture2D)Resources.Load("Gridly_Icon", typeof(Texture2D));

        }
        private void OnGUI()
        {
            GUILayout.Label(m_logo);
            GUILayout.Space(30);
            GUI.changed = false;
            GUILayout.Label("Select your database");
            db = arrData.databaseArr[EditorGUILayout.Popup("Databases", arrData.indexDb, arrData.databaseArr)];

            if (arrData.indexGrid == -1)
            {
                Refesh();
                return;
            }

            GUILayout.Space(10);
            GUILayout.Label("Select your grid");
            gridname = arrData.gridArr[EditorGUILayout.Popup("Grids", arrData.indexGrid, arrData.gridArr)];

            GUILayout.Space(10);
            GUILayout.Label("Select your view");
            view = arrData.viewArr[EditorGUILayout.Popup("Views", arrData.indexView, arrData.viewArr)];

            GUILayout.Space(10);
            if (GUILayout.Button("Update data from the new View"))
            {
                GridlyFunctionEditor.editor.UpdateNewDataFromView(arrData.indexDb, arrData.indexGrid, arrData.indexView);
            }

            if (GUI.changed)
                Refesh();
        }

        static string db;
        static string gridname;
        static string view;
        static void Refesh()
        {
            arrData.RefeshView(db, gridname, view);
        }

    }

}