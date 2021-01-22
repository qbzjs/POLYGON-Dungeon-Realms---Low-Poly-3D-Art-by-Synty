using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Events;
using System;
using UnityEngine.Events;

namespace DiasGames.ThirdPersonSystem.ClimbingSystem
{
    public class BuildClimbingCharacter : BuildBasicCharacter
    {
        [MenuItem("Dias Games/Climbing System/Build Climbing Character")]
        public new static void ShowWindow()
        {
            GetWindow<BuildClimbingCharacter>(true, "Climbing Character Builder");
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            label = "Climbing Character Builder";
            m_AnimatorPath = "Assets/Dias Games/Climbing System/Animator/Climbing Animator.controller";
        }

        protected override void AddModifiers()
        {
            base.AddModifiers();
                        
            ///
            DiasGames.ThirdPersonSystem.ClimbingSystem.ClimbIK m_ClimbIK = character.GetComponent<DiasGames.ThirdPersonSystem.ClimbingSystem.ClimbIK>();
            if (m_ClimbIK == null)
            {
                m_ClimbIK = Undo.AddComponent<DiasGames.ThirdPersonSystem.ClimbingSystem.ClimbIK>(character);
                UpdateModifierList(m_ClimbIK);
            }

            LayerMask m_FootLayers = (1 << 0) | (1 << 14) | (1 << 16) | (1 << 17) | (1 << 18) | (1 << 19) | (1 << 20) | (1 << 25) | (1 << 26);

            m_ClimbIK.m_FeetLayers = m_FootLayers;
            SerializedObject climbIK = new SerializedObject(m_ClimbIK);

            climbIK.FindProperty("m_FeetLayers").intValue = m_FootLayers;
            climbIK.FindProperty("m_HandOffset").vector3Value = new Vector3(0.15f,CharacterHeight - 0.35f, 0f);
            climbIK.FindProperty("m_HandOffsetOnHang").vector3Value = new Vector3(0.05f,CharacterHeight - 0.45f, 0f);

            climbIK.ApplyModifiedProperties();
        }

        protected override void AddAbilities()
        {
            base.AddAbilities();

            CharacterAudioManager audioManager = character.GetComponent<CharacterAudioManager>();

            AudioClip hopHorizontalClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Dias Games/Climbing System/Audio/male_hop_horizontal.wav");
            AudioClip hopUpClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Dias Games/Climbing System/Audio/male_hop_up.wav");
            AudioClip jumpClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Dias Games/Third Person System/Audio/male_jump.wav");

            FreeLocomotion m_FreeLocomotion = character.GetComponent<FreeLocomotion>();
            Strafe m_Strafe = character.GetComponent<Strafe>();
            JumpAbility m_Jump = character.GetComponent<JumpAbility>();
            FallAbility m_Fall = character.GetComponent<FallAbility>();
            CrouchAbility m_Crouch = character.GetComponent<CrouchAbility>();

            LayerMask m_FootLayers = (1 << 0) | (1 << 14) | (1 << 16) | (1 << 17) | (1 << 18) | (1 << 19) | (1 << 20) | (1 << 25) | (1 << 26);

            // Vault ability
            ClimbingSystem.VaultAbility m_Vault = character.GetComponent<ClimbingSystem.VaultAbility>();
            if (m_Vault == null)
            {
                m_Vault = Undo.AddComponent<ClimbingSystem.VaultAbility>(character);

                if (m_Vault.OnEnterAbilityEvent == null)
                    m_Vault.OnEnterAbilityEvent = new UnityEvent();

                UnityEventTools.AddObjectPersistentListener(m_Vault.OnEnterAbilityEvent, audioManager.PlayVoiceSound, hopHorizontalClip);
            }

            m_Vault.ClimbingMask = (1 << 21);
            ////

            // Step Up ability Setup
            ClimbingSystem.StepUpAbility m_StepUp = character.GetComponent<ClimbingSystem.StepUpAbility>();
            if (m_StepUp == null)
                m_StepUp = Undo.AddComponent<ClimbingSystem.StepUpAbility>(character);

            m_StepUp.ClimbingMask = (1 << 16);
            ////

            // Half Climb ability Setup
            ClimbingSystem.LowerClimbAbility m_LowerClimb = character.GetComponent<ClimbingSystem.LowerClimbAbility>();
            if (m_LowerClimb == null)
            {
                m_LowerClimb = Undo.AddComponent<ClimbingSystem.LowerClimbAbility>(character);

                if (m_LowerClimb.OnEnterAbilityEvent == null)
                    m_LowerClimb.OnEnterAbilityEvent = new UnityEvent();

                UnityEventTools.AddObjectPersistentListener(m_LowerClimb.OnEnterAbilityEvent, audioManager.PlayVoiceSound, hopUpClip);
            }

            m_LowerClimb.ClimbingMask = (1 << 17);
            ////

            // Climbing ability setup
            ClimbingSystem.ClimbingAbility m_Climbing = character.GetComponent<ClimbingSystem.ClimbingAbility>();
            if (m_Climbing == null)
                m_Climbing = Undo.AddComponent<ClimbingSystem.ClimbingAbility>(character);

            SerializedObject climb = new SerializedObject(m_Climbing);
            climb.FindProperty("m_ClimbableMask").intValue = (1 << 18) | (1 << 19);
            climb.FindProperty("m_CharacterOffsetFromLedge").vector3Value = new Vector3(0, CharacterHeight - 0.4f, 0.3f);
            climb.FindProperty("m_CharacterOffsetOnHang").vector3Value = new Vector3(0, CharacterHeight - 0.45f, 0.3f);
            climb.ApplyModifiedProperties();
            ////
            ///
            // Climb Jump ability setup
            ClimbingSystem.ClimbJump m_ClimbJump = character.GetComponent<ClimbingSystem.ClimbJump>();
            if (m_ClimbJump == null)
            {
                m_ClimbJump = Undo.AddComponent<ClimbingSystem.ClimbJump>(character);

                if (m_ClimbJump.OnEnterAbilityEvent == null)
                    m_ClimbJump.OnEnterAbilityEvent = new UnityEvent();

                UnityEventTools.AddObjectPersistentListener(m_ClimbJump.OnEnterAbilityEvent, audioManager.PlayVoiceSound, jumpClip);
            }

            ////
            ///
            ClimbingSystem.DropToClimbAbility m_DropToClimb = character.GetComponent<ClimbingSystem.DropToClimbAbility>();
            if (m_DropToClimb == null)
                m_DropToClimb = Undo.AddComponent<ClimbingSystem.DropToClimbAbility>(character);

            m_DropToClimb.DroppableLayer = (1 << 19) | (1 << 20);

            ClimbingSystem.LadderAbility m_Ladder = character.GetComponent<ClimbingSystem.LadderAbility>();
            if (m_Ladder == null)
                m_Ladder = Undo.AddComponent<ClimbingSystem.LadderAbility>(character);
            
            WallRun m_WallRun = character.GetComponent<WallRun>();
            if (m_WallRun == null)
                m_WallRun = Undo.AddComponent<WallRun>(character);
            
            WallClimb m_WallClimb = character.GetComponent<WallClimb>();
            if (m_WallClimb == null)
                m_WallClimb = Undo.AddComponent<WallClimb>(character);

            // -------------------------- SET IGNORE ABILITIES ---------------------------------- //

            // Climb ladder
            AddIgnoreAbility(m_Ladder, m_FreeLocomotion);
            AddIgnoreAbility(m_Ladder, m_Strafe);
            AddIgnoreAbility(m_Ladder, m_Fall);
            AddIgnoreAbility(m_Ladder, m_Jump);
            AddIgnoreAbility(m_Ladder, m_ClimbJump);

            // Step up
            AddIgnoreAbility(m_StepUp, m_FreeLocomotion);
            AddIgnoreAbility(m_StepUp, m_Strafe);

            // Lower climb
            AddIgnoreAbility(m_LowerClimb, m_FreeLocomotion);
            AddIgnoreAbility(m_LowerClimb, m_Strafe);
            AddIgnoreAbility(m_LowerClimb, m_Fall);
            AddIgnoreAbility(m_LowerClimb, m_ClimbJump);
            AddIgnoreAbility(m_LowerClimb, m_Jump);

            // Climbing
            AddIgnoreAbility(m_Climbing, m_Fall);
            AddIgnoreAbility(m_Climbing, m_Jump);
            AddIgnoreAbility(m_Climbing, m_ClimbJump);

            // Drop to climb
            AddIgnoreAbility(m_DropToClimb, m_FreeLocomotion);
            AddIgnoreAbility(m_DropToClimb, m_Strafe);
            AddIgnoreAbility(m_DropToClimb, m_Crouch);

            // Vault
            AddIgnoreAbility(m_Vault, m_FreeLocomotion);
            AddIgnoreAbility(m_Vault, m_Strafe);
            AddIgnoreAbility(m_Vault, m_Jump);
            AddIgnoreAbility(m_Vault, m_Fall);
            AddIgnoreAbility(m_Vault, m_Crouch);

            // Climb Jump
            AddIgnoreAbility(m_ClimbJump, m_FreeLocomotion);
            AddIgnoreAbility(m_ClimbJump, m_Strafe);
            AddIgnoreAbility(m_ClimbJump, m_Jump);
            AddIgnoreAbility(m_ClimbJump, m_Climbing);
            AddIgnoreAbility(m_ClimbJump, m_Ladder);
            AddIgnoreAbility(m_ClimbJump, m_WallClimb);

            // Wall Run
            AddIgnoreAbility(m_WallRun, m_Jump);
            AddIgnoreAbility(m_WallRun, m_ClimbJump);

            // Wall Climb
            AddIgnoreAbility(m_WallClimb, m_ClimbJump);
            AddIgnoreAbility(m_WallClimb, m_WallRun);
            AddIgnoreAbility(m_WallClimb, m_Jump);
            AddIgnoreAbility(m_WallClimb, m_Fall);
            AddIgnoreAbility(m_WallClimb, m_DropToClimb);

            // -------------------------------------------------------------------------------- //
        }
    }
}
