﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 简单的声音管理
/// </summary>
public class AudioManager:MonoBehaviour {
	private static AudioManager m_instance;
	public static AudioManager Instance{
		get{
			if(m_instance==null){
				GameObject go = new GameObject();
				return go.AddComponent<AudioManager>();
			}
			return m_instance;
		}
	}

	private AudioSource m_musicAS;

	void Awake(){
		if(m_instance!=null){
			Destroy(gameObject);
			return;
		}
		name = "[AudioManager]";
		m_instance = this;
		m_musicAS = GetComponent<AudioSource>();
		if(m_musicAS==null){
			m_musicAS = gameObject.AddComponent<AudioSource>();
			m_musicAS.loop=true;
			m_musicAS.playOnAwake = false;
		}
		DontDestroyOnLoad(gameObject);
	}


	//存储loop的music声音
	private Dictionary<string,AudioSource> m_musicAudio = new Dictionary<string, AudioSource>();
	private Dictionary<string,AudioSource> m_musicPathAudio = new Dictionary<string, AudioSource>();
	private List<string> m_removeGuids = new List<string>();

	#region Sound

	public float volume{
		get{return AudioListener.volume; }
		set{
			AudioListener.volume=value;
		}
	}

	/// <summary>
	/// 只播放一次的声音
	/// </summary>
	/// <param name="resourcePath">Resource path.</param>
	/// <param name="volume">Volume.</param>
	/// <param name="delay">delay.</param>
	public void PlaySoundEffect( string resourcePath , float volume = 1f , float delay=0f){
		if(AudioListener.volume<0.01f) return ;
		AudioClip clip = Resources.Load<AudioClip>(resourcePath);
		if(clip){
			clip.name = resourcePath;
			PlaySoundEffect(clip,volume,delay);
		}
		else
		{
			Debug.LogWarning("Sound not found: "+resourcePath);
		}
	}

	/// <summary>
	/// 只播放一次的声音
	/// </summary>
	/// <param name="clip">Clip.</param>
	/// <param name="volume">Volume.</param>
	/// <param name="delay">delay.</param>
	public void PlaySoundEffect( AudioClip clip , float volume = 1f, float delay=0f){
		if(AudioListener.volume<0.01f) return ;
		GameObject go = new GameObject(clip.name);
		go.transform.parent = transform;
		go.transform.position = Camera.main.transform.position;
		AudioSource audioSource = go.AddComponent<AudioSource>();
		audioSource.loop=false;
		audioSource.volume = volume;
		audioSource.clip = clip;
		if(delay<=0){
			audioSource.Play();
			Destroy(go,clip.length);
		}else{
			audioSource.PlayDelayed(delay);
			Destroy(go,clip.length+delay);
		}
	}
	/// <summary>
	/// Stops the sound effect.
	/// </summary>
	/// <param name="resourcePath">Resource path or AudioClip name.</param>
	public void StopSoundEffect(string resourcePath){
		AudioSource audio = null;
		foreach(AudioSource s in GetComponentsInChildren<AudioSource>()){
			if(s.transform!=transform && s.loop==false && s.name.LastIndexOf(resourcePath)>-1){
				audio = s;
				break;
			}
		}
		if(audio) Destroy(audio.gameObject);
	}
	#endregion


	void FixedUpdate(){
		if(m_musicAudio.Count>0){
			m_removeGuids.Clear();
			foreach(string guid in m_musicAudio.Keys){
				AudioSource source = m_musicAudio[guid];
				if(!source.loop && !source.isPlaying){
					m_removeGuids.Add(guid);
				}
			}
			for(int i=0;i<m_removeGuids.Count;++i){
				StopMusicByGUID(m_removeGuids[i]);
			}
		}
	}


	#region Music
	/// <summary>
	/// 循环播放音效
	/// </summary>
	/// <returns>此音效的id.</returns>
	/// <param name="resourcePath">Resource path.</param>
	/// <param name="volume">Volume.</param>
	/// <param name="dontDestroy">dontDestroy.</param>
	public string PlayMusic(string resourcePath , float volume = 1f ,bool isLoop = true,bool dontDestroy=false){
		if(m_musicPathAudio.ContainsKey(resourcePath)){
			AudioSource source = m_musicPathAudio[resourcePath];
			if(!source.isPlaying){
				source.loop = isLoop;
				source.Play();
			}
			foreach(string key in m_musicAudio.Keys){
				if(m_musicAudio[key]==source){
					return key;
				}
			}
		}
		else
		{
			AudioClip clip = Resources.Load<AudioClip>(resourcePath);
			if(clip){
				string id = PlayMusic(clip,volume,isLoop,dontDestroy);
				AudioSource source = m_musicAudio[id];
				m_musicPathAudio[resourcePath] = source;
				return id;
			}
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
	public string PlayMusic(AudioClip clip, float volume = 1f,bool isLoop = true,bool dontDestroy=false){
		string guid = Guid.NewGuid().ToString();
		GameObject go = new GameObject("Music_"+guid);
		if(dontDestroy){
			GameObject.DontDestroyOnLoad(go);
		}
		go.transform.parent = transform;
		go.transform.position = Camera.main.transform.position;
		AudioSource audioSource = go.AddComponent<AudioSource>();
		audioSource.loop=isLoop;
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
			string id=null;
			foreach(String path in m_musicPathAudio.Keys){
				if(m_musicPathAudio[path]== source){
					id = path;
					break;
				}
			}
			if(!string.IsNullOrEmpty(id)){
				m_musicPathAudio.Remove(id);
			}
			m_musicAudio.Remove(guid);
			GameObject.Destroy(source.gameObject);
		}
	}
	public void StopMusicByPath( string path){
		if(m_musicPathAudio.ContainsKey(path)){
			AudioSource source = m_musicPathAudio[path];
			source.Stop();
			string id=null;
			foreach(String guid in m_musicAudio.Keys){
				if(m_musicAudio[guid]== source){
					id = guid;
					break;
				}
			}
			if(!string.IsNullOrEmpty(id)){
				m_musicAudio.Remove(id);
			}
			m_musicPathAudio.Remove(path);
			GameObject.Destroy(source.gameObject);
		}
	}
	public AudioSource GetMusicByGUID( string guid ){
		if(m_musicAudio.ContainsKey(guid)){
			return m_musicAudio[guid];
		}
		return null;
	}
	public AudioSource GetMusicByPath( string path){
		if(m_musicPathAudio.ContainsKey(path)){
			return m_musicPathAudio[path];
		}
		return null;
	}


	/// <summary>
	/// 播放背景音乐
	/// </summary>
	/// <param name="clip">Clip.</param>
	/// <param name="volume">Volume.</param>
	/// <param name="isContinue">是否继续，false将重新开始播放.</param>
	public void PlayBgMusic( AudioClip clip , bool isContinue=true){
		if(isContinue && clip==m_musicAS.clip && m_musicAS.isPlaying) return;

		m_musicAS.clip = clip;
		m_musicAS.Play();
	}
	/// <summary>
	/// 停止背景音乐的播放
	/// </summary>
	public void StopBgMusic(){
		m_musicAS.Stop();
	}

	/// <summary>
	/// 背景音乐的音量
	/// </summary>
	/// <value>The background music volume.</value>
	public float bgMusicVolume{
		get{ return m_musicAS.volume ; }
		set{
			m_musicAS.volume = value;
		}
	}
	#endregion




	/// <summary>
	/// Stop All music and sounds effect
	/// </summary>
	/// <param name="stopBgMusic">If set to <c>true</c> contain background music.</param>
	public void StopAll(bool stopBgMusic = false){
		foreach(var item in m_musicAudio){
			AudioSource source = item.Value;
			source.Stop();
			GameObject.Destroy(source.gameObject);
		}
		m_musicAudio.Clear();
		m_musicPathAudio.Clear();

		System.Collections.Generic.List<AudioSource> sounds = new List<AudioSource>();
		foreach(AudioSource s in transform.GetComponentsInChildren<AudioSource>(true)){
			if(s.transform!=transform) sounds.Add(s);
		}
		for(int i=0;i<sounds.Count;++i){
			Destroy(sounds[i].gameObject);
		}

		if(stopBgMusic){
			StopBgMusic();
		}
	}
}
