using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdlessChaye.TouhouWaltz.Beats
{
	public class BeatJudgerManager
	{
		private List<BeatJudger> _judgers = new List<BeatJudger>();

		public void Init()
		{
			
		}

		public void AddJudger(BeatJudger judger)
		{
			_judgers.Add(judger);
		}

		public BeatJudger GetJudger(int index)
		{
			return _judgers[index];
		}

		public void Tick(float deltaTime)
		{
			if (BeatManager.Instance.IsPlaying == false)
				return;

			foreach(var judger in _judgers)
			{
				judger.Tick(deltaTime);
			}
		}

		public void PrepareGame()
		{
			foreach(var judger in _judgers)
			{
				judger.PrepareGame();
			}
		}

		public void ResetGame()
		{
			foreach(var judger in _judgers)
			{
				judger.ResetGame();
			}
		}

		public void ReadyToBePrepared()
		{
			foreach (var judger in _judgers)
			{
				judger.ReadyToBePrepared();
			}
		}
	}
}