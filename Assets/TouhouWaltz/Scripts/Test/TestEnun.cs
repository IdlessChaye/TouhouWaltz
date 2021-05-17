using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdlessChaye.TouhouWaltz.Test
{

	public class TestEnun : MonoBehaviour
	{
		private void Start()
		{
			A a = A.a;
			switch (a)
			{
				case A.a | A.b:
					Debug.Log("!!!");
					break;
			}

			//a = A.b;
			//switch (a)
			//{
			//	case A.a | A.b:
			//		Debug.Log("!!!");
			//		break;
			//}
		}
	}


	public enum A
	{
		a,
		b
	}
}