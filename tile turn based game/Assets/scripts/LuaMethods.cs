using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;
using System.Reflection;
using System;
using TileGame;
using System.Linq;

[MoonSharpUserData]
public class LuaMethods : MonoBehaviour
{
    private void Awake()
    {
        
    }

    public void AddUnityTypesToScript(Script script)
    {
        UserData.RegisterType<AccelerationEvent>();
        UserData.RegisterType<AnimatorClipInfo>();
        UserData.RegisterType<AnimatorStateInfo>();
        UserData.RegisterType<AnimatorTransitionInfo>();
        UserData.RegisterType<AudioConfiguration>();
        UserData.RegisterType<Color>();
        UserData.RegisterType<Mathf>();
        UserData.RegisterType<Vector2>();
        UserData.RegisterType<Vector3>();
        UserData.RegisterType<GameObject>();
        UserData.RegisterType<AudioClip>();
        UserData.RegisterType<Building>();
        UserData.RegisterType<BuildingController>();
        UserData.RegisterType<ButtonProperties>();
        UserData.RegisterType<Camera>();
        UserData.RegisterType<Canvas>();
        UserData.RegisterType<CharacterController>();
        UserData.RegisterType<DatabaseController>();
        UserData.RegisterType<Debug>();

    }
}
