using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdlessChaye.TouhouWaltz.Beats
{

	public class TestBeatMap : MonoBehaviour
	{
		public RectTransform rect;

		private bool isFollowing = false;
		public void Update()
		{
			if (isFollowing)
			{ 
				var pos = Input.mousePosition;
				rect.position = pos;
			}
		}
	}
}