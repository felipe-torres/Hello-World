using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AudioManager : MonoBehaviour
{
	public static AudioManager Instance { get; set; }
	public AudioSource[] BGAudios;
	public AudioSource SFXAudio;
	public AudioClip[] SFX;

	public enum SFXType { o1, o2, o3, or , yay, elevator }
	public int MusicChannelCount { get; set; }

	private void Awake()
	{
		Instance = this;
		MusicChannelCount = 0;
	}

	public void Start()
	{
		BGAudios[0].volume = 1;
		BGAudios[1].volume = 0;
		BGAudios[2].volume = 0;
		BGAudios[3].volume = 0;
	}

	public void AddNextChannel()
	{
		MusicChannelCount++;
		if (MusicChannelCount > 3) return;
		BGAudios[MusicChannelCount].DOFade(1f, 1f);
	}

	public void PlaySFX(SFXType type)
	{
		SFXAudio.pitch = 1.0f;
		switch (type)
		{
		case SFXType.o1:
			SFXAudio.PlayOneShot(SFX[0]);
			break;
		case SFXType.o2:
			SFXAudio.PlayOneShot(SFX[1]);
			break;
		case SFXType.o3:
			SFXAudio.PlayOneShot(SFX[2]);
			break;
		case SFXType.or:
			int index = Random.Range(0, 3);
			SFXAudio.PlayOneShot(SFX[index]);
			break;
		case SFXType.yay:
			SFXAudio.pitch = Random.Range(0.5f, 1.5f);
			SFXAudio.PlayOneShot(SFX[3], 0.6f);
			break;
		case SFXType.elevator:
			SFXAudio.PlayOneShot(SFX[4]);
			break;
		}
	}


}
