using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Gridly.Internal 
{
    public enum StateField
    {
        None,
        Exported,
    }
    [System.Serializable]
    public class ObjectField
    {
        public const string FILE_COLUMN_ID = "file";
        public StateField stateField;
        public Object obj;

    }
    [System.Serializable]
    public class Record
    {
        public string recordID;
        public List<Column> columns = new List<Column>();
        public List<ObjectField> objects = new List<ObjectField>();
        public string pathTag;
        public Record()
        {

        }

        public Record(Record record)
        {
            recordID = record.recordID;
            foreach (var i in record.columns)
                columns.Add(new Column(i.columnID,i.text));
        }
    }

    [System.Serializable]
    public class Column
    {
        public string columnID;
        public string text;

        public Column(string id, string text)
        {
            columnID = id;
            this.text = text;
        }
    }




}