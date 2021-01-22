using UnityEngine;
using UnityEditor;
using Cinemachine;

namespace DiasGames.ThirdPersonSystem
{
    public class SetupThirdPersonScene : EditorWindow
    {
        protected GUISkin windowSkin;
        protected GUISkin headerSkin;

        string setupLabel = string.Empty;
        MessageType messageType = MessageType.Info;


        [MenuItem("Dias Games/Third Person System/Setup Scene for Third Person System")]
        public static void ShowWindow()
        {
            GetWindow<SetupThirdPersonScene>(true, "Setup Scene for Third Person System");
        }

        private void OnEnable()
        {
            windowSkin = Resources.Load("ContentSkin") as GUISkin;
            headerSkin = Resources.Load("HeaderSkin") as GUISkin;
        }

        private void OnGUI()
        {
            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal(headerSkin.box);
            GUILayout.FlexibleSpace();

            GUILayout.Label("Setup Scene for Third Person System", headerSkin.label);

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical(windowSkin.box);
            EditorGUI.indentLevel++;

            EditorGUILayout.Space();

            EditorGUILayout.HelpBox("Click on the settings you want to add to your scene", MessageType.Info);

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Setup the Camera", windowSkin.button))
            {
                if (SetCamera())
                {
                    setupLabel = "Camera State Driven was added to your scene";
                    messageType = MessageType.Info;
                }
                else
                {
                    setupLabel = "Camera State Driven was not added because your scene already has it.";
                    messageType = MessageType.Warning;
                }
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Create the Game Controller", windowSkin.button))
            {
                if (CreateGameController())
                {
                    setupLabel = "Game Controller was created and added to your scene";
                    messageType = MessageType.Info;
                }
                else
                {
                    setupLabel = "Game Controller was not created because your scene already has a Game Controller.";
                    messageType = MessageType.Warning;
                }
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Add Canvas UI", windowSkin.button))
            {
                if (SetUI())
                {
                    setupLabel = "Canvas UI was added to your scene";
                    messageType = MessageType.Info;
                }
                else
                {
                    setupLabel = "Canvas UI was not added because your scene already has a Canvas UI.";
                    messageType = MessageType.Warning;
                }
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Set Level To Mobile", windowSkin.button))
            {
                if (SetMobile())
                {
                    setupLabel = "Mobile Buttons were added to the Scene";
                    messageType = MessageType.Info;
                }
                else
                {
                    setupLabel = "Mobile Buttons were not added because your scene already has Mobile Buttons.";
                    messageType = MessageType.Warning;
                }
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            if (setupLabel != string.Empty)
                EditorGUILayout.HelpBox(setupLabel, messageType);

            EditorGUILayout.Space();

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Close", windowSkin.button))
                Close();
            ///

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private bool SetCamera()
        {
            CinemachineBrain brain = FindObjectOfType<CinemachineBrain>();
            if (brain == null)
                Undo.AddComponent<CinemachineBrain>(Camera.main.transform.gameObject);

            CinemachineStateDrivenCamera camera = FindObjectOfType<CinemachineStateDrivenCamera>();
            if (camera == null)
            {
                camera = AssetDatabase.LoadAssetAtPath<CinemachineStateDrivenCamera>("Assets/Dias Games/Third Person System/Prefabs/Camera State Driven.prefab");
                Object newCam = PrefabUtility.InstantiatePrefab(camera);

                GameObject character = GameObject.FindGameObjectWithTag("Player");
                if (character != null)
                {
                    Transform target = character.transform.Find("Camera Track Pos");

                    SerializedObject cameraObj = new SerializedObject(newCam);
                    cameraObj.Update();

                    cameraObj.FindProperty("m_LookAt").objectReferenceValue = target;
                    cameraObj.FindProperty("m_Follow").objectReferenceValue = target;
                    cameraObj.FindProperty("m_AnimatedTarget").objectReferenceValue = character.GetComponent<Animator>();

                    cameraObj.ApplyModifiedProperties();
                }


                return true;
            }

            return false;
        }

        private bool CreateGameController()
        {
            GameObject controller = GameObject.FindGameObjectWithTag("GameController");
            if (controller == null)
            {
                controller = new GameObject("Game Controller");
                controller.tag = "GameController";
                controller.AddComponent<ObjectPooler>();
                controller.AddComponent<GlobalEvents>();
                return true;
            }

            return false;
        }

        private bool SetUI()
        {
            GameObject gameCanvas = GameObject.Find("UI");
            if (gameCanvas == null)
            {
                gameCanvas = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Dias Games/Third Person System/Prefabs/UI.prefab");
                PrefabUtility.InstantiatePrefab(gameCanvas);
                return true;
            }

            return false;
        }

        private bool SetMobile()
        {
            MobileInputController mobile = FindObjectOfType<MobileInputController>();
            if (mobile == null)
            {
                mobile = AssetDatabase.LoadAssetAtPath<MobileInputController>("Assets/Dias Games/Third Person System/Prefabs/Mobile Input Canvas.prefab");
                MobileInputController instantiated = PrefabUtility.InstantiatePrefab(mobile) as MobileInputController;

                // Set character reference
                SerializedObject mobileSerialized = new SerializedObject(instantiated);
                mobileSerialized.Update();
                mobileSerialized.FindProperty("m_Character").objectReferenceValue = FindObjectOfType<UnityInputManager>();
                mobileSerialized.ApplyModifiedProperties();

                return true;
            }

            return false;
        }
    }
}
