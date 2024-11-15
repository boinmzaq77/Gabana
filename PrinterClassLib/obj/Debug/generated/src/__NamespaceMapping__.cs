using System;

[assembly:global::Android.Runtime.NamespaceMapping (Java = "com.lvrenyang.io", Managed="Com.Lvrenyang.IO")]

delegate void _JniMarshal_PP_V (IntPtr jnienv, IntPtr klass);
delegate bool _JniMarshal_PP_Z (IntPtr jnienv, IntPtr klass);
delegate void _JniMarshal_PPI_V (IntPtr jnienv, IntPtr klass, int p0);
delegate bool _JniMarshal_PPI_Z (IntPtr jnienv, IntPtr klass, int p0);
delegate void _JniMarshal_PPII_V (IntPtr jnienv, IntPtr klass, int p0, int p1);
delegate void _JniMarshal_PPIIIII_V (IntPtr jnienv, IntPtr klass, int p0, int p1, int p2, int p3, int p4);
delegate void _JniMarshal_PPIIIIII_V (IntPtr jnienv, IntPtr klass, int p0, int p1, int p2, int p3, int p4, int p5);
delegate void _JniMarshal_PPIIIIIIIL_V (IntPtr jnienv, IntPtr klass, int p0, int p1, int p2, int p3, int p4, int p5, int p6, IntPtr p7);
delegate void _JniMarshal_PPIIIIIIL_V (IntPtr jnienv, IntPtr klass, int p0, int p1, int p2, int p3, int p4, int p5, IntPtr p6);
delegate void _JniMarshal_PPIIIIIL_V (IntPtr jnienv, IntPtr klass, int p0, int p1, int p2, int p3, int p4, IntPtr p5);
delegate void _JniMarshal_PPIIIIL_V (IntPtr jnienv, IntPtr klass, int p0, int p1, int p2, int p3, IntPtr p4);
delegate int _JniMarshal_PPIIIILII_I (IntPtr jnienv, IntPtr klass, int p0, int p1, int p2, int p3, IntPtr p4, int p5, int p6);
delegate void _JniMarshal_PPL_V (IntPtr jnienv, IntPtr klass, IntPtr p0);
delegate bool _JniMarshal_PPL_Z (IntPtr jnienv, IntPtr klass, IntPtr p0);
delegate bool _JniMarshal_PPLI_Z (IntPtr jnienv, IntPtr klass, IntPtr p0, int p1);
delegate int _JniMarshal_PPLII_I (IntPtr jnienv, IntPtr klass, IntPtr p0, int p1, int p2);
delegate void _JniMarshal_PPLII_V (IntPtr jnienv, IntPtr klass, IntPtr p0, int p1, int p2);
delegate bool _JniMarshal_PPLII_Z (IntPtr jnienv, IntPtr klass, IntPtr p0, int p1, int p2);
delegate int _JniMarshal_PPLIII_I (IntPtr jnienv, IntPtr klass, IntPtr p0, int p1, int p2, int p3);
delegate void _JniMarshal_PPLIII_V (IntPtr jnienv, IntPtr klass, IntPtr p0, int p1, int p2, int p3);
delegate void _JniMarshal_PPLIIIIII_V (IntPtr jnienv, IntPtr klass, IntPtr p0, int p1, int p2, int p3, int p4, int p5, int p6);
delegate bool _JniMarshal_PPLL_Z (IntPtr jnienv, IntPtr klass, IntPtr p0, IntPtr p1);
delegate void _JniMarshal_PPLLIIIII_V (IntPtr jnienv, IntPtr klass, IntPtr p0, IntPtr p1, int p2, int p3, int p4, int p5, int p6);
#if !NET
namespace System.Runtime.Versioning {
    [System.Diagnostics.Conditional("NEVER")]
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Constructor | AttributeTargets.Enum | AttributeTargets.Event | AttributeTargets.Field | AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Module | AttributeTargets.Property | AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
    internal sealed class SupportedOSPlatformAttribute : Attribute {
        public SupportedOSPlatformAttribute (string platformName) { }
    }
}
#endif

