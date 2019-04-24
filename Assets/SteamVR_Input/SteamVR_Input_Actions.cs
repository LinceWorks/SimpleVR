//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Valve.VR
{
    using System;
    using UnityEngine;
    
    
    public partial class SteamVR_Actions
    {
        
        private static SteamVR_Action_Vector2 p_simpleVR_Ingame_Move;
        
        private static SteamVR_Action_Boolean p_simpleVR_Ingame_Grab;
        
        private static SteamVR_Action_Boolean p_simpleVR_Ingame_TurnAround;
        
        private static SteamVR_Action_Skeleton p_simpleVR_Ingame_SkeletonLeftHand;
        
        private static SteamVR_Action_Skeleton p_simpleVR_Ingame_SkeletonRightHand;
        
        private static SteamVR_Action_Boolean p_simpleVR_Ingame_TurnLeft;
        
        private static SteamVR_Action_Boolean p_simpleVR_Ingame_TurnRight;
        
        private static SteamVR_Action_Vibration p_simpleVR_Ingame_Haptic;
        
        private static SteamVR_Action_Boolean p_simpleVR_Menu_Menu;
        
        private static SteamVR_Action_Boolean p_simpleVR_Menu_Enter;
        
        private static SteamVR_Action_Boolean p_simpleVR_Menu_Back;
        
        public static SteamVR_Action_Vector2 simpleVR_Ingame_Move
        {
            get
            {
                return SteamVR_Actions.p_simpleVR_Ingame_Move.GetCopy<SteamVR_Action_Vector2>();
            }
        }
        
        public static SteamVR_Action_Boolean simpleVR_Ingame_Grab
        {
            get
            {
                return SteamVR_Actions.p_simpleVR_Ingame_Grab.GetCopy<SteamVR_Action_Boolean>();
            }
        }
        
        public static SteamVR_Action_Boolean simpleVR_Ingame_TurnAround
        {
            get
            {
                return SteamVR_Actions.p_simpleVR_Ingame_TurnAround.GetCopy<SteamVR_Action_Boolean>();
            }
        }
        
        public static SteamVR_Action_Skeleton simpleVR_Ingame_SkeletonLeftHand
        {
            get
            {
                return SteamVR_Actions.p_simpleVR_Ingame_SkeletonLeftHand.GetCopy<SteamVR_Action_Skeleton>();
            }
        }
        
        public static SteamVR_Action_Skeleton simpleVR_Ingame_SkeletonRightHand
        {
            get
            {
                return SteamVR_Actions.p_simpleVR_Ingame_SkeletonRightHand.GetCopy<SteamVR_Action_Skeleton>();
            }
        }
        
        public static SteamVR_Action_Boolean simpleVR_Ingame_TurnLeft
        {
            get
            {
                return SteamVR_Actions.p_simpleVR_Ingame_TurnLeft.GetCopy<SteamVR_Action_Boolean>();
            }
        }
        
        public static SteamVR_Action_Boolean simpleVR_Ingame_TurnRight
        {
            get
            {
                return SteamVR_Actions.p_simpleVR_Ingame_TurnRight.GetCopy<SteamVR_Action_Boolean>();
            }
        }
        
        public static SteamVR_Action_Vibration simpleVR_Ingame_Haptic
        {
            get
            {
                return SteamVR_Actions.p_simpleVR_Ingame_Haptic.GetCopy<SteamVR_Action_Vibration>();
            }
        }
        
        public static SteamVR_Action_Boolean simpleVR_Menu_Menu
        {
            get
            {
                return SteamVR_Actions.p_simpleVR_Menu_Menu.GetCopy<SteamVR_Action_Boolean>();
            }
        }
        
        public static SteamVR_Action_Boolean simpleVR_Menu_Enter
        {
            get
            {
                return SteamVR_Actions.p_simpleVR_Menu_Enter.GetCopy<SteamVR_Action_Boolean>();
            }
        }
        
        public static SteamVR_Action_Boolean simpleVR_Menu_Back
        {
            get
            {
                return SteamVR_Actions.p_simpleVR_Menu_Back.GetCopy<SteamVR_Action_Boolean>();
            }
        }
        
        private static void InitializeActionArrays()
        {
            Valve.VR.SteamVR_Input.actions = new Valve.VR.SteamVR_Action[] {
                    SteamVR_Actions.simpleVR_Ingame_Move,
                    SteamVR_Actions.simpleVR_Ingame_Grab,
                    SteamVR_Actions.simpleVR_Ingame_TurnAround,
                    SteamVR_Actions.simpleVR_Ingame_SkeletonLeftHand,
                    SteamVR_Actions.simpleVR_Ingame_SkeletonRightHand,
                    SteamVR_Actions.simpleVR_Ingame_TurnLeft,
                    SteamVR_Actions.simpleVR_Ingame_TurnRight,
                    SteamVR_Actions.simpleVR_Ingame_Haptic,
                    SteamVR_Actions.simpleVR_Menu_Menu,
                    SteamVR_Actions.simpleVR_Menu_Enter,
                    SteamVR_Actions.simpleVR_Menu_Back};
            Valve.VR.SteamVR_Input.actionsIn = new Valve.VR.ISteamVR_Action_In[] {
                    SteamVR_Actions.simpleVR_Ingame_Move,
                    SteamVR_Actions.simpleVR_Ingame_Grab,
                    SteamVR_Actions.simpleVR_Ingame_TurnAround,
                    SteamVR_Actions.simpleVR_Ingame_SkeletonLeftHand,
                    SteamVR_Actions.simpleVR_Ingame_SkeletonRightHand,
                    SteamVR_Actions.simpleVR_Ingame_TurnLeft,
                    SteamVR_Actions.simpleVR_Ingame_TurnRight,
                    SteamVR_Actions.simpleVR_Menu_Menu,
                    SteamVR_Actions.simpleVR_Menu_Enter,
                    SteamVR_Actions.simpleVR_Menu_Back};
            Valve.VR.SteamVR_Input.actionsOut = new Valve.VR.ISteamVR_Action_Out[] {
                    SteamVR_Actions.simpleVR_Ingame_Haptic};
            Valve.VR.SteamVR_Input.actionsVibration = new Valve.VR.SteamVR_Action_Vibration[] {
                    SteamVR_Actions.simpleVR_Ingame_Haptic};
            Valve.VR.SteamVR_Input.actionsPose = new Valve.VR.SteamVR_Action_Pose[0];
            Valve.VR.SteamVR_Input.actionsBoolean = new Valve.VR.SteamVR_Action_Boolean[] {
                    SteamVR_Actions.simpleVR_Ingame_Grab,
                    SteamVR_Actions.simpleVR_Ingame_TurnAround,
                    SteamVR_Actions.simpleVR_Ingame_TurnLeft,
                    SteamVR_Actions.simpleVR_Ingame_TurnRight,
                    SteamVR_Actions.simpleVR_Menu_Menu,
                    SteamVR_Actions.simpleVR_Menu_Enter,
                    SteamVR_Actions.simpleVR_Menu_Back};
            Valve.VR.SteamVR_Input.actionsSingle = new Valve.VR.SteamVR_Action_Single[0];
            Valve.VR.SteamVR_Input.actionsVector2 = new Valve.VR.SteamVR_Action_Vector2[] {
                    SteamVR_Actions.simpleVR_Ingame_Move};
            Valve.VR.SteamVR_Input.actionsVector3 = new Valve.VR.SteamVR_Action_Vector3[0];
            Valve.VR.SteamVR_Input.actionsSkeleton = new Valve.VR.SteamVR_Action_Skeleton[] {
                    SteamVR_Actions.simpleVR_Ingame_SkeletonLeftHand,
                    SteamVR_Actions.simpleVR_Ingame_SkeletonRightHand};
            Valve.VR.SteamVR_Input.actionsNonPoseNonSkeletonIn = new Valve.VR.ISteamVR_Action_In[] {
                    SteamVR_Actions.simpleVR_Ingame_Move,
                    SteamVR_Actions.simpleVR_Ingame_Grab,
                    SteamVR_Actions.simpleVR_Ingame_TurnAround,
                    SteamVR_Actions.simpleVR_Ingame_TurnLeft,
                    SteamVR_Actions.simpleVR_Ingame_TurnRight,
                    SteamVR_Actions.simpleVR_Menu_Menu,
                    SteamVR_Actions.simpleVR_Menu_Enter,
                    SteamVR_Actions.simpleVR_Menu_Back};
        }
        
        private static void PreInitActions()
        {
            SteamVR_Actions.p_simpleVR_Ingame_Move = ((SteamVR_Action_Vector2)(SteamVR_Action.Create<SteamVR_Action_Vector2>("/actions/SimpleVR_Ingame/in/Move")));
            SteamVR_Actions.p_simpleVR_Ingame_Grab = ((SteamVR_Action_Boolean)(SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/SimpleVR_Ingame/in/Grab")));
            SteamVR_Actions.p_simpleVR_Ingame_TurnAround = ((SteamVR_Action_Boolean)(SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/SimpleVR_Ingame/in/TurnAround")));
            SteamVR_Actions.p_simpleVR_Ingame_SkeletonLeftHand = ((SteamVR_Action_Skeleton)(SteamVR_Action.Create<SteamVR_Action_Skeleton>("/actions/SimpleVR_Ingame/in/SkeletonLeftHand")));
            SteamVR_Actions.p_simpleVR_Ingame_SkeletonRightHand = ((SteamVR_Action_Skeleton)(SteamVR_Action.Create<SteamVR_Action_Skeleton>("/actions/SimpleVR_Ingame/in/SkeletonRightHand")));
            SteamVR_Actions.p_simpleVR_Ingame_TurnLeft = ((SteamVR_Action_Boolean)(SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/SimpleVR_Ingame/in/TurnLeft")));
            SteamVR_Actions.p_simpleVR_Ingame_TurnRight = ((SteamVR_Action_Boolean)(SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/SimpleVR_Ingame/in/TurnRight")));
            SteamVR_Actions.p_simpleVR_Ingame_Haptic = ((SteamVR_Action_Vibration)(SteamVR_Action.Create<SteamVR_Action_Vibration>("/actions/SimpleVR_Ingame/out/Haptic")));
            SteamVR_Actions.p_simpleVR_Menu_Menu = ((SteamVR_Action_Boolean)(SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/SimpleVR_Menu/in/Menu")));
            SteamVR_Actions.p_simpleVR_Menu_Enter = ((SteamVR_Action_Boolean)(SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/SimpleVR_Menu/in/Enter")));
            SteamVR_Actions.p_simpleVR_Menu_Back = ((SteamVR_Action_Boolean)(SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/SimpleVR_Menu/in/Back")));
        }
    }
}
