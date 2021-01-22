/*------------------------------------------------------------------------------*/

// Developed by Rodrigo Dias
// Contact: rodrigoaadias@hotmail.com

/*-----------------------------------------------------------------------------*/

using UnityEngine;

namespace DiasGames.ThirdPersonSystem
{
    public class JumpAbility : ThirdPersonAbility
    {
        [Tooltip("The maximum height of the character jump.")]
        [SerializeField] private float m_MaxJumpHeight = 1.6f;
        [Tooltip("The maximum horizontal speed that character can have during the jump")]
        [SerializeField] private float m_MaxHorSpeed = 7;

        [Tooltip("State of stationary jump")] [SerializeField] private string m_StationaryJumpState = "Air.JumpInPlace";

        private float VerticalSpeed { get { return Mathf.Sqrt(-2 * Physics.gravity.y * m_MaxJumpHeight); } }
        private bool m_MirrorJump = false;

        public override string GetEnterState()
        {
            if (Mathf.Approximately(m_InputManager.RelativeInput.magnitude, 0))
                return m_StationaryJumpState;

            m_MirrorJump = m_System.m_Animator.GetFloat("LeftFoot") > 0.6f;
            m_System.m_Animator.SetBool("Mirror", m_MirrorJump);
            return base.GetEnterState();
        }

        public override bool TryEnterAbility()
        {
            return m_System.IsGrounded;
        }

        public override void OnEnterAbility()
        {
            base.OnEnterAbility();

            m_UseRootMotion = false;
            m_UseVerticalRootMotion = false;

            if (!Mathf.Approximately(m_InputManager.RelativeInput.magnitude, 0))
                DoJump(VerticalSpeed);
            else
            {
                m_UseRootMotion = true;
                m_UseVerticalRootMotion = true;
            }
        }

        public override bool TryExitAbility()
        {
            return m_System.IsGrounded && !m_UseRootMotion;
        }

        public override void OnExitAbility()
        {
            base.OnExitAbility();

            m_UseRootMotion = false;
            m_UseVerticalRootMotion = false;
        }

        public override void FixedUpdateAbility()
        {
            base.FixedUpdateAbility();
            AddVelocityToJump();
        }

        /// <summary>
        /// Add velocity to direction of Jump
        /// </summary>
        public void AddVelocityToJump()
        {
            Vector3 vel = transform.forward * m_MaxHorSpeed; // Set velocity vector
            vel.y = m_System.m_Rigidbody.velocity.y; // Kepp vertical speed

            if (FreeOnMove(vel))
                m_System.m_Rigidbody.velocity = vel; // Set new velocity
        }


        /// <summary>
        /// Add force to character to make a jump
        /// </summary>
        /// <param name="power"></param>
        public void DoJump(float power)
        {
            // Change parameters to allow jumping
            m_System.GroundCheckDistance = 0.01f;
            m_System.IsGrounded = false;

            Vector3 direction = m_InputManager.RelativeInput.normalized;
            Vector3 velocity = direction * m_MaxHorSpeed + Vector3.up * power;

            m_System.m_Rigidbody.velocity = velocity;

            // Get Rotation target
            transform.rotation = GetRotationFromDirection(direction);
        }

        private void Reset()
        {
            m_EnterState = "Air.Jump";
            m_TransitionDuration = 0.1f;
            m_FinishOnAnimationEnd = true;
            m_UseRootMotion = false;

            m_UseInputStateToEnter = InputEnterType.ButtonDown;
            InputButton = InputReference.Jump;
        }
    }
}
