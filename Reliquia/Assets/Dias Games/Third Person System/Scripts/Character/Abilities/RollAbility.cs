/*------------------------------------------------------------------------------*/

// Developed by Rodrigo Dias
// Contact: rodrigoaadias@hotmail.com

/*-----------------------------------------------------------------------------*/

using UnityEngine;

namespace DiasGames.ThirdPersonSystem
{
    public class RollAbility : ThirdPersonAbility
    {
        [Tooltip("Speed during a roll")] public float speedOnRolling = 8f;
        [SerializeField] private float m_CapsuleHeight = 1f;
        
        public override bool TryEnterAbility()
        {
            return m_System.IsGrounded && m_InputManager.RelativeInput.magnitude > 0;
        }

        public override void OnEnterAbility()
        {
            base.OnEnterAbility();

            //playedSound = false;
            m_System.ChangeCapsuleSize(m_CapsuleHeight);

            if (!Mathf.Approximately(m_InputManager.RelativeInput.magnitude, 0))
                transform.rotation = GetRotationFromDirection(m_InputManager.RelativeInput); // Rotate character to direction of input
        }

        public override void FixedUpdateAbility()
        {
            base.FixedUpdateAbility();
            AddSpeedToRoll();
        }

        /// <summary>
        /// Move character to desired direction
        /// </summary>
        private void AddSpeedToRoll()
        {
            if (m_UseRootMotion)
                return;

            Vector3 vel = transform.forward * speedOnRolling;
            vel.y = m_System.m_Rigidbody.velocity.y;

            if (FreeOnMove(vel.normalized))
                m_System.m_Rigidbody.velocity = vel;
        }

        public override bool TryExitAbility()
        {
            return !m_System.IsGrounded;
        }

        private void Reset()
        {
            m_EnterState = "Roll";
            m_TransitionDuration = 0.1f;
            m_FinishOnAnimationEnd = true;
            m_UseRootMotion = false;
            m_UseInputStateToEnter = InputEnterType.ButtonDown;
            InputButton = InputReference.Roll;
        }
    }
}
