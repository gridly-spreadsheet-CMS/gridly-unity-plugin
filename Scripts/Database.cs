using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gridly.Internal
{
    [System.Serializable]
    public class Database
    {
        public string databaseName;
        public string databaseID;
        
        public Database(string id, string name)
        {
            databaseID = id;
            databaseName = name;
        }

        public List<Grid> grids = new List<Grid>();
    }

}