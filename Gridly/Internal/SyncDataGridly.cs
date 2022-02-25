using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gridly;
using Gridly.Internal;
using System;
using UnityEngine.Events;
//[ExecuteAlways]
namespace Gridly
{
    public class SyncDataGridly : MonoBehaviour
    {

        public float process { 
            get 
            {
                if (processNumberTotal != 0)
                    return (float)processDone / processNumberTotal;
                return 0;
            } 
        }

        public int processNumberTotal => GridlyFunction.dowloadedTotal;
        public int processDone => GridlyFunction.dowloadn;

        public bool syncOnAwake = true;

        public UnityEvent onDowloadComplete;
        static GridlyFunction gridlyFunction = new GridlyFunction();

        

        public static SyncDataGridly singleton;
        private void Awake()
        {
            if (singleton == null)
                singleton = this;
            else 
            { 
                Destroy(gameObject);
                return;
            }

            if (syncOnAwake)
                StartSync();
 
        }


        public void StartSync()
        {
            if (string.IsNullOrEmpty(UserData.singleton.keyAPI))
            {
                Debug.Log("Please enter yout api key to use this feature");
                return;
            }


            gridlyFunction.SetupDatabases();


            //apply new data when finish setup userlocal
            gridlyFunction.finishAction = Finish;
        }

        void Finish()
        {
            onDowloadComplete.Invoke();
        }


        private void Update()
        {
            GridlyFunction.process?.Invoke();

        }
    }

}