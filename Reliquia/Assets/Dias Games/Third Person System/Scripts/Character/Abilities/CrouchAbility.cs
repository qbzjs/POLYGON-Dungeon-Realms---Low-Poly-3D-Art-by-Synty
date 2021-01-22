using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiasGames.ThirdPersonSystem
{
    public class CrouchAbility : ThirdPersonAbility
    {
        [SerializeField] private LayerMask m_ObstacleMask = (1 << 0) | (1 << 14) | (1 << 17) | (1 << 18) | (1 << 19) | (1 << 25);
        [SerializeField] private float m_CapsuleHeight = 1f;
        [Tooltip("If character enter a region with lower height, must system auto crouch character?")]
        [SerializeField] private bool m_AutoCrouch = true;

        bool IsHeightEnoughToCrouch = true;       // Only crouch if height is enough

        public override bool TryEnterAbility()
        {
            if (m_System.IsGrounded && IsHeightEnoughToCrouch && m_System.m_Capsule.height > m_CapsuleHeight)
                return true;

            return false;
        }

        public override bool ForceEnterAbility()
        {
            return !IsFreeAbove() && IsHeightEnoughToCrouch && m_AutoCrouch && !(m_System.ActiveAbility is CrawlAbility);
        }

        public override void OnEnterAbility()
        {
            base.OnEnterAbility();
            m_System.ChangeCapsuleSize(m_CapsuleHeight);
        }

        public override void FixedUpdateAbility()
        {
            base.FixedUpdateAbility();

            m_System.CalculateMoveVars();
            m_System.UpdateMovementAnimator();
            m_System.RotateToDirection();

        }

        public override bool TryExitAbility()
        {
            bool inputToLeave = (m_UseInputStateToEnter == InputEnterType.ButtonPressing) ?
                !m_InputToEnter.IsPressed : m_InputStateSet;

            return (!m_System.IsGrounded || inputToLeave) && IsFreeAbove();
        }

        private bool IsFreeAbove()
        {
            Vector3 start = m_System.GroundPoint();
            RaycastHit hit;
            if (Physics.SphereCast(start, m_System.m_Capsule.radius, Vector3.up, out hit, m_System.CapsuleOriginalHeight, m_ObstacleMask))
            {
                if (hit.distance <= m_System.CapsuleOriginalHeight)
                {
                    IsHeightEnoughToCrouch = hit.distance + m_System.m_Capsule.radius >= m_CapsuleHeight;
                    return false;
                }
            }

            return true;
        }

        private void Reset()
        {
            m_EnterState = "Crouch";
            m_TransitionDuration = 0.2f;
            m_FinishOnAnimationEnd = false;
            m_UseRootMotion = true;
            m_UseInputStateToEnter = InputEnterType.ButtonDown;
            InputButton = InputReference.Crouch;
        }
    }
}