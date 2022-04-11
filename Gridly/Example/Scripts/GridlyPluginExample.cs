using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gridly;
using UnityEngine.UI;
namespace Gridly.Example
{
    public class GridlyPluginExample : MonoBehaviour
    {
        public static Text[] texts;
        public Text codeText;

        LangSupport currentLanguage
        {
            get => Project.singleton.targetLanguage;
            set => Project.singleton.targetLanguage = value;
        }

        List<LangSupport> languagesSupport => Project.singleton.langSupports;

        private void Start()
        {
            Refesh();
        }

        int index = 0;
        public void NextLanguage()
        {
            index++;
            if (index == languagesSupport.Count)
                index = 0;
            
            currentLanguage = languagesSupport[index];
            Refesh();


        }


        public void PreviousLanguage()
        {
            index--;
            if (index == -1)
                index = languagesSupport.Count-1;

            currentLanguage = languagesSupport[index];

            Refesh();
        }

        void Refesh()
        {
            //codeText.text = currentLanguage.languagesSuport.ToString();
            TranslareText[] translareText = FindObjectsOfType<Translator>();
            foreach(var i in translareText)
            {
                i.Refesh();
            }
            
        }

    }
}