using System;
using System.Diagnostics;

namespace MystiickCore;

public static class Debugger
{
    private static readonly object locker = new object();
    private static bool _showDebugInfo;

    public static bool ShowDebugInfo
    {
        get
        {
            lock (locker)
            {
                return _showDebugInfo;
            }
        }
        set
        {
            lock (locker)
            {
                _showDebugInfo = value;
            }
        }
    }

    [DebuggerStepThrough]
    public static void Break()
    {
#if DEBUG
        if (System.Diagnostics.Debugger.IsAttached)
        {
            System.Diagnostics.Debugger.Break();
        }
#endif
    }

    [DebuggerStepThrough]
    public static void Break(bool value)
    {
#if DEBUG
        if (value)
        {
            Break();
        }
#endif
    }

    [DebuggerStepThrough]
    public static void Break(Func<bool> value)
    {
#if DEBUG
        if (value.Invoke())
        {
            Break();
        }
#endif
    }
}
