using System;

namespace GameFramework.Debugger
{
    public interface IDebuggerWindowGroup : IDebuggerWindow
    {
        int DebuggerWindowCount
        {
            get;
        }

        int SelectedIndex
        {
            get;
            set;
        }

        IDebuggerWindow SelectedWindow
        {
            get;
        }

        string[] GetDebuggerWindowNames();

        IDebuggerWindow GetDebuggerWindow(string path);

        void RegisterDebuggerWindow(string path, IDebuggerWindow debuggerWindow);
    }
}
