using System;
using UnityEngine;

namespace UnityGameFramework.Runtime
{
    public abstract class GameFrameworkComponent : MonoBehaviour
    {
        protected virtual void Awake()
        {
            UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
        }
    }
}
