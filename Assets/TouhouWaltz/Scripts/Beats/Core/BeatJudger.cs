using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdlessChaye.TouhouWaltz.Beats
{
	public class BeatJudger
	{
		private BeatInputer _inputer;

		private BeatStopwatch _stopwatch;
		private IList<BeatNoteItem> _beatNoteItems;
		private int noteItemIndex = 0;

		private Queue<BaseBeatJudgeItem> _judgeItemQueue = new Queue<BaseBeatJudgeItem>();

		public BeatJudger(BeatInputer inputer)
		{
			_inputer = inputer;
			_stopwatch = BeatManager.Instance.Stopwatch;
		}

		public void PrepareGame()
		{
			_judgeItemQueue.Clear();
			noteItemIndex = 0;
			_beatNoteItems = BeatManager.Instance.BeatNoteItems.Values;
		}

		public void Tick(float deltaTime)
		{
			if (_stopwatch.IsRunning == false)
				return;

			float judgeTime = _stopwatch.PastTime;

			// Add to judge queue
			while (_beatNoteItems.Count > 0
				&& noteItemIndex < _beatNoteItems.Count
				&& _beatNoteItems[noteItemIndex].time < judgeTime + BeatConst.MaxJudgeTimeHalfWindow) 
			{
				var judgeItem = JudgeItemFactory(_beatNoteItems[noteItemIndex]);
				if (judgeItem == null)
				{
					Debug.LogError("BeatJudger Tick JudgeItemFactory. null.");
					return;
				}
				_judgeItemQueue.Enqueue(judgeItem);

				noteItemIndex++;
			}

			// Judge the first note
			if (_judgeItemQueue.Count == 0)
				return;

			BeatJudgeResult result = null;

			bool hasResult = _judgeItemQueue.Peek().JudgeAndGetResult(_inputer, judgeTime, out result);

			// Remove if note is finished
			if (_judgeItemQueue.Peek().IsFinished)
			{
				_judgeItemQueue.Dequeue();
			}

			// Deal with the result
			if (hasResult && result != null)
			{
				BeatManager.Instance.HandleJudgeResult(result);
			}
		}

		private BaseBeatJudgeItem JudgeItemFactory(BeatNoteItem noteItem)
		{
			if (noteItem == null)
			{
				Debug.LogError("Error JudgeItemFactory");
				return null;
			}

			switch (noteItem.noteType)
			{
				case BeatNoteType.BlueKick:
				case BeatNoteType.RedKick:
					return new KickJudgeItem
					{
						JudgeDeadlineTime = noteItem.time,
						NoteItem = noteItem
					};
				case BeatNoteType.BlueHold:
				case BeatNoteType.RedHold:
					return new HoldJudgeItem {
						JudgeDeadlineTime = noteItem.time,
						NoteItem = noteItem
					};
				default:
					break;

			}

			return null;
		}
	}

	public abstract class BaseBeatJudgeItem
	{
		public float JudgeDeadlineTime { get; set; }
		public BeatNoteItem NoteItem { get; set; }
		public bool IsFinished { get; protected set; } = false;

		public abstract bool JudgeAndGetResult(BeatInputer inputer, float judgeTime, out BeatJudgeResult result);

		protected BeatJudgeResult GenerateJudgeResult(BeatNoteItem noteItem,BeatJudgeResultType resultType,float judgeTime,float timespan)
		{
			return new BeatJudgeResult
			{
				noteItem = noteItem,
				resultType = resultType,
				judgeTime = judgeTime,
				timespan = timespan
			};
		}
	}

	public class KickJudgeItem : BaseBeatJudgeItem
	{
		public override bool JudgeAndGetResult(BeatInputer inputer, float judgeTime, out BeatJudgeResult result)
		{
			result = null;

			float timespan = NoteItem.time - judgeTime;
			float absTimespan = Mathf.Abs(timespan);

			// Timeout
			if (timespan < -BeatConst.BadJudgeTimeHalfWindow)
			{
				result = GenerateJudgeResult(NoteItem, BeatJudgeResultType.Miss, judgeTime, timespan);
				IsFinished = true;
				return true;
			}
			 
			if (inputer.IsDown == true)
			{
				if (absTimespan <= BeatConst.CoolJudgeTimeHalfWindow)
				{
					result = GenerateJudgeResult(NoteItem, BeatJudgeResultType.Cool, judgeTime, timespan);
					IsFinished = true;
					return true;
				}
				else if (absTimespan <= BeatConst.FineJudgeTimeHalfWindow)
				{
					result = GenerateJudgeResult(NoteItem, BeatJudgeResultType.Fine, judgeTime, timespan);
					IsFinished = true;
					return true;
				}
				else if (absTimespan <= BeatConst.GoodJudgeTimeHalfWindow)
				{
					result = GenerateJudgeResult(NoteItem, BeatJudgeResultType.Good, judgeTime, timespan);
					IsFinished = true;
					return true;
				}
				else if (absTimespan <= BeatConst.BadJudgeTimeHalfWindow)
				{
					result = GenerateJudgeResult(NoteItem, BeatJudgeResultType.Bad, judgeTime, timespan);
					IsFinished = true;
					return true;
				}
				return false;
			}
			return false;
		}
	}

	public class HoldJudgeItem : BaseBeatJudgeItem
	{
		private bool _isBeginHolding = false;
		public override bool JudgeAndGetResult(BeatInputer inputer, float judgeTime, out BeatJudgeResult result)
		{
			result = null;

			float timespan = JudgeDeadlineTime - judgeTime;
			float absTimespan = Mathf.Abs(timespan);

			if (_isBeginHolding == false)
			{
				if (timespan < -BeatConst.BadJudgeTimeHalfWindow)
				{
					result = GenerateJudgeResult(NoteItem, BeatJudgeResultType.Miss, judgeTime, timespan);
					IsFinished = true;
					return true;
				}

				if (inputer.IsDown == true)
				{
					if (absTimespan <= BeatConst.CoolJudgeTimeHalfWindow)
					{
						result = GenerateJudgeResult(NoteItem, BeatJudgeResultType.Cool, judgeTime, timespan);
						_isBeginHolding = true;
						JudgeDeadlineTime += NoteItem.holdTime;
						return true;
					}
					else if (absTimespan <= BeatConst.FineJudgeTimeHalfWindow)
					{
						result = GenerateJudgeResult(NoteItem, BeatJudgeResultType.Fine, judgeTime, timespan);
						_isBeginHolding = true;
						JudgeDeadlineTime += NoteItem.holdTime;
						return true;
					}
					else if (absTimespan <= BeatConst.GoodJudgeTimeHalfWindow)
					{
						result = GenerateJudgeResult(NoteItem, BeatJudgeResultType.Good, judgeTime, timespan);
						_isBeginHolding = true;
						JudgeDeadlineTime += NoteItem.holdTime;
						return true;
					}
					else if (absTimespan <= BeatConst.BadJudgeTimeHalfWindow)
					{
						result = GenerateJudgeResult(NoteItem, BeatJudgeResultType.Bad, judgeTime, timespan);
						IsFinished = true;
						return true;
					}
					return false;
				}
			}
			else if (_isBeginHolding == true)
			{
				if (inputer.Status == InputerStatus.Hold)
				{
					if (timespan < -BeatConst.BadJudgeTimeHalfWindow)
					{
						result = GenerateJudgeResult(NoteItem, BeatJudgeResultType.Miss, judgeTime, timespan);
						IsFinished = true;
						return true;
					}
					return false; // Keep holding, waiting for inputer is up.
				}
				
				if (inputer.IsUp)
				{
					if (timespan < -BeatConst.BadJudgeTimeHalfWindow)
					{
						UnityEngine.Debug.LogError("Can't pass. 不可能是这种情况，会因为一直按键而Miss。");
						IsFinished = true;
						return false;
					}
					else if (timespan > BeatConst.BadJudgeTimeHalfWindow) // 提前松手，判定Miss
					{
						result = GenerateJudgeResult(NoteItem, BeatJudgeResultType.Miss, judgeTime, timespan);
						IsFinished = true;
						return true;
					}
					else if (absTimespan <= BeatConst.CoolJudgeTimeHalfWindow)
					{
						result = GenerateJudgeResult(NoteItem, BeatJudgeResultType.Cool, judgeTime, timespan);
						IsFinished = true;
						return true;
					}
					else if (absTimespan <= BeatConst.FineJudgeTimeHalfWindow)
					{
						result = GenerateJudgeResult(NoteItem, BeatJudgeResultType.Fine, judgeTime, timespan);
						IsFinished = true;
						return true;
					}
					else if (absTimespan <= BeatConst.GoodJudgeTimeHalfWindow)
					{
						result = GenerateJudgeResult(NoteItem, BeatJudgeResultType.Good, judgeTime, timespan);
						IsFinished = true;
						return true;
					}
					else if (absTimespan <= BeatConst.BadJudgeTimeHalfWindow)
					{
						result = GenerateJudgeResult(NoteItem, BeatJudgeResultType.Bad, judgeTime, timespan);
						IsFinished = true;
						return true;
					}
					return false;
				}
			}

			return false;
		}
	}

}