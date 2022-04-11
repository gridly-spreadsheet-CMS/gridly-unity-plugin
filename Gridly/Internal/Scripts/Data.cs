
using UnityEngine;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

namespace Gridly.Internal
{
    public static class DataManager
    {
        public static void Save(this string key, int value)
        {
            PlayerPrefs.SetInt(key.Enrypt(), value);
        }

        public static void Save(this string key, float value)
        {
            PlayerPrefs.SetFloat(key.Enrypt(), value);
        }

        public static void Save(this string key, string value)
        {
            PlayerPrefs.SetString(key.Enrypt(), value);
        }

        public static void Save(this string key, bool value)
        {
            //key.Print(value.ToString()+" Save");
            if (value)
                PlayerPrefs.SetInt(key.Enrypt(), 1);
            else PlayerPrefs.SetInt(key.Enrypt(), 0);
        }

        public static void SetIsUse(this string key, bool isUse)
        {
            (key + ".isUse").Save(isUse);
        }

        public static bool GetIsUse(this string key)
        {
            return (key + ".isUse").Load();
        }

        public static int Load(this string key, int defaultVal)
        {
            return PlayerPrefs.GetInt(key.Enrypt(), defaultVal);
        }

        public static float Load(this string key, float defaultVal)
        {
            return PlayerPrefs.GetFloat(key.Enrypt(), defaultVal);
        }

        public static string Load(this string key, string defaultVal)
        {
            return PlayerPrefs.GetString(key.Enrypt(), defaultVal);
        }

        public static string RemoveLast(this string key, int num)
        {
            return key.Remove(key.Length - num, num);
        }
        public static bool Load(this string key, bool defaulVal)
        {
            //key.Print(defaulVal.ToString() + " Load");
            int defaultInt = 0;
            if (defaulVal) defaultInt = 1;

            return (PlayerPrefs.GetInt(key.Enrypt(), defaultInt) == 1);
        }

        public static bool Load(this string key)
        {
            return (PlayerPrefs.GetInt(key.Enrypt(), 0) == 1);
        }


        public static string CovertToRomandNumber(this int num)
        {
            if (num == 1)
                return "I";
            if (num == 2)
                return "II";
            if (num == 3)
                return "III";
            if (num == 4)
                return "IV";
            if (num == 5)
                return "V";
            return "...";
        }


    }

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
            //Debug.Log("saved + " + dataPath + "/" + typeof(T).Name + ".kietdeptrai");
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