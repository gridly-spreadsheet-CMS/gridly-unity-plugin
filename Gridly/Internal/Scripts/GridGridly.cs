using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Gridly.Internal
{
    [System.Serializable]
    public class ViewID
    {
        public string viewName;
        public string viewID;
        public ViewID(string viewName, string viewID)
        {
            this.viewName = viewName;
            this.viewID = viewID;
        }
    }
     
   
    [System.Serializable]
    public class Grid
    {
        public string databaseID;
        public string nameGrid;
        public string gridID;

        public string choesenViewID;
        public List<Record> records = new List<Record>();
        
        public void CopyThisBlankGridTo(ref Grid grid)
        {
         
            grid.databaseID = databaseID;
            grid.nameGrid = nameGrid;
            grid.gridID = gridID;
            //grid.syncSchedule = new SyncSchedule(syncSchedule);

        }
        public Grid()
        {

        }
    }

}