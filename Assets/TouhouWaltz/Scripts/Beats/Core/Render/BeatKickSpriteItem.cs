using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace IdlessChaye.TouhouWaltz.Beats
{
	public class BeatKickSpriteItem : BaseBeatSpriteItem
	{
		[SerializeField] protected Animation _ani;
		[SerializeField] protected GameObject _sprite;

		private Image _image;

		public override void Prepare(BeatNoteItem noteItem)
		{
			base.Prepare(noteItem);

			_image = _sprite.GetComponent<Image>();

			if (_image == null)
			{
				Debug.LogError("KickBeatSpriteItem Prepare");
				return;
			}

			if (BeatNoteType == BeatNoteType.BlueKick)
			{
				_image.color = BeatConst.blueKickColor;
			}
			else if (BeatNoteType == BeatNoteType.RedKick)
			{
				_image.color = BeatConst.redKickColor;
			}

			_ani.Play(BeatConst.SpriteAppearAniName);
		}

		public override bool HandleJudgeResult(BeatJudgeResultType result)
		{
			switch (result)
			{
				case BeatJudgeResultType.Cool:
				case BeatJudgeResultType.Fine:
				case BeatJudgeResultType.Good:
					_ani.Play(BeatConst.SpriteKickDisappearAniName);
					Disable();
					break;
				default:
					_ani.Play(BeatConst.SpriteBadDisappearAniName);
					Disable();
					break;
			}

			return true;
		}

		public override void SetPosition(float pastTime, float maxHalfTimeWindow)
		{
			float targetPosX = 0f;
			targetPosX = (_noteItem.time - pastTime) / maxHalfTimeWindow * (_bornPointPosX - _judgeLinePosX);
			_transform.anchoredPosition = new Vector2(targetPosX, 0);
		}
	}
}