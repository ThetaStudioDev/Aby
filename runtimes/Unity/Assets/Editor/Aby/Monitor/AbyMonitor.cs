using System.IO;

using UnityEngine;
using UnityEngine.UIElements;

using UnityEditor;
using UnityEditor.UIElements;

using Theta.Unity.Runtime;
using System;

namespace Theta.Unity.Editor.Aby
{
    /// <summary>
    /// TODO
    /// </summary>
    public class AbyRuntimeMonitorEditorWindow : EditorWindow
    {
        /// <summary>
        /// TODO
        /// </summary>
        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;

        /// <summary>
        /// TODO
        /// </summary>
        private static string m_LogFilePath = "examples/logs/stdout.log";

        /// <summary>
        /// TODO
        /// </summary>
        private static FileSystemWatcher m_LogWatcher;

        //--
        /// <summary>
        /// TODO
        /// </summary>
        [MenuItem("Theta/Aby Runtime Monitor")]
        public static void ShowWindow()
        {
            var abyMonitorWindow = GetWindow<AbyRuntimeMonitorEditorWindow>();
            abyMonitorWindow.titleContent = new GUIContent("Aby Monitor");
        }

        //--
        /// <summary>
        /// TODO
        /// </summary>
        public void Awake()
        {
            Debug.LogFormat("Monitoring log files at '{0}' ..", Path.GetFullPath(m_LogFilePath));
        }

        /// <summary>
        /// TODO
        /// </summary>
        public void OnEnable()
        {
            StartLogWatcher();
        }

        /// <summary>
        /// TODO
        /// </summary>
        public void StartLogWatcher()
        {
            if (m_LogWatcher == null)
            {
                m_LogWatcher = new FileSystemWatcher()
                {
                    Path = Path.GetDirectoryName(m_LogFilePath),
                    Filter = Path.GetFileName(m_LogFilePath),
                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size,
                    EnableRaisingEvents = true,
                };

                // TODO: Need to set this only once. Why can't we use `Awake`?
                m_LogWatcher.Changed += OnLogFileChanged;
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        private void OnDestroy()
        {
            StopLogWatcher();
        }

        /// <summary>
        /// TODO
        /// </summary>
        private void StopLogWatcher()
        {
            if (m_LogWatcher != null)
            {
                m_LogWatcher.EnableRaisingEvents = false;
                m_LogWatcher.Dispose();
            }
        }

        //--
        /// <summary>
        /// TODO
        /// </summary>
        public void CreateGUI()
        {
            rootVisualElement.Add(m_VisualTreeAsset.Instantiate());

            CreateEnvironmentMenu();
            CreateMasthead();
        }

        /// <summary>
        /// TODO
        /// </summary>
        private void CreateMasthead()
        {
            var viewport = rootVisualElement.Q<VisualElement>("Viewport");
            if (viewport == null)
            {
                Debug.LogError("Viewport element not found!");
            }
            else
            {
                var label = new Label("TODO: Setup log stream from Rust-side ..");
                viewport.Add(label);
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        private void CreateEnvironmentMenu()
        {
            var envMenu = rootVisualElement.Q<ToolbarMenu>("EnvironmentMenu");
            if (envMenu == null)
            {
                Debug.LogError("EnvironmentMenu element not found!");
            }
            else
            {
                envMenu.text = $"Active: Dev";
                envMenu.menu.AppendAction("Dev", OnEnvironmentMenuChange, DropdownMenuAction.Status.Checked);
                envMenu.menu.AppendAction("QA", OnEnvironmentMenuChange, DropdownMenuAction.Status.Normal);
                envMenu.menu.AppendAction("Prod", OnEnvironmentMenuChange, DropdownMenuAction.Status.Normal);
            }
        }

        //--
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnLogFileChanged(object source, FileSystemEventArgs changeEvent)
        {
            Debug.LogFormat("Caught log event for in {0}", changeEvent.FullPath);

            //var lines = File.ReadAllLines(changeEvent.FullPath);
            //Debug.LogFormat("Found New Log Lines:\n{0}", lines);

            //foreach (string line in lines)
            //{
            //    Debug.LogFormat("[Deno]:", line);
            //}
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="option"></param>
        private void OnEnvironmentMenuChange(DropdownMenuAction option)
        {
            if (option.status == DropdownMenuAction.Status.Checked)
            {
                Debug.LogFormat("Environment `{0}` Already Selected", option.name);

                // TODO: Trigger re-fetch of Envs from the project.
            }
            else
            {
                Debug.LogFormat("Environment Changed to `{0}`", option.name);

                // TODO: Set the current environment.
            }
        }
    }
}
