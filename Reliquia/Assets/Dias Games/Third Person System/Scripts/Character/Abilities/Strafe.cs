using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiasGames.ThirdPersonSystem
{
    public class Strafe : ThirdPersonAbility
    {
        [SerializeField] private PhysicMaterial m_IdleFriction = null;

        private Transform m_Camera;
        private float weight = 0;
        public bool LookForward { get; set; } = true;

        public override void Initialize(ThirdPersonSystem mainSystem, AnimatorManager animatorManager, UnityInputManager inputManager)
        {
            base.Initialize(mainSystem, animatorManager, inputManager);

            m_Camera = Camera.main.transform;
        }

        public override bool TryEnterAbility()
        {
            return m_System.IsZooming && m_System.IsGrounded;
        }

        public override bool TryExitAbility()
        {
            return !m_System.IsZooming || !m_System.IsGrounded;
        }

        public override void FixedUpdateAbility()
        {
            base.FixedUpdateAbility();

            m_System.CalculateMoveVars();
            m_System.UpdateMovementAnimator();
            m_System.RotateByCamera();

            if (Mathf.Approximately(m_System.FreeMoveDirection.magnitude, 0))
                m_System.m_Capsule.sharedMaterial = m_IdleFriction;
            else
                m_System.m_Capsule.sharedMaterial = m_AbilityPhysicMaterial;
        }

        
        private void OnAnimatorIK(int layerIndex)
        {
            if (layerIndex != 0)
                return;

            weight = Mathf.Lerp(weight, (Active && LookForward) ? 1 : 0, 0.15f);

            m_System.m_Animator.SetLookAtPosition(m_Camera.position + m_Camera.forward * 50f);
            m_System.m_Animator.SetLookAtWeight(weight, 0.5f, 1f, 1f);
        }

        private void Reset()
        {
            m_EnterState = "Strafe";
            m_TransitionDuration = 0.2f;
            m_FinishOnAnimationEnd = false;
            m_UseRootMotion = true;
            m_AllowCameraZoom = true;
        }
    }
}