using Android.App;
using Android.Arch.Lifecycle;
using Android.Util;
using Java.Interop;
using System;

namespace Gabana.Droid
{
    [Application]
    public class GabanaApplication : Application, ILifecycleObserver
    {
        static readonly string TAG = "GabanaApplication";
        static public bool IsForground;

        public GabanaApplication(IntPtr handle, Android.Runtime.JniHandleOwnership ownerShip) : base(handle, ownerShip)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            ProcessLifecycleOwner.Get().Lifecycle.AddObserver(this);
            IsForground = true;
        }

        [Lifecycle.Event.OnStart]
        [Export]
        public void onAppForegrounded()
        {
            Log.Debug(TAG, "App entered foreground state.");
            IsForground = true;
        }

        [Lifecycle.Event.OnStop]
        [Export]
        public void onAppBackgrounded()
        {
            Log.Debug(TAG, "App entered background state.");
            IsForground = false;
        }

    }
}