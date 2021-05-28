using UnityEngine.Networking;
using System;
using UnityEngine;

namespace Gridly.Internal
{
    public static class GridlyUtility
    {
        public static void Print(this object i, string text)
        {
            Debug.Log(text + " " + "<b>" + i + "</b>");
        }
        public static void Print(this object i)
        {
            Debug.Log("<b>" + i + "</b>");
        }

        public static bool CheckOutput(this string output)
        {

            if (output.Length == 0)
            {
                Debug.LogError("Something is wrong. Please check your key again");
                //EditorApplication.update = null;

                return false;
            }

            return true;
        }
    }

    public class GridlyFunction
    {

        public static GridlyFunction s = new GridlyFunction();
        static string gridlyDatatbasePath => "Assets/Gridly/Resources/Databases";

        #region viewEditor
        public void UpdateNewDataFromView(int dbIndex, int gridIndex, int viewIndex)
        {
            try
            {
                Grid grid = Project.singleton.databases[dbIndex]
                    .grids[gridIndex];

                grid.records.Clear();
                grid.viewIndex = viewIndex;
                if (grid != null)
                {
                    SetupRecords(grid, 0);
                }

            }
            catch
            {
                Debug.LogError("cant find the grid in your local data. Please check again your viewID, database NAME and the grid Name");

            }
        }

        #endregion

        #region Setup
        public void SetupDatabases()
        {
            dowloadn = 0;
            dowloadedTotal = 0;
            "Is setting up the database...".Print();
            UnityWebRequest unityWeb = UnityWebRequest.Get("https://api.gridly.com/v1/databases?projectId=" + Project.singleton.ProjectID);
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
                        Project.singleton.databases.Clear();

                        //Setup database
                        while (N[index].Count != 0)
                        {
                            Database database = new Database(N[index]["id"], N[index]["name"]);
                            SetupGrids(database);
                            Project.singleton.databases.Add(database);

                            index++;
                        }

                        //Done
                        SetDirty();
                    }

                }
            }
            CancelWhenDone(action, unityWeb);
        }
        public void SetupGrids(Database database)
        {
            UnityWebRequest unityWeb = UnityWebRequest.Get("https://api.gridly.com/v1/grids?dbId=" + database.databaseID);
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
                            Grid grid = new Grid();

                            grid.databaseID = database.databaseID;
                            grid.gridID = N[index]["id"];
                            grid.nameGrid = N[index]["name"];


                            grid.viewID.Add(new ViewID("Default View", N[index]["defaultAccessViewId"]));


                            SetupViewID(grid);

                            database.grids.Add(grid);
                            SetupRecords(grid, 0);
                            index++;
                        }

                        //Done
                        SetDirty();
                    }
                }
            }
            CancelWhenDone(SetupGrid, unityWeb);
        }
        public void SetupViewID(Grid grid)
        {
            UnityWebRequest unityWeb = UnityWebRequest.Get("https://api.gridly.com/v1/views?gridId=" + grid.gridID);
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

                            string name = N[index]["name"];
                            string id = N[index]["id"];

                            if (name != "Default View" && name != "Default view")
                            {
                                grid.viewID.Add(new ViewID(name, id));
                                (grid.nameGrid + " " + name).Print();
                            }

                            index++;
                        }

                        //Done
                        SetDirty();
                    }

                }
            };
            CancelWhenDone(action, unityWeb);

        }
        public static string GetRecordPage(string viewID, int i)
        {
            int begin = (i * step);

            Debug.Log("get record from " + (i * step) + " to: " + (i + 1) * step);
            string _ = "https://api.gridly.com/v1/views/" + viewID + "/records?page=%7B%22offset%22%3A+" + begin.ToString() + "%2C+%22limit%22%3A+" + (step).ToString() + "%7D";

            return _;
        }
        static int step = 1000;


        public void SetupRecords(Grid grid, int page)
        {
            
            int nprocess = 2;
            for (int i = 0 + page; i < nprocess + page; i++)
            {
                SetupRecords(grid, i, i == (nprocess + page) - 1);
            }
            
        }
        public static int dowloadn;
        public static int dowloadedTotal;
        public void SetupRecords(Grid grid, int page, bool autoDowload)
        {
            if (grid == null)
                return;
            ("Setting up record for " + grid.nameGrid).Print();
            dowloadn += 1;
            dowloadedTotal += 1;
            UnityWebRequest unityWeb = UnityWebRequest.Get(GetRecordPage(grid.choesenViewID, page));
            unityWeb.SetRequestHeader("Authorization", "ApiKey " + UserData.singleton.keyAPI);
            unityWeb.SendWebRequest();

            void action()
            {
                if (unityWeb.isDone)
                {
                 
                    var i = unityWeb.downloadHandler.text;
                    dowloadn -= 1;
                    
                    if (i.CheckOutput())
                    {

                        var N = JSON.Parse(i);
                        int index = 0;
                        //string _path = "";
                        doneOneProcess?.Invoke();
                        while (N[index].Count != 0)
                        {

                            Record record = new Record();

                            record.recordID = N[index]["id"];
                            record.pathTag = N[index]["path"];
                            int index1 = 0;
                            while (N[index]["cells"][index1].Count != 0)
                            {



                                string value = "";


                                int lengthVal = N[index]["cells"][index1]["value"].Count;
                                for (int indexValue = 0; indexValue <= lengthVal; indexValue++)
                                {
                                    if (value != "")
                                        value += ";";
                                    if (indexValue == 0)
                                        value += N[index]["cells"][index1]["value"];
                                    else
                                        value += N[index]["cells"][index1]["value"][indexValue];


                                }

                                record.columns.Add(new Column(N[index]["cells"][index1]["columnId"], value));

                                index1++;
                            }
                            grid.records.Add(record);

                            if (index == (step - 1) && autoDowload)
                            {
                                SetupRecords(grid, page + 1);
                                return;
                            }

                            index++;
                        }


                        if (dowloadn == 0)
                        {
                            "Finished downloading".Print();
                            finishAction?.Invoke();
                            SaveProject();
                        }

                        //Done
                        SetDirty();
                    }
                }
            }
            CancelWhenDone(action, unityWeb);


        }
        public void MergeRecord(Grid grid, int page)
        {
            if (grid == null)
                return;
            ("Setting up record for " + grid.nameGrid).Print();

            UnityWebRequest unityWeb = UnityWebRequest.Get(GetRecordPage(grid.choesenViewID, page));
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
                        //string _path = "";
                        while (N[index].Count != 0)
                        {

                            Record record = new Record();

                            record.recordID = N[index]["id"];
                            record.pathTag = N[index]["path"];
                            int index1 = 0;
                            while (N[index]["cells"][index1].Count != 0)
                            {
                                string value = "";

                                int lengthVal = N[index]["cells"][index1]["value"].Count;
                                for (int indexValue = 0; indexValue <= lengthVal; indexValue++)
                                {
                                    if (value != "")
                                        value += ";";
                                    if (indexValue == 0)
                                        value += N[index]["cells"][index1]["value"];
                                    else
                                        value += N[index]["cells"][index1]["value"][indexValue];


                                }

                                record.columns.Add(new Column(N[index]["cells"][index1]["columnId"], value));

                                index1++;
                            }

                            Record _tempRecord = grid.records.Find(x => x.recordID == record.recordID);
                            if (_tempRecord != null)
                            {
                                grid.records.Remove(_tempRecord);
                            }
                            grid.records.Add(record);
                            index++;
                        }


                        //Project.singleton.Save();
                    }
                }



            }
            CancelWhenDone(action, unityWeb);

        }
        #endregion

        public virtual void SetDirty()
        {
            
        }

        public virtual void SaveProject()
        {

        }

        public static Action process;
        public Action finishAction;
        public Action doneOneProcess;


        public void CancelWhenDone(Action action, UnityWebRequest unityWebRequest)
        {
            CancelWhenDone(action, unityWebRequest, UserData.singleton.showServerMess);
        }

        public virtual void CancelWhenDone(Action action, UnityWebRequest unityWebRequest, bool printServerMes)
        {
            action += () =>
            {
                if (unityWebRequest.isDone)
                {
                    process -= action.Invoke;
                    if (printServerMes)
                        ("Server Message: " + unityWebRequest.downloadHandler.text).Print();
                }
            };

            process += action.Invoke;
        }




    }

}
