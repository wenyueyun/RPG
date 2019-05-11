using GameFramework;
using GameFramework.Debugger;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;

namespace UnityGameFramework.Runtime
{
    [AddComponentMenu("Game Framework/Debugger"), DisallowMultipleComponent]
    public sealed class DebuggerComponent : GameFrameworkComponent
    {
        [Serializable]
        private sealed class ConsoleWindow : IDebuggerWindow
        {
            private sealed class LogNode
            {
                private readonly DateTime m_LogTime;

                private readonly LogType m_LogType;

                private readonly string m_LogMessage;

                private readonly string m_StackTrack;

                public DateTime LogTime
                {
                    get
                    {
                        return this.m_LogTime;
                    }
                }

                public LogType LogType
                {
                    get
                    {
                        return this.m_LogType;
                    }
                }

                public string LogMessage
                {
                    get
                    {
                        return this.m_LogMessage;
                    }
                }

                public string StackTrack
                {
                    get
                    {
                        return this.m_StackTrack;
                    }
                }

                public LogNode(LogType logType, string logMessage, string stackTrack)
                {
                    this.m_LogTime = DateTime.Now;
                    this.m_LogType = logType;
                    this.m_LogMessage = logMessage;
                    this.m_StackTrack = stackTrack;
                }
            }

            private LinkedList<DebuggerComponent.ConsoleWindow.LogNode> m_Logs = new LinkedList<DebuggerComponent.ConsoleWindow.LogNode>();

            private Vector2 m_LogScrollPosition = Vector2.zero;

            private Vector2 m_StackScrollPosition = Vector2.zero;

            private int m_InfoCount;

            private int m_WarningCount;

            private int m_ErrorCount;

            private int m_FatalCount;

            private LinkedListNode<DebuggerComponent.ConsoleWindow.LogNode> m_SelectedNode;

            [SerializeField]
            private bool m_LockScroll = true;

            [SerializeField]
            private int m_MaxLine = 300;

            [SerializeField]
            private string m_DateTimeFormat = "[HH:mm:ss.fff] ";

            [SerializeField]
            private bool m_InfoFilter = true;

            [SerializeField]
            private bool m_WarningFilter = true;

            [SerializeField]
            private bool m_ErrorFilter = true;

            [SerializeField]
            private bool m_FatalFilter = true;

            [SerializeField]
            private Color32 m_InfoColor = Color.white;

            [SerializeField]
            private Color32 m_WarningColor = Color.yellow;

            [SerializeField]
            private Color32 m_ErrorColor = Color.red;

            [SerializeField]
            private Color32 m_FatalColor = new Color(0.7f, 0.2f, 0.2f);

            public bool LockScroll
            {
                get
                {
                    return this.m_LockScroll;
                }
                set
                {
                    this.m_LockScroll = value;
                }
            }

            public int MaxLine
            {
                get
                {
                    return this.m_MaxLine;
                }
                set
                {
                    this.m_MaxLine = value;
                }
            }

            public string DateTimeFormat
            {
                get
                {
                    return this.m_DateTimeFormat;
                }
                set
                {
                    this.m_DateTimeFormat = (value ?? string.Empty);
                }
            }

            public bool InfoFilter
            {
                get
                {
                    return this.m_InfoFilter;
                }
                set
                {
                    this.m_InfoFilter = value;
                }
            }

            public bool WarningFilter
            {
                get
                {
                    return this.m_WarningFilter;
                }
                set
                {
                    this.m_WarningFilter = value;
                }
            }

            public bool ErrorFilter
            {
                get
                {
                    return this.m_ErrorFilter;
                }
                set
                {
                    this.m_ErrorFilter = value;
                }
            }

            public bool FatalFilter
            {
                get
                {
                    return this.m_FatalFilter;
                }
                set
                {
                    this.m_FatalFilter = value;
                }
            }

            public int InfoCount
            {
                get
                {
                    return this.m_InfoCount;
                }
            }

            public int WarningCount
            {
                get
                {
                    return this.m_WarningCount;
                }
            }

            public int ErrorCount
            {
                get
                {
                    return this.m_ErrorCount;
                }
            }

            public int FatalCount
            {
                get
                {
                    return this.m_FatalCount;
                }
            }

            public Color32 InfoColor
            {
                get
                {
                    return this.m_InfoColor;
                }
                set
                {
                    this.m_InfoColor = value;
                }
            }

            public Color32 WarningColor
            {
                get
                {
                    return this.m_WarningColor;
                }
                set
                {
                    this.m_WarningColor = value;
                }
            }

            public Color32 ErrorColor
            {
                get
                {
                    return this.m_ErrorColor;
                }
                set
                {
                    this.m_ErrorColor = value;
                }
            }

            public Color32 FatalColor
            {
                get
                {
                    return this.m_FatalColor;
                }
                set
                {
                    this.m_FatalColor = value;
                }
            }

            public void Initialize(params object[] args)
            {
                Application.logMessageReceived += new Application.LogCallback(this.OnLogMessageReceived);
            }

            public void Shutdown()
            {
                Application.logMessageReceived -= new Application.LogCallback(this.OnLogMessageReceived);
                this.Clear();
            }

            public void OnEnter()
            {
            }

            public void OnLeave()
            {
            }

            public void OnUpdate(float elapseSeconds, float realElapseSeconds)
            {
            }

            public void OnDraw()
            {
                this.RefreshCount();
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                if (GUILayout.Button("Clear All", new GUILayoutOption[]
                {
                    GUILayout.Width(100f)
                }))
                {
                    this.Clear();
                }
                this.m_LockScroll = GUILayout.Toggle(this.m_LockScroll, "Lock Scroll", "button", new GUILayoutOption[]
                {
                    GUILayout.Width(90f)
                });
                GUILayout.FlexibleSpace();
                this.m_InfoFilter = GUILayout.Toggle(this.m_InfoFilter, string.Format("Info ({0})", this.m_InfoCount.ToString()), new GUILayoutOption[]
                {
                    GUILayout.Width(90f)
                });
                this.m_WarningFilter = GUILayout.Toggle(this.m_WarningFilter, string.Format("Warning ({0})", this.m_WarningCount.ToString()), new GUILayoutOption[]
                {
                    GUILayout.Width(90f)
                });
                this.m_ErrorFilter = GUILayout.Toggle(this.m_ErrorFilter, string.Format("Error ({0})", this.m_ErrorCount.ToString()), new GUILayoutOption[]
                {
                    GUILayout.Width(90f)
                });
                this.m_FatalFilter = GUILayout.Toggle(this.m_FatalFilter, string.Format("Fatal ({0})", this.m_FatalCount.ToString()), new GUILayoutOption[]
                {
                    GUILayout.Width(90f)
                });
                GUILayout.EndHorizontal();
                GUILayout.BeginVertical("box", new GUILayoutOption[0]);
                if (this.m_LockScroll)
                {
                    this.m_LogScrollPosition.y = 3.40282347E+38f;
                }
                this.m_LogScrollPosition = GUILayout.BeginScrollView(this.m_LogScrollPosition, new GUILayoutOption[0]);
                bool flag = false;
                LinkedListNode<DebuggerComponent.ConsoleWindow.LogNode> linkedListNode = this.m_Logs.First;
                while (linkedListNode != null)
                {
                    switch (linkedListNode.Value.LogType)
                    {
                        case LogType.Error:
                            if (this.m_ErrorFilter)
                            {
                                goto IL_24B;
                            }
                            break;
                        case LogType.Assert:
                            goto IL_24B;
                        case LogType.Warning:
                            if (this.m_WarningFilter)
                            {
                                goto IL_24B;
                            }
                            break;
                        case LogType.Log:
                            if (this.m_InfoFilter)
                            {
                                goto IL_24B;
                            }
                            break;
                        case LogType.Exception:
                            if (this.m_FatalFilter)
                            {
                                goto IL_24B;
                            }
                            break;
                        default:
                            goto IL_24B;
                    }
                IL_290:
                    linkedListNode = linkedListNode.Next;
                    continue;
                IL_24B:
                    if (!GUILayout.Toggle(this.m_SelectedNode == linkedListNode, this.GetLogString(linkedListNode.Value), new GUILayoutOption[0]))
                    {
                        goto IL_290;
                    }
                    flag = true;
                    if (this.m_SelectedNode != linkedListNode)
                    {
                        this.m_SelectedNode = linkedListNode;
                        this.m_StackScrollPosition = Vector2.zero;
                        goto IL_290;
                    }
                    goto IL_290;
                }
                if (!flag)
                {
                    this.m_SelectedNode = null;
                }
                GUILayout.EndScrollView();
                GUILayout.EndVertical();
                GUILayout.BeginVertical("box", new GUILayoutOption[0]);
                this.m_StackScrollPosition = GUILayout.BeginScrollView(this.m_StackScrollPosition, new GUILayoutOption[]
                {
                    GUILayout.Height(100f)
                });
                if (this.m_SelectedNode != null)
                {
                    GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    Color32 logStringColor = this.GetLogStringColor(this.m_SelectedNode.Value.LogType);
                    GUILayout.Label(string.Format("<color=#{0}{1}{2}{3}><b>{4}</b></color>", new object[]
                    {
                        logStringColor.r.ToString("x2"),
                        logStringColor.g.ToString("x2"),
                        logStringColor.b.ToString("x2"),
                        logStringColor.a.ToString("x2"),
                        this.m_SelectedNode.Value.LogMessage
                    }), new GUILayoutOption[0]);
                    if (GUILayout.Button("COPY", new GUILayoutOption[]
                    {
                        GUILayout.Width(60f),
                        GUILayout.Height(30f)
                    }))
                    {
                        TextEditor textEditor = new TextEditor();
                        textEditor.text = string.Format("{0}\n\n{1}", this.m_SelectedNode.Value.LogMessage, this.m_SelectedNode.Value.StackTrack);
                        textEditor.OnFocus();
                        textEditor.Copy();
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Label(this.m_SelectedNode.Value.StackTrack, new GUILayoutOption[0]);
                }
                GUILayout.EndScrollView();
                GUILayout.EndVertical();
            }

            private void Clear()
            {
                this.m_Logs.Clear();
            }

            public void RefreshCount()
            {
                this.m_InfoCount = 0;
                this.m_WarningCount = 0;
                this.m_ErrorCount = 0;
                this.m_FatalCount = 0;
                for (LinkedListNode<DebuggerComponent.ConsoleWindow.LogNode> linkedListNode = this.m_Logs.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
                {
                    switch (linkedListNode.Value.LogType)
                    {
                        case LogType.Error:
                            this.m_ErrorCount++;
                            break;
                        case LogType.Warning:
                            this.m_WarningCount++;
                            break;
                        case LogType.Log:
                            this.m_InfoCount++;
                            break;
                        case LogType.Exception:
                            this.m_FatalCount++;
                            break;
                    }
                }
            }

            private void OnLogMessageReceived(string logMessage, string stackTrace, LogType logType)
            {
                if (logType == LogType.Assert)
                {
                    logType = LogType.Error;
                }
                this.m_Logs.AddLast(new DebuggerComponent.ConsoleWindow.LogNode(logType, logMessage, stackTrace));
                while (this.m_Logs.Count > this.m_MaxLine)
                {
                    this.m_Logs.RemoveFirst();
                }
            }

            private string GetLogString(DebuggerComponent.ConsoleWindow.LogNode logNode)
            {
                Color32 logStringColor = this.GetLogStringColor(logNode.LogType);
                return string.Format("<color=#{0}{1}{2}{3}>{4}{5}</color>", new object[]
                {
                    logStringColor.r.ToString("x2"),
                    logStringColor.g.ToString("x2"),
                    logStringColor.b.ToString("x2"),
                    logStringColor.a.ToString("x2"),
                    logNode.LogTime.ToString(this.m_DateTimeFormat),
                    logNode.LogMessage
                });
            }

            internal Color32 GetLogStringColor(LogType logType)
            {
                Color32 result = Color.white;
                switch (logType)
                {
                    case LogType.Error:
                        result = this.m_ErrorColor;
                        break;
                    case LogType.Warning:
                        result = this.m_WarningColor;
                        break;
                    case LogType.Log:
                        result = this.m_InfoColor;
                        break;
                    case LogType.Exception:
                        result = this.m_FatalColor;
                        break;
                }
                return result;
            }
        }

        private sealed class FpsCounter : IRecordMike
        {
            private const string MIKE_TAG = "FPS";

            private float m_UpdateInterval;

            private float m_CurrentFps;

            private int m_Frames;

            private float m_Accumulator;

            private float m_TimeLeft;

            public float UpdateInterval
            {
                get
                {
                    return this.m_UpdateInterval;
                }
                set
                {
                    if (value <= 0f)
                    {
                        Log.Error("Update interval is invalid.");
                        return;
                    }
                    this.m_UpdateInterval = value;
                    this.Reset();
                }
            }

            public float CurrentFps
            {
                get
                {
                    return this.m_CurrentFps;
                }
            }

            public FpsCounter(float updateInterval)
            {
                if (updateInterval <= 0f)
                {
                    Log.Error("Update interval is invalid.");
                    return;
                }
                this.m_UpdateInterval = updateInterval;
                this.Reset();
            }

            public string GetSample()
            {
                return this.m_CurrentFps.ToString();
            }

            public string GetTag()
            {
                return "FPS";
            }

            public void Update(float elapseSeconds, float realElapseSeconds)
            {
                this.m_Frames++;
                this.m_Accumulator += realElapseSeconds;
                this.m_TimeLeft -= realElapseSeconds;
                if (this.m_TimeLeft <= 0f)
                {
                    this.m_CurrentFps = ((this.m_Accumulator <= 0f) ? 0f : ((float)this.m_Frames / this.m_Accumulator));
                    this.m_Frames = 0;
                    this.m_Accumulator = 0f;
                    this.m_TimeLeft += this.m_UpdateInterval;
                }
            }

            private void Reset()
            {
                this.m_CurrentFps = 0f;
                this.m_Frames = 0;
                this.m_Accumulator = 0f;
                this.m_TimeLeft = 0f;
            }
        }

        private sealed class GraphicsInformationWindow : DebuggerComponent.ScrollableDebuggerWindowBase
        {
            protected override void OnDrawScrollableWindow()
            {
                GUILayout.Label("<b>Graphics Information</b>", new GUILayoutOption[0]);
                GUILayout.BeginVertical("box", new GUILayoutOption[0]);
                base.DrawItem("Device ID:", SystemInfo.graphicsDeviceID.ToString());
                base.DrawItem("Device Name:", SystemInfo.graphicsDeviceName);
                base.DrawItem("Device Vendor ID:", SystemInfo.graphicsDeviceVendorID.ToString());
                base.DrawItem("Device Vendor:", SystemInfo.graphicsDeviceVendor);
                base.DrawItem("Device Type:", SystemInfo.graphicsDeviceType.ToString());
                base.DrawItem("Device Version:", SystemInfo.graphicsDeviceVersion);
                base.DrawItem("Memory Size:", string.Format("{0} MB", SystemInfo.graphicsMemorySize.ToString()));
                base.DrawItem("Multi Threaded:", SystemInfo.graphicsMultiThreaded.ToString());
                base.DrawItem("Shader Level:", this.GetShaderLevelString(SystemInfo.graphicsShaderLevel));
                base.DrawItem("NPOT Support:", SystemInfo.npotSupport.ToString());
                base.DrawItem("Max Texture Size:", SystemInfo.maxTextureSize.ToString());
                // 不兼容
                // base.DrawItem("Max Cubemap Size:", SystemInfo.maxCubemapSize.ToString());
                base.DrawItem("Copy Texture Support:", SystemInfo.copyTextureSupport.ToString());
                base.DrawItem("Supported Render Target Count:", SystemInfo.supportedRenderTargetCount.ToString());
                base.DrawItem("Supports Sparse Textures:", SystemInfo.supportsSparseTextures.ToString());
                base.DrawItem("Supports 3D Textures:", SystemInfo.supports3DTextures.ToString());
                // 不兼容
                //base.DrawItem("Supports 3D Render Textures:", SystemInfo.supports3DRenderTextures.ToString());
                base.DrawItem("Supports 2D Array Textures:", SystemInfo.supports2DArrayTextures.ToString());
                base.DrawItem("Supports Shadows:", SystemInfo.supportsShadows.ToString());
                base.DrawItem("Supports Raw Shadow Depth Sampling:", SystemInfo.supportsRawShadowDepthSampling.ToString());
                base.DrawItem("Supports Render To Cubemap:", SystemInfo.supportsRenderToCubemap.ToString());
                base.DrawItem("Supports Compute Shader:", SystemInfo.supportsComputeShaders.ToString());
                base.DrawItem("Supports Instancing:", SystemInfo.supportsInstancing.ToString());
                base.DrawItem("Supports Image Effects:", SystemInfo.supportsImageEffects.ToString());
                base.DrawItem("Supports Cubemap Array Textures:", SystemInfo.supportsCubemapArrayTextures.ToString());
                base.DrawItem("Supports Motion Vectors:", SystemInfo.supportsMotionVectors.ToString());
                // 不兼容
                //base.DrawItem("Graphics UV Starts At Top:", SystemInfo.graphicsUVStartsAtTop.ToString());
                base.DrawItem("Uses Reversed ZBuffer:", SystemInfo.usesReversedZBuffer.ToString());
                GUILayout.EndVertical();
            }

            private string GetShaderLevelString(int shaderLevel)
            {
                return string.Format("Shader Model {0}.{1}", (shaderLevel / 10).ToString(), (shaderLevel % 10).ToString());
            }
        }

        private sealed class InputAccelerationInformationWindow : DebuggerComponent.ScrollableDebuggerWindowBase
        {
            protected override void OnDrawScrollableWindow()
            {
                GUILayout.Label("<b>Input Acceleration Information</b>", new GUILayoutOption[0]);
                GUILayout.BeginVertical("box", new GUILayoutOption[0]);
                base.DrawItem("Acceleration:", Input.acceleration.ToString());
                base.DrawItem("Acceleration Event Count:", Input.accelerationEventCount.ToString());
                base.DrawItem("Acceleration Events:", this.GetAccelerationEventsString(Input.accelerationEvents));
                GUILayout.EndVertical();
            }

            private string GetAccelerationEventString(AccelerationEvent accelerationEvent)
            {
                return string.Format("{0}, {1}", accelerationEvent.acceleration.ToString(), accelerationEvent.deltaTime.ToString());
            }

            private string GetAccelerationEventsString(AccelerationEvent[] accelerationEvents)
            {
                string[] array = new string[accelerationEvents.Length];
                for (int i = 0; i < accelerationEvents.Length; i++)
                {
                    array[i] = this.GetAccelerationEventString(accelerationEvents[i]);
                }
                return string.Join("; ", array);
            }
        }

        private sealed class InputCompassInformationWindow : DebuggerComponent.ScrollableDebuggerWindowBase
        {
            protected override void OnDrawScrollableWindow()
            {
                GUILayout.Label("<b>Input Compass Information</b>", new GUILayoutOption[0]);
                GUILayout.BeginVertical("box", new GUILayoutOption[0]);
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                if (GUILayout.Button("Enable", new GUILayoutOption[]
                {
                    GUILayout.Height(30f)
                }))
                {
                    Input.compass.enabled = true;
                }
                if (GUILayout.Button("Disable", new GUILayoutOption[]
                {
                    GUILayout.Height(30f)
                }))
                {
                    Input.compass.enabled = false;
                }
                GUILayout.EndHorizontal();
                base.DrawItem("Enabled:", Input.compass.enabled.ToString());
                base.DrawItem("Heading Accuracy:", Input.compass.headingAccuracy.ToString());
                base.DrawItem("Magnetic Heading:", Input.compass.magneticHeading.ToString());
                base.DrawItem("Raw Vector:", Input.compass.rawVector.ToString());
                base.DrawItem("Timestamp:", Input.compass.timestamp.ToString());
                base.DrawItem("True Heading:", Input.compass.trueHeading.ToString());
                GUILayout.EndVertical();
            }
        }

        private sealed class InputGyroscopeInformationWindow : DebuggerComponent.ScrollableDebuggerWindowBase
        {
            protected override void OnDrawScrollableWindow()
            {
                GUILayout.Label("<b>Input Gyroscope Information</b>", new GUILayoutOption[0]);
                GUILayout.BeginVertical("box", new GUILayoutOption[0]);
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                if (GUILayout.Button("Enable", new GUILayoutOption[]
                {
                    GUILayout.Height(30f)
                }))
                {
                    Input.gyro.enabled = true;
                }
                if (GUILayout.Button("Disable", new GUILayoutOption[]
                {
                    GUILayout.Height(30f)
                }))
                {
                    Input.gyro.enabled = false;
                }
                GUILayout.EndHorizontal();
                base.DrawItem("Enabled:", Input.gyro.enabled.ToString());
                base.DrawItem("Update Interval:", Input.gyro.updateInterval.ToString());
                base.DrawItem("Attitude:", Input.gyro.attitude.eulerAngles.ToString());
                base.DrawItem("Gravity:", Input.gyro.gravity.ToString());
                base.DrawItem("Rotation Rate:", Input.gyro.rotationRate.ToString());
                base.DrawItem("Rotation Rate Unbiased:", Input.gyro.rotationRateUnbiased.ToString());
                base.DrawItem("User Acceleration:", Input.gyro.userAcceleration.ToString());
                GUILayout.EndVertical();
            }
        }

        private sealed class InputLocationInformationWindow : DebuggerComponent.ScrollableDebuggerWindowBase
        {
            protected override void OnDrawScrollableWindow()
            {
                GUILayout.Label("<b>Input Location Information</b>", new GUILayoutOption[0]);
                GUILayout.BeginVertical("box", new GUILayoutOption[0]);
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                if (GUILayout.Button("Enable", new GUILayoutOption[]
                {
                    GUILayout.Height(30f)
                }))
                {
                    Input.location.Start();
                }
                if (GUILayout.Button("Disable", new GUILayoutOption[]
                {
                    GUILayout.Height(30f)
                }))
                {
                    Input.location.Stop();
                }
                GUILayout.EndHorizontal();
                base.DrawItem("Is Enabled By User:", Input.location.isEnabledByUser.ToString());
                base.DrawItem("Status:", Input.location.status.ToString());
                base.DrawItem("Horizontal Accuracy:", Input.location.lastData.horizontalAccuracy.ToString());
                base.DrawItem("Vertical Accuracy:", Input.location.lastData.verticalAccuracy.ToString());
                base.DrawItem("Longitude:", Input.location.lastData.longitude.ToString());
                base.DrawItem("Latitude:", Input.location.lastData.latitude.ToString());
                base.DrawItem("Altitude:", Input.location.lastData.altitude.ToString());
                base.DrawItem("Timestamp:", Input.location.lastData.timestamp.ToString());
                GUILayout.EndVertical();
            }
        }

        private sealed class InputSummaryInformationWindow : DebuggerComponent.ScrollableDebuggerWindowBase
        {
            protected override void OnDrawScrollableWindow()
            {
                GUILayout.Label("<b>Input Summary Information</b>", new GUILayoutOption[0]);
                GUILayout.BeginVertical("box", new GUILayoutOption[0]);
                base.DrawItem("Back Button Leaves App:", Input.backButtonLeavesApp.ToString());
                base.DrawItem("Device Orientation:", Input.deviceOrientation.ToString());
                base.DrawItem("Mouse Present:", Input.mousePresent.ToString());
                base.DrawItem("Mouse Position:", Input.mousePosition.ToString());
                base.DrawItem("Mouse Scroll Delta:", Input.mouseScrollDelta.ToString());
                base.DrawItem("Any Key:", Input.anyKey.ToString());
                base.DrawItem("Any Key Down:", Input.anyKeyDown.ToString());
                base.DrawItem("Input String:", Input.inputString);
                base.DrawItem("IME Is Selected:", Input.imeIsSelected.ToString());
                base.DrawItem("IME Composition Mode:", Input.imeCompositionMode.ToString());
                base.DrawItem("Compensate Sensors:", Input.compensateSensors.ToString());
                base.DrawItem("Composition Cursor Position:", Input.compositionCursorPos.ToString());
                base.DrawItem("Composition String:", Input.compositionString);
                GUILayout.EndVertical();
            }
        }

        private sealed class InputTouchInformationWindow : DebuggerComponent.ScrollableDebuggerWindowBase
        {
            protected override void OnDrawScrollableWindow()
            {
                GUILayout.Label("<b>Input Touch Information</b>", new GUILayoutOption[0]);
                GUILayout.BeginVertical("box", new GUILayoutOption[0]);
                base.DrawItem("Touch Supported:", Input.touchSupported.ToString());
                base.DrawItem("Touch Pressure Supported:", Input.touchPressureSupported.ToString());
                base.DrawItem("Stylus Touch Supported:", Input.stylusTouchSupported.ToString());
                base.DrawItem("Simulate Mouse With Touches:", Input.simulateMouseWithTouches.ToString());
                base.DrawItem("Multi Touch Enabled:", Input.multiTouchEnabled.ToString());
                base.DrawItem("Touch Count:", Input.touchCount.ToString());
                base.DrawItem("Touches:", this.GetTouchesString(Input.touches));
                GUILayout.EndVertical();
            }

            private string GetTouchString(Touch touch)
            {
                return string.Format("{0}, {1}, {2}, {3}, {4}", new object[]
                {
                    touch.position.ToString(),
                    touch.deltaPosition.ToString(),
                    touch.rawPosition.ToString(),
                    touch.pressure.ToString(),
                    touch.phase.ToString()
                });
            }

            private string GetTouchesString(Touch[] touches)
            {
                string[] array = new string[touches.Length];
                for (int i = 0; i < touches.Length; i++)
                {
                    array[i] = this.GetTouchString(touches[i]);
                }
                return string.Join("; ", array);
            }
        }

        private sealed class PathInformationWindow : DebuggerComponent.ScrollableDebuggerWindowBase
        {
            protected override void OnDrawScrollableWindow()
            {
                GUILayout.Label("<b>Path Information</b>", new GUILayoutOption[0]);
                GUILayout.BeginVertical("box", new GUILayoutOption[0]);
                base.DrawItem("Data Path:", Application.dataPath);
                base.DrawItem("Persistent Data Path:", Application.persistentDataPath);
                base.DrawItem("Streaming Assets Path:", Application.streamingAssetsPath);
                base.DrawItem("Temporary Cache Path:", Application.temporaryCachePath);
                GUILayout.EndVertical();
            }
        }

        private sealed class ProfilerInformationWindow : DebuggerComponent.ScrollableDebuggerWindowBase
        {
            private const int MBSize = 1048576;

            protected override void OnDrawScrollableWindow()
            {
                GUILayout.Label("<b>Profiler Information</b>", new GUILayoutOption[0]);
                GUILayout.BeginVertical("box", new GUILayoutOption[0]);
                base.DrawItem("Supported:", Profiler.supported.ToString());
                base.DrawItem("Enabled:", Profiler.enabled.ToString());
                base.DrawItem("Enable Binary Log:", (!Profiler.enableBinaryLog) ? "False" : string.Format("True, {0}", Profiler.logFile));
                base.DrawItem("Mono Used Size:", string.Format("{0} MB", ((float)Profiler.GetMonoUsedSize() / 1048576f).ToString("F3")));
                base.DrawItem("Mono Heap Size:", string.Format("{0} MB", ((float)Profiler.GetMonoHeapSize() / 1048576f).ToString("F3")));
                base.DrawItem("Used Heap Size:", string.Format("{0} MB", ((float)Profiler.usedHeapSize / 1048576f).ToString("F3")));
                base.DrawItem("Total Allocated Memory:", string.Format("{0} MB", ((float)Profiler.GetTotalAllocatedMemory() / 1048576f).ToString("F3")));
                base.DrawItem("Total Reserved Memory:", string.Format("{0} MB", ((float)Profiler.GetTotalReservedMemory() / 1048576f).ToString("F3")));
                base.DrawItem("Total Unused Reserved Memory:", string.Format("{0} MB", ((float)Profiler.GetTotalUnusedReservedMemory() / 1048576f).ToString("F3")));
                base.DrawItem("Temp Allocator Size:", string.Format("{0} MB", (Profiler.GetTempAllocatorSize() / 1048576f).ToString("F3")));
                GUILayout.EndVertical();
            }
        }

        private sealed class QualityInformationWindow : DebuggerComponent.ScrollableDebuggerWindowBase
        {
            private bool m_ApplyExpensiveChanges;

            protected override void OnDrawScrollableWindow()
            {
                GUILayout.Label("<b>Quality Level</b>", new GUILayoutOption[0]);
                GUILayout.BeginVertical("box", new GUILayoutOption[0]);
                int qualityLevel = QualitySettings.GetQualityLevel();
                base.DrawItem("Current Quality Level:", QualitySettings.names[qualityLevel]);
                this.m_ApplyExpensiveChanges = GUILayout.Toggle(this.m_ApplyExpensiveChanges, "Apply expensive changes on quality level change.", new GUILayoutOption[0]);
                int num = GUILayout.SelectionGrid(qualityLevel, QualitySettings.names, 3, "toggle", new GUILayoutOption[0]);
                if (num != qualityLevel)
                {
                    QualitySettings.SetQualityLevel(num, this.m_ApplyExpensiveChanges);
                }
                GUILayout.EndVertical();
                GUILayout.Label("<b>Rendering Information</b>", new GUILayoutOption[0]);
                GUILayout.BeginVertical("box", new GUILayoutOption[0]);
                base.DrawItem("Active Color Space:", QualitySettings.activeColorSpace.ToString());
                base.DrawItem("Desired Color Space:", QualitySettings.desiredColorSpace.ToString());
                base.DrawItem("Max Queued Frames:", QualitySettings.maxQueuedFrames.ToString());
                base.DrawItem("Pixel Light Count:", QualitySettings.pixelLightCount.ToString());
                base.DrawItem("Master Texture Limit:", QualitySettings.masterTextureLimit.ToString());
                base.DrawItem("Anisotropic Filtering:", QualitySettings.anisotropicFiltering.ToString());
                base.DrawItem("Anti Aliasing:", QualitySettings.antiAliasing.ToString());
                base.DrawItem("Realtime Reflection Probes:", QualitySettings.realtimeReflectionProbes.ToString());
                base.DrawItem("Billboards Face Camera Position:", QualitySettings.billboardsFaceCameraPosition.ToString());
                // 不兼容
                // base.DrawItem("Resolution Scaling Fixed DPI Factor:", QualitySettings.resolutionScalingFixedDPIFactor.ToString());
                GUILayout.EndVertical();
                GUILayout.Label("<b>Shadows Information</b>", new GUILayoutOption[0]);
                GUILayout.BeginVertical("box", new GUILayoutOption[0]);
                base.DrawItem("Shadow Resolution:", QualitySettings.shadowResolution.ToString());
                base.DrawItem("Shadow Quality:", QualitySettings.shadows.ToString());
                base.DrawItem("Shadow Projection:", QualitySettings.shadowProjection.ToString());
                base.DrawItem("Shadow Distance:", QualitySettings.shadowDistance.ToString());
                // 不兼容
                //base.DrawItem("Shadowmask Mode:", QualitySettings.shadowmaskMode.ToString());
                base.DrawItem("Shadow Near Plane Offset:", QualitySettings.shadowNearPlaneOffset.ToString());
                base.DrawItem("Shadow Cascades:", QualitySettings.shadowCascades.ToString());
                base.DrawItem("Shadow Cascade 2 Split:", QualitySettings.shadowCascade2Split.ToString());
                base.DrawItem("Shadow Cascade 4 Split:", QualitySettings.shadowCascade4Split.ToString());
                GUILayout.EndVertical();
                GUILayout.Label("<b>Other Information</b>", new GUILayoutOption[0]);
                GUILayout.BeginVertical("box", new GUILayoutOption[0]);
                base.DrawItem("Blend Weights:", QualitySettings.blendWeights.ToString());
                base.DrawItem("VSync Count:", QualitySettings.vSyncCount.ToString());
                base.DrawItem("LOD Bias:", QualitySettings.lodBias.ToString());
                base.DrawItem("Maximum LOD Level:", QualitySettings.maximumLODLevel.ToString());
                base.DrawItem("Particle Raycast Budget:", QualitySettings.particleRaycastBudget.ToString());
                base.DrawItem("Async Upload Time Slice:", string.Format("{0} ms", QualitySettings.asyncUploadTimeSlice.ToString()));
                base.DrawItem("Async Upload Buffer Size:", string.Format("{0} MB", QualitySettings.asyncUploadBufferSize.ToString()));
                base.DrawItem("Soft Particles:", QualitySettings.softParticles.ToString());
                base.DrawItem("Soft Vegetation:", QualitySettings.softVegetation.ToString());
                GUILayout.EndVertical();
            }
        }

        private sealed class RecorderWindow : DebuggerComponent.ScrollableDebuggerWindowBase
        {
            public DebuggerComponent.Recorder m_Recorder;

            private List<IRecordMike> activeMikes = new List<IRecordMike>();

            private List<IRecordMike> allMikes = new List<IRecordMike>();

            private GUIStyle recordingUIStyle = new GUIStyle();

            public void InitRecorder(List<IRecordMike> mikes)
            {
                this.recordingUIStyle.normal.textColor = Color.red;
                this.m_Recorder = new DebuggerComponent.Recorder();
                for (int i = 0; i < mikes.Count; i++)
                {
                    this.allMikes.Add(mikes[i]);
                }
            }

            protected override void OnDrawScrollableWindow()
            {
                string text = (!this.m_Recorder.IsStart) ? "Start" : "Stop";
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                if (GUILayout.Button(text, new GUILayoutOption[]
                {
                    GUILayout.Width(50f),
                    GUILayout.Height(40f)
                }))
                {
                    if (!this.m_Recorder.IsStart)
                    {
                        this.m_Recorder.StartRecord();
                    }
                    else
                    {
                        this.m_Recorder.EndRecord();
                    }
                }
                if (this.m_Recorder.IsStart)
                {
                    GUILayout.TextField("Recording", this.recordingUIStyle, new GUILayoutOption[0]);
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                for (int i = 0; i < this.allMikes.Count; i++)
                {
                    if (GUILayout.Button(this.allMikes[i].GetTag(), new GUILayoutOption[]
                    {
                        GUILayout.Width(100f),
                        GUILayout.Height(40f)
                    }))
                    {
                        if (this.m_Recorder.GetMikeIndexOf(this.allMikes[i].GetTag()) == -1)
                        {
                            if (this.m_Recorder.AddMike(this.allMikes[i]) != -1)
                            {
                                this.activeMikes.Add(this.allMikes[i]);
                            }
                        }
                        else if (this.m_Recorder.RemoveMike(this.allMikes[i].GetTag()))
                        {
                            this.activeMikes.Remove(this.allMikes[i]);
                        }
                    }
                }
                GUILayout.EndHorizontal();
                if (this.activeMikes.Count > 0)
                {
                    GUILayout.Label("<b>Active Mike</b>", new GUILayoutOption[0]);
                    for (int j = 0; j < this.activeMikes.Count; j++)
                    {
                        GUILayout.BeginVertical("box", new GUILayoutOption[0]);
                        base.DrawItem(this.activeMikes[j].GetTag(), this.activeMikes[j].GetSample());
                        GUILayout.EndVertical();
                    }
                }
            }
        }

        private sealed class RuntimeMemoryInformationWindow<T> : DebuggerComponent.ScrollableDebuggerWindowBase where T : UnityEngine.Object
        {
            private sealed class Sample
            {
                private readonly string m_Name;

                private readonly string m_Type;

                private readonly string m_Format;

                private readonly long m_Size;

                private bool m_Highlight;

                public string Name
                {
                    get
                    {
                        return this.m_Name;
                    }
                }

                public string Format
                {
                    get
                    {
                        return this.m_Format;
                    }
                }

                public string Type
                {
                    get
                    {
                        return this.m_Type;
                    }
                }

                public long Size
                {
                    get
                    {
                        return this.m_Size;
                    }
                }

                public bool Highlight
                {
                    get
                    {
                        return this.m_Highlight;
                    }
                    set
                    {
                        this.m_Highlight = value;
                    }
                }

                public Sample(string name, string type, long size, string format)
                {
                    this.m_Name = name;
                    this.m_Type = type;
                    this.m_Size = size;
                    this.m_Format = format;
                    this.m_Highlight = false;
                }
            }

            private const int ShowSampleCount = 300;

            private DateTime m_SampleTime = DateTime.MinValue;

            private long m_SampleSize;

            private long m_DuplicateSampleSize;

            private int m_DuplicateSimpleCount;

            private List<DebuggerComponent.RuntimeMemoryInformationWindow<T>.Sample> m_Samples = new List<DebuggerComponent.RuntimeMemoryInformationWindow<T>.Sample>();

            private bool m_DisplayFormat;

            protected override void OnDrawScrollableWindow()
            {
                string name = typeof(T).Name;
                GUILayout.Label(string.Format("<b>{0} Runtime Memory Information</b>", name), new GUILayoutOption[0]);
                GUILayout.BeginVertical("box", new GUILayoutOption[0]);
                if (GUILayout.Button(string.Format("Take Sample for {0}", name), new GUILayoutOption[]
                {
                    GUILayout.Height(30f)
                }))
                {
                    this.TakeSample();
                }
                if (this.m_SampleTime <= DateTime.MinValue)
                {
                    GUILayout.Label(string.Format("<b>Please take sample for {0} first.</b>", name), new GUILayoutOption[0]);
                }
                else
                {
                    if (this.m_DuplicateSimpleCount > 0)
                    {
                        GUILayout.Label(string.Format("<b>{0} {1}s ({2}) obtained at {3}, while {4} {1}s ({5}) might be duplicated.</b>", new object[]
                        {
                            this.m_Samples.Count.ToString(),
                            name,
                            this.GetSizeString(this.m_SampleSize),
                            this.m_SampleTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            this.m_DuplicateSimpleCount.ToString(),
                            this.GetSizeString(this.m_DuplicateSampleSize)
                        }), new GUILayoutOption[0]);
                    }
                    else
                    {
                        GUILayout.Label(string.Format("<b>{0} {1}s ({2}) obtained at {3}.</b>", new object[]
                        {
                            this.m_Samples.Count.ToString(),
                            name,
                            this.GetSizeString(this.m_SampleSize),
                            this.m_SampleTime.ToString("yyyy-MM-dd HH:mm:ss")
                        }), new GUILayoutOption[0]);
                    }
                    if (this.m_Samples.Count > 0)
                    {
                        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                        GUILayout.Label(string.Format("<b>{0} Name</b>", name), new GUILayoutOption[0]);
                        if (this.m_DisplayFormat)
                        {
                            GUILayout.Label("<b>Format</b>", new GUILayoutOption[]
                            {
                                GUILayout.Width(120f)
                            });
                        }
                        GUILayout.Label("<b>Type</b>", new GUILayoutOption[]
                        {
                            GUILayout.Width(120f)
                        });
                        GUILayout.Label("<b>Size</b>", new GUILayoutOption[]
                        {
                            GUILayout.Width(80f)
                        });
                        GUILayout.EndHorizontal();
                    }
                    int num = 0;
                    for (int i = 0; i < this.m_Samples.Count; i++)
                    {
                        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                        GUILayout.Label((!this.m_Samples[i].Highlight) ? this.m_Samples[i].Name : string.Format("<color=yellow>{0}</color>", this.m_Samples[i].Name), new GUILayoutOption[0]);
                        if (this.m_DisplayFormat)
                        {
                            GUILayout.Label((!this.m_Samples[i].Highlight) ? this.m_Samples[i].Format : string.Format("<color=yellow>{0}</color>", this.m_Samples[i].Format), new GUILayoutOption[]
                            {
                                GUILayout.Width(120f)
                            });
                        }
                        GUILayout.Label((!this.m_Samples[i].Highlight) ? this.m_Samples[i].Type : string.Format("<color=yellow>{0}</color>", this.m_Samples[i].Type), new GUILayoutOption[]
                        {
                            GUILayout.Width(120f)
                        });
                        GUILayout.Label((!this.m_Samples[i].Highlight) ? this.GetSizeString(this.m_Samples[i].Size) : string.Format("<color=yellow>{0}</color>", this.GetSizeString(this.m_Samples[i].Size)), new GUILayoutOption[]
                        {
                            GUILayout.Width(80f)
                        });
                        GUILayout.EndHorizontal();
                        num++;
                        if (num >= 300)
                        {
                            break;
                        }
                    }
                }
                GUILayout.EndVertical();
            }

            private void TakeSample()
            {
                this.m_SampleTime = DateTime.Now;
                this.m_SampleSize = 0L;
                this.m_DuplicateSampleSize = 0L;
                this.m_DuplicateSimpleCount = 0;
                this.m_Samples.Clear();
                this.m_DisplayFormat = false;
                T[] array = Resources.FindObjectsOfTypeAll<T>();
                for (int i = 0; i < array.Length; i++)
                {
                    long runtimeMemorySizeLong = Profiler.GetRuntimeMemorySize(array[i]);
                    this.m_SampleSize += runtimeMemorySizeLong;
                    this.m_Samples.Add(new DebuggerComponent.RuntimeMemoryInformationWindow<T>.Sample(array[i].name, array[i].GetType().Name, runtimeMemorySizeLong, this.GetObjectFormat(array[i])));
                }
                this.m_Samples.Sort(new Comparison<DebuggerComponent.RuntimeMemoryInformationWindow<T>.Sample>(this.SampleComparer));
                for (int j = 1; j < this.m_Samples.Count; j++)
                {
                    if (this.m_Samples[j].Name == this.m_Samples[j - 1].Name && this.m_Samples[j].Type == this.m_Samples[j - 1].Type && this.m_Samples[j].Size == this.m_Samples[j - 1].Size)
                    {
                        this.m_Samples[j].Highlight = true;
                        this.m_DuplicateSampleSize += this.m_Samples[j].Size;
                        this.m_DuplicateSimpleCount++;
                    }
                    if (this.m_Samples[j].Format != "--")
                    {
                        this.m_DisplayFormat = true;
                    }
                }
            }

            private string GetObjectFormat(T obj)
            {
                string result = "--";
                if (obj is Texture2D)
                {
                    Texture2D texture2D = obj as Texture2D;
                    result = texture2D.format.ToString();
                }
                else if (obj is RenderTexture)
                {
                    RenderTexture renderTexture = obj as RenderTexture;
                    result = renderTexture.format.ToString();
                }
                else if (obj is Cubemap)
                {
                    Cubemap cubemap = obj as Cubemap;
                    result = cubemap.format.ToString();
                }
                return result;
            }

            private string GetSizeString(long size)
            {
                if (size < 1024L)
                {
                    return string.Format("{0} Bytes", size.ToString());
                }
                if (size < 1048576L)
                {
                    return string.Format("{0} KB", ((float)size / 1024f).ToString("F2"));
                }
                if (size < 1073741824L)
                {
                    return string.Format("{0} MB", ((float)size / 1024f / 1024f).ToString("F2"));
                }
                if (size < 1099511627776L)
                {
                    return string.Format("{0} GB", ((float)size / 1024f / 1024f / 1024f).ToString("F2"));
                }
                return string.Format("{0} TB", ((float)size / 1024f / 1024f / 1024f / 1024f).ToString("F2"));
            }

            private int SampleComparer(DebuggerComponent.RuntimeMemoryInformationWindow<T>.Sample a, DebuggerComponent.RuntimeMemoryInformationWindow<T>.Sample b)
            {
                int num = b.Size.CompareTo(a.Size);
                if (num != 0)
                {
                    return num;
                }
                num = a.Type.CompareTo(b.Type);
                if (num != 0)
                {
                    return num;
                }
                return a.Name.CompareTo(b.Name);
            }
        }

        private sealed class SceneInformationWindow : DebuggerComponent.ScrollableDebuggerWindowBase
        {
            protected override void OnDrawScrollableWindow()
            {
                GUILayout.Label("<b>Scene Information</b>", new GUILayoutOption[0]);
                GUILayout.BeginVertical("box", new GUILayoutOption[0]);
                base.DrawItem("Scene Count:", SceneManager.sceneCount.ToString());
                base.DrawItem("Scene Count In Build Settings:", SceneManager.sceneCountInBuildSettings.ToString());
                Scene activeScene = SceneManager.GetActiveScene();
                base.DrawItem("Active Scene Name:", activeScene.name);
                base.DrawItem("Active Scene Path:", activeScene.path);
                base.DrawItem("Active Scene Build Index:", activeScene.buildIndex.ToString());
                base.DrawItem("Active Scene Is Dirty:", activeScene.isDirty.ToString());
                base.DrawItem("Active Scene Is Loaded:", activeScene.isLoaded.ToString());
                base.DrawItem("Active Scene Is Valid:", activeScene.IsValid().ToString());
                base.DrawItem("Active Scene Root Count:", activeScene.rootCount.ToString());
                GUILayout.EndVertical();
            }
        }

        private abstract class ScrollableDebuggerWindowBase : IDebuggerWindow
        {
            private const float TitleWidth = 240f;

            private Vector2 m_ScrollPosition = Vector2.zero;

            public virtual void Initialize(params object[] args)
            {
            }

            public virtual void Shutdown()
            {
            }

            public virtual void OnEnter()
            {
            }

            public virtual void OnLeave()
            {
            }

            public virtual void OnUpdate(float elapseSeconds, float realElapseSeconds)
            {
            }

            public void OnDraw()
            {
                this.m_ScrollPosition = GUILayout.BeginScrollView(this.m_ScrollPosition, new GUILayoutOption[0]);
                this.OnDrawScrollableWindow();
                GUILayout.EndScrollView();
            }

            protected abstract void OnDrawScrollableWindow();

            protected void DrawItem(string title, string content)
            {
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.Label(title, new GUILayoutOption[]
                {
                    GUILayout.Width(240f)
                });
                GUILayout.Label(content, new GUILayoutOption[0]);
                GUILayout.EndHorizontal();
            }
        }

        private sealed class SystemInformationWindow : DebuggerComponent.ScrollableDebuggerWindowBase
        {
            protected override void OnDrawScrollableWindow()
            {
                GUILayout.Label("<b>System Information</b>", new GUILayoutOption[0]);
                GUILayout.BeginVertical("box", new GUILayoutOption[0]);
                base.DrawItem("Device Unique ID:", SystemInfo.deviceUniqueIdentifier);
                base.DrawItem("Device Name:", SystemInfo.deviceName);
                base.DrawItem("Device Type:", SystemInfo.deviceType.ToString());
                base.DrawItem("Device Model:", SystemInfo.deviceModel);
                base.DrawItem("Processor Type:", SystemInfo.processorType);
                base.DrawItem("Processor Count:", SystemInfo.processorCount.ToString());
                base.DrawItem("Processor Frequency:", string.Format("{0} MHz", SystemInfo.processorFrequency.ToString()));
                base.DrawItem("Memory Size:", string.Format("{0} MB", SystemInfo.systemMemorySize.ToString()));
                base.DrawItem("Operating System Family:", SystemInfo.operatingSystemFamily.ToString());
                base.DrawItem("Operating System:", SystemInfo.operatingSystem);
                // 不兼容
                //base.DrawItem("Battery Status:", SystemInfo.batteryStatus.ToString());
                //base.DrawItem("Battery Level:", this.GetBatteryLevelString(SystemInfo.batteryLevel));
                base.DrawItem("Supports Audio:", SystemInfo.supportsAudio.ToString());
                base.DrawItem("Supports Location Service:", SystemInfo.supportsLocationService.ToString());
                base.DrawItem("Supports Accelerometer:", SystemInfo.supportsAccelerometer.ToString());
                base.DrawItem("Supports Gyroscope:", SystemInfo.supportsGyroscope.ToString());
                base.DrawItem("Supports Vibration:", SystemInfo.supportsVibration.ToString());
                base.DrawItem("Genuine:", Application.genuine.ToString());
                base.DrawItem("Genuine Check Available:", Application.genuineCheckAvailable.ToString());
                GUILayout.EndVertical();
            }

            private string GetBatteryLevelString(float batteryLevel)
            {
                if (batteryLevel < 0f)
                {
                    return "Unavailable";
                }
                return batteryLevel.ToString("P0");
            }
        }

        private sealed class TimeInformationWindow : DebuggerComponent.ScrollableDebuggerWindowBase
        {
            protected override void OnDrawScrollableWindow()
            {
                GUILayout.Label("<b>Time Information</b>", new GUILayoutOption[0]);
                GUILayout.BeginVertical("box", new GUILayoutOption[0]);
                base.DrawItem("Time Scale", string.Format("{0} [{1}]", Time.timeScale.ToString(), this.GetTimeScaleDescription(Time.timeScale)));
                base.DrawItem("Realtime Since Startup", Time.realtimeSinceStartup.ToString());
                base.DrawItem("Time Since Level Load", Time.timeSinceLevelLoad.ToString());
                base.DrawItem("Time", Time.time.ToString());
                base.DrawItem("Fixed Time", Time.fixedTime.ToString());
                base.DrawItem("Unscaled Time", Time.unscaledTime.ToString());
                // 不兼容
                //base.DrawItem("Fixed Unscaled Time", Time.fixedUnscaledTime.ToString());
                base.DrawItem("Fixed Unscaled Time", Time.unscaledTime.ToString());
                base.DrawItem("Delta Time", Time.deltaTime.ToString());
                base.DrawItem("Fixed Delta Time", Time.fixedDeltaTime.ToString());
                base.DrawItem("Unscaled Delta Time", Time.unscaledDeltaTime.ToString());
                // 不兼容
                //base.DrawItem("Fixed Unscaled Delta Time", Time.fixedUnscaledDeltaTime.ToString());
                base.DrawItem("Fixed Unscaled Delta Time", Time.unscaledDeltaTime.ToString());
                base.DrawItem("Smooth Delta Time", Time.smoothDeltaTime.ToString());
                base.DrawItem("Maximum Delta Time", Time.maximumDeltaTime.ToString());
                base.DrawItem("Maximum Particle Delta Time", Time.maximumParticleDeltaTime.ToString());
                base.DrawItem("Frame Count", Time.frameCount.ToString());
                base.DrawItem("Rendered Frame Count", Time.renderedFrameCount.ToString());
                base.DrawItem("Capture Framerate", Time.captureFramerate.ToString());
                // 不兼容
                //base.DrawItem("In Fixed Time Step", Time.inFixedTimeStep.ToString());
                GUILayout.EndVertical();
            }

            private string GetTimeScaleDescription(float timeScale)
            {
                if (timeScale <= 0f)
                {
                    return "Pause";
                }
                if (timeScale < 1f)
                {
                    return "Slower";
                }
                if (timeScale > 1f)
                {
                    return "Faster";
                }
                return "Normal";
            }
        }

        private sealed class WebPlayerInformationWindow : DebuggerComponent.ScrollableDebuggerWindowBase
        {
            protected override void OnDrawScrollableWindow()
            {
                GUILayout.Label("<b>Web Player Information</b>", new GUILayoutOption[0]);
                GUILayout.BeginVertical("box", new GUILayoutOption[0]);
                base.DrawItem("Absolute URL:", Application.absoluteURL);
                base.DrawItem("Streamed Bytes:", Application.streamedBytes.ToString());
                GUILayout.EndVertical();
            }
        }

        private sealed class Recorder
        {
            private sealed class Track
            {
                public readonly IRecordMike recordMike;

                public string recordString;

                public Track(IRecordMike recordMike)
                {
                    this.recordMike = recordMike;
                }
            }

            private string LOGPATH = Application.persistentDataPath + "/";

            private const float minInterval = 0.1f;

            private float timmer;

            private float m_UpdateInterval;

            private float m_TimeLeft;

            private List<DebuggerComponent.Recorder.Track> tracks = new List<DebuggerComponent.Recorder.Track>();

            private bool isStart;

            public bool IsStart
            {
                get
                {
                    return this.isStart;
                }
            }

            public int AddMike(IRecordMike _mike)
            {
                if (!this.isStart)
                {
                    this.tracks.Add(new DebuggerComponent.Recorder.Track(_mike));
                    return this.tracks.Count - 1;
                }
                return -1;
            }

            public bool RemoveMike(string _tag)
            {
                if (!this.isStart)
                {
                    this.tracks.RemoveAt(this.GetMikeIndexOf(_tag));
                }
                return !this.isStart;
            }

            public int GetMikeIndexOf(string _tag)
            {
                for (int i = 0; i < this.tracks.Count; i++)
                {
                    if (this.tracks[i].recordMike.GetTag().Equals(_tag))
                    {
                        return i;
                    }
                }
                return -1;
            }

            public IRecordMike[] GetRecordMikes()
            {
                IRecordMike[] array = new IRecordMike[this.tracks.Count];
                for (int i = 0; i < this.tracks.Count; i++)
                {
                    array[i] = this.tracks[i].recordMike;
                }
                return array;
            }

            public void SetRecorderInterval(float _updateInterval)
            {
                this.m_UpdateInterval = Mathf.Max(_updateInterval, 0.1f);
            }

            public void Update(float elapseSeconds, float realElapseSeconds)
            {
                if (!this.isStart)
                {
                    return;
                }
                this.m_TimeLeft -= realElapseSeconds;
                this.timmer += realElapseSeconds;
                if (this.m_TimeLeft <= 0f)
                {
                    this.EchoTraverse(this.timmer);
                    this.m_TimeLeft += this.m_UpdateInterval;
                }
            }

            public void StartRecord()
            {
                if (this.tracks.Count < 1 || this.isStart)
                {
                    return;
                }
                this.isStart = true;
                this.timmer = 0f;
                this.StartTraverse();
            }

            public void EndRecord()
            {
                if (this.isStart)
                {
                    this.EndTraverse();
                }
                this.Reset(false);
            }

            private void Reset(bool _resetMikes)
            {
                this.isStart = false;
                this.m_TimeLeft = 0f;
                this.timmer = 0f;
                if (_resetMikes)
                {
                    this.tracks.Clear();
                }
            }

            private void StartTraverse()
            {
            }

            private void EndTraverse()
            {
                if (!Directory.Exists(this.LOGPATH))
                {
                    Directory.CreateDirectory(this.LOGPATH);
                }
                for (int i = 0; i < this.tracks.Count; i++)
                {
                    string path = string.Concat(new string[]
                    {
                        this.LOGPATH,
                        DateTime.Now.ToString().Replace('/', '_').Replace(":", "-"),
                        " ",
                        this.tracks[i].recordMike.GetTag(),
                        ".log"
                    });
                    File.WriteAllText(path, this.tracks[i].recordString);
                }
            }

            private void EchoTraverse(float _timeLine)
            {
                for (int i = 0; i < this.tracks.Count; i++)
                {
                    string str = string.Concat(new object[]
                    {
                        _timeLine,
                        ",",
                        this.tracks[i].recordMike.GetSample(),
                        "\n"
                    });
                    DebuggerComponent.Recorder.Track expr_51 = this.tracks[i];
                    expr_51.recordString += str;
                }
            }
        }

        public class ToluaMemory : IRecordMike
        {
            private enum LuaGCOptions
            {
                LUA_GCSTOP,
                LUA_GCRESTART,
                LUA_GCCOLLECT,
                LUA_GCCOUNT,
                LUA_GCCOUNTB,
                LUA_GCSTEP,
                LUA_GCSETPAUSE,
                LUA_GCSETSTEPMUL
            }

            public class StateEnumerable : IEnumerator<KeyValuePair<IntPtr, int>>, IEnumerator, IDisposable
            {
                private DebuggerComponent.ToluaMemory tolua_memory;

                private IDictionaryEnumerator dict_iter;

                KeyValuePair<IntPtr, int> IEnumerator<KeyValuePair<IntPtr, int>>.Current
                {
                    get
                    {
                        IntPtr key = IntPtr.Zero;
                        if (this.dict_iter.Key is IntPtr)
                        {
                            key = (IntPtr)this.dict_iter.Key;
                        }
                        object value = this.dict_iter.Value;
                        int value2 = Convert.ToInt32(this.tolua_memory.method_LuaGC.Invoke(value, new object[]
                        {
                            3,
                            0
                        }));
                        return new KeyValuePair<IntPtr, int>(key, value2);
                    }
                }

                object IEnumerator.Current
                {
                    get
                    {
                        return ((IEnumerator<KeyValuePair<IntPtr, int>>)this).Current;
                    }
                }

                public StateEnumerable(DebuggerComponent.ToluaMemory _tolua_memory, IDictionaryEnumerator _dict_iter)
                {
                    this.tolua_memory = _tolua_memory;
                    this.dict_iter = _dict_iter;
                }

                public bool MoveNext()
                {
                    return this.tolua_memory != null && this.dict_iter.MoveNext();
                }

                public void Reset()
                {
                    this.dict_iter.Reset();
                }

                public void Dispose()
                {
                    this.dict_iter = null;
                }
            }

            private const string MIKE_TAG = "ToluaMem";

            private bool init_reflection;

            private IDictionary luastateMap;

            private string tolua_version;

            private MethodInfo method_LuaGC;

            private int information_frame_count;

            private long total_gc_count;

            public ToluaMemory()
            {
                this.InitToluaReflection();
            }

            private void InitToluaReflection()
            {
                try
                {
                    Type type = Type.GetType("LuaInterface.LuaState");
                    if (type != null)
                    {
                        MethodInfo method = type.GetMethod("LuaGC", BindingFlags.Instance | BindingFlags.Public);
                        if (method != null)
                        {
                            this.method_LuaGC = method;
                            FieldInfo field = type.GetField("stateMap", BindingFlags.Static | BindingFlags.NonPublic);
                            if (field != null)
                            {
                                this.luastateMap = (field.GetValue(null) as IDictionary);
                                Type type2 = Type.GetType("LuaInterface.LuaDLL");
                                if (type2 != null)
                                {
                                    FieldInfo field2 = type2.GetField("version", BindingFlags.Static | BindingFlags.Public);
                                    object value = field2.GetValue(null);
                                    if (value is string)
                                    {
                                        this.tolua_version = (string)field2.GetValue(null);
                                    }
                                    this.init_reflection = true;
                                }
                            }
                        }
                    }
                }
                catch (Exception message)
                {
                    //global::Debug.LogError(message);
                    Debug.LogError(message);
                }
            }

            public bool IsToluaValid()
            {
                return this.init_reflection;
            }

            public string GetToluaVersion()
            {
                return this.tolua_version;
            }

            public long GetAllToluaGCCount()
            {
                if (this.information_frame_count == Time.frameCount)
                {
                    return this.total_gc_count;
                }
                this.information_frame_count = Time.frameCount;
                if (this.luastateMap == null || this.method_LuaGC == null)
                {
                    this.total_gc_count = 0L;
                }
                else
                {
                    long num = 0L;
                    IDictionaryEnumerator enumerator = this.luastateMap.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        object value = enumerator.Value;
                        int num2 = Convert.ToInt32(this.method_LuaGC.Invoke(value, new object[]
                        {
                            3,
                            0
                        }));
                        num += (long)num2;
                    }
                    this.total_gc_count = num;
                }
                return this.total_gc_count;
            }

            public void DoToluaGCCollect()
            {
                if (this.luastateMap != null && this.method_LuaGC != null)
                {
                    IDictionaryEnumerator enumerator = this.luastateMap.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        object value = enumerator.Value;
                        this.method_LuaGC.Invoke(value, new object[]
                        {
                            2,
                            0
                        });
                    }
                }
            }

            string IRecordMike.GetSample()
            {
                return this.GetAllToluaGCCount().ToString();
            }

            string IRecordMike.GetTag()
            {
                return "ToluaMem";
            }

            public IEnumerator<KeyValuePair<IntPtr, int>> GetStateEnumerator()
            {
                if (this.luastateMap == null)
                {
                    return new DebuggerComponent.ToluaMemory.StateEnumerable(null, null);
                }
                return new DebuggerComponent.ToluaMemory.StateEnumerable(this, this.luastateMap.GetEnumerator());
            }
        }

        private class ToluaMemoryWindow : DebuggerComponent.ScrollableDebuggerWindowBase
        {
            private DebuggerComponent.ToluaMemory tolua_memory;

            public override void Initialize(params object[] args)
            {
                this.tolua_memory = (args[0] as DebuggerComponent.ToluaMemory);
            }

            protected override void OnDrawScrollableWindow()
            {
                GUILayout.Label("<b>Tolua Memory Information</b>", new GUILayoutOption[0]);
                GUILayout.BeginVertical("box", new GUILayoutOption[0]);
                bool flag = false;
                if (this.tolua_memory != null && this.tolua_memory.IsToluaValid())
                {
                    flag = true;
                }
                if (flag)
                {
                    base.DrawItem("Version:", this.tolua_memory.GetToluaVersion());
                    base.DrawItem("Total Memory:", string.Format("{0} MB", ((float)this.tolua_memory.GetAllToluaGCCount() / 1024f).ToString("F3")));
                    GUILayout.Label("State Map:", new GUILayoutOption[0]);
                    IEnumerator<KeyValuePair<IntPtr, int>> stateEnumerator = this.tolua_memory.GetStateEnumerator();
                    while (stateEnumerator.MoveNext())
                    {
                        KeyValuePair<IntPtr, int> current = stateEnumerator.Current;
                        base.DrawItem(string.Format("Ptr(0x{0})", current.Key), string.Format("{0} MB", ((float)current.Value / 1024f).ToString("F3")));
                    }
                    GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Collect Garbage", new GUILayoutOption[0]))
                    {
                        this.tolua_memory.DoToluaGCCollect();
                    }
                    GUILayout.EndHorizontal();
                }
                else
                {
                    GUILayout.Label("Tolua not found.", new GUILayoutOption[0]);
                }
                GUILayout.EndVertical();
            }
        }

        internal static readonly Rect DefaultIconRect = new Rect(10f, 30f, 100f, 60f);

        internal static readonly Rect DefaultWindowRect = new Rect(10f, 10f, 640f, 480f);

        internal static readonly float DefaultMaxWindowsWidth = 1024f;

        internal static readonly float DefaultMaxToolBarWidth = 960f;

        internal static readonly float DefaultWindowScale = 1f;

        private IDebuggerManager m_DebuggerManager;

        private Rect m_DragRect = new Rect(0f, 0f, 3.40282347E+38f, 25f);

        // 会在Awake重置
        private Rect m_IconRect = DebuggerComponent.DefaultIconRect;

        // 会在Awake重置
        private Rect m_WindowRect = DebuggerComponent.DefaultWindowRect;

        private float m_WindowScale = DebuggerComponent.DefaultWindowScale;

        [SerializeField]
        private GUISkin m_Skin;

        //[SerializeField]
        private DebuggerActiveWindowType m_ActiveWindow = DebuggerActiveWindowType.Open;

        //[SerializeField]
        private bool m_ShowFullWindow;

        private bool m_ShowGMPanel;

        private Action<bool> m_showPanelDelegate = null;

        [SerializeField]
        private DebuggerComponent.ConsoleWindow m_ConsoleWindow = new DebuggerComponent.ConsoleWindow();

        private DebuggerComponent.RecorderWindow m_RecorderWindow = new DebuggerComponent.RecorderWindow();

        private DebuggerComponent.SystemInformationWindow m_SystemInformationWindow = new DebuggerComponent.SystemInformationWindow();

        private DebuggerComponent.GraphicsInformationWindow m_GraphicsInformationWindow = new DebuggerComponent.GraphicsInformationWindow();

        private DebuggerComponent.InputSummaryInformationWindow m_InputSummaryInformationWindow = new DebuggerComponent.InputSummaryInformationWindow();

        private DebuggerComponent.InputTouchInformationWindow m_InputTouchInformationWindow = new DebuggerComponent.InputTouchInformationWindow();

        private DebuggerComponent.InputLocationInformationWindow m_InputLocationInformationWindow = new DebuggerComponent.InputLocationInformationWindow();

        private DebuggerComponent.InputAccelerationInformationWindow m_InputAccelerationInformationWindow = new DebuggerComponent.InputAccelerationInformationWindow();

        private DebuggerComponent.InputGyroscopeInformationWindow m_InputGyroscopeInformationWindow = new DebuggerComponent.InputGyroscopeInformationWindow();

        private DebuggerComponent.InputCompassInformationWindow m_InputCompassInformationWindow = new DebuggerComponent.InputCompassInformationWindow();

        private DebuggerComponent.PathInformationWindow m_PathInformationWindow = new DebuggerComponent.PathInformationWindow();

        private DebuggerComponent.SceneInformationWindow m_SceneInformationWindow = new DebuggerComponent.SceneInformationWindow();

        private DebuggerComponent.TimeInformationWindow m_TimeInformationWindow = new DebuggerComponent.TimeInformationWindow();

        private DebuggerComponent.QualityInformationWindow m_QualityInformationWindow = new DebuggerComponent.QualityInformationWindow();

        private DebuggerComponent.ProfilerInformationWindow m_ProfilerInformationWindow = new DebuggerComponent.ProfilerInformationWindow();

        private DebuggerComponent.WebPlayerInformationWindow m_WebPlayerInformationWindow = new DebuggerComponent.WebPlayerInformationWindow();

        private DebuggerComponent.RuntimeMemoryInformationWindow<UnityEngine.Object> m_RuntimeMemoryAllInformationWindow = new DebuggerComponent.RuntimeMemoryInformationWindow<UnityEngine.Object>();

        private DebuggerComponent.RuntimeMemoryInformationWindow<Texture> m_RuntimeMemoryTextureInformationWindow = new DebuggerComponent.RuntimeMemoryInformationWindow<Texture>();

        private DebuggerComponent.RuntimeMemoryInformationWindow<Mesh> m_RuntimeMemoryMeshInformationWindow = new DebuggerComponent.RuntimeMemoryInformationWindow<Mesh>();

        private DebuggerComponent.RuntimeMemoryInformationWindow<Material> m_RuntimeMemoryMaterialInformationWindow = new DebuggerComponent.RuntimeMemoryInformationWindow<Material>();

        private DebuggerComponent.RuntimeMemoryInformationWindow<AnimationClip> m_RuntimeMemoryAnimationClipInformationWindow = new DebuggerComponent.RuntimeMemoryInformationWindow<AnimationClip>();

        private DebuggerComponent.RuntimeMemoryInformationWindow<AudioClip> m_RuntimeMemoryAudioClipInformationWindow = new DebuggerComponent.RuntimeMemoryInformationWindow<AudioClip>();

        private DebuggerComponent.RuntimeMemoryInformationWindow<Font> m_RuntimeMemoryFontInformationWindow = new DebuggerComponent.RuntimeMemoryInformationWindow<Font>();

        private DebuggerComponent.RuntimeMemoryInformationWindow<GameObject> m_RuntimeMemoryGameObjectInformationWindow = new DebuggerComponent.RuntimeMemoryInformationWindow<GameObject>();

        private DebuggerComponent.RuntimeMemoryInformationWindow<Component> m_RuntimeMemoryComponentInformationWindow = new DebuggerComponent.RuntimeMemoryInformationWindow<Component>();

        private DebuggerComponent.RuntimeMemoryInformationWindow<AssetBundle> m_RuntimeMemoryAssetBundleInformationWindow = new DebuggerComponent.RuntimeMemoryInformationWindow<AssetBundle>();

        private DebuggerComponent.ToluaMemoryWindow m_ToluaMemoryWindow = new DebuggerComponent.ToluaMemoryWindow();

        private DebuggerComponent.FpsCounter m_FpsCounter;

        private DebuggerComponent.ToluaMemory m_ToluaMemory;

        public bool ActiveWindow
        {
            get
            {
                return this.m_DebuggerManager.ActiveWindow;
            }
            set
            {
                this.m_DebuggerManager.ActiveWindow = value;
                base.enabled = value;
            }
        }

        public bool ShowFullWindow
        {
            get
            {
                return this.m_ShowFullWindow;
            }
            set
            {
                this.m_ShowFullWindow = value;
            }
        }

        public Rect IconRect
        {
            get
            {
                return this.m_IconRect;
            }
            set
            {
                this.m_IconRect = value;
            }
        }

        public Rect WindowRect
        {
            get
            {
                return this.m_WindowRect;
            }
            set
            {
                this.m_WindowRect = value;
            }
        }

        public float WindowScale
        {
            get
            {
                return this.m_WindowScale;
            }
            set
            {
                this.m_WindowScale = value;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            // 设置屏幕大小
            float rectWidth = Screen.width * 0.8f;
            float rectHeight = Screen.height * 0.8f;
            m_WindowRect = new Rect(m_WindowRect.x, m_WindowRect.y, rectWidth, rectHeight);
            
            m_IconRect.x = Screen.width - 150 - m_IconRect.x;

            this.m_DebuggerManager = GameFrameworkEntry.GetModule<IDebuggerManager>();
            if (this.m_DebuggerManager == null)
            {
                Log.Fatal("Debugger manager is invalid.");
                return;
            }
            if (this.m_ActiveWindow == DebuggerActiveWindowType.Auto)
            {
                //this.ActiveWindow = global::Debug.isDebugBuild;
                this.ActiveWindow = UnityEngine.Debug.isDebugBuild;
            }
            else
            {
                this.ActiveWindow = (this.m_ActiveWindow == DebuggerActiveWindowType.Open);
            }
            this.m_FpsCounter = new DebuggerComponent.FpsCounter(0.5f);
            this.m_ToluaMemory = new DebuggerComponent.ToluaMemory();
            this.GetAllMikeInFields();
            this.m_RecorderWindow.m_Recorder.SetRecorderInterval(1f);
        }

        private void GetAllMikeInFields()
        {
            Type type = base.GetType();
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic);
            List<IRecordMike> list = new List<IRecordMike>();
            FieldInfo[] array = fields;
            for (int i = 0; i < array.Length; i++)
            {
                FieldInfo fieldInfo = array[i];
                object value = fieldInfo.GetValue(this);
                if (value is IRecordMike)
                {
                    list.Add(value as IRecordMike);
                }
            }
            this.m_RecorderWindow.InitRecorder(list);
        }

        private void Start()
        {
            this.RegisterDebuggerWindow("Console", this.m_ConsoleWindow, new object[0]);
            this.RegisterDebuggerWindow("Recorder", this.m_RecorderWindow, new object[0]);
            this.RegisterDebuggerWindow("Information/System", this.m_SystemInformationWindow, new object[0]);
            this.RegisterDebuggerWindow("Information/Graphics", this.m_GraphicsInformationWindow, new object[0]);
            this.RegisterDebuggerWindow("Information/Input/Summary", this.m_InputSummaryInformationWindow, new object[0]);
            this.RegisterDebuggerWindow("Information/Input/Touch", this.m_InputTouchInformationWindow, new object[0]);
            this.RegisterDebuggerWindow("Information/Input/Location", this.m_InputLocationInformationWindow, new object[0]);
            this.RegisterDebuggerWindow("Information/Input/Acceleration", this.m_InputAccelerationInformationWindow, new object[0]);
            this.RegisterDebuggerWindow("Information/Input/Gyroscope", this.m_InputGyroscopeInformationWindow, new object[0]);
            this.RegisterDebuggerWindow("Information/Input/Compass", this.m_InputCompassInformationWindow, new object[0]);
            this.RegisterDebuggerWindow("Information/Other/Scene", this.m_SceneInformationWindow, new object[0]);
            this.RegisterDebuggerWindow("Information/Other/Path", this.m_PathInformationWindow, new object[0]);
            this.RegisterDebuggerWindow("Information/Other/Time", this.m_TimeInformationWindow, new object[0]);
            this.RegisterDebuggerWindow("Information/Other/Quality", this.m_QualityInformationWindow, new object[0]);
            this.RegisterDebuggerWindow("Information/Other/Web Player", this.m_WebPlayerInformationWindow, new object[0]);
            this.RegisterDebuggerWindow("Profiler/ToluaMemory", this.m_ToluaMemoryWindow, new object[]
            {
                this.m_ToluaMemory
            });
            this.RegisterDebuggerWindow("Profiler/Summary", this.m_ProfilerInformationWindow, new object[0]);
            this.RegisterDebuggerWindow("Profiler/Memory/All", this.m_RuntimeMemoryAllInformationWindow, new object[0]);
            this.RegisterDebuggerWindow("Profiler/Memory/Texture", this.m_RuntimeMemoryTextureInformationWindow, new object[0]);
            this.RegisterDebuggerWindow("Profiler/Memory/Mesh", this.m_RuntimeMemoryMeshInformationWindow, new object[0]);
            this.RegisterDebuggerWindow("Profiler/Memory/Material", this.m_RuntimeMemoryMaterialInformationWindow, new object[0]);
            this.RegisterDebuggerWindow("Profiler/Memory/AnimationClip", this.m_RuntimeMemoryAnimationClipInformationWindow, new object[0]);
            this.RegisterDebuggerWindow("Profiler/Memory/AudioClip", this.m_RuntimeMemoryAudioClipInformationWindow, new object[0]);
            this.RegisterDebuggerWindow("Profiler/Memory/Font", this.m_RuntimeMemoryFontInformationWindow, new object[0]);
            this.RegisterDebuggerWindow("Profiler/Memory/GameObject", this.m_RuntimeMemoryGameObjectInformationWindow, new object[0]);
            this.RegisterDebuggerWindow("Profiler/Memory/Component", this.m_RuntimeMemoryComponentInformationWindow, new object[0]);
            this.RegisterDebuggerWindow("Profiler/Memory/AssetBundle", this.m_RuntimeMemoryAssetBundleInformationWindow, new object[0]);
        }

        private void Update()
        {
            this.m_WindowScale = (float)Screen.width / DebuggerComponent.DefaultMaxWindowsWidth;
            this.m_FpsCounter.Update(Time.deltaTime, Time.unscaledDeltaTime);
            this.m_RecorderWindow.m_Recorder.Update(Time.deltaTime, Time.unscaledDeltaTime);
        }

        private void OnGUI()
        {
            if (this.m_DebuggerManager == null || !this.m_DebuggerManager.ActiveWindow)
            {
                return;
            }
            GUISkin skin = GUI.skin;
            GUI.skin = this.m_Skin;
            //Matrix4x4 matrix = GUI.matrix;
            //GUI.matrix = Matrix4x4.Scale(new Vector3(this.m_WindowScale, this.m_WindowScale, 1f));
            if (this.m_ShowFullWindow)
            {
                this.m_WindowRect = GUILayout.Window(1, this.m_WindowRect, new GUI.WindowFunction(this.DrawWindow), "<b>DEBUGGER</b>", new GUILayoutOption[0]);
            }
            else
            {
                this.m_IconRect = GUILayout.Window(1, this.m_IconRect, new GUI.WindowFunction(this.DrawDebuggerWindowIcon), "<b>DEBUGGER</b>", new GUILayoutOption[0]);
            }

            //GUI.matrix = matrix;
            GUI.skin = skin;
        }

        public void RegisterDebuggerWindow(string path, IDebuggerWindow debuggerWindow, params object[] args)
        {
            this.m_DebuggerManager.RegisterDebuggerWindow(path, debuggerWindow, args);
        }

        public IDebuggerWindow GetDebuggerWindow(string path)
        {
            return this.m_DebuggerManager.GetDebuggerWindow(path);
        }

        private void DrawWindow(int windowId)
        {
            GUI.DragWindow(this.m_DragRect);
            this.DrawDebuggerWindowGroup(this.m_DebuggerManager.DebuggerWindowRoot);
        }

        private void DrawDebuggerWindowGroup(IDebuggerWindowGroup debuggerWindowGroup)
        {
            if (debuggerWindowGroup == null)
            {
                return;
            }
            List<string> list = new List<string>();
            string[] debuggerWindowNames = debuggerWindowGroup.GetDebuggerWindowNames();
            for (int i = 0; i < debuggerWindowNames.Length; i++)
            {
                list.Add(string.Format("<b>{0}</b>", debuggerWindowNames[i]));
            }
            if (debuggerWindowGroup == this.m_DebuggerManager.DebuggerWindowRoot)
            {
                list.Add("<b>Close</b>");
            }
            int num = GUILayout.Toolbar(debuggerWindowGroup.SelectedIndex, list.ToArray(), new GUILayoutOption[]
            {
                GUILayout.Height(30f),
                GUILayout.MaxWidth(DebuggerComponent.DefaultMaxToolBarWidth)
            });
            if (num >= debuggerWindowGroup.DebuggerWindowCount)
            {
                this.m_ShowFullWindow = false;
                return;
            }
            if (debuggerWindowGroup.SelectedIndex != num)
            {
                debuggerWindowGroup.SelectedWindow.OnLeave();
                debuggerWindowGroup.SelectedIndex = num;
                debuggerWindowGroup.SelectedWindow.OnEnter();
            }
            IDebuggerWindowGroup debuggerWindowGroup2 = debuggerWindowGroup.SelectedWindow as IDebuggerWindowGroup;
            if (debuggerWindowGroup2 != null)
            {
                this.DrawDebuggerWindowGroup(debuggerWindowGroup2);
            }
            if (debuggerWindowGroup.SelectedWindow != null)
            {
                debuggerWindowGroup.SelectedWindow.OnDraw();
            }
        }

        private void DrawDebuggerWindowIcon(int windowId)
        {
            GUI.DragWindow(this.m_DragRect);
            GUILayout.Space(25f);
            Color32 color = Color.white;
            this.m_ConsoleWindow.RefreshCount();
            if (this.m_ConsoleWindow.FatalCount > 0)
            {
                color = this.m_ConsoleWindow.GetLogStringColor(LogType.Exception);
            }
            else if (this.m_ConsoleWindow.ErrorCount > 0)
            {
                color = this.m_ConsoleWindow.GetLogStringColor(LogType.Error);
            }
            else if (this.m_ConsoleWindow.WarningCount > 0)
            {
                color = this.m_ConsoleWindow.GetLogStringColor(LogType.Warning);
            }
            else
            {
                color = this.m_ConsoleWindow.GetLogStringColor(LogType.Log);
            }

            string textGM = string.Format("<color=#{0}{1}{2}{3}><b>GM</b></color>", new object[]
            {
                color.r.ToString("x2"),
                color.g.ToString("x2"),
                color.b.ToString("x2"),
                color.a.ToString("x2"),
            });
            if (GUILayout.Button(textGM, new GUILayoutOption[]
            {
                GUILayout.Width(100f),
                GUILayout.Height(40f)
            }))
            {
                m_ShowFullWindow = false;

                m_ShowGMPanel = !m_ShowGMPanel;
                if (m_showPanelDelegate != null)
                    m_showPanelDelegate(m_ShowGMPanel);
            }

            string textDebugger = string.Format("<color=#{0}{1}{2}{3}><b>FPS: {4}</b></color>", new object[]
            {
                color.r.ToString("x2"),
                color.g.ToString("x2"),
                color.b.ToString("x2"),
                color.a.ToString("x2"),
                this.m_FpsCounter.CurrentFps.ToString("F2")
            });
            if (GUILayout.Button(textDebugger, new GUILayoutOption[]
            {
                GUILayout.Width(100f),
                GUILayout.Height(40f)
            }))
            {
                m_ShowFullWindow = true;
            }
        }

        public void SetShowPanelDelegate(Action<bool> del)
        {
            m_showPanelDelegate = del;
        }
    }
}
