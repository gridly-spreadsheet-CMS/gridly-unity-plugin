using UnityEngine.Networking;
using UnityEditor;
using System;
using UnityEngine;
using System.Text;
using System.Collections.Generic;
using System.IO;
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
        public void AddRecord(Record record, string viewID)
        {
            string a = "[{\"id\": \"" + record.recordID + "\",\"path\": \"" + record.pathTag + "\",\"cells\": [" + GetCode(record) + "]}]";
            //a.Print();
            AddUpdateRecord(a, viewID, true);
        }


        public void UpdateRecordLang(Record record, string viewID, Languages languages)
        {
            string a = "[{\"id\": \"" + record.recordID + "\",\"path\": \"" + record.pathTag + "\",\"cells\": [" + GetCodeLang(record, languages) + "]}]";
            //a.Print();
            AddUpdateRecord(a, viewID, false);
        }


        public void AddUpdateRecordAll(List<Record> records, string viewID, bool isAdd = true, bool isTargetLang = false)
        {
            const int limit = 1000;
            int length = records.Count;
            length /= limit;
            for(int i =0; i < length+1; i++)
            {
                AddUpdateRecordAll(records, viewID, i * limit, (i + 1) * limit, isAdd, isTargetLang);
            }

        }
        public void AddUpdateRecordAll(List<Record> records, string viewID, int from, int to, bool isAdd = true, bool isTargetLang = false)
        {
            
            string a = "[";
            int maxLength = records.Count;
            //Debug.Log(from + " " + to + " " + maxLength);
            for (int i =from; i<to; i++)
            {
                if (i >= maxLength)
                    break;

                Record record = records[i];

                if (i > 0)
                    a += ",";

                if(!isTargetLang)
                    a += "{\"id\": \"" + record.recordID + "\",\"path\": \"" + record.pathTag + "\",\"cells\": [" + GetCode(record) + "]}";
                else
                    a += "{\"id\": \"" + record.recordID + "\",\"path\": \"" + record.pathTag + "\",\"cells\": [" + GetCodeLang(record, UserData.singleton.mainLangEditor) + "]}";
            }

            a += "]";

            //a.Print();
            AddUpdateRecord(a, viewID, isAdd);
        }


        public void AddUpdateRecord(string input, string viewID ,bool isAdd = true)
        {
            byte[] data = Encoding.UTF8.GetBytes(input);
            DownloadHandler downloadHandler = new DownloadHandlerBuffer();
            UploadHandler uploadHandler = new UploadHandlerRaw(data);

            UnityWebRequest unityWeb;
            if (isAdd)
            {
                unityWeb = new UnityWebRequest("https://api.gridly.com/v1/views/" + viewID + "/records", "POST", downloadHandler, uploadHandler);
            }
            else
            {
                unityWeb = new UnityWebRequest("https://api.gridly.com/v1/views/" + viewID + "/records", "PATCH", downloadHandler, uploadHandler);
            }

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
                str += "{\"columnId\": \"" + record.columns[i].columnID + "\",\"value\": \"" + TextDataUploadHandle(record.columns[i].text, ref record) + "\"}";
            }


            return str;
        }
        static string GetCodeLang(Record record, Languages languages)
        {
            string str = "";
            int length = record.columns.Count;
            for (int i = 0; i < length; i++)
            {
                if (record.columns[i].columnID == languages.ToString())
                {
                    str += "{\"columnId\": \"" + record.columns[i].columnID + "\",\"value\": \"" + TextDataUploadHandle(record.columns[i].text, ref record) + "\"}";
                    break;
                }
            }


            return str;
        }


        public static string TextDataUploadHandle(string input, ref Record record)
        {
            string output = input;

            if (string.IsNullOrEmpty(input))
            {
                foreach (var i in record.columns)
                {
                    if (i.columnID == UserData.singleton.mainLangEditor.ToString())
                    {
                        output = i.text;
                        break;
                    }
                }
            }

            return output;
        }

        #endregion

        public void UploadFile(string viewID, string recordId, string path)
        {
            UnityWebRequest unityWeb;
            byte[] data = null;
            if (!File.Exists(path))
            {
                Debug.Log("path not exit");
                //return;
            }

            data = File.ReadAllBytes(path);

            double size = ((float)data.Length / 1024) / 1024;
            size = Math.Round(size, 2);
            Debug.Log("uploading "+ size +" MB");

            var splitPath = path.Split('/');
            string nameFile = splitPath[splitPath.Length - 1];

            WWWForm wWWForm = new WWWForm();
            wWWForm.AddBinaryData("file", data, nameFile);
            wWWForm.AddField("recordId", recordId);
            wWWForm.AddField("columnId", ObjectField.FILE_COLUMN_ID);
            
            unityWeb = UnityWebRequest.Post("https://api.gridly.com/v1/views/" + viewID + "/files", wWWForm);
            unityWeb.SetRequestHeader("Authorization", "ApiKey " + UserData.singleton.keyAPI);

            unityWeb.SendWebRequest();
            CancelWhenDone(() => { }, unityWeb);
            //UnityWebRequest.Post("",)
        }


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