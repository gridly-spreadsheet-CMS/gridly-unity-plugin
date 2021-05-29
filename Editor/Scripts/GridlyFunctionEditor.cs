using UnityEngine.Networking;
using UnityEditor;
using System;
using UnityEngine;
using System.Text;
namespace Gridly.Internal
{
    public class GridlyFunctionEditor : GridlyFunction
    {

        public static GridlyFunctionEditor editor = new GridlyFunctionEditor();
        #region UpdateDeleteAddRecord
        public void DeleteRecord(string recordID, string viewID, Action success)
        {
            string a = "{\"ids\": [\"" + recordID + "\"]}";

            byte[] data = Encoding.ASCII.GetBytes(a);

            DownloadHandler downloadHandler = new DownloadHandlerBuffer();
            UploadHandler uploadHandler = new UploadHandlerRaw(data);
            UnityWebRequest unityWeb = new UnityWebRequest("https://api.gridly.com/v1/views/" + viewID + "/records", "DELETE", downloadHandler, uploadHandler);
            unityWeb.SetRequestHeader("Content-Type", "application/json");
            unityWeb.SetRequestHeader("Authorization", "ApiKey " + UserData.singleton.keyAPI);
            unityWeb.SendWebRequest();
            void action()
            {
                if (unityWeb.isDone)
                {
                    if (string.IsNullOrWhiteSpace(unityWeb.downloadHandler.text))
                    {
                        success?.Invoke();
                    }
                    else
                    {
                        var N = JSON.Parse(unityWeb.downloadHandler.text);
                        if (N["code"] == "recordNotFound")
                            success?.Invoke();

                    }
                }
            }
            CancelWhenDone(action, unityWeb);
        }
        public void UpdateRecord(Record record, string viewID)
        {
            string a = "[{\"id\": \"" + record.recordID + "\",\"path\": \"" + record.pathTag + "\",\"cells\": [" + GetCode(record) + "]}]";

            a.Print();
            byte[] data = Encoding.ASCII.GetBytes(a);

            DownloadHandler downloadHandler = new DownloadHandlerBuffer();
            UploadHandler uploadHandler = new UploadHandlerRaw(data);
            UnityWebRequest unityWeb = new UnityWebRequest("https://api.gridly.com/v1/views/" + viewID + "/records", "PATCH", downloadHandler, uploadHandler);
            unityWeb.SetRequestHeader("Content-Type", "application/json");
            unityWeb.SetRequestHeader("Authorization", "ApiKey " + UserData.singleton.keyAPI);
            unityWeb.SendWebRequest();

            CancelWhenDone(() => { }, unityWeb);
        }
        public void AddRecord(Record record, string viewID)
        {


            string a = "[{\"id\": \"" + record.recordID + "\",\"path\": \"" + record.pathTag + "\",\"cells\": [" + GetCode(record) + "]}]";
            a.Print();
            byte[] data = Encoding.UTF8.GetBytes(a);


            DownloadHandler downloadHandler = new DownloadHandlerBuffer();
            UploadHandler uploadHandler = new UploadHandlerRaw(data);

            string url = "https://api.gridly.com/v1/views/" + viewID + "/records";
            UnityWebRequest unityWeb = new UnityWebRequest("https://api.gridly.com/v1/views/" + viewID + "/records", "POST", downloadHandler, uploadHandler);
            unityWeb.SetRequestHeader("Content-Type", "application/json");
            unityWeb.SetRequestHeader("Authorization", "ApiKey " + UserData.singleton.keyAPI);

            unityWeb.SendWebRequest();


            CancelWhenDone(() => { }, unityWeb);
        }
        static string GetCode(Record record)
        {
            string str = "";
            int length = record.columns.Count;
            for (int i = 0; i < length; i++)
            {
                if (i > 0)
                    str += ",";
                str += "{\"columnId\": \"" + record.columns[i].columnID + "\",\"value\": \"" + record.columns[i].text + "\"}";
            }


            return str;
        }
        #endregion

        public override void CancelWhenDone(Action action, UnityWebRequest unityWebRequest, bool printServerMes)
        {
            action += () =>
            {
                if (unityWebRequest.isDone)
                {
                    EditorApplication.update -= action.Invoke;
                    if (printServerMes)
                        ("Server Message: " + unityWebRequest.downloadHandler.text).Print();
                }
            };

            EditorApplication.update += action.Invoke;
        }


        public override void SaveProject()
        {
            Project.singleton.Save();
        }
        public override void SetDirty()
        {
            Project.singleton.setDirty();
        }

    }


}