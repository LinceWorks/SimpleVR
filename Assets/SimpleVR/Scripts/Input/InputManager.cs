using UnityEngine;
using Valve.VR;

namespace SimpleVR
{
	/// <summary>
	/// Automatically activates an action set on Start() and deactivates the set on OnDestroy(). Optionally deactivating all other sets as well.
	/// </summary>
	public class InputManager : MonoBehaviour
	{
		SteamVR_Input_Sources sources = SteamVR_Input_Sources.Any;
		public int priority = 0;
		public bool disableAllOtherActionSets = false;

		public bool activateOnStart = true;
		public bool deactivateOnDestroy = true;

		private SteamVR_ActionSet currentActionSet;

		private void Awake()
		{
			SteamVR.Initialize();
			//SteamVR_Input.Initialize();

			if (DataVR.Instance.input.currentActionSets != null && activateOnStart)
			{
				for (int i = 0; i < DataVR.Instance.input.currentActionSets.Length; i++)
				{
					DataVR.Instance.input.currentActionSets[i].Activate(sources, priority, disableAllOtherActionSets);
				}
			}
		}


		protected void OnEnable()
		{
			Input.InitializeBindings();
		}
		protected void OnDisable()
		{
			Input.CloseBindings();
		}

		//This below changed because SteamVR_Input changed (SteamVR.settings was DataVR.Instance.input.settings but with assemblies SteamVR can't have SimpleVR assembly reference because SimpleVR already has SteamVR assembly (can't be circular renferences))

		//		private void OnDestroy()
		//		{
		//			if (currentActionSet != null && deactivateOnDestroy)
		//			{
		//				//Debug.Log(string.Format("[SteamVR] Deactivating {0} action set.", actionSet.fullPath));
		//				currentActionSet.Deactivate();
		//			}
		//		}

		//#if UNITY_2017_1_OR_NEWER
		//		protected void OnEnable()
		//		{
		//			Application.onBeforeRender += OnBeforeRender;
		//			SteamVR_Events.System(EVREventType.VREvent_Quit).Listen(OnQuit);

		//			Input.InitializeBindings();
		//		}
		//		protected void OnDisable()
		//		{
		//			Application.onBeforeRender -= OnBeforeRender;
		//			SteamVR_Events.System(EVREventType.VREvent_Quit).Remove(OnQuit);

		//			Input.CloseBindings();
		//		}
		//		protected void OnBeforeRender()
		//		{
		//			PreCull();
		//		}
		//#else
		//        protected void OnEnable()
		//        {
		//            Camera.onPreCull += OnCameraPreCull;
		//            SteamVR_Events.System(EVREventType.VREvent_Quit).Listen(OnQuit);
		//        }
		//        protected void OnDisable()
		//        {
		//            Camera.onPreCull -= OnCameraPreCull;
		//            SteamVR_Events.System(EVREventType.VREvent_Quit).Remove(OnQuit);
		//        }
		//        protected void OnCameraPreCull(Camera cam)
		//        {
		//            if (!cam.stereoEnabled)
		//                return;

		//            PreCull();
		//        }
		//#endif

		//		protected static int lastFrameCount = -1;
		//		protected void PreCull()
		//		{
		//			// Only update poses on the first camera per frame.
		//			if (Time.frameCount != lastFrameCount)
		//			{
		//				lastFrameCount = Time.frameCount;

		//				SteamVR_Input.OnPreCull();
		//			}
		//		}

		//		protected void FixedUpdate()
		//		{
		//			SteamVR_Input.FixedUpdate();
		//		}

		//		protected void LateUpdate()
		//		{
		//			SteamVR_Input.LateUpdate();
		//		}

		//		protected void Update()
		//		{
		//			SteamVR_Input.Update();
		//		}

		//		protected void OnQuit(VREvent_t vrEvent)
		//		{
		//#if UNITY_EDITOR
		//			UnityEditor.EditorApplication.isPlaying = false;
		//#else
		//		    Application.Quit();
		//#endif
		//		}
	}
}
