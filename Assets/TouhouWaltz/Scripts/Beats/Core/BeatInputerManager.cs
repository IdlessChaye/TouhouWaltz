using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdlessChaye.TouhouWaltz.Beats
{
	public class BeatInputerManager
	{

		private List<BeatInputer> _inputers = new List<BeatInputer>();

		private bool _isListening;

		public void Init()
		{

		}

		public void PrepareGame()
		{
			foreach(var inputer in _inputers)
			{
				inputer.PrepareGame();
			}
		}

		public void AddInputer(BeatInputer inputer)
		{
			_inputers.Add(inputer);
		}

		public void SetInputerKey(int index, KeyCode newKeyCode)
		{
			_inputers[index].IsMouse = false;
			_inputers[index].Key = newKeyCode;
		}

		public void SetInputerMouse(int index, int mouseIndex)
		{
			_inputers[index].IsMouse = true;
			_inputers[index].Mouse = mouseIndex;
		}

		public void Tick(float deltaTime)
		{
			foreach(var inputer in _inputers)
			{
				inputer.ListenInput();
			}
		}

		public void Clear()
		{
			_inputers.Clear();
		}

		
	}
}