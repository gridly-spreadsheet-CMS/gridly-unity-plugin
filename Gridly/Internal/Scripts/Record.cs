using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Gridly.Internal 
{

    [System.Serializable]
    public class Record
    {
        public string recordID;
        public List<Column> columns = new List<Column>();
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