using System.Collections.Generic;
using UnityEngine;

namespace Assets.Gridly.Internal.Scripts
{
    [System.Serializable]
    public class GirldyView
    {

        public class Column
        {
            public string id { get; set; }
            public string name { get; set; }
            public bool? isSource { get; set; }
            public bool? isTarget { get; set; }
            public string languageCode { get; set; }
            public string type { get; set; }
            public string dependsOn { get; set; }
        }

        public class View
        {
            public string id { get; set; }
            public List<Column> columns { get; set; } = new List<Column>();
            public string gridId { get; set; }
            public string gridStatus { get; set; }
            public string name { get; set; }
        }
    }
}
