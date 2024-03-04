#![allow(unused)]

use std::rc::Rc;
use std::sync::Arc;
use std::sync::Mutex;
use std::fmt::Display;
use std::error::Error;
use std::ffi::CString;
use std::fs::OpenOptions;
use std::time::Duration;

use tokio::runtime::Runtime as TokioRuntime;
use tokio::runtime::Builder as TokioRuntimeBuilder;

use deno_runtime::permissions::PermissionsContainer;

use deno_runtime::deno_core::FsModuleLoader;
use deno_runtime::deno_core::PollEventLoopOptions;
use deno_runtime::deno_core::ModuleResolutionError;
use deno_runtime::deno_core::FeatureChecker;
use deno_runtime::deno_core::resolve_url_or_path;

use deno_runtime::worker::MainWorker;
use deno_runtime::worker::WorkerOptions;

use deno_runtime::BootstrapOptions;
use deno_runtime::UNSTABLE_GRANULAR_FLAGS;

use crate::stdio::JsRuntimeStdio;

#[cfg(feature="ffi")]
use crate::ffi::CLogCallback;

//---
/// TODO
pub struct JsRuntimeManager {
    /// TODO
    async_runtime: TokioRuntime,
    
    /// TODO
    stdio: JsRuntimeStdio,
    
    /// TODO
    /// 1, BroadcastChannel
    /// 2, Deno.cron
    /// 3, FFI
    /// 4, File System
    /// 5, HTTP
    /// 6, Key-Value
    /// 7, Net
    /// 8, Temporal
    /// 9, Proto
    /// 10, WebGPU
    /// 11, Web Worker
    unstable_features: Vec<i32>,
    
    /// TODO
    log_callback: Option<Arc<Mutex<CLogCallback>>>,
}

impl JsRuntimeManager {
    /// TODO
    pub fn try_new() -> Result<Self, std::io::Error> {
        // If the `stdio` feature is enabled, just use default stdio setup.
        #[cfg(feature = "stdio")]
        let js_stdio = JsRuntimeStdio::try_new(None, None, None)?;
        
        // Otherwise, re-route stdin, stdout, and stderr to temporary log files.
        #[cfg(not(feature = "stdio"))]
        let js_stdio = {
            tracing::info!("Feature `stdio` not enabled; Re-routing stdio to logs.");
            
            JsRuntimeStdio::try_new(
                Some(OpenOptions::new().read(true).write(true).create(true).open("./examples/logs/stdin.log")?),
                Some(OpenOptions::new().read(true).write(true).create(true).open("./examples/logs/stdout.log")?),
                Some(OpenOptions::new().read(true).write(true).create(true).open("./examples/logs/stderr.log")?),
            )?
        };
        
        // We don't need a TokioRuntime if anything else fails (so we create it last).
        let async_runtime = TokioRuntimeBuilder::new_current_thread()
            .enable_time()
            .enable_io()
            .build()?;
        
        let features = UNSTABLE_GRANULAR_FLAGS.iter()
            .map(|&feature| feature.2)
            .collect();
        
        Ok(JsRuntimeManager {
            async_runtime,
            stdio: js_stdio,
            unstable_features: features,
            log_callback: None,
        })
    }
    
    /// TODO1
    pub fn with_log_callback(mut self, log_callback: CLogCallback) {
        self.set_log_callback(log_callback)
    }
    
    /// TODO
    pub fn set_log_callback(&mut self, log_callback: CLogCallback) {
        self.log_callback = Some(Arc::new(Mutex::new(log_callback)));
    }
}

impl JsRuntimeManager {
    /// TODO
    pub fn capture_trace(&self) -> Result<u32, JsRuntimeError> {
        let Some(log_callback) = self.log_callback.as_ref().map(|d| d.clone()) else {
            return Err(JsRuntimeError::Unknown("Log callback not yet initialized."));
        };
        
        self.async_runtime.block_on(async move {
            let _ = self.send_log("TODO: Capture tracing spans from Rust ..");
            // let log_callback = log_callback.clone();
            
            let log_thread = tokio::spawn(async move {
                loop {
                    match log_callback.lock() {
                        Ok(log_callback) => {
                            // let message = CString::new(format!("TODO: CAPTURE TRACE #003 ({:?})", log_callback)).expect("Failed to create CString!");
                            // unsafe {
                            //     log_callback(message.as_ptr());
                            // }
                        }
                        Err(error) => {
                            tracing::error!("Couldn't get mutex lock: {:?}", error);
                        }
                    }
                    
                    tokio::time::sleep(Duration::from_secs(60)).await;
                    tracing::trace!("After Sleep ..");
                }
            });
        });
        
        Ok(0)
    }
    
    /// TODO
    pub(crate) fn send_log<T: ToString>(&self, message: T) -> Result<(), JsRuntimeError> {
        match &self.log_callback {
            Some(log_callback) => {
                match log_callback.lock() {
                    Ok(log_callback) => {
                        let c_string = CString::new(message.to_string())?;
                        unsafe {
                            log_callback(c_string.as_ptr());
                        }
                            }
                    Err(error) => {
                        
                    }
                }
                Ok(())
            }
            None => {
                Err(JsRuntimeError::Unknown("Log callback not yet initialized."))
            }
        }
    }
    
    /// TODO
    pub fn start(&self, main_specifier: &str) -> Result<u32, JsRuntimeError> {
        let stdio = self.stdio.try_clone_into()?;
        let current_dir = std::env::current_dir()?;
        #[cfg(feature = "verbose")]
        {
            self.send_log(format!("Current Dir is `{0:}`", current_dir.display()));
            tracing::debug!("Current Dir is {:}", current_dir.display());
        }
    
        // TODO: Move this to `ThetaRuntime::resolve_main_module(..)`.
        let main_module = resolve_url_or_path(main_specifier, &current_dir)?;
        #[cfg(feature = "verbose")]
        {
            self.send_log(format!("Resolved Main Module at {:}", main_module));
            tracing::debug!("Resolved Main Module at {:}", main_module);
        }
        
        // Run a "lite" Deno runtime, with only a core.
        //  - No worker and minimal extensions.
        //  - Useful for some testing and debug scenarios.
        #[cfg(feature = "lite")]
        self.async_runtime.block_on(async move {
            let mut js_runtime = JsRuntime::new(RuntimeOptions {
                module_loader: Some(Rc::new(FsModuleLoader)),
                extensions: vec![
                    // deno_runtime::deno_webidl::deno_webidl::init_ops_and_esm(),
                    // deno_runtime::deno_console::deno_console::init_ops_and_esm(),
                    // deno_runtime::deno_url::deno_url::init_ops_and_esm(),
                    // deno_runtime::deno_web::deno_web::init_ops_and_esm::<PermissionsContainer>(Arc::new(BlobStore::default()), None),
                ],
                ..Default::default()
            });
            
            if let Err(error) = js_runtime.execute_script_static("<prelude>", include_str!("./prelude.js")) {
                tracing::error!("Failed to run prelude script: {:}", error);
            }
            
            if let Err(error) = js_runtime.execute_script_static("<debug>", include_str!("./debug.js")) {
                tracing::error!("Failed to run debug setup script: {:}", error);
            }
            
            if let Err(error) = js_runtime.run_event_loop(PollEventLoopOptions::default()).await {
                tracing::error!("Failed to run main worker event loop: {:}", error);
            }
        });
        
        let mut worker = MainWorker::bootstrap_from_options(
            // TODO: Revist the Clone for `main_module`.
            main_module.clone(),
            PermissionsContainer::allow_all(),
            WorkerOptions {
                stdio,
                bootstrap: self.create_bootstrap_options(),
                feature_checker: self.create_feature_checker(),
                module_loader: Rc::new(FsModuleLoader),
                origin_storage_dir: Some(std::path::PathBuf::from("./examples/db")),
                extensions: vec![
                    //..
                ],
                ..Default::default()
            },
        );
        
        // Run the "not-lite", full Deno runtime.
        // Prefer this when you want all of Deno's features.
        #[cfg(not(feature = "lite"))]
        self.async_runtime.block_on(async move {
            // TODO: Revist the Clone for `main_module`.
            if let Err(error) = worker.execute_main_module(&main_module.clone()).await {
                tracing::error!("Failed to execute main module: {:}", error);
                let _ = self.send_log(format!("Failed to execute main module: {:}", error));
            }
            
            // let poll_options = PollEventLoopOptions::default();
            // let mut main_context = worker.js_runtime.main_context();
            // while true {
            //     match worker.js_runtime.poll_event_loop(&mut main_context, poll_options) {
            //         Poll::Ready(_) => {}
            //         Poll::Pending => {}
            //     }
            // }
            
            if let Err(error) = worker.js_runtime.run_event_loop(PollEventLoopOptions::default()).await {
                tracing::error!("Failed to run main worker event loop: {:}", error);
                let _ = self.send_log(format!("Failed to run main worker event loop: {:}", error));
            }
        });
    
        Ok(0)
    }
    
    /// TODO
    fn create_bootstrap_options(&self) -> BootstrapOptions {
        BootstrapOptions {
            unstable_features: self.unstable_features.clone(),
            ..Default::default()
        }
    }
    
    /// TODO
    fn create_feature_checker(&self) -> Arc<FeatureChecker> {
        let mut feature_checker = FeatureChecker::default();
        
        for feature in UNSTABLE_GRANULAR_FLAGS.iter() {
            feature_checker.enable_feature(feature.0);
        }
        
        Arc::new(feature_checker)
    }
}

/// TODO
#[derive(Debug)]
pub enum JsRuntimeError {
    /// A user-supplied module-name was invalid.
    InvalidModuleSpecifier(&'static str),
    
    /// The runtime detected a current or future invalid atomic state.
    InvalidState(u32),
    
    /// TODO
    ResourceError(&'static str, std::io::Error),
    
    /// TODO
    NulError(std::ffi::NulError),
    
    /// TODO
    ModuleError(ModuleResolutionError),
    
    /// An unknown error occurred.
    Unknown(&'static str),
}

impl Error for JsRuntimeError {}

impl Display for JsRuntimeError {
    /// TODO
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        f.write_str("TODO")
    }
}

impl From<std::io::Error> for JsRuntimeError {
    /// TODO
    fn from(error: std::io::Error) -> JsRuntimeError {
        JsRuntimeError::ResourceError("io", error)
    }
}

impl From<std::ffi::NulError> for JsRuntimeError {
    /// TODO
    fn from(error: std::ffi::NulError) -> JsRuntimeError {
        JsRuntimeError::NulError(error)
    }
}

impl From<ModuleResolutionError> for JsRuntimeError {
    /// TODO
    fn from(error: ModuleResolutionError) -> JsRuntimeError {
        JsRuntimeError::ModuleError(error)
    }
}
