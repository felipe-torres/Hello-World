using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Citizen base behaviour
/// </summary>
public class Citizen : MonoBehaviour
{
	private UnityEngine.AI.NavMeshAgent NavMeshAgent;
	public Transform TargetPlayer;
	public Animator m_Animator;

	void Awake()
	{
		NavMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
	}

	void Start()
	{

	}

	void Update()
	{
		NavMeshAgent.destination = TargetPlayer.position;
		UpdateAnimator(NavMeshAgent.velocity);
	}

	void UpdateAnimator(Vector3 move)
	{
		// update the animator parameters
		//m_Animator.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);
		//m_Animator.SetFloat("Turn", m_TurnAmount, 0.1f, Time.deltaTime);
		//m_Animator.SetBool("Crouch", m_Crouching);
		//m_Animator.SetBool("OnGround", m_IsGrounded);
		//if (!m_IsGrounded)
		//{
		//	m_Animator.SetFloat("Jump", m_Rigidbody.velocity.y);
		//}

		// calculate which leg is behind, so as to leave that leg trailing in the jump animation
		// (This code is reliant on the specific run cycle offset in our animations,
		// and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
		//float runCycle =
		//	Mathf.Repeat(
		//		m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime + m_RunCycleLegOffset, 1);
		//float jumpLeg = (runCycle < k_Half ? 1 : -1) * m_ForwardAmount;
		//if (m_IsGrounded)
		//{
		//	m_Animator.SetFloat("JumpLeg", jumpLeg);
		//}

		// the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
		// which affects the movement speed because of the root motion.
		//if (m_IsGrounded && move.magnitude > 0)
		if (move.magnitude > 0)
		{
			m_Animator.speed = 10;
		}
		else
		{
			// don't use that while airborne
			m_Animator.speed = 1;
		}
	}

	private void PlayerContact()
	{

	}

	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.CompareTag("Player"))
		{
			NavMeshAgent.Stop();
		}
	}

}
