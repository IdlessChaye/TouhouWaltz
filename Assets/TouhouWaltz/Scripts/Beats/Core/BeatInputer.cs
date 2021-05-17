using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdlessChaye.TouhouWaltz.Beats
{

	public delegate void OnBeatButtonDown();
	public delegate void OnBeatButtonUp();


	public enum InputerStatus
	{
		Release,
		Hold,
	}

	public class BeatInputer
	{
		public bool IsMouse { get; set; } = true;
		public KeyCode Key { get; set; }
		public int Mouse { get; set; }



		public InputerStatus Status { get; set; } = InputerStatus.Release;

		public bool IsUp { get; set; } = false;

		public bool IsDown { get; set; } = false;

		public event OnBeatButtonUp onBeatButtonUp;

		public event OnBeatButtonDown onBeatButtonDown;


		public void PrepareGame()
		{
			Status = InputerStatus.Release;
			IsUp = false;
			IsDown = false;
		}

		public void ListenInput()
		{
			IsUp = false;
			IsDown = false;

			if (IsMouse)
			{
				if (Input.GetMouseButtonDown(Mouse))
				{
					if (onBeatButtonDown != null)
						onBeatButtonDown.Invoke();

					IsDown = true;
					Status = InputerStatus.Hold;
				}
				else if (Input.GetMouseButtonUp(Mouse))
				{
					if (onBeatButtonUp != null)
						onBeatButtonUp.Invoke();

					IsUp = true;
					Status = InputerStatus.Release;
				}
			}
			else
			{
				if (Input.GetKeyDown(Key))
				{
					if (onBeatButtonDown != null)
						onBeatButtonDown.Invoke();

					IsDown = true;
					Status = InputerStatus.Hold;
				}
				else if (Input.GetKeyUp(Key))
				{
					if (onBeatButtonUp != null)
						onBeatButtonUp.Invoke();

					IsUp = true;
					Status = InputerStatus.Release;
				}
			}
		}
	}
}