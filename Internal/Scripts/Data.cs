
using UnityEngine;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

namespace Gridly.Internal
{
    public interface MyData<T>
    {

        T getInstance();
        void setInstance(T NewData);
    }
    public class Data<T> where T : MyData<T>, new()
    {

        public static void Save()
        {
            string dataPath = Application.persistentDataPath;
            var format = new BinaryFormatter();
            var stream = new FileStream(dataPath + "/" + typeof(T).Name + ".kietdeptrai", FileMode.Create);
           // Debug.Log("saved + " + dataPath + "/" + typeof(T).Name + ".kietdeptrai");
            format.Serialize(stream, new T().getInstance());
            stream.Close();
        }



        public static bool Load()
        {
            string path = typeof(T).Name;
            string dataPath = Application.persistentDataPath;
            if (!System.IO.File.Exists(dataPath + "/" + path + ".kietdeptrai"))
            {
                //Debug.Log("cant find data in " + dataPath + "/" + path + ".kietdeptrai");
                return false;
            }

            var ser = new BinaryFormatter();
            var stream = new FileStream(dataPath + "/" + path + ".kietdeptrai", FileMode.Open);
            new T().setInstance((T)ser.Deserialize(stream)); //Convert.ChangeType(ser.Deserialize(stream), typeof(T)));
            stream.Close();
            return true;
        }
    }
   
}