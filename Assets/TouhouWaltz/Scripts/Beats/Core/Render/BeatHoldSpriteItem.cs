using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using IdlessChaye.IdleToolkit;

namespace IdlessChaye.TouhouWaltz.Beats
{
	public class BeatHoldSpriteItem : BaseBeatSpriteItem
	{
		[SerializeField] protected Animation _preAni;
		[SerializeField] protected Animation _nextAni;

		[SerializeField] protected GameObject _preSprite;
		[SerializeField] protected GameObject _nextSprite;

		private Image _preImage;
		private Image _nextImage;

		private RectTransform _nextRectTransform;

		private bool _isBeginHolding = false;
		
		public override void Prepare(BeatNoteItem noteItem)
		{
			base.Prepare(noteItem);

			_preImage = _preSprite.GetComponent<Image>();
			_nextImage = _nextSprite.GetComponent<Image>();

			if (_preImage == null || _nextImage == null)
			{
				Debug.LogError("BeatHoldSpriteItem Prepare");
				return;
			}

			if (BeatNoteType == BeatNoteType.BlueHold)
			{
				_preImage.color = BeatConst.blueHoldColor;
				_nextImage.color = BeatConst.blueHoldColor;
			}
			else if (BeatNoteType == BeatNoteType.RedHold)
			{
				_preImage.color = BeatConst.redHoldColor;
				_nextImage.color = BeatConst.redHoldColor;
			}

			_nextRectTransform = _nextSprite.GetComponent<RectTransform>();
			if (_nextRectTransform == null)
				Debug.LogError("!");

			_preAni.Play(BeatConst.SpriteAppearAniName);
			_nextAni.Play(BeatConst.SpriteAppearAniName);
		}

		public override bool HandleJudgeResult(BeatJudgeResultType result)
		{
			if (_isBeginHolding == false)
			{
				_isBeginHolding = true;
				switch (result)
				{
					case BeatJudgeResultType.Cool:
					case BeatJudgeResultType.Fine:
					case BeatJudgeResultType.Good:
						_preAni.Play(BeatConst.SpriteKickDisappearAniName);
						CoroutineHelper.Instance.DelayCall(0.5f, () =>
						 {
							 if (_preSprite != null && _preSprite.activeSelf == true)
								_preSprite.SetActive(false);
						 });
						return false;
					default:
						_preAni.Play(BeatConst.SpriteBadDisappearAniName);
						_nextAni.Play(BeatConst.SpriteBadDisappearAniName);
						Disable();
						return true;
				}
			}
			else if (_isBeginHolding == true)
			{
				switch (result)
				{
					case BeatJudgeResultType.Cool:
					case BeatJudgeResultType.Fine:
					case BeatJudgeResultType.Good:
						_nextAni.Play(BeatConst.SpriteKickDisappearAniName);
						CoroutineHelper.Instance.DelayCall(0.5f, () =>
						{
							if (_nextSprite != null && _nextSprite.activeSelf == true)
								_nextSprite.SetActive(false);
						});
						Disable();
						return true;
					default:
						_nextAni.Play(BeatConst.SpriteBadDisappearAniName);
						Disable();
						return true;
				}
			}

			return false;
		}

		public override void SetPosition(float pastTime, float maxHalfTimeWindow)
		{
			float prevTargetPosX = 0f;
			prevTargetPosX = (_noteItem.time - pastTime) / maxHalfTimeWindow * (_bornPointPosX - _judgeLinePosX);
			_transform.anchoredPosition = new Vector2(prevTargetPosX, 0);

			float nextTargetPosX = 0f;
			nextTargetPosX = (_noteItem.holdTime - pastTime) / maxHalfTimeWindow * (_bornPointPosX - _judgeLinePosX);
			_nextRectTransform.anchoredPosition = new Vector2(nextTargetPosX, 0);
		}

	}
}