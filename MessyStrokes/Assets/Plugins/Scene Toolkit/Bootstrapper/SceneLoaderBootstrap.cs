using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CG.SceneToolkit.Bootstrap
{
    internal static class SceneLoaderBootstrap
    {
        internal static class SceneBootStrapper
        {
            [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
            public static void Load() => Object.DontDestroyOnLoad(Object.Instantiate(Resources.Load("Scene Manager")));
        }
    }
}