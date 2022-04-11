using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace Gridly.Internal
{
    public class ArrData
    {

        bool _init;
        public bool init => _init;
        public List<List<string>> numberList = new List<List<string>>();
        public int chosenIndexList = 0;
        public ArrData(int numberOfArr)
        {
            for (int i = 0; i < numberOfArr; i++)
            {
                numberList.Add(new List<string>());
            }
            
        }

        public string[] ToArr(int index)
        {
            return numberList[index].ToArray();
        }


    }
        
    public class GridlyArrData
    {
        bool _init;
        public bool init => _init;

        public string[] gridArr;
        public string[] keyArr;


        public int indexGrid;
        public int indexKey;
        public string keyID;
        public string searchKey;
        public string chosenGridName => gridArr[indexGrid];



        public void RefeshAll(string gridname, string keyID) 
        {
            _init = true;

            this.keyID = keyID;
            List<string> nameGrid = new List<string>();
            foreach (var i in Project.singleton.grids)
            {
                nameGrid.Add(i.nameGrid);
            }
            if(nameGrid.Count > 0)
                gridArr = nameGrid.ToArray();
            if (!string.IsNullOrEmpty(gridname))
            {
                
                indexGrid = GetIndex(gridname, gridArr);
            }

          

            if(grid != null)
            {
                List<string> nameKey = new List<string>();
                foreach(var i in grid.records)
                {
                    nameKey.Add(i.recordID);
                }

                if (!string.IsNullOrEmpty(searchKey))
                {
                    nameKey = nameKey.FindAll(x => x.Contains(searchKey));
                }


                keyArr = nameKey.ToArray();

                if (!string.IsNullOrEmpty(keyID))
                    indexKey = GetIndex(keyID, keyArr);
                
            }


        }
       
        int GetIndex(string select, string[] arr)
        {
            if (arr == null)
                return -1;
            int index = 0;
            foreach (var i in arr)
            {
                if (select == i)
                    return index;
                index += 1;
            }
            return 0;
        }




        public Grid grid
        {
            get
            {
                try
                {
                    return Project.singleton
                        .grids[indexGrid];
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
                    return grid.records.Find(x => x.recordID == keyID);
                        
                }
                catch
                {
                    return null;
                }
            }

        }
    }

}