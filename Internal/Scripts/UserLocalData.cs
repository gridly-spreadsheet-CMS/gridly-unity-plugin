using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Gridly.Internal
{

    [System.Serializable]
    public class UserLocalData : MyData<UserLocalData>
    {
        static UserLocalData _singleton;
        public static UserLocalData singleton 
        {
            get => _singleton;
            set { _singleton = value; } 
        }

        [HideInInspector]
        public List<Grid> grids = new List<Grid>();


        public UserLocalData()
        {

        }

        //add new patch to root data
        public void ApplyData()
        {
            List<Grid> deleteGrid = new List<Grid>();

            //Apply new data meta -> root
            foreach (var i in grids)
            {
                

                //find database
                Database database = Project.singleton.databases.Find(x => x.databaseID == i.databaseID);

                //if db Exit then find the grid
                if (database != null) {
                    Grid grid = database.grids.Find(x => x.gridID == i.gridID);

                    //if grid exit the remove the old grid from root db
                    SyncSchedule syncSchedule = null;
                    if (grid != null)
                    {
                        syncSchedule = new SyncSchedule(grid.syncSchedule);
                        database.grids.Remove(grid);
                    }
                    else
                    {
                        deleteGrid.Add(i);
                    }


                    //apply syncschedule from root data
                    if (syncSchedule != null)
                        i.syncSchedule = syncSchedule;

                    

                    //add new grid to root database
                    database.grids.Add(i);
                    
                }
                else
                {
                    deleteGrid.Add(i);
                }
            }

            foreach(var i in deleteGrid)
            {
                grids.Remove(i);
            }


            Data<UserLocalData>.Save();
        }

        public UserLocalData getInstance()
        {
            return singleton;
        }

        public void setInstance(UserLocalData newData)
        {
            singleton = newData;
        }
    }

}