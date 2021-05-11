using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gridly.Internal;
namespace Gridly{

    //[CreateAssetMenu(fileName = "Project", menuName = "Gridly/Project", order = 1)]
    public class Project: ScriptableObject
    {
        
        static Project _singleton;
        public string ProjectID;
        public Languages targetLanguage;
        public List<Database> databases;
        public static Project singleton {
            get 
            {
                Init();
                return _singleton; 
            }
            set 
            {
                Init();
                _singleton = value; 
            }
        }
        static void Init()
        {
            if (_singleton == null)
                _singleton = Resources.Load<Project>("Project");


        }




        /// <summary>
        /// Move to next language
        /// </summary>
        public void NextLanguage()
        {
            if (targetLanguage == Languages.zuZA)
                targetLanguage = (Languages)(0);
            else
            {
                targetLanguage = (Languages)((int)targetLanguage + 1);
            }
        }

        /// <summary>
        /// Move previous language
        /// </summary>
        public void PreviousLanguage()
        {
            if (targetLanguage == Languages.arSA)
                targetLanguage = Languages.zuZA;
            else
            {
                targetLanguage = (Languages)((int)targetLanguage - 1);
            }
        }





    }
}