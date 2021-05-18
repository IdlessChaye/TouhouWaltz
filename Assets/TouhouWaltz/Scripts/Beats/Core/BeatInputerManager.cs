using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdlessChaye.TouhouWaltz.Beats
{
	public class BeatInputerManager
	{

		private List<BaseBeatInputer> _inputers = new List<BaseBeatInputer>();

		public void Init()
		{

		}

		public void PrepareGame()
		{
			foreach(var inputer in _inputers)
			{
				inputer.PrepareGame();
			}
		}

		public void AddInputer(BaseBeatInputer inputer)
		{
			_inputers.Add(inputer);
		}

		public void Tick(float deltaTime)
		{
			if (BeatManager.Instance.IsPlaying == false)
				return;

			foreach(var inputer in _inputers)
			{
				inputer.ListenInput();
			}
		}
		
	}
}