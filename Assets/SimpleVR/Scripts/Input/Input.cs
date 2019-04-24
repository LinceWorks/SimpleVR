using UnityEngine;
using UnityEngine.SpatialTracking;
using Valve.VR;

namespace SimpleVR
{
	public static partial class Input
	{
		#region Pure Unity System
		public class Axis
		{

		}

		public class Button
		{
			private readonly string buttonCode;

			public bool Get { get { return UnityEngine.Input.GetButton(buttonCode); } }
			public bool Down { get { return UnityEngine.Input.GetButtonDown(buttonCode); } }
			public bool Up { get { return UnityEngine.Input.GetButtonUp(buttonCode); } }


			public Button(string buttonCode)
			{
				this.buttonCode = buttonCode;
			}
		}

		public static Button LeftTriggerButton = new Button("LeftTriggerPress");
		public static Button RightTriggerButton = new Button("RightTriggerPress");


		//Controller interactions
		//-----------------------------------------------------------
		private static bool Trigger(this HandVR handVR)
		{
			return handVR.IsLeft ? LeftTriggerButton.Get : RightTriggerButton.Get;
		}

		private static bool TriggerDown(this HandVR handVR)
		{
			return handVR.IsLeft ? LeftTriggerButton.Down : RightTriggerButton.Down;
		}

		private static bool TriggerUp(this HandVR handVR)
		{
			return handVR.IsLeft ? LeftTriggerButton.Up : RightTriggerButton.Up;
		}
		#endregion

		#region Example Actions With Listeners (for Monobehaviours)
		//public SteamVR_Input_Sources sources;
		//public SteamVR_Action_Boolean action;

		//private void OnEnable()
		//{
		//	if (action == null)
		//	{
		//		Debug.LogError("No test action assigned");
		//		return;
		//	}

		//	action.AddOnChangeListener(OnTestActionChange, sources);
		//}

		//private void OnDisable()
		//{
		//	if (action != null)
		//		action.RemoveOnChangeListener(OnTestActionChange, sources);
		//}

		//private void OnTestActionChange(SteamVR_Action_In actionIn)
		//{
		//	if (action.GetStateDown(sources))
		//	{
		//		Debug.Log("pressed down");
		//	}
		//}
		#endregion

		public static SteamVR_Input_Sources GetSteamVRInputSource(HandVR handVR)
		{
			return handVR.IsLeft ? SteamVR_Input_Sources.LeftHand : SteamVR_Input_Sources.RightHand;
		}

		//Ingame interactions
		//------------------------------------------------------------

		//Variables and Initializing
		public delegate void onBoolDelegate();
		static partial void SpecificInitializeBidnings();   //Specific implementation of this partial class for the game
		static partial void SpecificCloseBindings();    //Specific implementation of this partial class for the game

		public static void InitializeBindings()
		{
			SteamVR_Actions.SimpleVR_Ingame.TurnAround.onStateDown += TurnAroundDown;
			SteamVR_Actions.SimpleVR_Ingame.TurnLeft.onStateDown += TurnLeftDown;
			SteamVR_Actions.SimpleVR_Ingame.TurnRight.onStateDown += TurnRightDown;

			SpecificInitializeBidnings();
		}

		public static void CloseBindings()
		{
			SteamVR_Actions.SimpleVR_Ingame.TurnAround.onStateDown -= TurnAroundDown;
			SteamVR_Actions.SimpleVR_Ingame.TurnLeft.onStateDown -= TurnLeftDown;
			SteamVR_Actions.SimpleVR_Ingame.TurnRight.onStateDown -= TurnRightDown;

			SpecificCloseBindings();
		}

		//------------------------------------------------------------------------------------

		public static event onBoolDelegate OnTurnAroundDown;
		public static event onBoolDelegate OnTurnLeftDown;
		public static event onBoolDelegate OnTurnRightDown;

		private static void TurnAroundDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
		{
			OnTurnAroundDown?.Invoke();
		}

		private static void TurnLeftDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
		{
			OnTurnLeftDown?.Invoke();
		}

		private static void TurnRightDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
		{
			OnTurnRightDown?.Invoke();
		}

		//------------------------------------------------------------------------------------

		//Grab
		public static bool Grab()
		{
			return SteamVR_Actions.SimpleVR_Ingame.Grab.GetState(SteamVR_Input_Sources.Any);
		}
		public static bool Grab(this HandVR handVR)
		{
			//return handVR.Trigger();	//Pure Unity System
			return SteamVR_Actions.SimpleVR_Ingame.Grab.GetState(GetSteamVRInputSource(handVR));
		}

		public static bool GrabDown()
		{
			return SteamVR_Actions.SimpleVR_Ingame.Grab.GetStateDown(SteamVR_Input_Sources.Any);
		}
		public static bool GrabDown(this HandVR handVR)
		{
			//return handVR.TriggerDown();	//Pure Unity System
			return SteamVR_Actions.SimpleVR_Ingame.Grab.GetStateDown(GetSteamVRInputSource(handVR));
		}

		public static bool GrabUp()
		{
			return SteamVR_Actions.SimpleVR_Ingame.Grab.GetStateUp(SteamVR_Input_Sources.Any);
		}
		public static bool GrabUp(this HandVR handVR)
		{
			//return handVR.TriggerUp();	//Pure Unity System
			return SteamVR_Actions.SimpleVR_Ingame.Grab.GetStateUp(GetSteamVRInputSource(handVR));
		}

		//TurnAround
		public static bool TurnAround()
		{
			return SteamVR_Actions.SimpleVR_Ingame.TurnAround.GetState(SteamVR_Input_Sources.Any);
		}
		public static bool TurnAround(this HandVR handVR)
		{
			return SteamVR_Actions.SimpleVR_Ingame.TurnAround.GetState(GetSteamVRInputSource(handVR));
		}

		public static bool TurnAroundDown()
		{
			return SteamVR_Actions.SimpleVR_Ingame.TurnAround.GetStateDown(SteamVR_Input_Sources.Any);
		}
		public static bool TurnAroundDown(this HandVR handVR)
		{
			return SteamVR_Actions.SimpleVR_Ingame.TurnAround.GetStateDown(GetSteamVRInputSource(handVR));
		}

		public static bool TurnAroundUp()
		{
			return SteamVR_Actions.SimpleVR_Ingame.TurnAround.GetStateUp(SteamVR_Input_Sources.Any);
		}
		public static bool TurnAroundUp(this HandVR handVR)
		{
			return SteamVR_Actions.SimpleVR_Ingame.TurnAround.GetStateUp(GetSteamVRInputSource(handVR));
		}

		//TurnLeft
		public static bool TurnLeft()
		{
			return SteamVR_Actions.SimpleVR_Ingame.TurnLeft.GetState(SteamVR_Input_Sources.Any);
		}
		public static bool TurnLeft(this HandVR handVR)
		{
			return SteamVR_Actions.SimpleVR_Ingame.TurnLeft.GetState(GetSteamVRInputSource(handVR));
		}

		public static bool TurnLeftDown()
		{
			return SteamVR_Actions.SimpleVR_Ingame.TurnLeft.GetStateDown(SteamVR_Input_Sources.Any);
		}
		public static bool TurnLeftDown(this HandVR handVR)
		{
			return SteamVR_Actions.SimpleVR_Ingame.TurnLeft.GetStateDown(GetSteamVRInputSource(handVR));
		}

		public static bool TurnLeftUp()
		{
			return SteamVR_Actions.SimpleVR_Ingame.TurnLeft.GetStateUp(SteamVR_Input_Sources.Any);
		}
		public static bool TurnLeftUp(this HandVR handVR)
		{
			return SteamVR_Actions.SimpleVR_Ingame.TurnLeft.GetStateUp(GetSteamVRInputSource(handVR));
		}

		//TurnRight
		public static bool TurnRight()
		{
			return SteamVR_Actions.SimpleVR_Ingame.TurnRight.GetState(SteamVR_Input_Sources.Any);
		}
		public static bool TurnRight(this HandVR handVR)
		{
			return SteamVR_Actions.SimpleVR_Ingame.TurnRight.GetState(GetSteamVRInputSource(handVR));
		}

		public static bool TurnRightDown()
		{
			return SteamVR_Actions.SimpleVR_Ingame.TurnRight.GetStateDown(SteamVR_Input_Sources.Any);
		}
		public static bool TurnRightDown(this HandVR handVR)
		{
			return SteamVR_Actions.SimpleVR_Ingame.TurnRight.GetStateDown(GetSteamVRInputSource(handVR));
		}

		public static bool TurnRightUp()
		{
			return SteamVR_Actions.SimpleVR_Ingame.TurnRight.GetStateUp(SteamVR_Input_Sources.Any);
		}
		public static bool TurnRightUp(this HandVR handVR)
		{
			return SteamVR_Actions.SimpleVR_Ingame.TurnRight.GetStateUp(GetSteamVRInputSource(handVR));
		}

		//Move
		public static Vector2 Move()
		{
			return SteamVR_Actions.SimpleVR_Ingame.Move.GetAxis(SteamVR_Input_Sources.Any);
		}
		public static Vector2 Move(this HandVR handVR)
		{
			return SteamVR_Actions.SimpleVR_Ingame.Move.GetAxis(GetSteamVRInputSource(handVR));
		}

		//MENU INTERACTIONS
		//---------------------------------------------
		
		//Menu
		public static bool Menu()
		{
			return SteamVR_Actions.SimpleVR_Menu.Menu.GetState(SteamVR_Input_Sources.Any);
		}
		public static bool Menu(this HandVR handVR)
		{
			return SteamVR_Actions.SimpleVR_Menu.Menu.GetState(GetSteamVRInputSource(handVR));
		}

		public static bool MenuDown()
		{
			return SteamVR_Actions.SimpleVR_Menu.Menu.GetStateDown(SteamVR_Input_Sources.Any);
		}
		public static bool MenuDown(this HandVR handVR)
		{
			return SteamVR_Actions.SimpleVR_Menu.Menu.GetStateDown(GetSteamVRInputSource(handVR));
		}

		public static bool MenuUp()
		{
			return SteamVR_Actions.SimpleVR_Menu.Menu.GetStateUp(SteamVR_Input_Sources.Any);
		}
		public static bool MenuUp(this HandVR handVR)
		{
			return SteamVR_Actions.SimpleVR_Menu.Menu.GetStateUp(GetSteamVRInputSource(handVR));
		}

		//Enter
		public static bool Enter()
		{
			return SteamVR_Actions.SimpleVR_Menu.Enter.GetState(SteamVR_Input_Sources.Any);
		}
		public static bool Enter(this HandVR handVR)
		{
			return SteamVR_Actions.SimpleVR_Menu.Enter.GetState(GetSteamVRInputSource(handVR));
		}

		public static bool EnterDown()
		{
			return SteamVR_Actions.SimpleVR_Menu.Enter.GetStateDown(SteamVR_Input_Sources.Any);
		}
		public static bool EnterDown(this HandVR handVR)
		{
			return SteamVR_Actions.SimpleVR_Menu.Enter.GetStateDown(GetSteamVRInputSource(handVR));
		}

		public static bool EnterUp()
		{
			return SteamVR_Actions.SimpleVR_Menu.Enter.GetStateUp(SteamVR_Input_Sources.Any);
		}
		public static bool EnterUp(this HandVR handVR)
		{
			return SteamVR_Actions.SimpleVR_Menu.Enter.GetStateUp(GetSteamVRInputSource(handVR));
		}

		//Back
		public static bool Back()
		{
			return SteamVR_Actions.SimpleVR_Menu.Back.GetState(SteamVR_Input_Sources.Any);
		}
		public static bool Back(this HandVR handVR)
		{
			return SteamVR_Actions.SimpleVR_Menu.Back.GetState(GetSteamVRInputSource(handVR));
		}

		public static bool BackDown()
		{
			return SteamVR_Actions.SimpleVR_Menu.Back.GetStateDown(SteamVR_Input_Sources.Any);
		}
		public static bool BackDown(this HandVR handVR)
		{
			return SteamVR_Actions.SimpleVR_Menu.Back.GetStateDown(GetSteamVRInputSource(handVR));
		}

		public static bool BackUp()
		{
			return SteamVR_Actions.SimpleVR_Menu.Back.GetStateUp(SteamVR_Input_Sources.Any);
		}
		public static bool BackUp(this HandVR handVR)
		{
			return SteamVR_Actions.SimpleVR_Menu.Back.GetStateUp(GetSteamVRInputSource(handVR));
		}
	}

	public static partial class Output
	{
		/// <summary>
		/// Trigger the haptics at a certain time for a certain length
		/// </summary>
		/// <param name="handVR">The HandVR, which device you would like to execute the haptic action.</param>
		/// <param name="secondsFromNow">How long from the current time to execute the action (in seconds - can be 0)</param>
		/// <param name="durationSeconds">How long the haptic action should last (in seconds)</param>
		/// <param name="frequency">How often the haptic motor should bounce (0 - 320 in hz. The lower end being more useful)</param>
		/// <param name="amplitude">How intense the haptic action should be (0 - 1)</param>
		public static void Haptic(this HandVR handVR, float secondsFromNow, float durationSeconds, float frequency, float amplitude)
		{
			Haptic(secondsFromNow, durationSeconds, frequency, amplitude, Input.GetSteamVRInputSource(handVR));
		}

		/// <summary>
		/// Trigger the haptics at a certain time for a certain length
		/// </summary>
		/// <param name="secondsFromNow">How long from the current time to execute the action (in seconds - can be 0)</param>
		/// <param name="durationSeconds">How long the haptic action should last (in seconds)</param>
		/// <param name="frequency">How often the haptic motor should bounce (0 - 320 in hz. The lower end being more useful)</param>
		/// <param name="amplitude">How intense the haptic action should be (0 - 1)</param>
		/// <param name="inputSource">The device you would like to execute the haptic action. Any if the action is not device specific.</param>
		public static void Haptic(float secondsFromNow, float durationSeconds, float frequency, float amplitude, SteamVR_Input_Sources inputSource)
		{
			SteamVR_Actions.SimpleVR_Ingame.Haptic.Execute(secondsFromNow, durationSeconds, frequency, amplitude, inputSource);
		}
	}
}