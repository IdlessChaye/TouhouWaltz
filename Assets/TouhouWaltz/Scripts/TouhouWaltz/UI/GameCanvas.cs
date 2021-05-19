using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IdlessChaye.TouhouWaltz
{
	public class GameCanvas : MonoBehaviour
	{
		#region
		private static GameCanvas _instance;
		public static GameCanvas Instance => _instance;
		#endregion

		public Text player;
		public Text boom;
		public Text power;

		private void Awake()
		{
			_instance = this;
		}

		public void SetPlayer(int num)
		{
			player.text = "Player: " + num.ToString();
		}

		public void SetBoom(int num)
		{
			boom.text = "Boom: " + num.ToString();
		}

		public void SetPower(int num)
		{
			power.text = "Power: " + num.ToString();
		}



	}
}
