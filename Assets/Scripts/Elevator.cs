using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Elevator : MonoBehaviour
{
	public Transform Door;
	public Transform CitizenParent;

	public bool IsFree = true;

	// Use this for initialization
	void Start ()
	{
		Door.DOLocalMoveY(2f, 0f);
		Descend(0f);
	}

	public void Ascend()
	{
		StartCoroutine(AscendSequence());
	}

	private IEnumerator AscendSequence()
	{
		IsFree = false;
		transform.DOLocalMoveY(117f, 2f);
		AudioManager.Instance.PlaySFX(AudioManager.SFXType.elevator);
		yield return new WaitForSeconds(2f);
		Door.DOLocalMoveY(-581f, 0.5f);
		yield return new WaitForSeconds(0.5f);
		foreach (Transform child in CitizenParent) 
		{
			if(child.GetComponent<Citizen>())
			{
				child.GetComponent<Rigidbody>().isKinematic = false;
 				child.GetComponent<Citizen>().NavMeshAgent.enabled = true;
			}
		}
		foreach (Transform child in CitizenParent) 
		{
			if(child.GetComponent<Citizen>())
			{
				child.transform.parent = null;
			}
		}
	}

	public void Descend(float Time = -1)
	{
		StartCoroutine(DescendSequence(Time));
	}

	private IEnumerator DescendSequence(float Time = -1)
	{
		Door.DOLocalMoveY(2f, Time == -1 ? 0.5f : Time);
		yield return new WaitForSeconds(Time == -1 ? 0.5f : Time);
		transform.DOLocalMoveY(-1700f, Time == -1 ? 2f : Time);
		yield return new WaitForSeconds(Time == -1 ? 0.5f : Time);
		IsFree = true;
	}
}
