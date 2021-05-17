using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

using IdlessChaye.IdleToolkit;

using UnityEditor;

namespace IdlessChaye.TouhouWaltz.Beats
{
	public class BeatMapCreater : MonoBehaviour
	{
		public AudioSource audio;

		public string mapName;
		public string arranger = "Chaye";
		public float maxRenderHalfTimeWindow = BeatConst.RenderHalfTimeWindow;

		private BeatStopwatch stopwatch = new BeatStopwatch();
		private SortedDictionary<float, BeatNoteItem> mappedButtons = new SortedDictionary<float, BeatNoteItem>();
		private BeatNoteItem _lastNoteItem;

		//private SortedList<float, MappingButton> displayingButtons = new SortedList<float, MappingButton>();

		private void Start()
		{
			stopwatch.Reset();
			SetupAudioClip();
		}

		private void OnGUI()
		{
			if (GUILayout.Button("播放音乐"))
			{ 
				audio.Play();
				stopwatch.Start();
			}

			if (GUILayout.Button("停止音乐"))
			{
				audio.Stop();
				stopwatch.Stop();
			}

			if (GUILayout.Button("暂停音乐"))
			{
				audio.Pause();
				stopwatch.Pause();
			}

			if (GUILayout.Button("保存音乐"))
			{
				OnMapFinish();
			}
		}


		private void Update()
		{
			float deltaTime = Time.deltaTime;
			float judgeTime = stopwatch.PastTime;

			if (stopwatch.IsRunning)
			{
				if (Input.GetMouseButtonDown(0))
				{
					if (_lastNoteItem != null)
					{
						float holdTime = judgeTime - _lastNoteItem.time;
						_lastNoteItem.holdTime = holdTime;
						mappedButtons.Add(_lastNoteItem.time, _lastNoteItem);
						_lastNoteItem = null;
					}
					else
					{ 
						var noteItem = new BeatNoteItem
						{
							time = judgeTime,
							holdTime = 0,
							noteType = BeatNoteType.RedKick,
							noteDownEvent = BeatNoteEvent.None,
							noteUpEvent = BeatNoteEvent.None,
							noteHoldEvent = BeatNoteEvent.None
						};

						if (Input.GetKeyDown(KeyCode.A))
						{
							_lastNoteItem = noteItem;
						}
						else
						{
							mappedButtons.Add(judgeTime, noteItem);
						}
					}
				}

				stopwatch.Tick(deltaTime);
			}
		}


		private void SetupAudioClip()
		{
			string filePath = EditorUtility.OpenFilePanel("Select music file", Application.dataPath + "/Resources/Beats/Music", "mp3");
			string[] path = filePath.Split('/');
			string clipName = path[path.Length - 1].Split('.')[0];

			mapName = clipName;

			AudioClip clip = Resources.Load(Path.Combine("Beats/Music", clipName)) as AudioClip;

			this.audio.clip = clip;
		}


		private void OnMapFinish()
		{
			stopwatch.Stop();

			BeatMapData buttonData = new BeatMapData();

			uint id = 0;
			foreach (KeyValuePair<float, BeatNoteItem> pair in this.mappedButtons)
			{
				var noteItem = pair.Value;
				noteItem.id = id++;
				buttonData.beats.Add(noteItem);
			}

			buttonData.mapName = mapName;
			buttonData.arranger = arranger;
			buttonData.maxRenderHalfTimeWindow = maxRenderHalfTimeWindow;

			string filePath = EditorUtility.SaveFilePanel("Save localization data file", Application.streamingAssetsPath, "", "json");

			if (!string.IsNullOrEmpty(filePath))
			{
				string dataAsJson = JsonUtility.ToJson(buttonData);
				File.WriteAllText(filePath, dataAsJson);

				Debug.Log(filePath);
				Debug.Log(dataAsJson);
			}
		}

	}
}