using System;
using System.Collections.Generic;
using Android.Runtime;
using Java.Interop;

namespace Com.Lvrenyang.IO {

	// Metadata.xml XPath interface reference: path="/api/package[@name='com.lvrenyang.io']/interface[@name='IOCallBack']"
	[Register ("com/lvrenyang/io/IOCallBack", "", "Com.Lvrenyang.IO.IOCallBackInvoker")]
	public partial interface IOCallBack : IJavaObject, IJavaPeerable {
		// Metadata.xml XPath method reference: path="/api/package[@name='com.lvrenyang.io']/interface[@name='IOCallBack']/method[@name='OnClose' and count(parameter)=0]"
		[Register ("OnClose", "()V", "GetOnCloseHandler:Com.Lvrenyang.IO.IOCallBackInvoker, PrinterClassLib")]
		void OnClose ();

		// Metadata.xml XPath method reference: path="/api/package[@name='com.lvrenyang.io']/interface[@name='IOCallBack']/method[@name='OnOpen' and count(parameter)=0]"
		[Register ("OnOpen", "()V", "GetOnOpenHandler:Com.Lvrenyang.IO.IOCallBackInvoker, PrinterClassLib")]
		void OnOpen ();

	}

	[global::Android.Runtime.Register ("com/lvrenyang/io/IOCallBack", DoNotGenerateAcw=true)]
	internal partial class IOCallBackInvoker : global::Java.Lang.Object, IOCallBack {
		static readonly JniPeerMembers _members = new XAPeerMembers ("com/lvrenyang/io/IOCallBack", typeof (IOCallBackInvoker));

		static IntPtr java_class_ref {
			get { return _members.JniPeerType.PeerReference.Handle; }
		}

		[global::System.Diagnostics.DebuggerBrowsable (global::System.Diagnostics.DebuggerBrowsableState.Never)]
		[global::System.ComponentModel.EditorBrowsable (global::System.ComponentModel.EditorBrowsableState.Never)]
		public override global::Java.Interop.JniPeerMembers JniPeerMembers {
			get { return _members; }
		}

		[global::System.Diagnostics.DebuggerBrowsable (global::System.Diagnostics.DebuggerBrowsableState.Never)]
		[global::System.ComponentModel.EditorBrowsable (global::System.ComponentModel.EditorBrowsableState.Never)]
		protected override IntPtr ThresholdClass {
			get { return class_ref; }
		}

		[global::System.Diagnostics.DebuggerBrowsable (global::System.Diagnostics.DebuggerBrowsableState.Never)]
		[global::System.ComponentModel.EditorBrowsable (global::System.ComponentModel.EditorBrowsableState.Never)]
		protected override global::System.Type ThresholdType {
			get { return _members.ManagedPeerType; }
		}

		IntPtr class_ref;

		public static IOCallBack GetObject (IntPtr handle, JniHandleOwnership transfer)
		{
			return global::Java.Lang.Object.GetObject<IOCallBack> (handle, transfer);
		}

		static IntPtr Validate (IntPtr handle)
		{
			if (!JNIEnv.IsInstanceOf (handle, java_class_ref))
				throw new InvalidCastException ($"Unable to convert instance of type '{JNIEnv.GetClassNameFromInstance (handle)}' to type 'com.lvrenyang.io.IOCallBack'.");
			return handle;
		}

		protected override void Dispose (bool disposing)
		{
			if (this.class_ref != IntPtr.Zero)
				JNIEnv.DeleteGlobalRef (this.class_ref);
			this.class_ref = IntPtr.Zero;
			base.Dispose (disposing);
		}

		public IOCallBackInvoker (IntPtr handle, JniHandleOwnership transfer) : base (Validate (handle), transfer)
		{
			IntPtr local_ref = JNIEnv.GetObjectClass (((global::Java.Lang.Object) this).Handle);
			this.class_ref = JNIEnv.NewGlobalRef (local_ref);
			JNIEnv.DeleteLocalRef (local_ref);
		}

		static Delegate cb_OnClose;
#pragma warning disable 0169
		static Delegate GetOnCloseHandler ()
		{
			if (cb_OnClose == null)
				cb_OnClose = JNINativeWrapper.CreateDelegate (new _JniMarshal_PP_V (n_OnClose));
			return cb_OnClose;
		}

		static void n_OnClose (IntPtr jnienv, IntPtr native__this)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Com.Lvrenyang.IO.IOCallBack> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.OnClose ();
		}
#pragma warning restore 0169

		IntPtr id_OnClose;
		public unsafe void OnClose ()
		{
			if (id_OnClose == IntPtr.Zero)
				id_OnClose = JNIEnv.GetMethodID (class_ref, "OnClose", "()V");
			JNIEnv.CallVoidMethod (((global::Java.Lang.Object) this).Handle, id_OnClose);
		}

		static Delegate cb_OnOpen;
#pragma warning disable 0169
		static Delegate GetOnOpenHandler ()
		{
			if (cb_OnOpen == null)
				cb_OnOpen = JNINativeWrapper.CreateDelegate (new _JniMarshal_PP_V (n_OnOpen));
			return cb_OnOpen;
		}

		static void n_OnOpen (IntPtr jnienv, IntPtr native__this)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Com.Lvrenyang.IO.IOCallBack> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.OnOpen ();
		}
#pragma warning restore 0169

		IntPtr id_OnOpen;
		public unsafe void OnOpen ()
		{
			if (id_OnOpen == IntPtr.Zero)
				id_OnOpen = JNIEnv.GetMethodID (class_ref, "OnOpen", "()V");
			JNIEnv.CallVoidMethod (((global::Java.Lang.Object) this).Handle, id_OnOpen);
		}

	}
}
