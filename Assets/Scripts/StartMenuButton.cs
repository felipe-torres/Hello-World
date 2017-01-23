using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StartMenuButton : MonoBehaviour
{
	public GameObject TextGO;
	public Color TurnedOnColor;
	public Color TurnedOffColor;

	public void PointerEnter()
	{
		DOTween.Kill(GetInstanceID() + "Button");
		TextGO.GetComponent<Renderer>().material.DOColor(TurnedOnColor, "_EmissionColor", 0.5f).SetId(GetInstanceID() + "Button");
	}

	public void PointerExit()
	{
		DOTween.Kill(GetInstanceID() + "Button");
		TextGO.GetComponent<Renderer>().material.DOColor(TurnedOffColor, "_EmissionColor", 0.5f).SetId(GetInstanceID() + "Button");
	}
}
