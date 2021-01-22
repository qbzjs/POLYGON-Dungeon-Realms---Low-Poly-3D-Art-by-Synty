/*------------------------------------------------------------------------------*/

// Developed by Rodrigo Dias
// Contact: rodrigoaadias@hotmail.com

/*-----------------------------------------------------------------------------*/

using UnityEngine;

namespace DiasGames.ThirdPersonSystem.ClimbingSystem
{
    public class StepUpAbility : ThirdPersonAbstractClimbing
    {
        public override bool TryEnterAbility()
        {
            if (!m_System.IsGrounded)
                return false;

            if (HasFoundLedge(out frontHit) && m_InputManager.RelativeInput.z >= -0.1f)
            {
                if (FreeAboveLedge())
                    return true;
            }

            return base.TryEnterAbility();
        }

        public override void OnEnterAbility()
        {
            base.OnEnterAbility();
            m_System.m_Capsule.enabled = false; // Deactivate collider
        }

        private void Reset()
        {
            m_EnterState = "Climb.Step Up";
            m_TransitionDuration = 0.1f;
            m_FinishOnAnimationEnd = true;
            m_UseRootMotion = true;
            m_UseVerticalRootMotion = true;
            m_UseLaunchMath = false;

            m_CastCapsuleRadius = 0.15f;
            m_VerticalLinecastStartPoint = 0.6f;
            m_VerticalLinecastEndPoint = 0.15f;
            m_MaxDistanceToFindLedge = 0.5f;

            m_CharacterOffsetFromLedge = new Vector3(0, 0.55f, 0.3f);
        }
    }
}