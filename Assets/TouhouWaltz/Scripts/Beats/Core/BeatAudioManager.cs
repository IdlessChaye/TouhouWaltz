using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdlessChaye.TouhouWaltz.Beats
{
	public class BeatAudioManager
	{
		public string ClipName { get; set; }

		private AudioSource audio;

		private Dictionary<string, AudioClip> _dict = new Dictionary<string, AudioClip>();

		public BeatAudioManager(AudioSource audioSource)
		{
			if (audioSource == null)
				UnityEngine.Debug.LogError("BeatAudioManager BeatAudioManager");

			audio = audioSource;
			audio.loop = false;
		}

		public void Init()
		{
			
		}

		public void PrepareGame()
		{
			LoadMusic();
		}

		public void PlayAudio()
		{
			if (audio.clip != null)
			{
				audio.Play();
			}
		}

		public void PauseAudio()
		{
			if (audio.clip != null)
			{
				audio.Pause();
			}
		}

		private void LoadMusic()
		{
			var clip = GetAudioClip();
			if (clip != null)
			{
				this.audio.clip = clip;
			}
		}

		private AudioClip GetAudioClip()
		{
			if (_dict.ContainsKey(ClipName))
			{
				return _dict[ClipName];
			}
			else
			{
				string fullPath = System.IO.Path.Combine(BeatConst.BeatAudioClipPath, ClipName);
				AudioClip clip = Resources.Load(fullPath) as AudioClip;
				if (clip == null)
				{
					Debug.LogError("GetAudioClip");
					return null;
				}
				_dict.Add(ClipName, clip);
				return clip;
			}
		}
	}
}