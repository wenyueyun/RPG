﻿using System;

namespace GameFramework.Debugger
{
    public interface IDebuggerWindow
    {
        void Initialize(params object[] args);

        void Shutdown();

        void OnEnter();

        void OnLeave();

        void OnUpdate(float elapseSeconds, float realElapseSeconds);

        void OnDraw();
    }
}
