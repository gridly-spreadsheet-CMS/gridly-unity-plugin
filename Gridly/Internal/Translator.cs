using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

using Gridly.Example;
namespace Gridly
{
    public interface TranslareText
    {
        void Refesh();
    }
    public class Translator : MonoBehaviour, TranslareText
    {
        public TextMeshProUGUI textMeshPro;
        public Text text;

        [HideInInspector]
        public string grid;

        [HideInInspector]
        public string key;

        

        void OnEnable()
        {
            Refesh();

        }

        public void Refesh()
        {
            if (textMeshPro != null)
            {
                textMeshPro.text = GridlyLocal.GetStringData(grid, key);
                textMeshPro.font = Project.singleton.targetLanguage.tmFont;
            }

            if (text != null)
            {
                text.text = GridlyLocal.GetStringData(grid, key);
                text.font = Project.singleton.targetLanguage.font;
            }

            if (text == null && textMeshPro == null) { }
                //Debug.LogWarning("text,textMeshPro fields is empty " + gameObject.name);

        }
    }
}