using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiasGames.ThirdPersonSystem
{
    public class CrawlAbility : ThirdPersonAbility
    {
        [SerializeField] private LayerMask m_ObstacleMask = (1 << 0) | (1 << 14) | (1 << 18) | (1 << 19) | (1 << 25);
        [SerializeField] private float m_CapsuleHeight = 0.5f;
        [SerializeField] private float m_TurnSpeed = 50f;

        [Tooltip("If character enter a region with lower height, must system auto crawl character?")]
        [SerializeField] private bool m_AutoCrawl = true;

        [SerializeField] private string m_CrawlingState = "Crawl";
        [SerializeField] private string m_FinishCrawl = "Crawl To Stand";


        public override bool TryEnterAbility()
        {
            if (m_System.IsGrounded)
                return true;

            return false;
        }

        public override bool ForceEnterAbility()
        {
            return !IsFreeAbove() && m_System.IsGrounded && m_AutoCrawl;
        }

        public override void OnEnterAbility()
        {
            base.OnEnterAbility();
            m_System.ChangeCapsuleSize(m_CapsuleHeight);
            m_FinishOnAnimationEnd = false;
        }

        public override void FixedUpdateAbility()
        {
            base.FixedUpdateAbility();

            if (m_AnimatorManager.HasFinishedAnimation(m_EnterState))
                SetState(m_CrawlingState);

            if (m_CurrentStatePlaying != m_CrawlingState)
                return;

            m_System.CalculateMoveVars();
            m_System.UpdateMovementAnimator();
            m_System.RotateToDirection(0, m_TurnSpeed);

            // Try exit ability
            bool inputToLeave = (m_UseInputStateToEnter == InputEnterType.ButtonPressing) ?
                !m_InputToEnter.IsPressed : m_InputStateSet;

            if ((!m_System.IsGrounded || inputToLeave) && IsFreeAbove())
            {
                SetState(m_FinishCrawl);
                m_FinishOnAnimationEnd = true;
            }
        }


        private bool IsFreeAbove()
        {
            Vector3 start = transform.position;
            RaycastHit hit;
            if (Physics.SphereCast(start, m_System.m_Capsule.radius, Vector3.up, out hit, m_System.CapsuleOriginalHeight, m_ObstacleMask))
            {
                if (hit.distance <= m_CapsuleHeight + 0.25f && hit.distance > m_CapsuleHeight)
                    return false;
            }

            return true;
        }

        private void Reset()
        {
            m_EnterState = "Stand to Crawl";
            m_TransitionDuration = 0.25f;
            m_FinishOnAnimationEnd = false;
            m_UseRootMotion = true;
            m_UseInputStateToEnter = InputEnterType.ButtonDown;
            InputButton = InputReference.Crawl;
        }
    }
}