using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace Gridly.Internal
{
    public class GridlyArrData
    {
        bool _init;
        public bool init => _init;
        public string[] databaseArr;
        public string[] gridArr;
        public string[] keyArr;
        public string[] viewArr;
        public int indexDb;
        public int indexGrid;
        public int indexKey;
        public int indexView;
        public void RefeshKey(string dbname, string gridname, string keyID)
        {
            if (dbname == null)
                dbname = "";
            if (gridname == null)
                gridname = "";
            if (keyID == null)
                keyID = "";
            RefeshAll(dbname, gridname, null, keyID);

        }
        public void RefeshView(string dbname, string gridname, string viewID)
        {
            if (dbname == null)
                dbname = "";
            if (gridname == null)
                gridname = "";
            if (viewID == null)
                viewID = "";
            RefeshAll(dbname, gridname, viewID, null);
        }
        public void RefeshAll(string dbname, string gridname, string viewID, string keyID) 
        {
            _init = true;
            int length = 0;
            Database database = null;

            //databse
            length = Project.singleton.databases.Count;
            databaseArr = new string[length];
            for (int i = 0; i < length; i++)
            {
                databaseArr[i] = Project.singleton.databases[i].databaseName;
            }
            indexDb = GetIndex(dbname, databaseArr);
            if (length != 0 && indexDb == -1)
                indexDb = 0;
            

            //grid
            if (gridname == null)
                return;
            if (indexDb != -1)
            {

                database = Project.singleton.databases[indexDb];
                length = database.grids.Count;
                gridArr = new string[length];
                for (int i = 0; i < length; i++)
                {
                    gridArr[i] = database.grids[i].nameGrid;
                }
                indexGrid = GetIndex(gridname, gridArr);
                if (length != 0 && indexGrid == -1)
                    indexGrid = 0;
            }

            //view
            if (viewID != null)
            {
                if (indexGrid != -1)
                {
                    Grid grid = database.grids[indexGrid];
                    length = grid.viewID.Count;
                    viewArr = new string[length];
                    for (int i = 0; i < length; i++)
                    {
                        viewArr[i] = grid.viewID[i].viewName;
                    }
                    indexView = GetIndex(viewID, viewArr);
                    if (length != 0 && indexView == -1)
                        indexView = 0;
                }

                else
                {
                    viewArr = new string[0];
                    indexView = -1;
                }

            }
            else indexView = -1;

            if (keyID != null)
            {
                if (indexGrid != -1)
                {
                    Grid grid = database.grids[indexGrid];
                    length = grid.records.Count;
                    List<string> keyArrList = new List<string>();

                    
                    for (int i = 0; i < length; i++)
                    {
                        keyArrList.Add(grid.records[i].recordID);
                    }

                    if (!string.IsNullOrEmpty(searchKey))
                    {
                        keyArrList = keyArrList.FindAll(x => x.Contains(searchKey));

                    }
                    keyArr = keyArrList.ToArray();

                    indexKey = GetIndex(keyID, keyArr);
                    if (length != 0 && indexKey == -1)
                        indexKey = 0;


                }

                else
                {
                    keyArr = new string[0];
                    indexKey = -1;
                }
            }

        }
       
        int GetIndex(string select, string[] arr)
        {
            int index = 0;
            foreach (var i in arr)
            {
                if (select == i)
                    return index;
                index += 1;
            }
            return -1;
        }


        public string searchKey;
        public string chosenDbName => databaseArr[indexDb];
        public string chosenGridName => gridArr[indexGrid];
        public string chosenViewName => viewArr[indexView];

        public Grid grid
        {
            get
            {
                try
                {
                    return Project.singleton.databases[indexDb]
                        .grids[indexGrid];
                }
                catch
                {
                    return null;
                }
            }
            
        }

        public Database chosenDb
        {
            get
            {
                try
                {
                    return Project.singleton.databases[indexDb];
                }
                catch
                {
                    return null;
                }
            }
        }
        public Record chosenRecord
        {
            get
            {
                try
                {
                    return grid.records[indexKey];
                        
                }
                catch
                {
                    return null;
                }
            }

        }
    }

}