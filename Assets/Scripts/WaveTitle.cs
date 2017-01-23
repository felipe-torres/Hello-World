using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WaveTitle : MonoBehaviour {

	// Use this for initialization
	void Start () {
		transform.DOLocalRotate(new Vector3(0, 0, -58f), 0);

		transform.DOLocalRotate(new Vector3(0, 0, 58f), 1).SetEase(Ease.InOutCubic).SetLoops(-1, LoopType.Yoyo);
	}
}
