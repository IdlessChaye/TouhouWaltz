using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdlessChaye.TouhouWaltz
{
	public class MonoBeatManager : MonoBehaviour
	{

		[SerializeField]
		private AudioSource _audio = null;

		public AudioSource Audio => _audio;


	}
}