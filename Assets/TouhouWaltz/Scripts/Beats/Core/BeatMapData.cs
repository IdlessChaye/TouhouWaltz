using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdlessChaye.TouhouWaltz.Beats
{

	[System.Serializable]
	public class BeatMapData
	{
		public List<BeatNoteItem> beats = new List<BeatNoteItem>();
		public string mapName;
		public string arranger;
		public float maxRenderHalfTimeWindow;
	}

	[System.Serializable]
	public class BeatNoteItem
	{
		public uint id;
		public float time;
		public float holdTime;
		public BeatNoteType noteType;
		public BeatNoteEvent noteDownEvent;
		public BeatNoteEvent noteUpEvent;
		public BeatNoteEvent noteHoldEvent;
	}

	public enum BeatNoteType : byte
	{
		RedKick,
		BlueKick,
		RedHold,
		BlueHold,
		RedBlueHitComba,
		None,
	}

	public enum BeatNoteEvent : byte
	{
		SpellCard,
		None,
	}

}