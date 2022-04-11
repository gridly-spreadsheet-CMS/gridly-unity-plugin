using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridlyRecord 
{
    public class Cells
    {
        public string columnId { get; set; }

        public string Value { get; set; }

        public string? dependencyStatus { get; set; }

    }
    public class Records
    {
        public string id { get; set; }
        public List<Cells> cells { get; set; }
        public string path { get; set; }

        public Records()
        {
            cells = new List<Cells>();
        }

    }
}
