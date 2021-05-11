using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Gridly.Internal 
{
#if Gridly_UseSeparateData
    [CreateAssetMenu(fileName = "New Record", menuName = "Gridly/Add New Record", order = 1)]
#endif
    [System.Serializable]
    public class Record
#if Gridly_UseSeparateData
        : ScriptableObject
#endif
    {
        public string recordID;
        public string pathTag;
        public List<Column> columns = new List<Column>();

        public Record()
        {

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