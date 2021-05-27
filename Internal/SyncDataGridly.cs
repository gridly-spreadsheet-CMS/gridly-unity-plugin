using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gridly;
using Gridly.Internal;
using System;
using UnityEngine.Events;
//[ExecuteAlways]
namespace Gridly
{
    public class SyncDataGridly : MonoBehaviour
    {
        [HideInInspector]
        public float process;
        [HideInInspector]
        public int processNumberTotal => GridlyFunction.dowloadedTotal;

        [HideInInspector]
        public int processDone;

        public UnityEvent onDowloadComplete;
        GridlyFunction gridlyFunction = new GridlyFunction();
        private void Awake()
        {

            if (Data<UserLocalData>.Load() == false)
            {

                if (UserLocalData.singleton == null)
                {
                    UserLocalData.singleton = new UserLocalData();
                }
            }
            else // if exit save file then apply data
            {
                UserLocalData.singleton.ApplyData();
            }

            //check schedule
            foreach (var i in Project.singleton.databases)
                foreach (var j in i.grids)
                {
                    if (j.syncSchedule.Enable)
                    {
                        double dayTotal = DateTime.Now.Subtract(j.syncSchedule.lastestUpdateTime).TotalDays;
                        if (
                            (j.syncSchedule.AutoUpdate == SyncSchedule.SyncType.WhenOpen)||
                            (j.syncSchedule.AutoUpdate == SyncSchedule.SyncType.Daily && dayTotal >= 1) ||
                            (j.syncSchedule.AutoUpdate == SyncSchedule.SyncType.Weekly && dayTotal >= 7) ||
                            (j.syncSchedule.AutoUpdate == SyncSchedule.SyncType.Monthly && dayTotal >= 30)
                            )
                        {

                            //find the grid from userlocal
                            Internal.Grid grid = UserLocalData.singleton.grids.Find(x => x.gridID == j.gridID && x.databaseID == i.databaseID);
                            
                            //delete old grid from user local
                            if(grid != null)
                            {
                                UserLocalData.singleton.grids.Remove(grid);
                            }

                            //add new grid to user local
                            Internal.Grid grid2 = new Internal.Grid();
                            j.CopyThisBlankGridTo(ref grid2);
                            grid2.databaseID = i.databaseID;

                            grid2.syncSchedule.lastestUpdateTime = DateTime.Now; // update time
                            StartSyn(grid2);

                            UserLocalData.singleton.grids.Add(grid2);


                        }
                        else // if don't have a schedule
                        {

                            //find the grid from userlocal
                            Internal.Grid grid = UserLocalData.singleton.grids.Find(x => x.gridID == j.gridID && x.databaseID == i.databaseID);

                            //delete old grid from user local
                            if (grid != null)
                            {
                                UserLocalData.singleton.grids.Remove(grid);
                            }

                        }

                    }

                }

            gridlyFunction.doneOneProcess = DoneOneP;
           

            //If there is no process here, it's done
            if (processNumberTotal == 0)
                Finish(false);

            //apply new data when finish setup userlocal
            gridlyFunction.finishAction = Finish;
        }

        void Finish()
        {
            Finish(true);
        }

        void Finish(bool patch)
        {
            onDowloadComplete.Invoke();
            if(patch)
                UserLocalData.singleton.ApplyData();
        }
        void DoneOneP()
        {
            processDone += 1;

        }
        void StartSyn(Gridly.Internal.Grid j)
        {
            gridlyFunction.SetupRecords(j, 0); 
        }

        private void Update()
        {
            GridlyFunction.process?.Invoke();

            if (processNumberTotal != 0)
                process = (float)processDone / processNumberTotal;
        }
    }

}