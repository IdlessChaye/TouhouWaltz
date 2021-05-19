using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdlessChaye.TouhouWaltz
{
	public static class Const
	{
		public static KeyCode KeyChangeSC = KeyCode.Q;
		public static KeyCode KeyUseSC = KeyCode.R;
		public static KeyCode KeyLowMoveSpeed = KeyCode.LeftShift;
		public static KeyCode KeyBoom = KeyCode.Space;

		public static KeyCode KeyMoveUp = KeyCode.W;
		public static KeyCode KeyMoveDown = KeyCode.S;
		public static KeyCode KeyMoveLeft = KeyCode.A;
		public static KeyCode KeyMoveRight = KeyCode.D;

		public static bool IsMouseBeatRedClick = true;
		public static uint MouseBeatRedClick = 0;
		public static KeyCode KeyBeatRedClick = KeyCode.None;

		public const float HighMoveSpeedDefault = 5;
		public const float LowMoveSpeedDefault = 3;

		public const int HPDefault = 2;
		public const int BoomDefault = 2;
		public const int PowerDefault = 0;

		public const float JudgeLengthBulletSpeed = 5f;
		public const float MinBulletSpeed = 0.5f;
		public const float MaxBulletSpeed = 10f;

		public const float BulletSizeBig = 5f;
		public const float BulletSizeMiddle = 3f;
		public const float BulletSizeSmall = 2f;


	}
}
