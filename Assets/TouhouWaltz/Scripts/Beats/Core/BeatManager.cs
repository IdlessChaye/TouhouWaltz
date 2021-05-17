using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

using IdlessChaye.IdleToolkit;
using DG.Tweening;


namespace IdlessChaye.TouhouWaltz.Beats
{
	public class BeatManager
	{
		#region Singleton
		private static BeatManager _instance = new BeatManager();
		public static BeatManager Instance => _instance;
		#endregion

		public bool IsPlaying { get; private set; }
		public string MapName { get; set; }
		public string ClipName { get; set; }


		private MonoBeatManager _mono;
		private BeatInputerManager _beatInputerManager;
		private BeatAudioManager _beatAudioManager;
		private BeatJudgerManager _beatJudgerManager;
		private BeatResultManager _beatResultManager;
		private BeatRenderManager _beatRenderManager;
		//public BeatAudioManager BeatAudioManager => _beatAudioManager;

		private BeatMapData _beatMapData;
		public BeatMapData BeatMapData => _beatMapData;
		private BeatStopwatch stopwatch = new BeatStopwatch();
		public BeatStopwatch Stopwatch => stopwatch;
		private SortedList<float, BeatNoteItem> beatNoteItems = new SortedList<float, BeatNoteItem>();
		public SortedList<float, BeatNoteItem> BeatNoteItems => beatNoteItems;
		private GameObject _beatCanvas;

		public void Init()
		{
			if (_mono != null)
				return;

			var managerPrefab = Resources.Load<GameObject>(BeatConst.BeatManagerPrefabPath) as GameObject;
			var manager = GameObject.Instantiate(managerPrefab);
			_mono = manager.GetComponent<MonoBeatManager>();

			if (_mono == null)
			{
				UnityEngine.Debug.LogError("BeatManager Init()");
				return;
			}

			//stopwatch.OnPastTimeChanged += (float time) => Debug.Log(time);

			_beatInputerManager = new BeatInputerManager();
			_beatInputerManager.Init();
			var leftMouseInputer = new BeatInputer { IsMouse = true, Mouse = 0 };
			_beatInputerManager.AddInputer(leftMouseInputer); 
			//leftMouseInputer.onBeatButtonDown += () => { UnityEngine.Debug.Log("Mouse 0 Down"); };
			//leftMouseInputer.onBeatButtonUp += () => { UnityEngine.Debug.Log("Mouse 0 Up"); };
			//var rightMouseInputer = new BeatInputer { IsMouse = true, Mouse = 1 };
			//rightMouseInputer.onBeatButtonUp += () => {
			//	PrepareGame("100-200+tanigon - 渋滞の楽しみ方", "100-200+tanigon - 渋滞の楽しみ方");
			//	StartGame();
			//};
			//_beatInputerManager.AddInputer(rightMouseInputer);

			_beatAudioManager = new BeatAudioManager(_mono.Audio);
			_beatAudioManager.Init();

			_beatJudgerManager = new BeatJudgerManager();
			_beatJudgerManager.Init();
			var leftJudger = new BeatJudger(leftMouseInputer);
			_beatJudgerManager.AddJudger(leftJudger);

			_beatResultManager = new BeatResultManager();
			_beatResultManager.Init();
			_beatResultManager.OnNoteResult += (result) =>
			{
				Debug.Log(result.noteItem.noteType + " judge:" + result.resultType +"   time:" + result.noteItem.time);
			};

			var canvasPrefab = Resources.Load<GameObject>(BeatConst.BeatCanvasPrefabPath) as GameObject;
			_beatCanvas = GameObject.Instantiate(canvasPrefab);
			_beatRenderManager = _beatCanvas.GetComponent<BeatRenderManager>();
			if (_beatRenderManager == null)
			{
				UnityEngine.Debug.LogError("BeatManager Init()");
				return;
			}
			_beatRenderManager.Init();
			_beatResultManager.OnComboChanged += (combo) =>
			{
				_beatRenderManager.ComboText.text = "Combo: " + combo;
				//_beatRenderManager.ComboText.DOText("Combo: " + combo, 0.2f);
				float value = 50;
				Tweener tweener = DOTween.To(() => value, (x) => value = x, 30, 0.2f)
					.OnUpdate(() => _beatRenderManager.ComboText.fontSize = (int)value);
				tweener.Play();
				
			};
			_beatResultManager.OnNoteResult += (result) =>
			{
				var str = "Time: " + stopwatch.PastTime + "  ";
				str += "Judge: " + result.resultType.ToString();
				_beatRenderManager.JudgeResultText.text = str;
				//_beatRenderManager.JudgeResultText.DOText(str, 0.2f);
				float value = 50;
				Tweener tweener = DOTween.To(() => value, (x) => value = x, 30, 0.2f)
					.OnUpdate(() => _beatRenderManager.JudgeResultText.fontSize = (int)value);
				tweener.Play();
			};
		}

		public void PrepareGame(string clipName, string mapName)
		{
			ClipName = clipName;
			MapName = mapName;

			_beatAudioManager.ClipName = this.ClipName;
			_beatAudioManager.PrepareGame();

			LoadGameData(mapName);
			stopwatch.Reset();

			_beatInputerManager.PrepareGame();
			_beatJudgerManager.PrepareGame();
			_beatResultManager.PrepareGame();
			_beatRenderManager.PrepareGame();
		}

		public void StartGame()
		{
			IsPlaying = true; // During the MusicStartDelay, pause the game, ie need to be paused.
			var ie = CoroutineHelper.Instance.DelayCall(BeatConst.MusicStartDelay, () =>
			{
				IsPlaying = true;
				stopwatch.Start();
				_beatAudioManager.PlayAudio();
			});
		}

		public void PauseGame()
		{
			IsPlaying = false;
			stopwatch.Pause();
			_beatAudioManager.PauseAudio();
		}

		public void ResumeGame()
		{
			IsPlaying = true;
			stopwatch.Start();
			_beatAudioManager.PlayAudio();
		}

		// FixedUpdate
		public void Tick(float deltaTime)
		{
			stopwatch.Tick(deltaTime);
			_beatInputerManager.Tick(deltaTime);
			_beatJudgerManager.Tick(deltaTime);
			_beatResultManager.Tick(deltaTime);
			_beatRenderManager.Tick(deltaTime);
		}

		private bool LoadGameData(string fileName)
		{
			this.beatNoteItems.Clear();

			string filePath = Path.Combine(Application.streamingAssetsPath, BeatConst.BeatMapPath, fileName + ".json");
			if (File.Exists(filePath))
			{
				string dataAsJson = File.ReadAllText(filePath);
				var mapData = JsonConvert.DeserializeObject<BeatMapData>(dataAsJson);
				if (mapData == null)
				{
					UnityEngine.Debug.LogError("BeatManager LoadGameData");
					return false;
				}
				_beatMapData = mapData;

				for (int i = 0; i < mapData.beats.Count; ++i)
				{
					this.beatNoteItems.Add(mapData.beats[i].time, mapData.beats[i]);
				}
				return true;
			}
			else
			{
				Debug.LogError(filePath);
				UnityEngine.Debug.LogError("BeatManager LoadGameData");
			}
			return false;
		}

		public void HandleJudgeResult(BeatJudgeResult result)
		{
			_beatResultManager.HandleJudgeResult(result);
			_beatRenderManager.HandleJudgeResult(result);
		}


		public void LogTime(string context)
		{
			UnityEngine.Debug.Log(context + "  time: " + BeatManager.Instance.Stopwatch.PastTime);
		}

	}
}