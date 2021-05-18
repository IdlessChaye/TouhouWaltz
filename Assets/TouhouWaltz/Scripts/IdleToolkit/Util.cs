using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdlessChaye.IdleToolkit
{
	public static class Util
	{
		public static void TrySetActive(GameObject go, bool isActive)
		{
			if (go != null && go.activeSelf != isActive)
			{
				go.SetActive(isActive);
			}
		}
	}
}