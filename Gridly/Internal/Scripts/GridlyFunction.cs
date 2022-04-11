using UnityEngine.Networking;
using System;
using UnityEngine;
using System.Threading.Tasks;

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
            string output;
            int length = i.ToString().Length;


            output = i.ToString();


            if (length > 600)
            {
                output = i.ToString().Substring(0, 600);
                output += "...";
            }
            
            Debug.Log("<b>" + output + "</b>");
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


        #region Setup
        public void SetupDatabases()
        {
            RefeshDowloadTotal();

            
            foreach(var i in Project.singleton.grids)
            {
                if (i.choesenViewID == "###")
                    continue;
                i.records.Clear(); //COMMENTED
                SetupRecords(i, 0);
            }
        }

        public static string GetRecordPage(string viewID, int i)
        {
            int begin = (i * step);

            Debug.Log("get record from " + (i * step) + " to: " + (i + 1) * step);
            string _ = "https://api.gridly.com/v1/views/" + viewID + "/records?page=%7B%22offset%22%3A+" + begin.ToString() + "%2C+%22limit%22%3A+" + (step).ToString() + "%7D";

            return _;
        }
        static int step = 1000;


        public async void SetupRecords(Grid grid, int page)
        {
            
            int nprocess = 2; //download 2k record
            for (int i = 0 + page; i < nprocess + page; i++)
            {
                await SetupRecords(grid, i, i == (nprocess + page) - 1);
            }
            
        }
        public static int dowloadn;
        public static int dowloadedTotal;
        public static bool isDowloading => dowloadn < dowloadedTotal;
        public void RefeshDowloadTotal()
        {
            dowloadn = 0;
            dowloadedTotal = 0;
        }

        public async Task SetupRecords(Grid grid, int page, bool autoDowload)
        {
            if (grid == null)
                return;
            ("Setting up record for " + grid.nameGrid).Print();
            dowloadedTotal += 1;

            UnityWebRequest unityWeb = UnityWebRequest.Get(GetRecordPage(grid.choesenViewID, page));
            unityWeb.SetRequestHeader("Authorization", "ApiKey " + UserData.singleton.keyAPI);
            unityWeb.SendWebRequest();

            while (!unityWeb.isDone)
                await Task.Yield();

                Debug.Log(unityWeb.isDone);
                if (unityWeb.isDone)
                {
                    dowloadn += 1;
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
                            grid.records.Add(record);

                            //done 1 process check
                            if (index == (step - 1))
                            {
                                doneOneProcess?.Invoke();
                            }

                            if (index == (step - 1) && autoDowload)
                            {
                                SetupRecords(grid, page + 1);
                                return;
                            }

                            index++;
                        }


                        if (dowloadn == dowloadedTotal)
                        {
                            "Finished downloading".Print();
                            doneOneProcess?.Invoke();
                            finishAction?.Invoke();
                            SaveProject();
                        }

                        //Done
                        SetDirty();

                      
                       
                        
                    }
                }
           


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
        public Action doneOneProcess { get; set; }


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
