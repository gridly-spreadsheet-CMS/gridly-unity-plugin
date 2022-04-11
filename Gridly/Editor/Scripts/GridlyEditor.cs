using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;
using System.IO;
using UnityEngine.UIElements;
using Unity.EditorCoroutines.Editor;
using Assets.Gridly.Internal.Scripts;
using Newtonsoft.Json;
using System.Threading.Tasks;

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
        public static List<string> listLanguage;
        public static string[] listGridlyColumnIds;
        public static string gridlyResponse;
        public static void OnEnableEditor()
        {

            try
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
            catch
            {

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
        public static GridlySetting window;
        static string viewID;

        [MenuItem("Tools/Gridly/Setup Setting", false, 0)]
        private static async void InitWindow()
        {
            await initGridlyColumns();
            InitData();
            window = (GridlySetting)GetWindow(typeof(GridlySetting), true, "Gridly Setting Window - " + GridlyInfo.ver);
            Vector2 vector2 = new Vector2(600, 500);
            window.minSize = vector2;
            window.maxSize = vector2;
            window.Show();


        }




        [MenuItem("Tools/Gridly/Export/Export All", false, 100)]
        private static void ExportAll()
        {
            if (EditorUtility.DisplayDialog("Confirm Export", "Are you sure you want to export everything to Gridly?. It will overwrite the old data including translations.", "Yes", "Cancel"))
            {
                foreach (var i in Project.singleton.grids)
                {
                    viewID = i.choesenViewID;
                    GridlyFunctionEditor.editor.AddUpdateRecordAll(i.records, viewID, true, false);
                }
            }
        }

        static void InitData()
        {

            init = true;
            OnEnableEditor();


        }

        public static async Task initGridlyColumns()
        {
            viewID = "";
            List<string> columns = new List<string>();


            foreach (var i in Project.singleton.grids)
            {
                viewID = i.choesenViewID;

            }

            if (!string.IsNullOrEmpty(viewID))
            {

                await GridlyFunctionEditor.getGridlyColumnIds(viewID);



                Assets.Gridly.Internal.Scripts.GirldyView.View gridlyView = JsonConvert.DeserializeObject<Assets.Gridly.Internal.Scripts.GirldyView.View>(gridlyResponse);


                foreach (Assets.Gridly.Internal.Scripts.GirldyView.Column column in gridlyView.columns)
                {
                    if (column.type != null)
                    {
                        columns.Add(column.id);
                    }
                }

                listGridlyColumnIds = columns.ToArray();
                //await Task.Yield();
            }




        }


        #endregion

        static bool init;
        private void OnGUI()
        {
            if (!init)
                InitData();
            GUI.changed = false;
            var centeredStyle = GUI.skin.GetStyle("Label");
            centeredStyle.alignment = TextAnchor.UpperCenter;
            GUILayout.Label(m_logo, centeredStyle);
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            OnGUI_ToggleEnumBig("Gridly Setup", ref mCurrentViewMode, eViewMode.Setting, null, null);
            OnGUI_ToggleEnumBig("Languages", ref mCurrentViewMode, eViewMode.Languages, null, null);

            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            if (mCurrentViewMode == eViewMode.Setting)
                SettingWin();
            else if (mCurrentViewMode == eViewMode.Languages)
            {
                if (Project.singleton.grids == null)
                {
                    Debug.LogError("You don't have any grids added to your project.");
                    return;
                }
                else
                {
                    LanguageWin();
                }

            }

        }



        void SettingWin()
        {



            GUILayout.Label("Enter your API key here:", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            string _APIkey = EditorGUILayout.PasswordField(UserData.singleton.keyAPI);
            if (EditorGUI.EndChangeCheck())
            {
                UserData.singleton.keyAPI = _APIkey;
                UserData.singleton.setDirty();

            }

            GUILayout.Label("Enter screenshots root folder here:", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            string screenshotPath = EditorGUILayout.TextField(UserData.singleton.screenshotPath);
            if (EditorGUI.EndChangeCheck())
            {
                UserData.singleton.screenshotPath = screenshotPath;
                UserData.singleton.setDirty();
            }




            #region viewID
            GUILayout.Space(10);
            GUILayout.Label("Enter your ViewID here:", EditorStyles.boldLabel);
            m_Scroll = GUILayout.BeginScrollView(m_Scroll, TextStyle, GUILayout.Height(150));

            Grid removeGrid = null;
            foreach (var i in Project.singleton.grids)
            {
                EditorGUI.BeginChangeCheck();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Name", GUILayout.Width(50));
                i.nameGrid = GUILayout.TextField(i.nameGrid);

                GUILayout.Label("ViewID", GUILayout.Width(50));
                i.choesenViewID = GUILayout.TextField(i.choesenViewID, GUILayout.ExpandWidth(false), GUILayout.Width(140));
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    removeGrid = i;
                }
                GUILayout.EndHorizontal();

                if (EditorGUI.EndChangeCheck())
                {
                    Project.singleton.setDirty();
                    InitWindow();
                }
            }

            if (removeGrid != null) Project.singleton.grids.Remove(removeGrid);


            #region add new grid "+"

            if (GUILayout.Button("+", EditorStyles.toolbarButton, GUILayout.Width(30)))
            {
                Project.singleton.grids.Add(new Grid());
            }


            GUILayout.EndScrollView();
            #endregion


            #endregion



            GUILayout.Space(10);
            EditorGUI.BeginChangeCheck();
            UserData.singleton.showServerMess = GUILayout.Toggle(UserData.singleton.showServerMess, "Print server messages to the console");
            if (EditorGUI.EndChangeCheck())
                UserData.singleton.setDirty();

            GUILayout.Space(10);
            if (GridlyFunctionEditor.isDowloading)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Dowloading...");
                if (GUILayout.Button("Cancel", GUILayout.Width(50)))
                {
                    GridlyFunctionEditor.editor.RefeshDowloadTotal();
                    EditorApplication.update = null;
                }
                GUILayout.EndHorizontal();

            }

            //setup
            GUILayout.Space(10);
            EditorGUILayout.HelpBox("Download and setup all data", MessageType.Info);
            if (GUILayout.Button(new GUIContent() { text = "Import All", tooltip = "Download all data from Gridly" }))
            {

                GridlyFunctionEditor.editor.doneOneProcess += TermEditor.Refesh;
                GridlyFunctionEditor.editor.doneOneProcess += TermEditor.RepaintThis;

                GridlyFunctionEditor.editor.SetupDatabases();
            }

            GUILayout.Space(10);
            if (GUILayout.Button("Clear local data"))
            {
                if (EditorUtility.DisplayDialog("Confirm delete", "Are you sure you want to delete the local data", "Yes", "Cancel"))
                {
                    try
                    {
                        TermEditor.window.Close();
                    }
                    catch { }
                    Project.singleton.grids = new List<Grid>();
                    Project.singleton.Save();
                }
            }

        }

        async void LanguageWin()
        {




            int deleteIndex = -1;
            #region list Lang
            m_Scroll = GUILayout.BeginScrollView(m_Scroll, TextStyle, GUILayout.MinHeight(300), GUILayout.ExpandHeight(false));
            if (listGridlyColumnIds != null)
            {
                SerializedObject serializedObject = new SerializedObject(Project.singleton);
                SerializedProperty property = serializedObject.FindProperty("langSupports");
                int selectedIndex = 0;
                string columnIdSelect = "";
                int columnIdSelectedIndex;


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
                    int selectedIndexOfColumn = ArrayUtility.IndexOf(listGridlyColumnIds, langSupport.name);
                    selectedIndexOfColumn = EditorGUILayout.Popup(new GUIContent("Column ID in Gridly", "Select columnId from Gridly for this language."), selectedIndexOfColumn, listGridlyColumnIds);

                    if (GUILayout.Button(new GUIContent() { text = "Set source", tooltip = "Set this language as main language in editor" }))
                    {
                        TermEditor.Refesh();
                        TermEditor.RepaintThis();
                        if (TermEditor.window != null)
                            TermEditor.window.OnGUI();
                        UserData.singleton.mainLangEditor = langSupport.languagesSuport;
                        UserData.singleton.setDirty();
                    }

                    langSupport.languagesSuport = (Languages)EditorGUILayout.EnumPopup("Select Langauge", langSupport.languagesSuport);

                    if (EditorGUI.EndChangeCheck() && !string.IsNullOrEmpty(name))
                    {
                        langSupport.name = listGridlyColumnIds[selectedIndexOfColumn];
                        Project.singleton.setDirty();
                    }
                    GUILayout.EndHorizontal();




                    //font
                    SerializedProperty font = property.GetArrayElementAtIndex(i).FindPropertyRelative("font");
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(font, true);
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


                    //screenshotColumn
                    EditorGUI.BeginChangeCheck();
                    GUILayout.BeginHorizontal();

                    if (ArrayUtility.IndexOf(listGridlyColumnIds, langSupport.screenshotColumnId) >= 0)
                    {
                        selectedIndex = ArrayUtility.IndexOf(listGridlyColumnIds, langSupport.screenshotColumnId);
                    }
                    columnIdSelectedIndex = EditorGUILayout.Popup("Screenshot column ID", selectedIndex, listGridlyColumnIds);
                    columnIdSelect = listGridlyColumnIds[columnIdSelectedIndex];

                    if (GUILayout.Button(new GUIContent() { text = "Generate column", tooltip = "Generate column for screenshot" }))
                    {
                        viewID = "";
                        foreach (var view in Project.singleton.grids)
                        {
                            viewID = view.choesenViewID;
                        }


                        await GridlyFunctionEditor.createGridlyColumn(viewID, langSupport.name, "files", false, false);
                        Project.singleton.setDirty();
                        window.Close();
                        InitWindow();


                    }
                    if (EditorGUI.EndChangeCheck())
                    {
                        langSupport.screenshotColumnId = columnIdSelect;
                        //Refesh();
                        Project.singleton.setDirty();
                        return;
                    }

                    GUILayout.EndHorizontal();


                    GUILayout.Space(15);
                }

                GUILayout.EndScrollView();


                string idToDelete = "";
                if (listGridlyColumnIds != null)
                {
                    if (deleteIndex != -1)
                    {

                        if (EditorUtility.DisplayDialog("Confirm delete", "Are you sure you want to delete the selected language", "Yes", "Cancel"))
                        {


                            Languages langDelete = Project.singleton.langSupports[deleteIndex].languagesSuport;
                            idToDelete = Project.singleton.langSupports[deleteIndex].name;
                            foreach (var grid in Project.singleton.grids)
                            {
                                foreach (var i in grid.records)
                                {
                                    i.columns.RemoveAll(x => x.columnID == langDelete.ToString());
                                }
                            }

                            Project.singleton.langSupports.RemoveAt(deleteIndex);
                            Project.singleton.setDirty();



                        }
                        if (EditorUtility.DisplayDialog("Confirm delete column from Gridly", "Are you sure you want to delete the selected column from Gridly", "Yes", "Cancel"))
                        {
                            await GridlyFunctionEditor.deleteGridlyColumn(viewID, idToDelete);
                            await GridlyFunctionEditor.deleteGridlyColumn(viewID, idToDelete + "_Screenshot");
                        }

                    }
                }
            }
            #endregion



            #region AddLang

            GUILayout.Space(3);
            search = GUILayout.TextField(search, GUI.skin.GetStyle("ToolbarSeachTextField"));

            GUILayout.BeginHorizontal();
            List<string> final = listLanguage.FindAll(x => x.Contains(search));
            selectLang = EditorGUILayout.Popup("Select language", selectLang, final.ToArray(), EditorStyles.toolbarDropDown);
            if (GUILayout.Button("Add"))
            {
                Languages language = (Languages)System.Enum.Parse(typeof(Languages), final[selectLang]);
                if (listGridlyColumnIds.Length > 0)
                {
                    //a void duplicate
                    foreach (var i in Project.singleton.langSupports)
                        if (i.languagesSuport == language)
                            return;
                }

                var langSup = new LangSupport() { name = language.ToString(), languagesSuport = language };
                Project.singleton.langSupports.Add(langSup);

                //try add pre-font
                try
                {
                    int count = Project.singleton.langSupports.Count;
                    langSup.font = Project.singleton.langSupports[count - 2].font;
                    langSup.tmFont = Project.singleton.langSupports[count - 2].tmFont;
                    await GridlyFunctionEditor.createGridlyColumn(viewID, langSup.name, "language", true, false);
                    await GridlyFunctionEditor.SetDependency(langSup.name, viewID);
                    window.Close();
                    InitWindow();
                }
                catch
                {

                }

                Project.singleton.setDirty();
            }
            GUILayout.EndHorizontal();
            #endregion

        }


    }

}