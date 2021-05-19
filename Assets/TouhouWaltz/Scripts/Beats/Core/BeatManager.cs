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
		public string MapName { get; private set; }
		public string ClipName { get; private set; }


		private MonoBeatManager _mono;
		private BeatInputerManager _beatInputerManager;
		private BeatAudioManager _beatAudioManager;
		private BeatJudgerManager _beatJudgerManager;
		private BeatResultManager _beatResultManager;
		private BeatRenderManager _beatRenderManager;
		private GameObject _beatCanvas;

		public BeatResultManager BeatResultManager => _beatResultManager;

		private BeatMapData _beatMapData;
		public BeatMapData BeatMapData => _beatMapData;
		private BeatStopwatch _stopwatch = new BeatStopwatch();
		public BeatStopwatch Stopwatch => _stopwatch;
		private SortedList<float, BeatNoteItem> beatNoteItems = new SortedList<float, BeatNoteItem>();
		public SortedList<float, BeatNoteItem> BeatNoteItems => beatNoteItems;

		public void Init()
		{
			if (_mono != null)
				return;

			var managerPrefab = Resources.Load<GameObject>(BeatConst.BeatManagerPrefabPath) as GameObject;
			var manager = GameObject.Instantiate(managerPrefab);
			_mono = manager.GetComponent<MonoBeatManager>();


			_beatInputerManager = new BeatInputerManager();
			_beatInputerManager.Init();
			var leftMouseInputer = new BeatMouseInputer(0);
			_beatInputerManager.AddInputer(leftMouseInputer); 

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
			_beatRenderManager.Init();
			_beatRenderManager.IsFollowingMouse = true;

			_beatResultManager.OnComboChanged += (combo) =>
			{
				_beatRenderManager.ComboText.text = "Combo: " + combo;
				float value = 50;
				Tweener tweener = DOTween.To(() => value, (x) => value = x, 30, 0.2f)
					.OnUpdate(() => _beatRenderManager.ComboText.fontSize = (int)value);
				tweener.Play();
				
			};
			_beatResultManager.OnNoteResult += (result) =>
			{
				var str = "Time: " + _stopwatch.PastTime + "  ";
				str += "Judge: " + result.resultType.ToString();
				_beatRenderManager.JudgeResultText.text = str;
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

			_beatAudioManager.PrepareGame(ClipName);
			LoadGameData(mapName);
			_stopwatch.Reset();

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
				_stopwatch.Start();
				_beatAudioManager.PlayAudio();
			});
		}

		public void PauseGame()
		{
			IsPlaying = false;
			_stopwatch.Pause();
			_beatAudioManager.PauseAudio();
		}

		public void ResumeGame()
		{
			IsPlaying = true;
			_stopwatch.Start();
			_beatAudioManager.PlayAudio();
		}

		public void Tick(float deltaTime)
		{
			_stopwatch.Tick(deltaTime);
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



		// Return to have prepared game.
		public void ResetGame()
		{
			IsPlaying = false;
			_stopwatch.Reset();

			_beatAudioManager.StopAudio();
			_beatJudgerManager.ResetGame();
			_beatResultManager.ResetGame();
			_beatRenderManager.ResetGame();
		}

		// Return to have initialized BeatManager.
		public void ReadyToBePrepared()
		{
			IsPlaying = false;
			MapName = null;
			ClipName = null;

			_beatMapData = null;
			beatNoteItems.Clear();
			_stopwatch.Reset();

			_beatAudioManager.ReadyToBePrepared();
			_beatJudgerManager.ReadyToBePrepared();
			_beatResultManager.ReadyToBePrepared();
			_beatRenderManager.ReadyToBePrepared();
		}

		// Return to there is not any beats.
		public void DestroyAll()
		{
			//TODO
		}

	}
}