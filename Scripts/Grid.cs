using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Gridly.Internal
{

    [System.Serializable]
    public class Grid
#if Gridly_UseSeparateData
        : ScriptableObject
#endif
    {
        public string nameGrid;
        public string gridID;
        public string viewID;
        public List<Record> records = new List<Record>();

        public Grid()
        {

        }
    }

}