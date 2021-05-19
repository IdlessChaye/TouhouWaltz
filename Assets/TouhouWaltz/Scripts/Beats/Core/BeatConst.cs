using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdlessChaye.TouhouWaltz.Beats
{
	public static class BeatConst
	{
		public const string BeatManagerPrefabPath = "Beats/Prefabs/BeatManager";
		public const string BeatMapPath = "Beats/Map";
		public const string BeatAudioClipPath = "Beats/Music";
		public const string BeatCanvasPrefabPath = "Beats/Prefabs/BeatCanvas";
		public const string BeatKickSpriteItemPrefabPath = "Beats/Prefabs/KickSpriteItem";
		public const string BeatHoldSpriteItemPrefabPath = "Beats/Prefabs/HoldSpriteItem";


		public const float RenderHalfTimeWindow = 1.5f;
		public const float CoolJudgeTimeHalfWindow = 0.08f;
		public const float FineJudgeTimeHalfWindow = 0.15f;
		public const float GoodJudgeTimeHalfWindow = 0.2f;
		public const float BadJudgeTimeHalfWindow = 0.3f;
		public const float MaxJudgeTimeHalfWindow = 0.35f;

		public const float MusicStartDelay = 2f;


		public static Color blueKickColor = new Color(0, 1, 0);
		public static Color redKickColor = new Color(1, 0, 0);
		public static Color blueHoldColor = new Color(1f, 1, 0.2f);
		public static Color redHoldColor = new Color(1, 1f, 0.2f);

		public const string SpriteAppearAniName = "SpriteAppear";
		public const string SpriteBadDisappearAniName = "SpriteBadDisappear";
		public const string SpriteKickDisappearAniName = "SpriteKickDisappear";

		public const string music000 = "経験値上昇中☆";

	}
}