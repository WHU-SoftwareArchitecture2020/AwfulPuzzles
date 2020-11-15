using UnityEngine;
using System.Collections;

/// <summary>
/// 音乐管理器
/// 单例模式
/// </summary>
public class MusicManager : MonoBehaviour {

	private static MusicManager _instance = null;
	/// <summary>
	/// 暴露Instance便于单例调用
	/// </summary>
	public static MusicManager Instance {
		get {
			if (_instance == null) {
				GameObject obj = new GameObject("MusicManager");
				_instance = obj.AddComponent<MusicManager>();
				_audioSource = obj.AddComponent<AudioSource>();
				DontDestroyOnLoad(obj);
			}
			return _instance;
		}
	}

	/// <summary>
	/// AudioSource组件
	/// </summary>
	private static AudioSource _audioSource = null;
	
	/// <summary>
	/// 获取AudioSource组件
	/// </summary>
	/// <returns></returns>
	public  AudioSource GetAudioSource() {
		return _audioSource;
	}

	/// <summary>
	/// 设置音频片段
	/// </summary>
	/// <param name="ac"></param>
	public void SetMusic(AudioClip ac) {

		_audioSource.clip = ac;
	}

	/// <summary>
	/// 播放
	/// </summary>
	public void Play() {
		_audioSource.Play();
        _audioSource.loop = true;
    }
}
