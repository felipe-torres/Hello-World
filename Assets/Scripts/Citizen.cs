using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// Citizen base behaviour
/// </summary>
public class Citizen : MonoBehaviour
{
	public UnityEngine.AI.NavMeshAgent NavMeshAgent { get; set;}
	public Transform TargetPlayer;
	public Animator m_Animator;
	public ParticleSystem DestroyEffect;

	public bool IsMoving { get; set; }

	void Awake()
	{
		NavMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
		NavMeshAgent.enabled = false;
	}

	void OnEnable()
    {
        TargetPlayer = GameObject.FindGameObjectWithTag("Player").transform;
    }

	void Start()
	{
		IsMoving = true;
	}

	void Update()
	{
		if (IsMoving && NavMeshAgent.enabled)
		{
			NavMeshAgent.destination = TargetPlayer.position;
		}
	}

	/// <summary>
	/// Happily explode this citizen
	/// </summary>
	public void Explode()
	{
		StartCoroutine(ExplodeSequence());
	}

	private IEnumerator ExplodeSequence()
	{
		//NavMeshAgent.Stop();
		IsMoving = false;
		NavMeshAgent.enabled = false;
		yield return new WaitForSeconds(0.1f);
		GetComponent<Rigidbody>().AddExplosionForce (15f, TargetPlayer.transform.position, 15f, 30f, ForceMode.VelocityChange);
		AudioManager.Instance.PlaySFX(AudioManager.SFXType.yay);
		yield return new WaitForSeconds(0.2f);
		DestroyEffect.Play();
		transform.DOScale(0f, 2.0f);
		yield return new WaitForSeconds(2.0f);
		Destroy(this.gameObject);
	}

	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.CompareTag("Player"))
		{
			Hug();
		}
	}

	/// <summary>
	/// Happily hug the player
	/// </summary>
	public void Hug()
	{
		StartCoroutine(HugSequence());
	}

	private IEnumerator HugSequence()
	{
		//NavMeshAgent.Stop();
		IsMoving = false;
		NavMeshAgent.enabled = false;
		GetComponent<Rigidbody>().detectCollisions = false;
		GetComponent<Rigidbody>().isKinematic = true;
		yield return new WaitForSeconds(0.1f);
		DestroyEffect.Play();
		m_Animator.SetTrigger("Hug");
		transform.SetParent(TargetPlayer.transform);
		TargetPlayer.GetComponent<Player>().speed*=0.8f;
		TargetPlayer.GetComponent<Player>().EnemiesStuck++;
	}


}
