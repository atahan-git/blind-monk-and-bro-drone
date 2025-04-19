using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundVisualizer : MonoBehaviour
{
	 int raycastCount = 200;
	 float maxRange = 20f;

	private AudioSource audioSource;
	private float timer = 0f;
	private bool wasPlayingLastFrame = false;
	private AudioClip lastClip = null;

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
	}

	private void Update()
	{
		AudioClip currentClip = audioSource.clip;
		bool isPlaying = audioSource.isPlaying && currentClip != null;

		// Detect clip change
		if (currentClip != lastClip)
		{
			lastClip = currentClip;
			timer = 0f; // Reset timer on clip change
		}

		// Detect audio start
		if (isPlaying && !wasPlayingLastFrame)
		{
			timer = 0f;
			PerformRaycastBurst();
		}

		// Active timer
		if (isPlaying)
		{
			timer += Time.deltaTime;

			if (timer >= 1f)
			{
				timer = 0f;
				PerformRaycastBurst();
			}
		}

		// Reset timer when audio stops
		if (!isPlaying && wasPlayingLastFrame)
		{
			timer = 0f;
		}

		wasPlayingLastFrame = isPlaying;
	}

	void PerformRaycastBurst() {
		var hitPoints = new List<Vector3>();

		float range = Mathf.Clamp(audioSource.volume * maxRange, 1f, maxRange);

		for (int i = 0; i < raycastCount; i++)
		{
			Vector3 dir = Random.onUnitSphere;
			Ray ray = new Ray(transform.position, dir);
			if (Physics.Raycast(ray, out RaycastHit hit, range))
			{
				hitPoints.Add(hit.point);
			}
		}
		
		GPUSoundDotsRenderer.s.AddDotList(hitPoints, 1.5f);
	}
}