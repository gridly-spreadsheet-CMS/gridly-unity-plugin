using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Gridly.Internal{ 
    public static class GridlyUtility
    {
        public static bool CheckOutput(this string output)
        {

            if (output.Length == 0)
            {
                Debug.LogError("Something is wrong. Please check your key again");
                EditorApplication.update = null;
                
                return false;
            }

            return true;
        }

        public static void Print(this object i)
        {
            Debug.Log(i);
        }

        public static void Error(this object i)
        {
            Debug.LogError(i);
        }


        public static void Save(this Object i)
        {
            EditorUtility.SetDirty(i);
            AssetDatabase.SaveAssets();
        }

        //public static string test => ;
    }

}