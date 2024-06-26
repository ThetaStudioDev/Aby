use std::fs::File;

use deno_runtime::deno_io::Stdio as DenoStdio;
use deno_runtime::deno_io::StdioPipe as DenoStdioPipe;

// #[repr(C)]
pub struct JsRuntimeStdio {
    stdin: File,
    stdout: File,
    stderr: File,
}

impl JsRuntimeStdio {
    pub fn try_new(stdin: Option<File>, stdout: Option<File>, stderr: Option<File>) -> Result<Self, std::io::Error> {
        Ok(JsRuntimeStdio {
            stdin: match stdin {
                Some(file) => file,
                None => deno_runtime::deno_io::STDIN_HANDLE.try_clone()?,
            },
            stdout: match stdout {
                Some(file) => file,
                None => deno_runtime::deno_io::STDOUT_HANDLE.try_clone()?,
            },
            stderr: match stderr {
                Some(file) => file,
                None => deno_runtime::deno_io::STDERR_HANDLE.try_clone()?,
            },
        })
    }
}

impl JsRuntimeStdio {
    /// Turn an instance of JsRuntimeStdio into a deno_runtime::io::Stdio,
    /// by cloning the inner file handles.
    ///
    /// TODO: This shoiuld probably be a `try_clone_into()`
    pub fn try_clone_into(&self) -> Result<DenoStdio, std::io::Error> {
        Ok(DenoStdio {
            stdin: DenoStdioPipe::File(self.stdin.try_clone()?),
            stdout: DenoStdioPipe::File(self.stdout.try_clone()?),
            stderr: DenoStdioPipe::File(self.stderr.try_clone()?),
        })
    }
}
