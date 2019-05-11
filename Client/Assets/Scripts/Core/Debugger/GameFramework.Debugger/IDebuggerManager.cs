using System;

namespace GameFramework.Debugger
{
    public interface IDebuggerManager
    {
        bool ActiveWindow
        {
            get;
            set;
        }

        IDebuggerWindowGroup DebuggerWindowRoot
        {
            get;
        }

        void RegisterDebuggerWindow(string path, IDebuggerWindow debuggerWindow, params object[] args);

        IDebuggerWindow GetDebuggerWindow(string path);

        bool SelectDebuggerWindow(string path);
    }
}
