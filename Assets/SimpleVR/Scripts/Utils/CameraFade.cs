using UnityEngine;
using System.Collections;

namespace CameraFading
{
	/// <summary>
	/// Simple class to fade camera view to a color. Must be attached to camera.
	/// Author: Daniel Castaño Estrella (daniel.c.estrella@gmail.com)
	/// Based on this example: https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnPostRender.html
	/// 
	/// INSTRUCTIONS
	/// Use namespace CameraFading (using CameraFading;)
	///	Call CameraFade.In or CameraFade.Out functions passing duration,from anywhere in your code.
	///	Set color with CameraFade.Color
	///	Set alpha manually with CameraFade.Alpha
	///	
	/// EXTRA
	/// Possibility to force restart Fade In and Fade Out functions when called.
	/// Possibility to force fixed duration when fades don't restart. By default fades use a fraction of duration if they start from middle-state-alpha.
	/// Possibility to pass callback functions or lambda functions. Example:
	/// 
	/// CameraFading.CameraFade.Out(() =>
	///	{
	///		Debug.Log("fade out finished");
	///	});
	///	
	/// </summary>
	public class CameraFade : MonoBehaviour
	{
		/// <summary>
		/// Static reference to instance
		/// </summary>
		public static CameraFade Instance { get; private set; }

		/// <summary>
		/// Static reference to the color.
		/// Color to fade.
		/// It sets the color of the material used to do fades.
		/// </summary>
		public static Color Color
		{
			get { if (Instance == null) return Color.clear; return Instance.color; }
			set { if (Instance == null) return; Instance.color = new Color(value.r, value.g, value.b, Instance.alpha); Instance.material.SetColor(Instance.materialColorID, Instance.color); }
		}

		public static float Alpha
		{
			get { if (Instance == null) return 0; return Instance.alpha; }
			set { if (Instance == null) return; if (Instance.alpha != value) { Instance.alpha = value; Instance.color.a = value; Instance.material.SetColor(Instance.materialColorID, Instance.color); } }
		}

		//color of the fade
		[SerializeField] private Color color = new Color(0, 0, 0, 0);
		//current alpha value
		[SerializeField] private float alpha = 0;

		//material used to do fades.
		private Material material;
		//property ID of the color of the material. Faster than using a string.
		private int materialColorID;

		/// <summary>
		/// Set reference to instance
		/// </summary>
		void OnEnable()
		{
			if (!material)
			{
				materialColorID = Shader.PropertyToID("_Color");

				var shader = Shader.Find("Hidden/Internal-Colored");
				material = new Material(shader);
				material.hideFlags = HideFlags.HideAndDontSave;
				// Turn off backface culling, depth writes, depth test.
				material.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
				material.SetInt("_ZWrite", 0);
				material.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);

				Color = color;
				material.SetColor(materialColorID, color);
			}

			Instance = this;
		}

		/// <summary>
		/// Set instance reference to null
		/// </summary>
		void OnDisable()
		{
			Instance = null;
		}

		/// <summary>
		/// Add a quad in front of camera to fade with color.
		/// </summary>
		public void OnPostRender()
		{
#if UNITY_EDITOR
			//To change fade color and alpha using inspector variables
			if (alpha != color.a || material.GetColor(materialColorID) != color)
			{
				color.a = alpha;
				material.SetColor(materialColorID, color);
			}
#endif

			GL.PushMatrix();
			GL.LoadOrtho();

			// activate the first shader pass (in this case we know it is the only pass)
			material.SetPass(0);
			// draw a quad over whole screen
			GL.Begin(GL.QUADS);
			GL.Vertex3(0, 0, 0);
			GL.Vertex3(1, 0, 0);
			GL.Vertex3(1, 1, 0);
			GL.Vertex3(0, 1, 0);
			GL.End();

			GL.PopMatrix();
		}

		/// <summary>
		/// Call this method to Fade In.
		/// </summary>
		/// <param name="duration">1 by default. How long does it take?</param>
		/// <param name="restart">false by default. If true it starts from alpha = 1.</param>
		/// <param name="fixedDuration">false by default. If false fade's duration is proportional to alpha change, if true it allways take same full duration. Only makes change if restart parameter is false.</param>
		public static void In(float duration = 1, bool restart = false, bool fixedDuration = false)
		{
			if (Instance == null) return;

			Instance.StopAllCoroutines();
			Instance.StartCoroutine(Instance.CameraFadeIn(duration, restart, fixedDuration));
		}

		/// <summary>
		/// Call this method to Fade In with a Callback Action.
		/// </summary>
		/// <param name="Callback">Method or lambda function to call after fade is finished.</param>
		/// <param name="duration">1 by default. How long does it take?</param>
		/// <param name="restart">false by default. If true it starts from alpha = 1.</param>
		/// <param name="fixedDuration">false by default. If false fade's duration is proportional to alpha change, if true it allways take same full duration. Only makes change if restart parameter is false.</param>
		public static void In(System.Action Callback, float duration = 1, bool restart = false, bool fixedDuration = false)
		{
			if (Instance == null) return;

			Instance.StopAllCoroutines();
			Instance.StartCoroutine(Instance.CameraFadeIn(Callback, duration, restart, fixedDuration));
		}

		/// <summary>
		/// Call this method to Fade Out.
		/// </summary>
		/// <param name="duration">1 by default. How long does it take?</param>
		/// <param name="restart">false by default. If true it starts from alpha = 1.</param>
		/// <param name="fixedDuration">false by default. If false fade's duration is proportional to alpha change, if true it allways take same full duration. Only makes change if restart parameter is false.</param>
		public static void Out(float duration = 1, bool restart = false, bool fixedDuration = false)
		{
			if (Instance == null) return;

			Instance.StopAllCoroutines();
			Instance.StartCoroutine(Instance.CameraFadeOut(duration, restart, fixedDuration));
		}

		/// <summary>
		/// Call this method to Fade Out with a Callback Action.
		/// </summary>
		/// <param name="Callback">Method or lambda function to call after fade is finished.</param>
		/// <param name="duration">1 by default. How long does it take?</param>
		/// <param name="restart">false by default. If true it starts from alpha = 1.</param>
		/// <param name="fixedDuration">false by default. If false fade's duration is proportional to alpha change, if true it allways take same full duration. Only makes change if restart parameter is false.</param>
		public static void Out(System.Action Callback, float duration = 1, bool restart = false, bool fixedDuration = false)
		{
			if (Instance == null) return;

			Instance.StopAllCoroutines();
			Instance.StartCoroutine(Instance.CameraFadeOut(Callback, duration, restart, fixedDuration));
		}

		/// <summary>
		/// Coroutine to Fade In.
		/// </summary>
		/// <param name="duration">1 by default. How long does it take?</param>
		/// <param name="restart">false by default. If true it starts from alpha = 1.</param>
		/// <param name="fixedDuration">false by default. If false fade's duration is proportional to alpha change, if true it allways take same full duration. Only makes change if restart parameter is false.</param>
		/// <returns></returns>
		private IEnumerator CameraFadeIn(float duration, bool restart, bool fixedDuration)
		{
			//Debug.Log("_______________________ init Fade In");
			//Debug.Log("init time: " + Time.time);

			if (restart) Alpha = 1;
			else if (fixedDuration) duration /= Alpha;

			for (float i = Alpha; (i - Time.deltaTime / duration) > 0; i -= Time.deltaTime / duration)
			{
				Alpha = i;
				yield return null;
			}
			//Debug.Log("time end: " + Time.time);

			Alpha = 0;
		}

		/// <summary>
		/// Coroutine to Fade In with a Callback
		/// </summary>
		/// <param name="Callback">Method or lambda function to call after fade is finished.</param>
		/// <param name="duration">1 by default. How long does it take?</param>
		/// <param name="restart">false by default. If true it starts from alpha = 1.</param>
		/// <param name="fixedDuration">false by default. If false fade's duration is proportional to alpha change, if true it allways take same full duration. Only makes change if restart parameter is false.</param>
		/// <returns></returns>
		private IEnumerator CameraFadeIn(System.Action Callback, float duration, bool restart, bool fixedDuration)
		{
			//Debug.Log("_______________________ init Fade In");
			//Debug.Log("init time: " + Time.time);

			if (restart) Alpha = 1;
			else if (fixedDuration) duration /= Alpha;

			for (float i = Alpha; (i - Time.deltaTime / duration) > 0; i -= Time.deltaTime / duration)
			{
				Alpha = i;
				yield return null;
			}
			//Debug.Log("time end: " + Time.time);

			Alpha = 0;
			Callback();
		}

		/// <summary>
		/// Coroutine to Fade Out
		/// </summary>
		/// <param name="duration">1 by default. How long does it take?</param>
		/// <param name="restart">false by default. If true it starts from alpha = 1.</param>
		/// <param name="fixedDuration">false by default. If false fade's duration is proportional to alpha change, if true it allways take same full duration. Only makes change if restart parameter is false.</param>
		/// <returns></returns>
		private IEnumerator CameraFadeOut(float duration, bool restart, bool fixedDuration)
		{
			//Debug.Log("_______________________ init Fade Out");
			//Debug.Log("init time: " + Time.time);

			if (restart) Alpha = 0;
			else if (fixedDuration) duration /= (1 - Alpha);

			for (float i = Alpha; (i + Time.deltaTime / duration) < 1; i += Time.deltaTime / duration)
			{
				Alpha = i;
				yield return null;
			}
			//Debug.Log("time end: " + Time.time);

			Alpha = 1;
		}

		/// <summary>
		/// Coroutine to Fade Out with a Callback
		/// </summary>
		/// <param name="Callback">Method or lambda function to call after fade is finished.</param>
		/// <param name="duration">1 by default. How long does it take?</param>
		/// <param name="restart">false by default. If true it starts from alpha = 1.</param>
		/// <param name="fixedDuration">false by default. If false fade's duration is proportional to alpha change, if true it allways take same full duration. Only makes change if restart parameter is false.</param>
		/// <returns></returns>
		private IEnumerator CameraFadeOut(System.Action Callback, float duration, bool restart, bool fixedDuration)
		{
			//Debug.Log("_______________________ init Fade Out");
			//Debug.Log("init time: " + Time.time);

			if (restart) Alpha = 0;
			else if (fixedDuration) duration /= (1 - Alpha);

			for (float i = Alpha; (i + Time.deltaTime / duration) < 1; i += Time.deltaTime / duration)
			{
				Alpha = i;
				yield return null;
			}
			//Debug.Log("time end: " + Time.time);

			Alpha = 1;
			Callback();
		}
	}
}
