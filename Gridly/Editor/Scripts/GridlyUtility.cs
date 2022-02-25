using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Gridly.Internal{ 
    public enum ColorDebug 
    {
        green,
        red,
        purple,
        yellow
    }
    public static class GridlyUtilityEditor
    {



        public static void Error(this object i)
        {
            Debug.LogError(i);
        }


        public static void Save(this Object i)
        {
            EditorUtility.SetDirty(i);
            AssetDatabase.SaveAssets();
        }

        public static void setDirty(this Object i)
        {
            EditorUtility.SetDirty(i);
        }

        
    }

}