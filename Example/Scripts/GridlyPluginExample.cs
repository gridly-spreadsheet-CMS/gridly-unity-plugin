using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gridly;
using UnityEngine.UI;

public class GridlyPluginExample : MonoBehaviour
{
    public List<Languages> languagesSupport;
    public Text[] texts;
    public Text codeText;

    Languages currentLanguage 
    {
        get => Project.singleton.targetLanguage;
        set => Project.singleton.targetLanguage = value;
    }

    private void Start()
    {
        Refesh();
    }
    public void NextLanguage()
    {
        int index = languagesSupport.IndexOf(currentLanguage) + 1;
        if (index == languagesSupport.Count)
            index = 0;

        currentLanguage = languagesSupport[index];

        Refesh();
    }


    public void PreviousLanguage()
    {
        int index = languagesSupport.IndexOf(currentLanguage) - 1;
        if (index == 0)
            index = languagesSupport.Count - 1;
        currentLanguage = languagesSupport[index];

        Refesh();
    }

    void Refesh()
    {
        texts[0].text = "customer.gametexts.prev.1".GetStringData();
        texts[1].text = "customer.gametexts.next.1".GetStringData();
        texts[2].text = "customer.gametexts.cat.1".GetStringData();
        codeText.text = Project.singleton.targetLanguage.ToString();
    }

}
