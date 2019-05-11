using System;
using System.Collections.Generic;

namespace GameFramework.Debugger
{
    internal sealed class DebuggerManager : GameFrameworkModule, IDebuggerManager
    {
        private sealed class DebuggerWindowGroup : IDebuggerWindowGroup, IDebuggerWindow
        {
            private readonly List<KeyValuePair<string, IDebuggerWindow>> m_DebuggerWindows;

            private int m_SelectedIndex;

            private string[] m_DebuggerWindowNames;

            public int DebuggerWindowCount
            {
                get
                {
                    return this.m_DebuggerWindows.Count;
                }
            }

            public int SelectedIndex
            {
                get
                {
                    return this.m_SelectedIndex;
                }
                set
                {
                    this.m_SelectedIndex = value;
                }
            }

            public IDebuggerWindow SelectedWindow
            {
                get
                {
                    if (this.m_SelectedIndex >= this.m_DebuggerWindows.Count)
                    {
                        return null;
                    }
                    return this.m_DebuggerWindows[this.m_SelectedIndex].Value;
                }
            }

            public DebuggerWindowGroup()
            {
                this.m_DebuggerWindows = new List<KeyValuePair<string, IDebuggerWindow>>();
                this.m_SelectedIndex = 0;
                this.m_DebuggerWindowNames = null;
            }

            public void Initialize(params object[] args)
            {
            }

            public void Shutdown()
            {
                foreach (KeyValuePair<string, IDebuggerWindow> current in this.m_DebuggerWindows)
                {
                    current.Value.Shutdown();
                }
                this.m_DebuggerWindows.Clear();
            }

            public void OnEnter()
            {
                this.SelectedWindow.OnEnter();
            }

            public void OnLeave()
            {
                this.SelectedWindow.OnLeave();
            }

            public void OnUpdate(float elapseSeconds, float realElapseSeconds)
            {
                this.SelectedWindow.OnUpdate(elapseSeconds, realElapseSeconds);
            }

            public void OnDraw()
            {
            }

            private void RefreshDebuggerWindowNames()
            {
                this.m_DebuggerWindowNames = new string[this.m_DebuggerWindows.Count];
                int num = 0;
                foreach (KeyValuePair<string, IDebuggerWindow> current in this.m_DebuggerWindows)
                {
                    this.m_DebuggerWindowNames[num++] = current.Key;
                }
            }

            public string[] GetDebuggerWindowNames()
            {
                return this.m_DebuggerWindowNames;
            }

            public IDebuggerWindow GetDebuggerWindow(string path)
            {
                if (string.IsNullOrEmpty(path))
                {
                    return null;
                }
                int num = path.IndexOf('/');
                if (num < 0 || num >= path.Length - 1)
                {
                    return this.InternalGetDebuggerWindow(path);
                }
                string name = path.Substring(0, num);
                string path2 = path.Substring(num + 1);
                DebuggerManager.DebuggerWindowGroup debuggerWindowGroup = (DebuggerManager.DebuggerWindowGroup)this.InternalGetDebuggerWindow(name);
                if (debuggerWindowGroup == null)
                {
                    return null;
                }
                return debuggerWindowGroup.GetDebuggerWindow(path2);
            }

            public bool SelectDebuggerWindow(string path)
            {
                if (string.IsNullOrEmpty(path))
                {
                    return false;
                }
                int num = path.IndexOf('/');
                if (num < 0 || num >= path.Length - 1)
                {
                    return this.InternalSelectDebuggerWindow(path);
                }
                string name = path.Substring(0, num);
                string path2 = path.Substring(num + 1);
                DebuggerManager.DebuggerWindowGroup debuggerWindowGroup = (DebuggerManager.DebuggerWindowGroup)this.InternalGetDebuggerWindow(name);
                return debuggerWindowGroup != null && this.InternalSelectDebuggerWindow(name) && debuggerWindowGroup.SelectDebuggerWindow(path2);
            }

            public void RegisterDebuggerWindow(string path, IDebuggerWindow debuggerWindow)
            {
                if (string.IsNullOrEmpty(path))
                {
                    throw new GameFrameworkException("Path is invalid.");
                }
                int num = path.IndexOf('/');
                if (num < 0 || num >= path.Length - 1)
                {
                    if (this.InternalGetDebuggerWindow(path) != null)
                    {
                        throw new GameFrameworkException("Debugger window has been registered.");
                    }
                    this.m_DebuggerWindows.Add(new KeyValuePair<string, IDebuggerWindow>(path, debuggerWindow));
                    this.RefreshDebuggerWindowNames();
                }
                else
                {
                    string text = path.Substring(0, num);
                    string path2 = path.Substring(num + 1);
                    DebuggerManager.DebuggerWindowGroup debuggerWindowGroup = (DebuggerManager.DebuggerWindowGroup)this.InternalGetDebuggerWindow(text);
                    if (debuggerWindowGroup == null)
                    {
                        if (this.InternalGetDebuggerWindow(text) != null)
                        {
                            throw new GameFrameworkException("Debugger window has been registered, can not create debugger window group.");
                        }
                        debuggerWindowGroup = new DebuggerManager.DebuggerWindowGroup();
                        this.m_DebuggerWindows.Add(new KeyValuePair<string, IDebuggerWindow>(text, debuggerWindowGroup));
                        this.RefreshDebuggerWindowNames();
                    }
                    debuggerWindowGroup.RegisterDebuggerWindow(path2, debuggerWindow);
                }
            }

            private IDebuggerWindow InternalGetDebuggerWindow(string name)
            {
                foreach (KeyValuePair<string, IDebuggerWindow> current in this.m_DebuggerWindows)
                {
                    if (current.Key == name)
                    {
                        return current.Value;
                    }
                }
                return null;
            }

            private bool InternalSelectDebuggerWindow(string name)
            {
                for (int i = 0; i < this.m_DebuggerWindows.Count; i++)
                {
                    if (this.m_DebuggerWindows[i].Key == name)
                    {
                        this.m_SelectedIndex = i;
                        return true;
                    }
                }
                return false;
            }
        }

        private readonly DebuggerManager.DebuggerWindowGroup m_DebuggerWindowRoot;

        private bool m_ActiveWindow;

        internal override int Priority
        {
            get
            {
                return -1;
            }
        }

        public bool ActiveWindow
        {
            get
            {
                return this.m_ActiveWindow;
            }
            set
            {
                this.m_ActiveWindow = value;
            }
        }

        public IDebuggerWindowGroup DebuggerWindowRoot
        {
            get
            {
                return this.m_DebuggerWindowRoot;
            }
        }

        public DebuggerManager()
        {
            this.m_DebuggerWindowRoot = new DebuggerManager.DebuggerWindowGroup();
            this.m_ActiveWindow = false;
        }

        internal override void Update(float elapseSeconds, float realElapseSeconds)
        {
            if (!this.m_ActiveWindow)
            {
                return;
            }
            this.m_DebuggerWindowRoot.OnUpdate(elapseSeconds, realElapseSeconds);
        }

        internal override void Shutdown()
        {
            this.m_ActiveWindow = false;
            this.m_DebuggerWindowRoot.Shutdown();
        }

        public void RegisterDebuggerWindow(string path, IDebuggerWindow debuggerWindow, params object[] args)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new GameFrameworkException("Path is invalid.");
            }
            if (debuggerWindow == null)
            {
                throw new GameFrameworkException("Debugger window is invalid.");
            }
            this.m_DebuggerWindowRoot.RegisterDebuggerWindow(path, debuggerWindow);
            debuggerWindow.Initialize(args);
        }

        public IDebuggerWindow GetDebuggerWindow(string path)
        {
            return this.m_DebuggerWindowRoot.GetDebuggerWindow(path);
        }

        public bool SelectDebuggerWindow(string path)
        {
            return this.m_DebuggerWindowRoot.SelectDebuggerWindow(path);
        }
    }
}
