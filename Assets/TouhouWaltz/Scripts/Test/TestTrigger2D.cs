using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdlessChaye.TouhouWaltz
{
	public class TestTrigger2D : MonoBehaviour
	{
		private void OnTriggerStay2D(Collider2D collision)
		{
			if (collision.tag == "GameController")
			{
				print(collision);
			}
		}
	}
}
