using UnityEngine;
using UnityEditor;
using UnityEditor.Events;
using System;
using UnityEngine.Events;

namespace DiasGames.ThirdPersonSystem
{
    public class BuildBasicCharacter : EditorWindow
    {
        // Styles parameters --------------------- ///

        protected GUISkin headerSkin;
        protected string label = "";

        // -------------------------------------- ///


        protected GameObject character; // Property to set character
        protected SerializedObject system, modifier;

        protected PhysicMaterial zeroFriction;
        protected PhysicMaterial groundFriction;
        protected float CharacterHeight = 1.8f;

        protected Animator m_Animator;
        protected string m_AnimatorPath = "Assets/Dias Games/Third Person System/Animator/Third Person Animator.controller";
        protected string m_HelpInstruction = "This window will help you to create your character. Start selecting your scene character";

        [MenuItem("Dias Games/Third Person System/Build Basic Character")]
        public static void ShowWindow()
        {
            GetWindow<BuildBasicCharacter>(true, "Basic Character Builder");
        }

        protected virtual void OnEnable()
        {
            headerSkin = Resources.Load("HeaderSkin") as GUISkin;
            label = "Basic Character Builder";
        }


        protected void DrawImageHeader()
        {
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(headerSkin.box);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.Label(label, headerSkin.label);

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();
        }

        protected virtual void OnGUI()
        {
            GUILayout.Space(10);

            // Set label style for header
            GUIStyle m_Style = new GUIStyle(EditorStyles.label);
            m_Style.wordWrap = true;
            m_Style.fontStyle = FontStyle.Bold;

            DrawImageHeader();
            EditorGUILayout.Space();

            // Draw GUI label and fields
            GUILayout.Label(m_HelpInstruction, m_Style);

            ShowCharacterOptions();

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Build Character", GUILayout.Width(300), GUILayout.Height(20)) && character != null)
                CreateCharacter();


            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            ///

        }

        protected virtual void ShowCharacterOptions()
        {
            character = EditorGUILayout.ObjectField("Character", character, typeof(GameObject), true) as GameObject;
            if (character == null)
            {
                EditorGUILayout.HelpBox("Please select a character to build.", MessageType.Error, false);
            }

            CharacterHeight = EditorGUILayout.FloatField("Character Height", CharacterHeight);

        }

        protected virtual void CreateCharacter()
        {
            BuildCharacter();
            CreateRagdoll();
            Close();
        }

        public virtual void BuildCharacter()
        {
            zeroFriction = AssetDatabase.LoadAssetAtPath<PhysicMaterial>("Assets/Dias Games/Third Person System/Physics Materials/ZeroFriction.physicMaterial");
            groundFriction = AssetDatabase.LoadAssetAtPath<PhysicMaterial>("Assets/Dias Games/Third Person System/Physics Materials/GroundFriction.physicMaterial");

            #region Set Tag and Layer

            character.tag = "Player";
            character.layer = 15;

            #endregion

            #region Animator Builder
            m_Animator = character.GetComponent<Animator>();
            if (m_Animator == null)
                m_Animator = Undo.AddComponent<Animator>(character);

            m_Animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
            m_Animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;

            m_Animator.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(m_AnimatorPath);

            #endregion

            #region Rigidbody Builder
            Rigidbody m_Rigidbody = character.GetComponent<Rigidbody>();
            if (m_Rigidbody == null)
                m_Rigidbody = Undo.AddComponent<Rigidbody>(character);

            m_Rigidbody.angularDrag = 0;
            m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            #endregion

            #region Capsule Collider Builder
            CapsuleCollider m_Capsule = character.GetComponent<CapsuleCollider>();
            if (m_Capsule == null)
                m_Capsule = Undo.AddComponent<CapsuleCollider>(character);

            m_Capsule.radius = 0.3f;
            m_Capsule.height = CharacterHeight;
            m_Capsule.center = new Vector3(0, m_Capsule.height * 0.5f, 0);
            m_Capsule.material = zeroFriction;
            #endregion

            #region Audio Builder

            CharacterAudioManager audio = character.GetComponent<CharacterAudioManager>();
            if (audio == null)
            {
                audio = Undo.AddComponent<CharacterAudioManager>(character);

                GameObject effectSource = new GameObject("Effect Audio Source");
                effectSource.transform.parent = character.transform;
                effectSource.transform.localPosition = Vector3.zero;

                // Create audio sources on character
                AudioSource voice = Undo.AddComponent<AudioSource>(m_Animator.GetBoneTransform(HumanBodyBones.Head).gameObject);
                AudioSource effect = Undo.AddComponent<AudioSource>(effectSource);

                SerializedObject audioManager = new SerializedObject(audio);

                audioManager.FindProperty("m_VoiceSource").objectReferenceValue = voice;
                audioManager.FindProperty("m_EffectSource").objectReferenceValue = effect;

                audioManager.ApplyModifiedProperties();
            }
            #endregion

            #region Create Character Component

            ThirdPersonSystem m_Controller = character.GetComponent<ThirdPersonSystem>();
            if (m_Controller == null)
                m_Controller = Undo.AddComponent<ThirdPersonSystem>(character);

            m_Controller.GroundMask = (1 << 0) | (1 << 14) | (1 << 16) | (1 << 17) | (1 << 18) | (1 << 19) | (1 << 20) | (1 << 25) | (1 << 26);

            ModifierManager modifierManager = character.GetComponent<ModifierManager>();
            if (modifierManager == null)
                modifierManager = Undo.AddComponent<ModifierManager>(character);

            system = new SerializedObject(m_Controller);
            modifier = new SerializedObject(modifierManager);

            #endregion

            // Camera Track
            if(character.transform.Find("Camera Track Pos") == null)
            {
                GameObject track = new GameObject("Camera Track Pos");
                Undo.RegisterCreatedObjectUndo(track, "Camera Track Pos");

                track.transform.parent = character.transform;
                track.transform.localPosition = Vector3.up * 1.5f;
            }

            AddAbilities();

            AddModifiers();
        }

        protected virtual void AddModifiers()
        {
            // Health
            Health m_Health = character.GetComponent<Health>();
            if (m_Health == null)
            {
                m_Health = Undo.AddComponent<Health>(character);

                SerializedObject healthObj = new SerializedObject(m_Health);

                AudioClip m_HurtClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Dias Games/Third Person System/Audio/male_hurt.wav");
                AudioClip m_DeathClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Dias Games/Third Person System/Audio/male_death.wav");

                healthObj.FindProperty("m_HurtClip").objectReferenceValue = m_HurtClip;
                healthObj.FindProperty("m_DeathClip").objectReferenceValue = m_DeathClip;

                healthObj.ApplyModifiedProperties();

                UpdateModifierList(m_Health);
            }

        }

        protected void UpdateModifierList(Modifier newModifier)
        {
            SerializedProperty list = modifier.FindProperty("m_Modifiers");

            newModifier.hideFlags = HideFlags.HideInInspector;
            list.InsertArrayElementAtIndex(list.arraySize);
            list.GetArrayElementAtIndex(list.arraySize - 1).objectReferenceValue = newModifier;

            modifier.ApplyModifiedProperties();
        }

        protected virtual void AddAbilities()
        {
            if (character.GetComponent<DiasGames.ThirdPersonSystem.AnimatorManager>() == null)
                Undo.AddComponent<DiasGames.ThirdPersonSystem.AnimatorManager>(character);

            if (character.GetComponent<DiasGames.ThirdPersonSystem.UnityInputManager>() == null)
                Undo.AddComponent<DiasGames.ThirdPersonSystem.UnityInputManager>(character);

            FreeLocomotion m_FreeLocomotion = character.GetComponent<FreeLocomotion>();
            if (m_FreeLocomotion == null)
            {
                m_FreeLocomotion = Undo.AddComponent<FreeLocomotion>(character);

                SerializedObject locomotion = new SerializedObject(m_FreeLocomotion);

                locomotion.FindProperty("m_IdlePhysicMaterial").objectReferenceValue = groundFriction;
                locomotion.FindProperty("m_AbilityPhysicMaterial").objectReferenceValue = zeroFriction;

                locomotion.ApplyModifiedProperties();
            }

            Strafe m_Strafe = character.GetComponent<Strafe>();
            if (m_Strafe == null)
            {
                m_Strafe = Undo.AddComponent<Strafe>(character);

                SerializedObject strafe = new SerializedObject(m_Strafe);

                strafe.FindProperty("m_IdleFriction").objectReferenceValue = groundFriction;
                strafe.FindProperty("m_AbilityPhysicMaterial").objectReferenceValue = zeroFriction;

                strafe.ApplyModifiedProperties();
            }

            DiasGames.ThirdPersonSystem.JumpAbility m_Jump = character.GetComponent<DiasGames.ThirdPersonSystem.JumpAbility>();
            if (m_Jump == null)
            {
                m_Jump = Undo.AddComponent<DiasGames.ThirdPersonSystem.JumpAbility>(character);
                m_Jump.AbilityPhysicMaterial = zeroFriction;

                AudioClip jumpClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Dias Games/Third Person System/Audio/male_jump.wav");
                CharacterAudioManager manager = character.GetComponent<CharacterAudioManager>();
                if (manager != null)
                {
                    if (m_Jump.OnEnterAbilityEvent == null)
                        m_Jump.OnEnterAbilityEvent = new UnityEvent();

                    UnityEventTools.AddObjectPersistentListener(m_Jump.OnEnterAbilityEvent, manager.PlayVoiceSound, jumpClip);
                }
            }

            DiasGames.ThirdPersonSystem.FallAbility m_Fall = character.GetComponent<DiasGames.ThirdPersonSystem.FallAbility>();
            if (m_Fall == null)
            {
                m_Fall = Undo.AddComponent<DiasGames.ThirdPersonSystem.FallAbility>(character);
                m_Fall.AbilityPhysicMaterial = zeroFriction;
            }

            DiasGames.ThirdPersonSystem.RollAbility m_Roll = character.GetComponent<DiasGames.ThirdPersonSystem.RollAbility>();
            if (m_Roll == null)
                m_Roll = Undo.AddComponent<DiasGames.ThirdPersonSystem.RollAbility>(character);


            DiasGames.ThirdPersonSystem.CrouchAbility m_Crouch = character.GetComponent<DiasGames.ThirdPersonSystem.CrouchAbility>();
            if (m_Crouch == null)
                m_Crouch = Undo.AddComponent<DiasGames.ThirdPersonSystem.CrouchAbility>(character);

            m_Crouch.AbilityPhysicMaterial = groundFriction;

            CrawlAbility m_Crawl = character.GetComponent<CrawlAbility>();
            if (m_Crawl == null)
                m_Crawl = Undo.AddComponent<CrawlAbility>(character);

            m_Crawl.AbilityPhysicMaterial = groundFriction;
            // -------------------------- SET IGNORE ABILITIES ---------------------------------- //

            // Strafe
            AddIgnoreAbility(m_Strafe, m_FreeLocomotion);

            // Jump
            AddIgnoreAbility(m_Jump, m_FreeLocomotion);
            AddIgnoreAbility(m_Jump, m_Strafe);

            // Roll
            AddIgnoreAbility(m_Roll, m_FreeLocomotion);
            AddIgnoreAbility(m_Roll, m_Strafe);
            AddIgnoreAbility(m_Roll, m_Crouch);

            //Crouch
            AddIgnoreAbility(m_Crouch, m_FreeLocomotion);
            AddIgnoreAbility(m_Crouch, m_Strafe);
            AddIgnoreAbility(m_Crouch, m_Crawl);

            //Crawl
            AddIgnoreAbility(m_Crawl, m_FreeLocomotion);
            AddIgnoreAbility(m_Crawl, m_Strafe);
            AddIgnoreAbility(m_Crawl, m_Crouch);

            // -------------------------------------------------------------------------------- //

        }

        protected void AddIgnoreAbility(ThirdPersonAbility abilityTarget, ThirdPersonAbility abilityToAdd)
        {
            if (abilityTarget == null || abilityToAdd == null)
                return;

            SerializedObject ability = new SerializedObject(abilityTarget);
            SerializedProperty IgnoreAbilities = ability.FindProperty("IgnoreAbilities");

            int i = IgnoreAbilities.arraySize;
            IgnoreAbilities.arraySize++;
            IgnoreAbilities.GetArrayElementAtIndex(i).objectReferenceValue = abilityToAdd;

            ability.ApplyModifiedProperties();
        }

        private void CreateRagdoll()
        {
            var ragdollType = Type.GetType("UnityEditor.RagdollBuilder, UnityEditor");
            var windowsOpened = Resources.FindObjectsOfTypeAll(ragdollType);

            // Open Ragdoll window 
            if (windowsOpened == null || windowsOpened.Length == 0)
            {
                EditorApplication.ExecuteMenuItem("GameObject/3D Object/Ragdoll...");
                windowsOpened = Resources.FindObjectsOfTypeAll(ragdollType);
            }

            if (windowsOpened != null && windowsOpened.Length > 0)
            {
                ScriptableWizard ragdollWindow = windowsOpened[0] as ScriptableWizard;

                SetRagdollBoneValue(ragdollWindow, "pelvis", HumanBodyBones.Hips);
                SetRagdollBoneValue(ragdollWindow, "leftHips", HumanBodyBones.LeftUpperLeg);
                SetRagdollBoneValue(ragdollWindow, "leftKnee", HumanBodyBones.LeftLowerLeg);
                SetRagdollBoneValue(ragdollWindow, "leftFoot", HumanBodyBones.LeftFoot);
                SetRagdollBoneValue(ragdollWindow, "rightHips", HumanBodyBones.RightUpperLeg);
                SetRagdollBoneValue(ragdollWindow, "rightKnee", HumanBodyBones.RightLowerLeg);
                SetRagdollBoneValue(ragdollWindow, "rightFoot", HumanBodyBones.RightFoot);

                SetRagdollBoneValue(ragdollWindow, "leftArm", HumanBodyBones.LeftUpperArm);
                SetRagdollBoneValue(ragdollWindow, "leftElbow", HumanBodyBones.LeftLowerArm);
                SetRagdollBoneValue(ragdollWindow, "rightArm", HumanBodyBones.RightUpperArm);
                SetRagdollBoneValue(ragdollWindow, "rightElbow", HumanBodyBones.RightLowerArm);

                SetRagdollBoneValue(ragdollWindow, "middleSpine", HumanBodyBones.Spine);
                SetRagdollBoneValue(ragdollWindow, "head", HumanBodyBones.Head);
            }
        }

        private void SetRagdollBoneValue(ScriptableWizard window, string fieldName, HumanBodyBones bone)
        {
            var field = window.GetType().GetField(fieldName);
            field.SetValue(window, m_Animator.GetBoneTransform(bone));
            m_Animator.GetBoneTransform(bone).gameObject.layer = 13;
        }
    }
}
