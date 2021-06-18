using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlexandreDialogues
{
	//[CreateAssetMenu(fileName = "HelpFile", menuName = "ScriptableObjects/Help File")]
	public class HelpSO : ScriptableObject
	{
		[System.Serializable]
		public class HelpText
		{
			public string m_title;
			public string[] m_text;
		}


		public string m_title;
		public string m_date;
		public string m_author;
		public string m_link;

		public HelpText[] m_helpTexts;
	}
}