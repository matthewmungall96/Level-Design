using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniProject;
using UnityEngine.Events;

public class UniverseController : MonoBehaviour
{
	public static bool Swapping
	{
		get; private set;
	}

	[SerializeField]
	private TwinCameraController _twinCameras;
	[Header("Swap Effect Stuff")]
	[SerializeField]
	private Vingette _vingette;
	[SerializeField]
	private AnimationCurve _innerVingette;
	[SerializeField]
	private AnimationCurve _outerVingette;
	[SerializeField]
	private AnimationCurve _saturation;
	[SerializeField]
	private Camera[] _cameras;
	[SerializeField]
	private AnimationCurve _fov;
	[SerializeField]
	private AnimationCurve _timeScale;
	[SerializeField]
	private Transform _itemTransform;
	[SerializeField]
	private AnimationCurve _itemPosition;
    [SerializeField]
    private KeyCode SwapInput = KeyCode.E;

	private AudioSource _audio;
	private bool _swapTiggered;
	private readonly float _swapTime = 0.85f;

    [HideInInspector]
    public Universes currentUniverse;

    public static OnUniverseChanged onUniverseChanged;

	void Awake()
	{
		_audio = GetComponent<AudioSource>();
        currentUniverse = Universes.A;
    }

	void SwapUniverses()
	{
		Swapping = true;
		StartCoroutine(SwapAsync());
	}

	void Update()
	{
		if (!Swapping && Input.GetKeyDown(SwapInput))
		{
			StartCoroutine(SwapAsync());
		}
	}

	/// <summary>
	/// Controls a bunch of stuff like vingette and FoV over time and calls the swap cameras function after a fixed duration.
	/// </summary>
	IEnumerator SwapAsync()
	{
		Swapping = true;
		_swapTiggered = false;

		_audio.PlayOneShot(_audio.clip);

		for (float t = 0; t < 1.0f; t += Time.unscaledDeltaTime * 1.2f)
		{
			for (int i = 0; i < _cameras.Length; i++)
			{
				_cameras[i].fieldOfView = _fov.Evaluate(t);
			}
			//_vingette.MinRadius = _innerVingette.Evaluate(t);
			//_vingette.MaxRadius = _outerVingette.Evaluate(t);
			//_vingette.Saturation = _saturation.Evaluate(t);
			Time.timeScale = _timeScale.Evaluate(t);

			//_itemTransform.localPosition = new Vector3(-0.5f, -0.5f, _itemPosition.Evaluate(t));

			if (t > _swapTime && !_swapTiggered)
			{
				_swapTiggered = true;
				_twinCameras.SwapCameras();

                // Toggle the current universe indicator
                currentUniverse = (currentUniverse == Universes.A) ? Universes.B : Universes.A;
                onUniverseChanged.Invoke((int)currentUniverse);
			}

			yield return null;
		}

		// technically a huge lag spike could cause this to be missed in the coroutine so double check it here.
		while (!_swapTiggered)
		{
			_swapTiggered = true;
			_twinCameras.SwapCameras();

            // Toggle the current universe indicator
            currentUniverse = (currentUniverse == Universes.A) ? Universes.B : Universes.A;
            onUniverseChanged.Invoke((int)currentUniverse);
        }

        for (int i = 0; i < _cameras.Length; i++)
		{
			_cameras[i].fieldOfView = _fov.Evaluate(1.0f);
		}

		//_vingette.MinRadius = _innerVingette.Evaluate(1.0f);
		//_vingette.MaxRadius = _outerVingette.Evaluate(1.0f);
		//_vingette.Saturation = 1.0f;
		//_itemTransform.localPosition = new Vector3(-0.5f, -0.5f, 0.5f);

		Time.timeScale = 1.0f;

		Swapping = false;
	}
}

// Warning: Only set up for 2 universes, adding more won't work
public enum Universes
{
    A,
    B
}

// Event Class to expose unity event to inspector
public class OnUniverseChanged : UnityEvent<int>
{
}
