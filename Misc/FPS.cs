// LICENSE: Public Domain

using System;
using UnityEngine;

namespace FPS
{
	[KSPAddon(KSPAddon.Startup.EveryScene, false)]
	public class FPSIndicator : MonoBehaviour
	{
		// Compute average FPS over this many frames
		private const int NUM_SAMPLES = 10;

		private float[] times;
		private int cur_sample = 0;

		private Rect pos;
		private string fps;
		private GUIStyle style = null;

		FPSIndicator()
		{
			times = new float[NUM_SAMPLES];

			float time = Time.realtimeSinceStartup;
			for (int i = 0; i < NUM_SAMPLES; i++)
				times[i] = time;
		}

		public void LateUpdate()
		{
			float time = Time.realtimeSinceStartup;
			float delta = time - times[cur_sample];
			times[cur_sample] = time;
			cur_sample = (cur_sample+1) % NUM_SAMPLES;

			fps = (NUM_SAMPLES / delta).ToString("F1");
		}

		public void OnGUI()
		{
			if (style == null)
			{
				pos = new Rect(Screen.width - 60, 39, 50, 60);

				style = new GUIStyle(GUI.skin.label);
				style.alignment = TextAnchor.UpperRight;
				style.normal.textColor = new Color(0.8f, 0.8f, 0.8f, 0.6f);
			}

			GUI.Label(pos, fps, style);
		}
	}
}

