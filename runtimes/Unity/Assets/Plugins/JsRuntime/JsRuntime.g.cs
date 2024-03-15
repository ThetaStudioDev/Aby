#if !UNITY_WEBGL
// <auto-generated>
// This code is generated by csbindgen.
// DON'T CHANGE THIS DIRECTLY.
// </auto-generated>
#pragma warning disable CS8500
#pragma warning disable CS8981
using System;
using System.Runtime.InteropServices;


namespace Theta.Unity.Runtime
{
    public static unsafe partial class JsRuntime
    {
#if UNITY_IOS && !UNITY_EDITOR
        const string __DllName = "__Internal";
#else
        const string __DllName = "JsRuntime";
#endif
        



        /// <summary>Initialize a global static `JsRuntime`` instance.  Use this when you want to create a single, managed instance of Deno's `MainWorker` for use in another managed environment.</summary>
        [DllImport(__DllName, EntryPoint = "js_runtime__bootstrap", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern CBootstrapResult bootstrap(CBootstrapOptions options);

        /// <summary>TODO</summary>
        [DllImport(__DllName, EntryPoint = "js_runtime__verify_log_callback", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void verify_log_callback(verify_log_callback__cb_delegate _cb);

        /// <summary>TODO</summary>
        [DllImport(__DllName, EntryPoint = "js_runtime__get_state", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern CJsRuntimeState get_state();

        /// <summary>TODO: Return a CJsRuntimeStartResult (repr(C)) for state.</summary>
        [DllImport(__DllName, EntryPoint = "js_runtime__start", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern CStartResult start(CStartOptions options);


    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe partial struct CBootstrapOptions
    {
        public byte* thread_prefix;
        public CJsRuntimeConfig js_runtime_config;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe partial struct CJsRuntimeConfig
    {
        public byte* db_dir;
        public byte* log_dir;
        public CJsRuntimeLogLevel log_level;
        public void* log_callback_fn;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe partial struct CStartOptions
    {
        public byte* main_module_specifier;
    }


    public enum CBootstrapResult : uint
    {
        Ok = 0,
        UnknownError = 1,
        JsRuntimeMissing = 2,
        JsRuntimeFailed = 3,
        LogCaptureFailed = 4,
    }

    public enum CJsRuntimeLogLevel : uint
    {
        None = 0,
        Error = 1,
        Warning = 2,
        Info = 3,
        Debug = 4,
        Trace = 5,
    }

    public enum CJsRuntimeState : uint
    {
        None = 0,
        Cold = 1,
        Startup = 2,
        Warm = 3,
        Failed = 4,
        Panic = 5,
        Shutdown = 6,
    }

    public enum CStartResult : uint
    {
        Ok = 0,
        Err = 1,
        BindingErr = 2,
        JsRuntimeErr = 3,
    }


}
#endif