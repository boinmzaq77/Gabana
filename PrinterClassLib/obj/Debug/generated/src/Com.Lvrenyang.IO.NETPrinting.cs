//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

#nullable restore
using System;
using System.Collections.Generic;
using Android.Runtime;
using Java.Interop;

namespace Com.Lvrenyang.IO {

	// Metadata.xml XPath class reference: path="/api/package[@name='com.lvrenyang.io']/class[@name='NETPrinting']"
	[global::Android.Runtime.Register ("com/lvrenyang/io/NETPrinting", DoNotGenerateAcw=true)]
	public partial class NETPrinting : global::Com.Lvrenyang.IO.IO {
		static readonly JniPeerMembers _members = new XAPeerMembers ("com/lvrenyang/io/NETPrinting", typeof (NETPrinting));

		internal static new IntPtr class_ref {
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
			get { return _members.JniPeerType.PeerReference.Handle; }
		}

		[global::System.Diagnostics.DebuggerBrowsable (global::System.Diagnostics.DebuggerBrowsableState.Never)]
		[global::System.ComponentModel.EditorBrowsable (global::System.ComponentModel.EditorBrowsableState.Never)]
		protected override global::System.Type ThresholdType {
			get { return _members.ManagedPeerType; }
		}

		protected NETPrinting (IntPtr javaReference, JniHandleOwnership transfer) : base (javaReference, transfer)
		{
		}

		// Metadata.xml XPath constructor reference: path="/api/package[@name='com.lvrenyang.io']/class[@name='NETPrinting']/constructor[@name='NETPrinting' and count(parameter)=0]"
		[Register (".ctor", "()V", "")]
		public unsafe NETPrinting () : base (IntPtr.Zero, JniHandleOwnership.DoNotTransfer)
		{
			const string __id = "()V";

			if (((global::Java.Lang.Object) this).Handle != IntPtr.Zero)
				return;

			try {
				var __r = _members.InstanceMethods.StartCreateInstance (__id, ((object) this).GetType (), null);
				SetHandle (__r.Handle, JniHandleOwnership.TransferLocalRef);
				_members.InstanceMethods.FinishCreateInstance (__id, this, null);
			} finally {
			}
		}

		static Delegate cb_Close;
#pragma warning disable 0169
		static Delegate GetCloseHandler ()
		{
			if (cb_Close == null)
				cb_Close = JNINativeWrapper.CreateDelegate (new _JniMarshal_PP_V (n_Close));
			return cb_Close;
		}

		static void n_Close (IntPtr jnienv, IntPtr native__this)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Com.Lvrenyang.IO.NETPrinting> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.Close ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='com.lvrenyang.io']/class[@name='NETPrinting']/method[@name='Close' and count(parameter)=0]"
		[Register ("Close", "()V", "GetCloseHandler")]
		public virtual unsafe void Close ()
		{
			const string __id = "Close.()V";
			try {
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, null);
			} finally {
			}
		}

		static Delegate cb_Open_Ljava_lang_String_I;
#pragma warning disable 0169
		static Delegate GetOpen_Ljava_lang_String_IHandler ()
		{
			if (cb_Open_Ljava_lang_String_I == null)
				cb_Open_Ljava_lang_String_I = JNINativeWrapper.CreateDelegate (new _JniMarshal_PPLI_Z (n_Open_Ljava_lang_String_I));
			return cb_Open_Ljava_lang_String_I;
		}

		static bool n_Open_Ljava_lang_String_I (IntPtr jnienv, IntPtr native__this, IntPtr native_IPAddress, int PortNumber)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Com.Lvrenyang.IO.NETPrinting> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			var IPAddress = JNIEnv.GetString (native_IPAddress, JniHandleOwnership.DoNotTransfer);
			bool __ret = __this.Open (IPAddress, PortNumber);
			return __ret;
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='com.lvrenyang.io']/class[@name='NETPrinting']/method[@name='Open' and count(parameter)=2 and parameter[1][@type='java.lang.String'] and parameter[2][@type='int']]"
		[Register ("Open", "(Ljava/lang/String;I)Z", "GetOpen_Ljava_lang_String_IHandler")]
		public virtual unsafe bool Open (string IPAddress, int PortNumber)
		{
			const string __id = "Open.(Ljava/lang/String;I)Z";
			IntPtr native_IPAddress = JNIEnv.NewString ((string)IPAddress);
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [2];
				__args [0] = new JniArgumentValue (native_IPAddress);
				__args [1] = new JniArgumentValue (PortNumber);
				var __rm = _members.InstanceMethods.InvokeVirtualBooleanMethod (__id, this, __args);
				return __rm;
			} finally {
				JNIEnv.DeleteLocalRef (native_IPAddress);
			}
		}

		static Delegate cb_SetCallBack_Lcom_lvrenyang_io_IOCallBack_;
#pragma warning disable 0169
		static Delegate GetSetCallBack_Lcom_lvrenyang_io_IOCallBack_Handler ()
		{
			if (cb_SetCallBack_Lcom_lvrenyang_io_IOCallBack_ == null)
				cb_SetCallBack_Lcom_lvrenyang_io_IOCallBack_ = JNINativeWrapper.CreateDelegate (new _JniMarshal_PPL_V (n_SetCallBack_Lcom_lvrenyang_io_IOCallBack_));
			return cb_SetCallBack_Lcom_lvrenyang_io_IOCallBack_;
		}

		static void n_SetCallBack_Lcom_lvrenyang_io_IOCallBack_ (IntPtr jnienv, IntPtr native__this, IntPtr native_callBack)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Com.Lvrenyang.IO.NETPrinting> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			var callBack = (global::Com.Lvrenyang.IO.IOCallBack)global::Java.Lang.Object.GetObject<global::Com.Lvrenyang.IO.IOCallBack> (native_callBack, JniHandleOwnership.DoNotTransfer);
			__this.SetCallBack (callBack);
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='com.lvrenyang.io']/class[@name='NETPrinting']/method[@name='SetCallBack' and count(parameter)=1 and parameter[1][@type='com.lvrenyang.io.IOCallBack']]"
		[Register ("SetCallBack", "(Lcom/lvrenyang/io/IOCallBack;)V", "GetSetCallBack_Lcom_lvrenyang_io_IOCallBack_Handler")]
		public virtual unsafe void SetCallBack (global::Com.Lvrenyang.IO.IOCallBack callBack)
		{
			const string __id = "SetCallBack.(Lcom/lvrenyang/io/IOCallBack;)V";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [1];
				__args [0] = new JniArgumentValue ((callBack == null) ? IntPtr.Zero : ((global::Java.Lang.Object) callBack).Handle);
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, __args);
			} finally {
				global::System.GC.KeepAlive (callBack);
			}
		}

	}
}
