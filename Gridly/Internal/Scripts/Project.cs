using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gridly.Internal;
using TMPro;
namespace Gridly
{

    [System.Serializable]
    public class LangSupport
    {
        public Languages languagesSuport;
        public string name;
        public string screenshotColumnId;

        public Font font;
        public TMP_FontAsset tmFont;
    }


    //[CreateAssetMenu(fileName = "Project", menuName = "Gridly/Project", order = 1)]
    public class Project : ScriptableObject
    {

        static Project _singleton;
        public string ProjectID;


        private string chosenLangCodeName;
        public LangSupport targetLanguage
        {

            set { chosenLangCodeName = value.languagesSuport.ToString(); }
            get
            {
                LangSupport _ = langSupports.Find(x => x.languagesSuport.ToString() == chosenLangCodeName);
                try
                {

                    if (_ == null)
                    {
                        //Debug.Log("Cant find the language: " + chosenLangCodeName);
                        return langSupports[0];
                    }
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
        public List<Gridly.Internal.Grid> grids;
        //[HideInInspector]
        public List<LangSupport> langSupports;
        [HideInInspector]
        public List<string> LangsToTakeScreenshotList;
        [HideInInspector]
        public int LastSelectedLangIndexToAdd = 0;
        [HideInInspector]
        public int LastSelectedLangIndexToRemove = 0;
        [HideInInspector]
        public List<string> DataToSend;
        [HideInInspector]
        public List<string> DataToSendSelectedItems;
        [HideInInspector]
        public bool SendIfChanged = false;


        public Internal.Grid GetGrid(string name)
        {
            return grids.Find(x => x.nameGrid == name);
        }

        public static Project singleton
        {
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


        public int getIndexChosenLang
        {
            get
            {
                int index = 0;
                foreach (var i in langSupports)
                {
                    if (i.languagesSuport.ToString() == chosenLangCodeName)
                        return index;
                    index += 1;
                }
                return 0;
            }
        }
        void SetChosenLanguageCode(string langCode)
        {
            chosenLangCodeName = langCode;
            TranslareText[] translareTexts = FindObjectsOfType<Translator>();
            foreach (var i in translareTexts)
                i.Refesh();
        }
        public void SetChosenLanguageCode(Languages languages)
        {
            SetChosenLanguageCode(languages.ToString());
        }

    }
}