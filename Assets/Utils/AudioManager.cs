using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// 简单的声音管理
/// </summary>
public class AudioManager {
	private static AudioManager m_instance;
	public static AudioManager GetInstance(){
		if(m_instance==null) m_instance = new AudioManager();
		return m_instance;
	}

	//存储loop的music声音
	private Dictionary<string,AudioSource> m_musicAudio = new Dictionary<string, AudioSource>();


	#region Sound
	/// <summary>
	/// 只播放一次的声音
	/// </summary>
	/// <param name="resourcePath">Resource path.</param>
	/// <param name="volume">Volume.</param>
	public void PlaySoundEffect( string resourcePath , float volume = 1f){
		if(AudioListener.volume<0.01f) return ;
		AudioClip clip = Resources.Load<AudioClip>(resourcePath);
		if(clip){
			PlaySoundEffect(clip,volume);
		}
	}
	/// <summary>
	/// 只播放一次的声音
	/// </summary>
	/// <param name="clip">Clip.</param>
	/// <param name="volume">Volume.</param>
	public void PlaySoundEffect( AudioClip clip , float volume = 1f){
		if(AudioListener.volume<0.01f) return ;
		AudioSource.PlayClipAtPoint(clip,Camera.main.transform.position);
	}
	#endregion


	#region Music
	/// <summary>
	/// 循环播放音效
	/// </summary>
	/// <returns>此音效的id.</returns>
	/// <param name="resourcePath">Resource path.</param>
	/// <param name="volume">Volume.</param>
	/// <param name="dontDestroy">dontDestroy.</param>
	public string PlayMusic(string resourcePath , float volume = 1f ,bool dontDestroy=false){
		AudioClip clip = Resources.Load<AudioClip>(resourcePath);
		if(clip){
			return PlayMusic(clip,volume,dontDestroy);
		}
		return null;
	}
	/// <summary>
	/// 循环播放音效
	/// </summary>
	/// <returns>此音效的id.</returns>
	/// <param name="clip">Clip.</param>
	/// <param name="volume">Volume.</param>
	/// <param name="dontDestroy">dontDestroy.</param>
	public string PlayMusic(AudioClip clip, float volume = 1f,bool dontDestroy=false){
		string guid = Guid.NewGuid().ToString();
		GameObject go = new GameObject("Music_"+guid);
		if(dontDestroy){
			GameObject.DontDestroyOnLoad(go);
		}
		go.transform.position = Camera.main.transform.position;
		AudioSource audioSource = go.AddComponent<AudioSource>();
		audioSource.loop=true;
		audioSource.volume = volume;
		audioSource.clip = clip;
		audioSource.Play();
		m_musicAudio[guid] = audioSource;
		return guid;
	}
	public void StopMusicByGUID( string guid){
		if(m_musicAudio.ContainsKey(guid)){
			AudioSource source = m_musicAudio[guid];
			source.Stop();
			GameObject.Destroy(source.gameObject);
			m_musicAudio.Remove(guid);
		}
	}
	public AudioSource GetMusicByGUID( string guid ){
		if(m_musicAudio.ContainsKey(guid)){
			return m_musicAudio[guid];
		}
		return null;
	}

	public void StopAllMusic(){
		foreach(var item in m_musicAudio){
			AudioSource source = item.Value;
			source.Stop();
			GameObject.Destroy(source.gameObject);
		}
		m_musicAudio.Clear();
	}
	#endregion


	/// <summary>
	/// 设置整体的音量，会影响所有的声音，开关音效可以用这个控制
	/// </summary>
	/// <param name="volume">Volume.</param>
	public void SetListenerVolume( float volume){
		AudioListener.volume = volume;
	}
}
