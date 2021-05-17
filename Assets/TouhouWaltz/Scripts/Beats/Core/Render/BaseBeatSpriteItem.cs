using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using IdlessChaye.IdleToolkit;

namespace IdlessChaye.TouhouWaltz.Beats
{
	public abstract class BaseBeatSpriteItem : MonoBehaviour
	{
		protected BeatNoteItem _noteItem;

		public uint ID => _noteItem.id;
		public BeatNoteType BeatNoteType => _noteItem.noteType;

		protected RectTransform _transform;

		protected float _judgeLinePosX { get; private set; }
		protected float _bornPointPosX { get; private set; }

		private void Awake()
		{
			_transform = this.GetComponent<RectTransform>();
		}

		public virtual void Prepare(BeatNoteItem noteItem)
		{
			_noteItem = noteItem;
			_judgeLinePosX = BeatRenderManager.JudgeLinePosX;
			_bornPointPosX = BeatRenderManager.BornPointPosX;
		}

		/// <summary>
		/// 返回是否结束音符判断
		/// </summary>
		/// <param name="result"></param>
		/// <returns></returns>
		public abstract bool HandleJudgeResult(BeatJudgeResultType result);

		public abstract void SetPosition(float pastTime, float maxHalfTimeWindow);

		protected void Disable()
		{
			CoroutineHelper.Instance.DelayCall(0.5f, () =>
			{
				if (gameObject != null)
					Destroy(this.gameObject);
			});
		}
	}

	

}