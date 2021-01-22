/*------------------------------------------------------------------------------*/

// Developed by Rodrigo Dias
// Contact: rodrigoaadias@hotmail.com

/*-----------------------------------------------------------------------------*/

using UnityEngine;

namespace DiasGames.ThirdPersonSystem.ClimbingSystem
{
    public class LowerClimbAbility : ThirdPersonAbstractClimbing
    {
        public override bool TryEnterAbility()
        {
            if (HasFoundLedge(out frontHit))
            {
                if (FreeAboveLedge())
                    return true;
            }

            return base.TryEnterAbility();
        }

        public override bool ForceEnterAbility()
        {
            if (m_UseInputStateToEnter == InputEnterType.Noone)
                return false;

            if (!m_System.IsGrounded)
                return TryEnterAbility();

            return false;
        }

        public override void OnEnterAbility()
        {
            base.OnEnterAbility();
            m_System.m_Capsule.enabled = false; // Deactivate collider
        }

        private void Reset()
        {
            m_EnterState = "Climb.Lower Climb";
            m_TransitionDuration = 0.1f;
            m_FinishOnAnimationEnd = true;
            m_UseRootMotion = true;
            m_UseVerticalRootMotion = true;

            m_CastCapsuleRadius = 0.2f;
            m_VerticalLinecastStartPoint = 1.1f;
            m_VerticalLinecastEndPoint = 0.4f;
            m_MaxDistanceToFindLedge = 1f;
            m_CharacterOffsetFromLedge = new Vector3(0, 0.75f, 0.45f);
        }
    }
}
