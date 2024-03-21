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
        [DllImport(__DllName, EntryPoint = "aby__bootstrap", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern CBootstrapResult c_bootstrap(CBootstrapOptions options);

        /// <summary>TODO</summary>
        [DllImport(__DllName, EntryPoint = "aby__verify_log_callback", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void c_verify_log_callback(c_verify_log_callback__cb_delegate _cb);

        [DllImport(__DllName, EntryPoint = "aby__construct_runtime", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern CJsRuntime* c_construct_runtime(CJsRuntimeConfig config);

        [DllImport(__DllName, EntryPoint = "aby__send_broadcast", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void c_send_broadcast(CJsRuntime* cself, uint message);

        [DllImport(__DllName, EntryPoint = "aby__exec_module", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern CStartResult c_exec_module(CJsRuntime* cself, CExecModuleOptions options);

        [DllImport(__DllName, EntryPoint = "aby__free_runtime", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void c_free_runtime(CJsRuntime* obj_ptr);

        /// <summary>TODO</summary>
        [DllImport(__DllName, EntryPoint = "aby__get_state", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern CJsRuntimeState c_get_state();

        /// <summary>TODO: Return a CJsRuntimeStartResult (repr(C)) for state.</summary>
        [DllImport(__DllName, EntryPoint = "aby__start", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern CStartResult c_start(CExecModuleOptions options);


    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe partial struct CBootstrapOptions
    {
        public byte* thread_prefix;
        public CJsRuntimeConfig js_runtime_config;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe partial struct CJsRuntime
    {
        public CJsRuntimeConfig config;
        public InMemoryBroadcastChannel broadcast;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe partial struct CJsRuntimeConfig
    {
        [MarshalAs(UnmanagedType.U1)] public bool inspector_wait;
        public ushort inspector_port;
        public byte* db_dir;
        public byte* log_dir;
        public CJsRuntimeLogLevel log_level;
        public void* log_callback_fn;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe partial struct CExecModuleOptions
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
        FailedCreateAsyncRuntime = 4,
        FailedFetchingWorkDirErr = 5,
        DataDirInvalidErr = 6,
        LogDirInvalidErr = 7,
        MainModuleInvalidErr = 8,
        MainModuleUninitializedErr = 9,
        FailedModuleExecErr = 10,
        FailedEventLoopErr = 11,
    }


}
#endif