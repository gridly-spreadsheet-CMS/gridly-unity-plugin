using UnityEngine.Networking;
using UnityEditor;
using System;
using UnityEngine;
using System.Text;

namespace Gridly.Internal
{
    public static class GridlyEditor
    {
        static string projectIDListURL => "https://api.gridly.com/v1/projects";
        static string databaseListURL => "https://api.gridly.com/v1/databases?projectId=" + Project.singleton.ProjectID;
        static string gridsListURL => "https://api.gridly.com/v1/grids?dbId=";
        static string gridlyDatatbasePath => "Assets/Gridly/Resources/Databases";
        
       
        public static void ShowProjectIDs()
        {
            
            UnityWebRequest unityWeb = UnityWebRequest.Get(projectIDListURL);
            unityWeb.SetRequestHeader("Authorization", "ApiKey " + UserData.singleton.keyAPI);
            unityWeb.SendWebRequest();

            void action()
            {
                if (unityWeb.isDone)
                {
                    var i = unityWeb.downloadHandler.text;
                    if (i.CheckOutput())
                    {
                        var N = JSONNode.Parse(i);

                        int index = 0;
                        while (N[index].Count != 0)
                        {
                            string name = "";
                            string id = "";
                            ("Name: " + N[index]["name"] + " | ID: " + N[index]["id"]).Print();
                            index++;
                        }
                    }

                }
            };


            CancelWhenDone(action, unityWeb);

        }

        public static void SetupDatabases()
        {
            "Is setting up the database...".Print();
            UnityWebRequest unityWeb = UnityWebRequest.Get(databaseListURL);
            unityWeb.SetRequestHeader("Authorization", "ApiKey " + UserData.singleton.keyAPI);
            unityWeb.SendWebRequest();
            void action()
            {
                if (unityWeb.isDone)
                {
                    var i = unityWeb.downloadHandler.text;
                    if (i.CheckOutput())
                    {
                        ClearDatabases();
                        var N = JSONNode.Parse(i);

                        int index = 0;
                        Project.singleton.databases.Clear();

                        //Setup database
                        while (N[index].Count != 0)
                        {
                            Database database = new Database(N[index]["id"], N[index]["name"]);
#if Gridly_UseSeparateData
                            AssetDatabase.CreateFolder(gridlyDatatbasePath, N[index]["name"]);
#endif
                            SetupGrids(database);
                            Project.singleton.databases.Add(database);

                            index++;
                        }




                        EditorUtility.SetDirty(Project.singleton);
                        AssetDatabase.SaveAssets();

                    }

                }
            }
            CancelWhenDone(action, unityWeb);
        }

        public static void SetupGrids(Database database)
        {
            UnityWebRequest unityWeb = UnityWebRequest.Get(gridsListURL + database.databaseID);
            unityWeb.SetRequestHeader("Authorization", "ApiKey " + UserData.singleton.keyAPI);
            unityWeb.SendWebRequest();

            void SetupGrid()
            {
                if (unityWeb.isDone)
                {
                    ("Setting up grids for " + database.databaseName).Print();
                    var i = unityWeb.downloadHandler.text;
                    if (i.CheckOutput())
                    {
                        var N = JSON.Parse(i);
                        int index = 0;
                        string _path = "";
                        while (N[index].Count != 0)
                        {
                            _path = gridlyDatatbasePath + "/" + database.databaseName;


#if Gridly_UseSeparateData
                            AssetDatabase.CreateFolder(_path, N[index]["name"]);
                            AssetDatabase.CreateFolder(_path + "/" + N[index]["name"], "Records");
                            Grid grid = ScriptableObject.CreateInstance<Grid>();
                          
#else
                            Grid grid = new Grid();

#endif

                            grid.gridID = N[index]["id"];
                            grid.nameGrid = N[index]["name"];
                            grid.viewID = N[index]["defaultAccessViewId"];



#if Gridly_UseSeparateData
                            try
                            {
                                AssetDatabase.CreateAsset(grid, _path+"/"+N[index]["name"]+"/"+ N[index]["name"]+".asset");
                                
                            }
                            catch(Exception e)
                            {
                                e.Message.Error();
                                break;
                            }
#endif
                            database.grids.Add(grid);
                            SetupRecords(grid, _path + "/" + N[index]["name"] + "/Records",0);
                            index++;
                        }
                        EditorUtility.SetDirty(Project.singleton);
                        AssetDatabase.SaveAssets();
                    }
                }
            }
            CancelWhenDone(SetupGrid, unityWeb);


        }


        public static string GetRecordPage(string viewID,int i)
        {
            int begin = (i * step);
            
            string _ = "https://api.gridly.com/v1/views/" + viewID + "/records?page=%7B%22offset%22%3A+" + begin.ToString() + "%2C+%22limit%22%3A+" + ((i + 1) * step).ToString() + "%7D";
            
            return _;
        }
        static int step = 1000;

        public static void SetupRecords(Grid grid, string pathRecord, int page)
        {
            ("Setting up record for " + grid.nameGrid).Print();

            UnityWebRequest unityWeb = UnityWebRequest.Get(GetRecordPage(grid.viewID, page));
            unityWeb.SetRequestHeader("Authorization", "ApiKey " + UserData.singleton.keyAPI);
            unityWeb.SendWebRequest();

            void action()
            {
                if (unityWeb.isDone)
                {
                    var i = unityWeb.downloadHandler.text;
                    if (i.CheckOutput())
                    {
                        
                        var N = JSON.Parse(i);
                        int index = 0;
                        string _path = "";
                        while (N[index].Count != 0)
                        {
#if Gridly_UseSeparateData
                            Record record = ScriptableObject.CreateInstance<Record>();
#else
                            Record record = new Record();
#endif
                            record.recordID = N[index]["id"];
                            record.pathTag = N[index]["path"];
                            int index1 = 0;
                            while (N[index]["cells"][index1].Count != 0)
                            {



                                record.columns.Add(new Column(N[index]["cells"][index1]["columnId"], N[index]["cells"][index1]["value"]));

                                /*
                                int lengthVal = N[index]["cells"][index1]["value"].Count;
                                for (int indexValue = 0; indexValue <= lengthVal; indexValue++)
                                {
                                    Debug.Log(N[index]["cells"][index1]["columnId"] + " " + N[index]["cells"][index1]["value"].Count);
                                    if(indexValue==0)
                                        record.columns.Add(new Column(N[index]["cells"][index1]["columnId"], N[index]["cells"][index1]["value"]));
                                    else
                                    {
                                        record.columns.Add(new Column(N[index]["cells"][index1]["columnId"], N[index]["cells"][index1]["value"][indexValue]));
                                    }
                                    
                                }
                               */
                                
                                index1++;
                            }
                            grid.records.Add(record);
#if Gridly_UseSeparateData
                            AssetDatabase.CreateAsset(record, pathRecord+"/"+record.recordID+ ".asset");
#endif
                            
                            if (index == (step-1))
                            {
                                SetupRecords(grid, pathRecord, page+1); 
                            }

                            index++;
                        }

#if Gridly_UseSeparateData
                        EditorUtility.SetDirty(grid);
#else
                        EditorUtility.SetDirty(Project.singleton);
#endif
                        AssetDatabase.SaveAssets();



                    }
                }
                

                    
            }
            CancelWhenDone(action, unityWeb);
            

        }

        public static void ClearDatabases()
        {
            if (AssetDatabase.IsValidFolder(gridlyDatatbasePath))
            {
                AssetDatabase.DeleteAsset(gridlyDatatbasePath);
            }
#if Gridly_UseSeparateData
            AssetDatabase.CreateFolder("Assets/Gridly/Resources", "Databases");
#endif
        }


        public static void CancelWhenDone(Action action, UnityWebRequest unityWebRequest)
        {
            action += () =>
            {
                if (unityWebRequest.isDone)
                {
                    EditorApplication.update -= action.Invoke;
                    ("Server Message: " + unityWebRequest.downloadHandler.text).Print();
                }
            };

            EditorApplication.update += action.Invoke;
        }

    }


    [CustomEditor(typeof(Project))]
    public class GridlyToolForProject: Editor
    {
        Texture m_logo;
        private void OnEnable()
        {
            m_logo = (Texture2D)Resources.Load("Gridly_Icon", typeof(Texture2D));

        }
        public override void OnInspectorGUI()
        {
            //Logo
            GUILayout.BeginHorizontal("box");
            GUILayout.FlexibleSpace();
            GUILayout.Label(m_logo);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            //
            base.OnInspectorGUI();
            if (GUILayout.Button("Save"))
            {
                ((Project)target).Save();
            }


        }
    }

#if  Gridly_UseSeparateData
    [CustomEditor(typeof(Grid))]
    public class GridlyToolForGrid: Editor
    {

        string _addRecord;
        string _updateRecord;
        string _deleteRecord;
        Texture m_logo;
        private void OnEnable()
        {
            m_logo = (Texture2D)Resources.Load("Gridly_Icon", typeof(Texture2D));

        }
        public override void OnInspectorGUI()
        {
            //Logo
            GUILayout.BeginHorizontal("box");
            GUILayout.FlexibleSpace();
            GUILayout.Label(m_logo);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            //
            base.OnInspectorGUI();
            if (GUILayout.Button("Save"))
            {
                ((Grid)target).Save();
            }

            GUILayout.Space(20);
            GUILayout.Label("____________GRIDLY TOOL_________________________________________________________________________________");
            GUILayout.Space(20);

            //Add Record
            GUILayout.Label("Enter the ID of the record you just ADDED:", EditorStyles.boldLabel);
            GUILayout.Space(5);
            _addRecord = GUILayout.TextField(_addRecord);
            GUILayout.Space(5);
            if (GUILayout.Button("Add Record To Server"))
            {
                Grid grid = (Grid)target;
                Record record = grid.records.Find(x => x.recordID == _addRecord);

                //check record
                if (record == null)
                {
                    ("cant find your recordID in grid. Please double-check that you added the recordID to the grid and make sure you entered the correct recordID").Error();
                    return;
                }

                AddRecord(record, grid.viewID);

            }



            //Update Record
            GUILayout.Space(30);
            GUILayout.Label("Enter the ID of the record you want to UPDATE:", EditorStyles.boldLabel);
            _updateRecord = GUILayout.TextField(_updateRecord);
            if (GUILayout.Button("Update Record To Server"))
            {
                Grid grid = (Grid)target;
                Record record = grid.records.Find(x => x.recordID == _updateRecord);

                // check record
                if(record == null)
                {
                    ("cant find your recordID in grid. Please double-check that you added the recordID to the grid and make sure you entered the correct recordID").Error();
                    return;
                }

                UpdateRecord(record, grid.viewID);
            }



            //Delete Record
            GUILayout.Space(30);
            GUILayout.Label("Enter the ID of the record you want to DELETE:", EditorStyles.boldLabel);
            _deleteRecord = GUILayout.TextField(_deleteRecord);
            if (GUILayout.Button("Update Record To Server"))
            {
                Grid grid = (Grid)target;

                //remove if record in grid list
                Record record = grid.records.Find(x => x.recordID == _deleteRecord);
                grid.records.Remove(record);
                grid.Save();

                
                grid.records.Find(x => x.recordID == _deleteRecord);
                
                DeleteRecord(_deleteRecord, grid.viewID);
            }


        }


        string GetCode(Record record)
        {
            string str = "";
            int length = record.columns.Count;
            for (int i = 0; i < length; i++)
            {
                if (i > 0)
                    str += ",";
                str += "{\"columnId\": \""+ record.columns[i].columnID+"\",\"value\": \""+record.columns[i].text+"\"}";
            }


            return str;
        }

        //Delete record on gridly
        public void DeleteRecord(string recordID, string viewID)
        {
            string a = "{\"ids\": [\"{" + recordID+"}\"]}";
            byte[] data = Encoding.ASCII.GetBytes(a);

            DownloadHandler downloadHandler = new DownloadHandlerBuffer();
            UploadHandler uploadHandler = new UploadHandlerRaw(data);
            UnityWebRequest unityWeb = new UnityWebRequest("https://api.gridly.com/v1/views/" + viewID + "/records", "DELETE", downloadHandler, uploadHandler);
            unityWeb.SetRequestHeader("Content-Type", "application/json");
            unityWeb.SetRequestHeader("Authorization", "ApiKey " + UserData.singleton.keyAPI);
            unityWeb.SendWebRequest();
            GridlyEditor.CancelWhenDone(() => { }, unityWeb);
        }


        //Update record on girdly
        public void UpdateRecord(Record record, string viewID)
        {
            string a = "[{\"id\": \"" + record.recordID + "\",\"path\": \"" + record.pathTag + "\",\"cells\": [" + GetCode(record) + "]}]";

            //a.Print();
            byte[] data = Encoding.ASCII.GetBytes(a);

            DownloadHandler downloadHandler = new DownloadHandlerBuffer();
            UploadHandler uploadHandler = new UploadHandlerRaw(data);
            UnityWebRequest unityWeb = new UnityWebRequest("https://api.gridly.com/v1/views/" + viewID + "/records", "PATCH", downloadHandler, uploadHandler);
            unityWeb.SetRequestHeader("Content-Type", "application/json");
            unityWeb.SetRequestHeader("Authorization", "ApiKey " + UserData.singleton.keyAPI);
            unityWeb.SendWebRequest();

            GridlyEditor.CancelWhenDone(()=> { }, unityWeb);
        }


        //Add new record on gridly
        public void AddRecord(Record record, string viewID)
        {


            string a = "[{\"id\": \""+record.recordID+"\",\"path\": \""+record.pathTag+"\",\"cells\": ["+GetCode(record)+"]}]";

            //a.Print();
            byte[] data = Encoding.ASCII.GetBytes(a);


            DownloadHandler downloadHandler = new DownloadHandlerBuffer();
            UploadHandler uploadHandler = new UploadHandlerRaw(data);
            UnityWebRequest unityWeb = new UnityWebRequest("https://api.gridly.com/v1/views/"+viewID+"/records", "POST", downloadHandler, uploadHandler);
            unityWeb.SetRequestHeader("Content-Type", "application/json");
            unityWeb.SetRequestHeader("Authorization", "ApiKey " + UserData.singleton.keyAPI);
            unityWeb.SendWebRequest();


            GridlyEditor.CancelWhenDone(()=> { }, unityWeb);
        }



    }


    [CustomEditor(typeof(Record))]
    public class GirdlyToolForRecord : Editor
    {
        Texture m_logo;
        private void OnEnable()
        {
            m_logo = (Texture2D)Resources.Load("Gridly_Icon", typeof(Texture2D));

        }
        public override void OnInspectorGUI()
        {

            //Logo
            GUILayout.BeginHorizontal("box");
            GUILayout.FlexibleSpace();
            GUILayout.Label(m_logo);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            //


            base.OnInspectorGUI();
            if (GUILayout.Button("Save"))
            {
                ((Record)target).Save();
            }

        }

    }


#endif

}