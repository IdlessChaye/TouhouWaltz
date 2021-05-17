using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IdlessChaye.TouhouWaltz.Beats
{
	public class BeatRenderManager : MonoBehaviour
	{
		public float MaxRenderHalfTimeWindow { get; private set; }

		[SerializeField] private Text _comboText;
		public Text ComboText => _comboText;
		[SerializeField] private Text _judgeResultText;
		public Text JudgeResultText => _judgeResultText;

		[SerializeField] private RectTransform _judgeLine;
		[SerializeField] private RectTransform _bornPoint;
		public static float JudgeLinePosX { get; private set; }
		public static float BornPointPosX { get; private set; }

		[SerializeField] private RectTransform _noteDead;
		[SerializeField] private RectTransform _noteAlive;

		private GameObject _kickSpriteItemPrefab;
		private GameObject _holdSpriteItemPrefab;

		private BeatStopwatch _stopwatch;
		private IList<BeatNoteItem> _beatNoteItems;
		private int _spriteItemIndex = 0;

		private Queue<BaseBeatSpriteItem> _spriteItemQueue = new Queue<BaseBeatSpriteItem>();

		public void Init()
		{
			_stopwatch = BeatManager.Instance.Stopwatch;
			JudgeLinePosX = _judgeLine.anchoredPosition.x;
			BornPointPosX = _bornPoint.anchoredPosition.x;
			_kickSpriteItemPrefab = Resources.Load<GameObject>(BeatConst.BeatKickSpriteItemPrefabPath);
			if (_kickSpriteItemPrefab == null)
				Debug.LogError("!");
			_holdSpriteItemPrefab = Resources.Load<GameObject>(BeatConst.BeatHoldSpriteItemPrefabPath);
			if (_holdSpriteItemPrefab == null)
				Debug.LogError("!");
		}

		public void PrepareGame()
		{
			_spriteItemQueue.Clear();
			_spriteItemIndex = 0;
			MaxRenderHalfTimeWindow = BeatManager.Instance.BeatMapData.maxRenderHalfTimeWindow;
			if (MaxRenderHalfTimeWindow == 0)
				MaxRenderHalfTimeWindow = BeatConst.RenderHalfTimeWindow;
			_beatNoteItems = BeatManager.Instance.BeatNoteItems.Values;
		}

		public void Tick(float deltaTime)
		{
			if (_stopwatch.IsRunning == false)
				return;

			float judgeTime = _stopwatch.PastTime;

			// Add to judge queue
			while (_beatNoteItems.Count > 0
				&& _spriteItemIndex < _beatNoteItems.Count
				&& _beatNoteItems[_spriteItemIndex].time < judgeTime + MaxRenderHalfTimeWindow)
			{
				var spriteItem = SpriteItemFactory(_beatNoteItems[_spriteItemIndex]);
				if (spriteItem == null)
				{
					Debug.LogError("beatsprite tick SpriteItemFactory. null.");
					return;
				}
				_spriteItemQueue.Enqueue(spriteItem);

				_spriteItemIndex++;
			}

			foreach(var spriteItem in _spriteItemQueue)
			{
				spriteItem.SetPosition(judgeTime, MaxRenderHalfTimeWindow);
			}

		}


		public void HandleJudgeResult(BeatJudgeResult result)
		{
			if (result == null)
				return;

			var peek = _spriteItemQueue.Peek();
			if (peek == null)
			{
				Debug.LogError("!");
				return;
			}

			var isFinished = peek.HandleJudgeResult(result.resultType);
			if (isFinished)
			{
				_spriteItemQueue.Dequeue();
			}
		}

		private BaseBeatSpriteItem SpriteItemFactory(BeatNoteItem noteItem)
		{
			BeatNoteType type = noteItem.noteType;
			BaseBeatSpriteItem spriteItem = null;
			switch (type)
			{
				case BeatNoteType.BlueKick:
				case BeatNoteType.RedKick:
					spriteItem = InnerSpriteItemFactory(_kickSpriteItemPrefab).GetComponent<BaseBeatSpriteItem>();
					break;
				case BeatNoteType.BlueHold:
				case BeatNoteType.RedHold:
					spriteItem = InnerSpriteItemFactory(_holdSpriteItemPrefab).GetComponent<BaseBeatSpriteItem>();
					break;
				default:
					Debug.LogError("!!!!");
					break;
			}

			if (spriteItem !=null)
				spriteItem.Prepare(noteItem);

			return spriteItem;
		}

		private GameObject InnerSpriteItemFactory(GameObject prefab)
		{
			var go = Instantiate(prefab, _noteAlive, false);
			go.transform.SetAsFirstSibling();
			return go;
		}
	}
}