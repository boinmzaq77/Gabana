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

	// Metadata.xml XPath class reference: path="/api/package[@name='com.lvrenyang.io']/class[@name='IO']"
	[global::Android.Runtime.Register ("com/lvrenyang/io/IO", DoNotGenerateAcw=true)]
	public partial class IO : global::Java.Lang.Object {
		static readonly JniPeerMembers _members = new XAPeerMembers ("com/lvrenyang/io/IO", typeof (IO));

		internal static IntPtr class_ref {
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

		protected IO (IntPtr javaReference, JniHandleOwnership transfer) : base (javaReference, transfer)
		{
		}

		// Metadata.xml XPath constructor reference: path="/api/package[@name='com.lvrenyang.io']/class[@name='IO']/constructor[@name='IO' and count(parameter)=0]"
		[Register (".ctor", "()V", "")]
		public unsafe IO () : base (IntPtr.Zero, JniHandleOwnership.DoNotTransfer)
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

		static Delegate cb_IsOpened;
#pragma warning disable 0169
		static Delegate GetIsOpenedHandler ()
		{
			if (cb_IsOpened == null)
				cb_IsOpened = JNINativeWrapper.CreateDelegate (new _JniMarshal_PP_Z (n_IsOpened));
			return cb_IsOpened;
		}

		static bool n_IsOpened (IntPtr jnienv, IntPtr native__this)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Com.Lvrenyang.IO.IO> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			return __this.IsOpened;
		}
#pragma warning restore 0169

		public virtual unsafe bool IsOpened {
			// Metadata.xml XPath method reference: path="/api/package[@name='com.lvrenyang.io']/class[@name='IO']/method[@name='IsOpened' and count(parameter)=0]"
			[Register ("IsOpened", "()Z", "GetIsOpenedHandler")]
			get {
				const string __id = "IsOpened.()Z";
				try {
					var __rm = _members.InstanceMethods.InvokeVirtualBooleanMethod (__id, this, null);
					return __rm;
				} finally {
				}
			}
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
			var __this = global::Java.Lang.Object.GetObject<global::Com.Lvrenyang.IO.IO> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.OnClose ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='com.lvrenyang.io']/class[@name='IO']/method[@name='OnClose' and count(parameter)=0]"
		[Register ("OnClose", "()V", "GetOnCloseHandler")]
		protected virtual unsafe void OnClose ()
		{
			const string __id = "OnClose.()V";
			try {
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, null);
			} finally {
			}
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
			var __this = global::Java.Lang.Object.GetObject<global::Com.Lvrenyang.IO.IO> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.OnOpen ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='com.lvrenyang.io']/class[@name='IO']/method[@name='OnOpen' and count(parameter)=0]"
		[Register ("OnOpen", "()V", "GetOnOpenHandler")]
		protected virtual unsafe void OnOpen ()
		{
			const string __id = "OnOpen.()V";
			try {
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, null);
			} finally {
			}
		}

		static Delegate cb_PauseHeartBeat;
#pragma warning disable 0169
		static Delegate GetPauseHeartBeatHandler ()
		{
			if (cb_PauseHeartBeat == null)
				cb_PauseHeartBeat = JNINativeWrapper.CreateDelegate (new _JniMarshal_PP_V (n_PauseHeartBeat));
			return cb_PauseHeartBeat;
		}

		static void n_PauseHeartBeat (IntPtr jnienv, IntPtr native__this)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Com.Lvrenyang.IO.IO> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.PauseHeartBeat ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='com.lvrenyang.io']/class[@name='IO']/method[@name='PauseHeartBeat' and count(parameter)=0]"
		[Register ("PauseHeartBeat", "()V", "GetPauseHeartBeatHandler")]
		public virtual unsafe void PauseHeartBeat ()
		{
			const string __id = "PauseHeartBeat.()V";
			try {
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, null);
			} finally {
			}
		}

		static Delegate cb_Read_arrayBIII;
#pragma warning disable 0169
		static Delegate GetRead_arrayBIIIHandler ()
		{
			if (cb_Read_arrayBIII == null)
				cb_Read_arrayBIII = JNINativeWrapper.CreateDelegate (new _JniMarshal_PPLIII_I (n_Read_arrayBIII));
			return cb_Read_arrayBIII;
		}

		static int n_Read_arrayBIII (IntPtr jnienv, IntPtr native__this, IntPtr native_buffer, int offset, int count, int timeout)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Com.Lvrenyang.IO.IO> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			var buffer = (byte[]) JNIEnv.GetArray (native_buffer, JniHandleOwnership.DoNotTransfer, typeof (byte));
			int __ret = __this.Read (buffer, offset, count, timeout);
			if (buffer != null)
				JNIEnv.CopyArray (buffer, native_buffer);
			return __ret;
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='com.lvrenyang.io']/class[@name='IO']/method[@name='Read' and count(parameter)=4 and parameter[1][@type='byte[]'] and parameter[2][@type='int'] and parameter[3][@type='int'] and parameter[4][@type='int']]"
		[Register ("Read", "([BIII)I", "GetRead_arrayBIIIHandler")]
		public virtual unsafe int Read (byte[] buffer, int offset, int count, int timeout)
		{
			const string __id = "Read.([BIII)I";
			IntPtr native_buffer = JNIEnv.NewArray (buffer);
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [4];
				__args [0] = new JniArgumentValue (native_buffer);
				__args [1] = new JniArgumentValue (offset);
				__args [2] = new JniArgumentValue (count);
				__args [3] = new JniArgumentValue (timeout);
				var __rm = _members.InstanceMethods.InvokeVirtualInt32Method (__id, this, __args);
				return __rm;
			} finally {
				if (buffer != null) {
					JNIEnv.CopyArray (native_buffer, buffer);
					JNIEnv.DeleteLocalRef (native_buffer);
				}
				global::System.GC.KeepAlive (buffer);
			}
		}

		static Delegate cb_ResumeHeartBeat;
#pragma warning disable 0169
		static Delegate GetResumeHeartBeatHandler ()
		{
			if (cb_ResumeHeartBeat == null)
				cb_ResumeHeartBeat = JNINativeWrapper.CreateDelegate (new _JniMarshal_PP_V (n_ResumeHeartBeat));
			return cb_ResumeHeartBeat;
		}

		static void n_ResumeHeartBeat (IntPtr jnienv, IntPtr native__this)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Com.Lvrenyang.IO.IO> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			__this.ResumeHeartBeat ();
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='com.lvrenyang.io']/class[@name='IO']/method[@name='ResumeHeartBeat' and count(parameter)=0]"
		[Register ("ResumeHeartBeat", "()V", "GetResumeHeartBeatHandler")]
		public virtual unsafe void ResumeHeartBeat ()
		{
			const string __id = "ResumeHeartBeat.()V";
			try {
				_members.InstanceMethods.InvokeVirtualVoidMethod (__id, this, null);
			} finally {
			}
		}

		static Delegate cb_Write_arrayBII;
#pragma warning disable 0169
		static Delegate GetWrite_arrayBIIHandler ()
		{
			if (cb_Write_arrayBII == null)
				cb_Write_arrayBII = JNINativeWrapper.CreateDelegate (new _JniMarshal_PPLII_I (n_Write_arrayBII));
			return cb_Write_arrayBII;
		}

		static int n_Write_arrayBII (IntPtr jnienv, IntPtr native__this, IntPtr native_buffer, int offset, int count)
		{
			var __this = global::Java.Lang.Object.GetObject<global::Com.Lvrenyang.IO.IO> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			var buffer = (byte[]) JNIEnv.GetArray (native_buffer, JniHandleOwnership.DoNotTransfer, typeof (byte));
			int __ret = __this.Write (buffer, offset, count);
			if (buffer != null)
				JNIEnv.CopyArray (buffer, native_buffer);
			return __ret;
		}
#pragma warning restore 0169

		// Metadata.xml XPath method reference: path="/api/package[@name='com.lvrenyang.io']/class[@name='IO']/method[@name='Write' and count(parameter)=3 and parameter[1][@type='byte[]'] and parameter[2][@type='int'] and parameter[3][@type='int']]"
		[Register ("Write", "([BII)I", "GetWrite_arrayBIIHandler")]
		public virtual unsafe int Write (byte[] buffer, int offset, int count)
		{
			const string __id = "Write.([BII)I";
			IntPtr native_buffer = JNIEnv.NewArray (buffer);
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [3];
				__args [0] = new JniArgumentValue (native_buffer);
				__args [1] = new JniArgumentValue (offset);
				__args [2] = new JniArgumentValue (count);
				var __rm = _members.InstanceMethods.InvokeVirtualInt32Method (__id, this, __args);
				return __rm;
			} finally {
				if (buffer != null) {
					JNIEnv.CopyArray (native_buffer, buffer);
					JNIEnv.DeleteLocalRef (native_buffer);
				}
				global::System.GC.KeepAlive (buffer);
			}
		}

	}
}
