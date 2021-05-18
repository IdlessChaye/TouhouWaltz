using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdlessChaye.TouhouWaltz.Beats
{
	public enum InputerStatus
	{
		Release,
		Hold,
	}

	public abstract class BaseBeatInputer
	{
		public InputerStatus Status { get; protected set; } = InputerStatus.Release;

		public bool IsUp { get; protected set; } = false;

		public bool IsDown { get; protected set; } = false;

		public void PrepareGame()
		{
			Status = InputerStatus.Release;
			IsUp = false;
			IsDown = false;
		}

		public virtual void ListenInput()
		{
			IsUp = false;
			IsDown = false;
		}
	}


	public class BeatMouseInputer : BaseBeatInputer
	{
		public int Mouse { get; private set; }

		public BeatMouseInputer(int mouse)
		{
			Mouse = mouse;
		}

		public override void ListenInput()
		{
			base.ListenInput();

			if (Input.GetMouseButtonDown(Mouse))
			{
				IsDown = true;
				Status = InputerStatus.Hold;
			}
			else if (Input.GetMouseButtonUp(Mouse))
			{
				IsUp = true;
				Status = InputerStatus.Release;
			}
		}
	}

	public class BeatKeyInputer : BaseBeatInputer
	{
		public KeyCode Key { get; private set; }

		public BeatKeyInputer(KeyCode keyCode)
		{
			Key = keyCode;
		}

		public override void ListenInput()
		{
			base.ListenInput();

			if (Input.GetKeyDown(Key))
			{
				IsDown = true;
				Status = InputerStatus.Hold;
			}
			else if (Input.GetKeyUp(Key))
			{
				IsUp = true;
				Status = InputerStatus.Release;
			}
		}

	}
}