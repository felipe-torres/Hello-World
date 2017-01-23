using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
using UnityEngine.Windows.Speech;
#endif
using DG.Tweening;

/// <summary>
/// Wave man
/// </summary>
public class Player : MonoBehaviour
{
	public GameObject WaveWavePrefab;

	public ParticleSystem WaveWaveParticleSystem;
	public Animator animador;

	public float WaveWaveRadius = 1f;
	public float WaveWaveReach = 1f;

	private Rigidbody rb;
	// Movement
	private float h, v;
	public float speed = 2.0f;
	public float turnSmoothing = 15f;
	public bool CanMove { get; set; }
	private Vector3 moveDirection = Vector3.zero;

	private Camera MainCamera;
	private Vector3 cameraForward;
	private Vector3 cameraRight;

	public int EnemiesStuck { get; set; }

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
	KeywordRecognizer keywordRecognizer;
#endif
	public string[] KeywordsArray;
	public TextMesh speechText;

	void Awake()
	{
		rb = GetComponent<Rigidbody>();
		CanMove = false;
		//CanMove = true;

		MainCamera = Camera.main;

		CalculateCameraForward();
	}


#if UNITY_EDITOR || UNITY_STANDALONE_WIN

	// Use this for initialization
	void Start ()
	{
		print(PhraseRecognitionSystem.isSupported);

		keywordRecognizer = new KeywordRecognizer(KeywordsArray);
		keywordRecognizer.OnPhraseRecognized += OnKeywordsRecognized;

		keywordRecognizer.Start();


	}

	void OnKeywordsRecognized(PhraseRecognizedEventArgs args)
	{
		print(args.text + " , " + args.confidence);
		Wave();

		if (args.text == "fuck you" || args.text == "chinga tu madre" || args.text == "jodete" || args.text == "fuck off" || args.text == "jodanse"
			|| args.text == "get out of here" || args.text == "get good")
		{
			foreach (Transform t in transform)
			{
				if (t.CompareTag("Citizen"))
				{
					t.GetComponent<Rigidbody>().isKinematic = false;
					t.GetComponent<Citizen>().Explode();
					speed = 2;
				}
			}
		}

		StartCoroutine(SpeechTextRoutine(args.text));
	}

#endif

	private IEnumerator SpeechTextRoutine(string text)
	{
		speechText.text = text;
		speechText.transform.position = transform.position + Vector3.up * 3.5f;

		speechText.gameObject.SetActive(true);
		speechText.transform.DOMoveY(1f + transform.position.y + 3.5f, 1.0f).SetEase(Ease.OutBack);
		speechText.transform.DOScale(0.5f, 1.0f).SetEase(Ease.OutBack);

		yield return new WaitForSeconds(1.0f);

		speechText.gameObject.SetActive(false);

	}

	// Update is called once per frame
	void Update ()
	{
		if ((Input.GetButtonDown("Fire1") || Input.GetButtonDown("Jump")) && CanMove)
		{
			Wave();
		}

		v = Input.GetAxisRaw("Horizontal");
		h = Input.GetAxisRaw("Vertical");
	}

	public void Wave()
	{
		animador.SetTrigger("Hola");
		WaveWaveParticleSystem.Play();

		AudioManager.Instance.PlaySFX(AudioManager.SFXType.or);

		Collider[] colliders = Physics.OverlapSphere (transform.position + transform.forward * WaveWaveReach, WaveWaveRadius);
		foreach (Collider col in colliders)
		{
			if (col.gameObject.CompareTag("Citizen"))
			{
				col.gameObject.GetComponent<Citizen>().Explode();
				print(col.gameObject.name);
			}
		}
	}

	/// <summary>Calculates every fixed frame movement. It is here because movement is physics based.</summary>
	void FixedUpdate()
	{
		MovementManagement(h, v);

		// Add down force to make player fall more rapidly
		rb.AddForce(Physics.gravity * rb.mass);
	}

	void MovementManagement(float horizontal, float vertical)
	{
		if (CanMove)
		{
			moveDirection = new Vector3(horizontal, moveDirection.y, vertical);
			Vector3 cameraMoveDirection = (horizontal * -cameraForward + vertical * cameraRight);

			Vector3 v3 = cameraMoveDirection * speed + Physics.gravity;
			v3.y = rb.velocity.y;
			rb.velocity = v3;

			if (Mathf.Abs(horizontal) > 0.01f || Mathf.Abs(vertical) > 0.01f)
				Rotating(horizontal, vertical);
			animador.SetFloat("RunSpeed", (Mathf.Abs(horizontal) + Mathf.Abs(vertical)) * (speed));
		}

	}

	/// <summary>Calculates this transforms rotation based on a horizontal and vertical input.</summary>
	void Rotating(float horizontal, float vertical)
	{
		Vector3 targetDirection = (horizontal * -cameraForward + vertical * cameraRight);
		Quaternion targetRotation = Quaternion.identity;
		if (targetDirection != Vector3.zero)
			targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
		Quaternion newRotation = Quaternion.Lerp(rb.rotation, targetRotation, turnSmoothing * Time.deltaTime);
		rb.MoveRotation(newRotation);
	}

	void CalculateCameraForward()
	{
		cameraForward = MainCamera.transform.TransformDirection(-Vector3.forward);
		cameraForward.y = 0f;
		cameraForward = cameraForward.normalized;
		cameraRight = new Vector3(-cameraForward.z, 0f, cameraForward.x);
	}
}
