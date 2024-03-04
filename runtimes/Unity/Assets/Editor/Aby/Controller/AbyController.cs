using System.ComponentModel;

using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.SceneManagement;

using Theta.Unity.Runtime;

namespace Theta.Unity.Editor.Aby
{
    /// <summary>
    /// TODO
    /// </summary>\
    [InitializeOnLoad]
    public class AbyControllerEditorWindow : EditorWindow
    {
        /// <summary>
        /// TODO
        /// </summary>
        const int DEFAULT_SERVICE_PORT = 9000;

        /// <summary>
        /// The expected Exit Code used to ask the dev script to reload
        /// the window, instead of just shutting down.
        /// </summary>
        const int RELOAD_CLIENT_EXIT_CODE = 100;

        /// <summary>
        /// TODO
        /// </summary>
        [SerializeField]
        private AbyEnv m_CurrentEnvironment;

        /// <summary>
        /// TODO
        /// </summary>
        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;

        /// <summary>
        /// TODO
        /// </summary>
        // private readonly State m_BindingContext = new State();

        /// <summary>
        /// TODO
        /// </summary>
        static AbyControllerEditorWindow()
        {
            // Mount a heirarchy gui event handler.
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemGUI;
        }

        //--
        /// <summary>
        /// TODO
        /// </summary>
        private void Awake()
        {
            // TODO
        }

        /// <summary>
        /// TODO
        /// </summary>
        private void OnDestroy()
        {
            // TODO
        }

        /// <summary>
        /// TODO
        /// </summary>
        [MenuItem("Theta/Aby Controller")]
        public static void ShowWindow()
        {
            var abyControllerWindow = GetWindow<AbyControllerEditorWindow>();
            abyControllerWindow.titleContent = new GUIContent("Aby Controller");
        }

        //--
        /// <summary>
        /// Mounts the root `VisualElement` and inits the start/reload buttons.
        /// </summary>
        public void CreateGUI()
        {
            // Mount the admin ui and bind state.
            var editorVisualTree = m_VisualTreeAsset.Instantiate();
            rootVisualElement.Add(editorVisualTree);
            rootVisualElement.Bind(new SerializedObject(this));

            // Mount environment assets and pick one to use with the editor window..
            // TODO: Move this to a static value on AbyEnvConfig itself.
            var guids = AssetDatabase.FindAssets("t:AbyEnvConfig");
            if (guids.Length > 0)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                m_CurrentEnvironment = AssetDatabase.LoadAssetAtPath<AbyEnv>(path);
                //rootVisualElement.Bind(new SerializedObject(m_CurrentEnvironment));
            }

            var stateLabel = rootVisualElement.Q<Label>("RuntimeState");
            if (stateLabel != null)
            {
                stateLabel.text = $"Runtime State: {JsRuntime.State}";
            }
            else
            {
                Debug.LogWarning("RuntimeState element not found ..");
            }

            var toggleButton = rootVisualElement.Q<Button>("ToggleButton");
            if (toggleButton != null)
            {
                toggleButton.clicked += OnToggleButtonClicked;
            }
            else
            {
                Debug.LogError("ToggleButton element not found!");
            }


            var reloadButton = rootVisualElement.Q<Button>("ReloadButton");
            if (reloadButton == null)
            {
                Debug.LogError("ReloadButton element not found!");
            }
            else
            {
                reloadButton.clicked += OnReloadButtonClicked;
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        public void OnGUI()
        {
            //..
        }

        /// <summary>
        /// TODO
        /// </summary>
        private static void OnHierarchyWindowItemGUI(int instanceID, Rect selectionRect)
        {
            // Debug.LogFormat("Found GUI item: {0}", instanceID);
        }

        /// <summary>
        /// TODO: Setup an observer for status display.
        /// </summary>
        private void OnToggleButtonClicked()
        {
            if (JsRuntime.IsRunning == false)
            {
                JsRuntime.StartServiceThread();
            }
            else
            {
                JsRuntime.StopServiceThread();
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        private void OnReloadButtonClicked()
        {
            Debug.Log("Attempting to reload plugin.");

            // We need to exit play mode first so we can (optionally) save
            // and safely run shutdown operations.
            EditorApplication.ExitPlaymode();

            // `ExitPlaymore` doesn't complete until "later", so we defer
            // the rest of the operation until then.
            EditorApplication.delayCall += DelayedOnReloadButtonClicked;
        }

        /// <summary>
        /// TODO
        /// </summary>
        private void DelayedOnReloadButtonClicked()
        {
            EditorApplication.delayCall -= DelayedOnReloadButtonClicked;

            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                if (ConfirmEditorRestart())
                {
                    EditorApplication.Exit(RELOAD_CLIENT_EXIT_CODE); // Request reload.
                }
            }
        }

        //--
        /// <summary>
        /// TODO
        /// </summary>
        private bool ConfirmEditorRestart()
        {
            return EditorUtility.DisplayDialog(
                "Restart Editor?",
                $"You'll need to restart the editor before changes take effect.",
                "Yes pls! (Recommended)",
                "No thx."
            );
        }
    }
}
