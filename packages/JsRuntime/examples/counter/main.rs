#![allow(unused)]

use std::process::ExitCode;
use std::ffi::CStr;
use std::ffi::CString;

use anyhow::Result;

use js_runtime::logging::ffi::CJsRuntimeLogLevel;
use js_runtime::runtime::ffi::CJsRuntimeConfig;
use js_runtime::bootstrap::ffi::CBootstrapOptions;
use js_runtime::start::ffi::CExecModuleOptions;

//---
/// TODO
const TRACING_FILTER: &str = "counter=trace,js_runtime=trace,info";

// TODO: Binding generator macro for c-safe functions.
// Turn this:
// #[cwrap::bind(extern "C" aby_runtime; panic=true)]
fn safe_log_callback(message: &str) {
    tracing::trace!("[Capt'd] {:}", message);
}
// .. into something like this:
extern "C" fn log_callback(message: *const std::os::raw::c_char) {
    use safe_log_callback as wrapped_fn;
    
    unsafe {
        if !message.is_null() {
            let c_str =  CStr::from_ptr(message);
            let message = c_str.to_str().expect("Failed to unpack log message!");
            wrapped_fn(message);
        }
    }
}

//---
fn main() -> Result<ExitCode> {
    // Setup logging facilities, etc.
    js_runtime::tracing::mount(TRACING_FILTER);
    
    let thread_prefix = CString::new("JS_RUNTIME_EXAMPLE")?;
    let main_module_path = CString::new("./examples/counter/main.js")?;
    let db_dir = CString::new("./examples/counter/db")?;
    let log_dir = CString::new("./examples/counter/logs")?;
    
    unsafe {
        // Step 1:
        //   Run the bootstrap operation to mount resources and otherwise
        //   prepare the process, ffi boundary, etc.
        let runtime = js_runtime::runtime::ffi::c_construct_runtime({
            CJsRuntimeConfig {
                inspector_wait: false,
                inspector_port: 9223, // TODO: Dynamic port assignment.
                db_dir: db_dir.as_ptr(),
                log_dir: log_dir.as_ptr(),
                log_level: CJsRuntimeLogLevel::Trace,
                log_callback_fn: log_callback,
            }
        });
        
        // Step 2:
        //   Start the global `MainWorker`. Status 0 is good, 1+ is bad.
        let status = js_runtime::runtime::ffi::c_exec_module(runtime, CExecModuleOptions {
            main_module_specifier: main_module_path.as_ptr(),
        });
        
        tracing::debug!("JsRuntime exited with status {:?}", status);
        
        // Yay! <3
        Ok(ExitCode::SUCCESS)
    }
}
