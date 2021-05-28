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
    public class SyncSchedule
    {
        public enum SyncType
        {
            Never,
            WhenOpen,
            Daily,
            Weekly,
            Monthly,
        }
        public bool Enable => AutoUpdate != SyncType.Never;
        public SyncType AutoUpdate;
        public SyncType EditorAutoUpdate;
        public System.DateTime lastestUpdateTime;
        public SyncSchedule(SyncSchedule copy)
        {
            AutoUpdate = copy.AutoUpdate;
            EditorAutoUpdate = copy.EditorAutoUpdate;
            lastestUpdateTime = copy.lastestUpdateTime;
        }

        public SyncSchedule()
        {

        }
    }
    [System.Serializable]
    public class Grid
    {
        public string databaseID;
        public string nameGrid;
        public string gridID;
        public int viewIndex;
        public List<ViewID> viewID = new List<ViewID>();
        public SyncSchedule syncSchedule = new SyncSchedule();
        public string choesenViewID 
        {
            get
            {
                try
                {
                    return viewID[viewIndex].viewID;
                }
                catch
                {
                    viewIndex = 0;
                    return viewID[viewIndex].viewID;
                }
            }
        }
        public List<Record> records = new List<Record>();
        
        public void CopyThisBlankGridTo(ref Grid grid)
        {
            grid.databaseID = databaseID;
            grid.nameGrid = nameGrid;
            grid.gridID = gridID;
            grid.viewIndex = viewIndex;
            grid.syncSchedule = new SyncSchedule(syncSchedule);
            grid.viewID = new List<ViewID>(viewID);

        }
        public Grid()
        {

        }
    }

}