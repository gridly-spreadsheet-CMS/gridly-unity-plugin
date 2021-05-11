using UnityEngine;
namespace Gridly.Internal
{
	//[CreateAssetMenu(fileName = "UserData", menuName = "Gridly/UserData", order = 1)]

	public class UserData : ScriptableObject
	{
		static UserData _singleton;
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

		public string keyAPI;
		public bool isGeneratedData;
		public bool previousUseSepData;
		public bool useSeparateData;
		static void Init()
        {
			if (_singleton == null)
				_singleton = Resources.Load<UserData>("UserData");
		}

    }

}
