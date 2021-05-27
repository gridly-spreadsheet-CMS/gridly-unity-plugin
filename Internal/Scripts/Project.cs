using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gridly.Internal;
using TMPro;
namespace Gridly{

    [System.Serializable]
    public class LangSupport
    {
        public Languages languagesSuport;
        public string name;
        
        public Font font;
        public TMP_FontAsset tmFont;
    }

    //[CreateAssetMenu(fileName = "Project", menuName = "Gridly/Project", order = 1)]
    public class Project : ScriptableObject
    {

        static Project _singleton;
        public string ProjectID;
        private string chosenLangCodeName;
        public LangSupport targetLanguage {
            set { chosenLangCodeName = value.languagesSuport.ToString(); }
            get
            {
                LangSupport _ = langSupports.Find(x => x.name == chosenLangCodeName);
                try
                {
                    if (_ == null)
                        return langSupports[0];
                }
                catch
                {
                    Debug.LogError("no supported language");
                    return null;
                }
                return _;
            }
        }
        [HideInInspector]
        public List<Database> databases;
        //[HideInInspector]
        public List<LangSupport> langSupports;

        public static Project singleton {
            get
            {
                Init();
                return _singleton;
            }
            set
            {

                try
                {
                    _singleton = value;
                }
                catch
                {
                    Init();
                    _singleton = value;
                }
            }
        }
        static void Init()
        {
            if (_singleton == null)
                _singleton = Resources.Load<Project>("Project");


        }
        public int getIndexChosenLang {
            get
            {
                int index = 0;
                foreach(var i in langSupports)
                {
                    if (i.languagesSuport.ToString() == chosenLangCodeName)
                        return index;
                    index += 1;
                }
                return 0;
            }
        }
        public void SetChosenLanguageCode(string langCode)
        {
            chosenLangCodeName = langCode;
        }
        public Internal.Grid getGrid(string dbName, string gridName)
        {
            Internal.Grid grid = databases.Find(x => x.databaseName == dbName).grids.Find(x => x.nameGrid == gridName);
            return grid;
        }




    }
}