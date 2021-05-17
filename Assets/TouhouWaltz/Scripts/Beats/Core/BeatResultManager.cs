using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdlessChaye.TouhouWaltz.Beats
{
	public enum BeatJudgeResultType
	{
		Cool,
		Fine,
		Good,
		Bad,
		Miss,
		None,
	}

	public class BeatJudgeResult
	{
		public BeatNoteItem noteItem;
		public BeatJudgeResultType resultType;
		public float judgeTime;
		public float timespan;
	}

	public delegate void OnComboChanged(uint combo);
	public delegate void OnNoteResult(BeatJudgeResult result);

	public class BeatResultManager 
	{

		public uint Combo;

		public event OnComboChanged OnComboChanged;
		public event OnNoteResult OnNoteResult;

		public void Init()
		{

		}

		public void PrepareGame()
		{
			Combo = 0;
		}

		public void Tick(float deltaTime)
		{

		}

		public void HandleJudgeResult(BeatJudgeResult result)
		{
			if (result == null)
				return;

			if (IsResultSuccess(result))
				Combo++;
			else
				Combo = 0;

			if (OnNoteResult != null)
				OnNoteResult(result);

			if (OnComboChanged != null)
				OnComboChanged(Combo);
		}

		private bool IsResultSuccess(BeatJudgeResult result)
		{
			if (result.resultType == BeatJudgeResultType.Cool
				|| result.resultType == BeatJudgeResultType.Fine
				|| result.resultType == BeatJudgeResultType.Good)
			{
				return true;
			}
			return false;
		}
	}
}