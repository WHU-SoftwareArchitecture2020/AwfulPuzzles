using UnityEngine;
using System.Collections;

/// <summary>
/// 单例生产者
/// </summary>
public class SingletonSpawner : MonoBehaviour {

	/// <summary>
	/// 音频片段
	/// </summary>
	public AudioClip audioClip;
	
	// Use this for initialization
	void Start () {

		MusicManager.Instance.SetMusic(audioClip);		//设置背景音乐使用的音频片段
		MusicManager.Instance.GetAudioSource().volume = 0.5f;	//设置音量
		MusicManager.Instance.Play();		//播放背景音乐
	}
	
}
