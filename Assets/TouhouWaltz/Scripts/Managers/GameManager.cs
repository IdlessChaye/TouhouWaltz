﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IdlessChaye.TouhouWaltz.Beats;
using UnityEditor;

namespace IdlessChaye.TouhouWaltz
{
	public class GameManager : MonoBehaviour
	{
		public bool hasBeatManager;

		public string clipName;

		private void Awake()
		{
			if (hasBeatManager)
				BeatManager.Instance.Init();
		}
		private void Start()
		{
			
		}

		private void OnGUI()
		{
			TestBeatManager();
		}

		private void Update()
		{
			float deltaTime = Time.deltaTime;

			if (hasBeatManager)
			{ 
				// 如果放在FixedUpdate的话，Inputer可能会识别不到输入事件
				BeatManager.Instance.Tick(deltaTime);
			}
		}

		private void TestBeatManager()
		{
			if (GUILayout.Button("播放歌谱"))
			{
				if (BeatManager.Instance.IsPlaying == false)
				{
					BeatManager.Instance.PrepareGame(clipName, clipName);
					BeatManager.Instance.StartGame();
				}
			}
			if (GUILayout.Button("PrepareGame"))
			{
				BeatManager.Instance.PrepareGame(clipName, clipName);
			}
			if (GUILayout.Button("StartGame"))
			{
				if (BeatManager.Instance.IsPlaying == false)
					BeatManager.Instance.StartGame();
			}
			if (GUILayout.Button("PauseGame"))
			{
				if (BeatManager.Instance.IsPlaying == true)
					BeatManager.Instance.PauseGame();
			}
			if (GUILayout.Button("ResumeGame"))
			{
				if (BeatManager.Instance.IsPlaying == false)
					BeatManager.Instance.ResumeGame();
			}
			if (GUILayout.Button("ResetGame"))
			{
				BeatManager.Instance.ResetGame();
			}
			if (GUILayout.Button("ReadyToBePrepared"))
			{
				BeatManager.Instance.ReadyToBePrepared();
			}
		}
	}
}