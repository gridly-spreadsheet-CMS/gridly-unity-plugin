using UnityEngine;
using System.Collections.Generic;
namespace Gridly.Internal
{
	//[CreateAssetMenu(fileName = "UserData", menuName = "Gridly/UserData", order = 1)]

	[System.Serializable]
	public class RecordTemp
    {

		public string viewID;
		public ActionRecord actionRecord;
		public Record record;
    }
	public enum ActionRecord
	{
		Add,
		Delete
	}
	public class UserData : ScriptableObject
	{
		static UserData _singleton;
		public Languages mainLangEditor = Languages.enUS;
		public static UserData singleton
		{
            set
            {
				Init();
				_singleton = value;
            }
			get 
			{
				Init();
				return _singleton;
			}
			
		}


		[SerializeField]
		private string _screenshotPath = "";
		public string screenshotPath { get => _screenshotPath.Decrypt(); set => _screenshotPath = value.Enrypt(); }

		[SerializeField]
		private bool _uploadImages = false;
		public bool uploadImages { get => _uploadImages; set => _uploadImages = value; }

		[SerializeField]
		private string _keyAPI = "";
		public string keyAPI { get => _keyAPI.Decrypt(); set => _keyAPI = value.Enrypt(); }

		public bool showServerMess;
		static void Init()
        {
			if (_singleton == null)
				_singleton = Resources.Load<UserData>("UserData");
		}


		
    }

}
