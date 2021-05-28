using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Gridly;
using Gridly.Internal;
public class ShowProcessDowloadToScreen : MonoBehaviour
{
    public SyncDataGridly sync;
    public Text process;
    public Text numberOfProces;
    public GameObject downloadingFromServer;
    public Image image;

    bool isFinish;
    public void Update()
    {
        numberOfProces.text = sync.processDone + "/" + sync.processNumberTotal;
        if (!isFinish)
            process.text = ((int)(sync.process * 100)).ToString() + "%";
        else process.text = "Finish";
        image.fillAmount = sync.process;
    }

    public void Finish()
    {
        isFinish = true;
        downloadingFromServer.SetActive(false);

 
    }
}
