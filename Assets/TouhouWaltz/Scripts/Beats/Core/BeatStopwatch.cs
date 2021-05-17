using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdlessChaye.TouhouWaltz.Beats
{
	public delegate void OnPastTimeChanged(float pastTime);

	public class BeatStopwatch 
	{
		public float PastTime { get; private set; }

		public bool IsRunning { get; private set; }

		public event OnPastTimeChanged OnPastTimeChanged;

		public void Reset()
		{
			PastTime = 0;
			IsRunning = false;
		}

		public void Stop()
		{
			Reset();
		}

		public void Pause()
		{
			IsRunning = false;
		}

		public void Start()
		{
			IsRunning = true;
		}

		public void Tick(float time)
		{
			if (IsRunning)
				PastTime += time;

			if (OnPastTimeChanged != null)
				OnPastTimeChanged.Invoke(PastTime);
		}
	}
}