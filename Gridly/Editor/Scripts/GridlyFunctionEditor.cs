using UnityEngine.Networking;
using UnityEditor;
using System;
using System.IO;
using UnityEngine;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;
using Unity.EditorCoroutines.Editor;
using Newtonsoft.Json;
using System.Linq;

namespace Gridly.Internal
{
    /* - find the keys where stored in translator script
     * - deleting a lang, delete the col from gridly
     * 
     */
    public class GridlyFunctionEditor : GridlyFunction
    {


        public static GridlyFunctionEditor editor = new GridlyFunctionEditor();

        #region GetColumn IDs

        public static async Task getGridlyColumnIds(string viewID)
        {
            Debug.Log("Getting columnIds from Gridly server from view: " + viewID);

                using (UnityWebRequest webRequest = UnityWebRequest.Get("https://api.gridly.com/v1/views/" + viewID))
                {
                    webRequest.SetRequestHeader("Authorization", "ApiKey " + UserData.singleton.keyAPI);
                    // Request and wait for the desired page.
                    var req = webRequest.SendWebRequest();

                    var waitForOneSecond = new EditorWaitForSeconds(3.0f);

                    while (!req.isDone)
                        await Task.Yield();

                    if (webRequest.result == UnityWebRequest.Result.Success)
                    {
                        GridlyEditor.gridlyResponse = webRequest.downloadHandler.text;
                        Debug.Log("ColumnIds fetched from Gridly server!");
                    }
                    else
                    {
                        Debug.Log(webRequest.error);
                    }



                }




        }

        #endregion

        #region Create screenshot column
        public static async Task<string> createGridlyColumn(string viewID, string langCode, string type, bool isTarget, bool isSource)
        {
            string columnIdAndName;
            string a;
            if (type == "files")
            {
                columnIdAndName = langCode + "_Screenshot";
                a = "{\"name\" : \"" + columnIdAndName + "\", \"type\" : \"" + type + "\", \"id\" : \"" + columnIdAndName + "\"}";
            }
            else
            {
                columnIdAndName = langCode;
                a = "{\"name\" : \"" + columnIdAndName + "\", \"type\" : \"" + type + "\", \"id\" : \"" + columnIdAndName + "\", \"languageCode\" : \"" + langCode + "\", \"isTarget\":\"" + isTarget + "\", \"isSource\": \"" + isSource + "\"}";
            }

            Debug.Log(a);

            var webRequest = new UnityWebRequest("https://api.gridly.com/v1/views/" + viewID + "/columns", "POST");
            byte[] jsonToSend = Encoding.ASCII.GetBytes(a);
            webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Authorization", "ApiKey " + UserData.singleton.keyAPI);
            webRequest.SetRequestHeader("Content-Type", "application/json");
            var req = webRequest.SendWebRequest();


            while (!req.isDone)
                await Task.Yield();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Column for screenshots with name: " + columnIdAndName + " has been created.");
                return columnIdAndName;
            }
            else
            {
                Debug.Log(webRequest.error);
                return null;
            }

        }
        #endregion

        #region Delete Gridly column
        public static async Task deleteGridlyColumn(string viewID, string columnId)
        {

            var webRequest = new UnityWebRequest("https://api.gridly.com/v1/views/" + viewID + "/columns/" + columnId, "DELETE");
            Debug.Log("https://api.gridly.com/v1/views/" + viewID + "/columns/" + columnId);


            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Authorization", "ApiKey " + UserData.singleton.keyAPI);
            webRequest.SetRequestHeader("Content-Type", "application/json");
            var req = webRequest.SendWebRequest();


            while (!req.isDone)
                await Task.Yield();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Column deleted with ID: " + columnId + " has been deleted.");
            }
            else
            {
                Debug.Log(webRequest.error);
            }

        }

        #endregion

        #region Image upload
        public async Task uploadImage(string viewID, string langCode, string recordId, string sceneName)
        {
            string rootPath = UserData.singleton.screenshotPath;
            string screenshotPath = Path.Combine(rootPath, langCode, sceneName + ".png");

            string columnIdAndName = langCode + "_Screenshot";

            Debug.Log(File.Exists(screenshotPath));

            if (File.Exists(screenshotPath))
            {
                await removeImage(viewID, langCode, recordId, sceneName);

                WWWForm form = new WWWForm();
                form.AddBinaryData("file", File.ReadAllBytes(screenshotPath));
                form.AddField("recordId", recordId);
                form.AddField("columnId", columnIdAndName);
                form.AddField("path", sceneName);

                Debug.Log(viewID + " " + recordId + " " + sceneName + " " + columnIdAndName);

                UnityWebRequest webRequest = UnityWebRequest.Post("https://api.gridly.com/v1/views/" + viewID + "/files", form);
                webRequest.SetRequestHeader("Authorization", "ApiKey " + UserData.singleton.keyAPI);

                var req = webRequest.SendWebRequest();

                while (!req.isDone)
                {
                    await Task.Yield();
                }

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("Screenshot uploaded to column: " + columnIdAndName + " and record: " + recordId);
                }
                else
                {
                    Debug.Log(webRequest.error);
                }
            }
        }

        public async Task removeImage(string viewID, string langCode, string recordId, string sceneName)
        {

            string columnIdAndName = langCode + "_Screenshot";

            string a = "[ {  \"id\": \"" + recordId + "\", \"cells\": [{ \"columnId\": \"" + columnIdAndName + "\", \"value\": [] }] } ]";


            var webRequest = new UnityWebRequest("https://api.gridly.com/v1/views/" + viewID + "/records", "PATCH");
            byte[] jsonToSend = Encoding.ASCII.GetBytes(a);
            webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Authorization", "ApiKey " + UserData.singleton.keyAPI);
            webRequest.SetRequestHeader("Content-Type", "application/json");
            var req = webRequest.SendWebRequest();


            while (!req.isDone)
                await Task.Yield();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Screenshot record with ID: " + recordId + " cleared.");
            }
            else
            {
                Debug.Log(webRequest.error);
            }


        }
        #endregion


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
            for (int i = 0; i < length + 1; i++)
            {
                AddUpdateRecordAll(records, viewID, i * limit, (i + 1) * limit, isAdd, isTargetLang);
            }

        }

        private async Task<List<GridlyRecord.Records>> getSourceColumn(string viewID, string columnID)
        {
            List<GridlyRecord.Records> gridlyRecords = new List<GridlyRecord.Records>();

            Debug.Log("Getting columnIds from Gridly server.");

            using (UnityWebRequest webRequest = UnityWebRequest.Get("https://api.gridly.com/v1/views/" + viewID + "/records?columnIds=" + columnID))
            {
                webRequest.SetRequestHeader("Authorization", "ApiKey " + UserData.singleton.keyAPI);
                // Request and wait for the desired page.
                var req = webRequest.SendWebRequest();

                var waitForOneSecond = new EditorWaitForSeconds(3.0f);

                while (!req.isDone)
                    await Task.Yield();

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("GetSourceCol: " + webRequest.downloadHandler.text);
                    gridlyRecords = JsonConvert.DeserializeObject<List<GridlyRecord.Records>>(webRequest.downloadHandler.text);
                    Debug.Log("ColumnIds fetched from Gridly server!");
                }
                else
                {
                    Debug.Log(webRequest.error);
                }



            }
            Debug.Log(gridlyRecords.Count);


            return gridlyRecords;

        }

        private bool isChangedRecord(List<GridlyRecord.Records> records, string text)
        {
            bool exist = records.Where(txt => txt.cells.Where(x => x.Value == text).Any()).Any();

            return exist;

        }

        public async void AddUpdateRecordAll(List<Record> records, string viewID, int from, int to, bool isAdd = true, bool isTargetLang = false)
        {
            string columnId = GetColumnId(records[0], UserData.singleton.mainLangEditor);
            List<GridlyRecord.Records> sourceColumn = await getSourceColumn(viewID, columnId);


            int recordsToSend = 0;

            string a = "[";
            int maxLength = records.Count;
            //Debug.Log(from + " " + to + " " + maxLength);
            for (int i = from; i < to; i++)
            {
                if (i >= maxLength)
                    break;

                Record record = records[i];

                if (Project.singleton.SendIfChanged)
                {

                    if (!isChangedRecord(sourceColumn, record.columns.Where(txt => txt.columnID == columnId).FirstOrDefault().text))
                    {
                        if (Project.singleton.DataToSendSelectedItems.Where(d => d.Contains("Source text")).Any())
                        {
                            if (!isTargetLang)
                            {
                                if (i > 0 && a[a.Length - 1] != ',')
                                    a += ",";
                                a += "{\"id\": \"" + record.recordID + "\",\"path\": \"" + record.pathTag + "\",\"cells\": [" + GetCode(record) + "]}";
                            }
                            else
                            {


                                if (recordsToSend > 0 && a[a.Length - 1] != ',')
                                    a += ",";
                                recordsToSend++;
                                a += "{\"id\": \"" + record.recordID + "\",\"path\": \"" + record.pathTag + "\",\"cells\": [" + GetCodeLang(record, UserData.singleton.mainLangEditor) + "]}";

                            }
                            if (Project.singleton.DataToSendSelectedItems.Where(d => d.Contains("screenshots")).Any())
                            {
                                Debug.Log(UserData.singleton.mainLangEditor);
                                await uploadImage(viewID, GetColumnId(record, UserData.singleton.mainLangEditor), record.recordID, record.pathTag);
                            }
                        }

                        if (Project.singleton.DataToSendSelectedItems.Where(d => d.Contains("All target")).Any())
                        {
                            foreach (LangSupport lang in Project.singleton.langSupports)
                            {
                                if (UserData.singleton.mainLangEditor.ToString() != lang.name)
                                    await uploadImage(viewID, lang.name, record.recordID, record.pathTag);
                            }
                        }
                        else
                        {
                            foreach (string lang in Project.singleton.DataToSendSelectedItems)
                            {
                                if (lang.Length == 4)
                                {
                                    await uploadImage(viewID, lang, record.recordID, record.pathTag);
                                }
                            }
                        }


                    }
                }
                else
                {
                    if (Project.singleton.DataToSendSelectedItems.Where(d => d.Contains("Source text")).Any())
                    {
                        if (!isTargetLang)
                        {
                            if (i > 0 && a[a.Length - 1] != ',')
                                a += ",";
                            a += "{\"id\": \"" + record.recordID + "\",\"path\": \"" + record.pathTag + "\",\"cells\": [" + GetCode(record) + "]}";
                        }
                        else
                        {


                            if (recordsToSend > 0 && a[a.Length - 1] != ',')
                                a += ",";
                            recordsToSend++;
                            a += "{\"id\": \"" + record.recordID + "\",\"path\": \"" + record.pathTag + "\",\"cells\": [" + GetCodeLang(record, UserData.singleton.mainLangEditor) + "]}";

                        }
                        if (Project.singleton.DataToSendSelectedItems.Where(d => d.Contains("screenshots")).Any())
                        {
                            await uploadImage(viewID, GetColumnId(record, UserData.singleton.mainLangEditor), record.recordID, record.pathTag);
                        }
                    }

                    if (Project.singleton.DataToSendSelectedItems.Where(d => d.Contains("All target")).Any())
                    {
                        foreach (LangSupport lang in Project.singleton.langSupports)
                        {
                            if (UserData.singleton.mainLangEditor.ToString() != lang.name)
                                await uploadImage(viewID, lang.name, record.recordID, record.pathTag);
                        }
                    }
                    else
                    {
                        foreach (string lang in Project.singleton.DataToSendSelectedItems)
                        {
                            if (lang.Length == 4)
                            {
                                await uploadImage(viewID, lang, record.recordID, record.pathTag);
                            }
                        }
                    }
                }
            }

            a += "]";

            //a.Print();
            if (recordsToSend > 0)
            {
                Debug.Log("Sending " + recordsToSend + " records to gridly.");
                //Debug.LogError(a);

                AddUpdateRecord(a, viewID, isAdd);
            }
            else
            {
                Debug.Log("No records to send.");
            }
        }


        public void AddUpdateRecord(string a, string viewID, bool isAdd = true)
        {
            byte[] data = Encoding.UTF8.GetBytes(a);
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

        public static async Task<string> SetDependency(string targetColmnId, string viewID)
        {
                
            string sourceColumnId = UserData.singleton.mainLangEditor.ToString();

            Debug.Log("{\"sourceColumnId\": \"" + sourceColumnId + "\", \"targetColumnId\": \"" + targetColmnId + "\" }");

            string a = "{\"sourceColumnId\": \"" + sourceColumnId + "\", \"targetColumnId\": \"" + targetColmnId + "\" }";


            var webRequest = new UnityWebRequest("https://api.gridly.com/v1/views/" + viewID + "/dependencies", "POST");
            byte[] jsonToSend = Encoding.ASCII.GetBytes(a);
            webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Authorization", "ApiKey " + UserData.singleton.keyAPI);
            webRequest.SetRequestHeader("Content-Type", "application/json");
            var req = webRequest.SendWebRequest();


            while (!req.isDone)
                await Task.Yield();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Dependency created.");
                return webRequest.result.ToString();
            }
            else
            {
                Debug.LogError(webRequest.error);
                return webRequest.error;
            }

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
        static string GetColumnId(Record record, Languages languages)
        {

            int length = record.columns.Count;
            for (int i = 0; i < length; i++)
            {
                if (record.columns[i].columnID == languages.ToString())
                {
                    return record.columns[i].columnID;

                }
            }


            return null;
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