using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiasGames.ThirdPersonSystem.ClimbingSystem
{
    public class LadderAbility : ThirdPersonAbility
    {
        [Tooltip("State to climb ladder on top")] [SerializeField] private string m_ClimbUp = "Climb Up.Brace Climb Up";
        [Tooltip("State to jump above ladder")] [SerializeField] private string m_HopUpStart = "Climb Up.Hop Up Start";
        [Tooltip("State to jump right")] [SerializeField] private string m_HopRightStart = "Climb Right.Hop Start";
        [Tooltip("State to jump left")] [SerializeField] private string m_HopLeftStart = "Climb Left.Hop Start";

        [SerializeField] private bool m_UseLaunchMath = true;
        [Tooltip("Ladder mask")] [SerializeField] private LayerMask m_LadderMask = (1 << 20);
        [Tooltip("Offset from forward face of ladder")] [SerializeField] private float m_OffsetFromLadder = 0.35f;
        [Tooltip("Box cast size")] [SerializeField] private Vector3 m_BoxCastSize = new Vector3(1.2f, 0.75f, 1.2f);
        [Tooltip("Positioning smothness on ladder")] [SerializeField] private float transitionTimeToPosition = 0.05f;
                
        public LayerMask LadderMask { get { return m_LadderMask; } }
        public bool CalculateLaunchParameters { get { return m_UseLaunchMath; } }

        // Internal var

        private bool updatingPosition = false;
        private Vector3 EnterTargetPosition = Vector3.zero;
        private float deltaPos;
        private float time = 0;
        private Quaternion EnterTargetRotation = Quaternion.identity;

        private Collider m_CurrentLadderCollider;
        private Collider m_LastLadderCollider = null;

        private enum LadderCastResult { Bottom, Both, Top, Noone}
        private LadderCastResult CurrentCastResult = LadderCastResult.Noone;
        private ClimbJumpType m_JumpDirection = ClimbJumpType.Noone;

        private ClimbJump m_ClimbJump;

        private Vector3 TargetPosition
        {
            get
            {
                Vector3 point = m_CurrentLadderCollider.ClosestPoint(m_CurrentLadderCollider.bounds.center - m_CurrentLadderCollider.transform.forward);
                point -= m_OffsetFromLadder * m_CurrentLadderCollider.transform.forward;
                point.y = transform.position.y;

                point = m_CurrentLadderCollider.transform.InverseTransformPoint(point);
                point.x = 0;
                point = m_CurrentLadderCollider.transform.TransformPoint(point);

                return point;
            }
        }
        public Vector3 GrabPosition { get { return transform.position + Vector3.up * m_System.m_Capsule.height; } }

        protected void Awake()
        {
            m_ClimbJump = GetComponent<ClimbJump>();
        }

        private void OnEnable()
        {
            m_ClimbJump.OnExitEvent += ResetLadder;
        }

        private void OnDisable()
        {
            m_ClimbJump.OnExitEvent -= ResetLadder;
        }

        private void ResetLadder()
        {
            m_LastLadderCollider = null;
        }

        public override bool TryEnterAbility()
        {
            if (AbilityExitFixedTime + 0.1f > Time.fixedTime)
                return false;

            LadderCastResult result = FoundLadder();
            if (result == LadderCastResult.Both)
            {
                if (m_System.ActiveAbility is ClimbJump)
                {
                    if (m_ClimbJump.JumpType != ClimbJumpType.FromGroundToHang ||
                        m_ClimbJump.JumpType != ClimbJumpType.FromGroundToLower)
                    {
                        if (m_ClimbJump.JumpTimeToTarget * 0.8f + m_ClimbJump.AbilityEnterFixedTime > Time.fixedTime)
                            return false;
                    }
                }

                return m_LastLadderCollider != m_CurrentLadderCollider && m_CurrentLadderCollider != null;
            }
            else if (result != LadderCastResult.Top)
                m_LastLadderCollider = null;

            return base.TryEnterAbility();
        }

        public override bool ForceEnterAbility()
        {
            if (m_UseInputStateToEnter == InputEnterType.Noone)
                return false;

            if (m_System.IsGrounded)
                m_LastLadderCollider = null;
            else
                return TryEnterAbility();

            return base.ForceEnterAbility();
        }


        public override void OnEnterAbility()
        {
            base.OnEnterAbility();

            m_System.m_Rigidbody.useGravity = false;
            m_System.m_Rigidbody.velocity = Vector3.zero;

            updatingPosition = true;
            EnterTargetPosition = TargetPosition;
            if (EnterTargetPosition.y < m_CurrentLadderCollider.bounds.min.y)
                EnterTargetPosition.y = m_CurrentLadderCollider.bounds.min.y;

            EnterTargetRotation = GetRotationFromDirection(m_CurrentLadderCollider.transform.forward);
            deltaPos = Vector3.Distance(EnterTargetPosition, transform.position) / transitionTimeToPosition;
            time = 0;

            m_JumpDirection = ClimbJumpType.Noone;
            CurrentCastResult = LadderCastResult.Both;
        }


        public override void FixedUpdateAbility()
        {
            base.FixedUpdateAbility();

            // Adjust position on start
            if (updatingPosition)
            {
                transform.position = Vector3.MoveTowards(transform.position, EnterTargetPosition, deltaPos * Time.deltaTime);
                transform.rotation = Quaternion.Lerp(transform.rotation, EnterTargetRotation, 0.2f);
                time += Time.deltaTime;

                if (Mathf.Approximately(Vector3.Distance(transform.position, EnterTargetPosition), 0) || time > transitionTimeToPosition)
                    updatingPosition = false;

                return;
            }

            CurrentCastResult = FoundLadder();

            if (CurrentCastResult == LadderCastResult.Both)
            {
                transform.position = TargetPosition;
                transform.rotation = Quaternion.Lerp(transform.rotation, 
                    GetRotationFromDirection(m_CurrentLadderCollider.transform.forward), 0.2f);
            }
        }

        public override void UpdateAbility()
        {
            base.UpdateAbility();

            // Forward value to be used in the animator controller
            float forwardValue = m_InputManager.Move.y;

            // Constantly check ladder
            switch (CurrentCastResult)
            {
                case LadderCastResult.Bottom:

                    // Climb Up
                    if (FreeAbove())
                    {
                        SetState(m_ClimbUp);
                        m_System.m_Capsule.enabled = false;
                    }
                    else // Check to Hop Up
                    {
                        if (m_InputManager.jumpButton.WasPressed && forwardValue > 0.5f)
                        {
                            SetState(m_HopUpStart);
                            m_JumpDirection = ClimbJumpType.Up;
                        }
                    }

                    // Stop Moving Up
                    forwardValue = Mathf.Clamp(forwardValue, -1f, 0);
                    break;
                case LadderCastResult.Top:
                    //Stop climbing down
                    forwardValue = Mathf.Clamp(forwardValue, 0, 1f);
                    break;
            }

            if (m_JumpDirection == ClimbJumpType.Noone)
            {
                if (m_InputManager.jumpButton.WasPressed)
                {
                    // Side Jumps
                    if (m_InputManager.Move.x > 0.5f)
                    {
                        SetState(m_HopRightStart);
                        m_JumpDirection = ClimbJumpType.Right;
                        return;
                    }
                    if (m_InputManager.Move.x < -0.5f)
                    {
                        SetState(m_HopLeftStart);
                        m_JumpDirection = ClimbJumpType.Left;
                        return;
                    }
                }

                if (m_InputManager.dropButton.WasPressed)
                    m_JumpDirection = ClimbJumpType.Drop;
            }

            m_AnimatorManager.SetForwardParameter(forwardValue, 0.05f);
        }

        public override bool TryExitAbility()
        {
            if (m_JumpDirection == ClimbJumpType.Drop || CurrentCastResult == LadderCastResult.Noone)
                return true;

            return m_System.IsGrounded && m_InputManager.Move.y < -0.5f;
        }

        public override void OnExitAbility()
        {
            base.OnExitAbility();
            m_System.m_Rigidbody.useGravity = true;
            m_System.m_Capsule.enabled = true;

            m_LastLadderCollider = m_CurrentLadderCollider;

            switch (m_JumpDirection)
            {
                case ClimbJumpType.Up:
                    m_ClimbJump.StartHopUp(GrabPosition - Vector3.up * m_System.m_Capsule.radius, GrabPosition + transform.forward * m_OffsetFromLadder, m_UseLaunchMath);
                    m_LastLadderCollider = null;
                    break;
                case ClimbJumpType.Right:
                    m_ClimbJump.StartClimbJump(m_JumpDirection, transform.right, GrabPosition, m_System.m_Capsule.height, m_UseLaunchMath);
                    break;
                case ClimbJumpType.Left:
                    m_ClimbJump.StartClimbJump(m_JumpDirection, -transform.right, GrabPosition, m_System.m_Capsule.height, m_UseLaunchMath);
                    break;
                case ClimbJumpType.Back:
                    break;
                case ClimbJumpType.Drop:
                    break;
                case ClimbJumpType.Noone:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Overlap ladders around and return Cast Result
        /// </summary>
        /// <returns></returns>
        private LadderCastResult FoundLadder()
        {
            Vector3 topCenter = GrabPosition;
            Vector3 bottomCenter = transform.position + Vector3.down * m_BoxCastSize.y * 0.5f;

            Collider[] topColliders = Physics.OverlapBox(topCenter, m_BoxCastSize * 0.5f, transform.rotation, m_LadderMask, QueryTriggerInteraction.Collide);
            Collider[] bottomColliders = Physics.OverlapBox(bottomCenter, m_BoxCastSize * 0.5f, transform.rotation, m_LadderMask, QueryTriggerInteraction.Collide);

            if(topColliders.Length > 0 && bottomColliders.Length > 0)
            {
                m_CurrentLadderCollider = topColliders[0];
                return LadderCastResult.Both;
            }

            if(topColliders.Length > 0)
            {
                m_CurrentLadderCollider = topColliders[0];
                return LadderCastResult.Top;
            }

            if(bottomColliders.Length > 0)
            {
                m_CurrentLadderCollider = bottomColliders[0];
                return LadderCastResult.Bottom;
            }

            return LadderCastResult.Noone;
        }

        private bool FreeAbove()
        {
            Vector3 Start = transform.position + Vector3.up * (m_System.m_Capsule.height + 0.5f);
            RaycastHit hit;
            if (!Physics.SphereCast(Start, 0.25f, transform.forward, out hit, m_OffsetFromLadder + m_BoxCastSize.z, Physics.AllLayers, QueryTriggerInteraction.Ignore))
                return true;

            return false;
        }

        private void Reset()
        {
            m_EnterState = "Climb.Ladder";
            m_TransitionDuration = 0.2f;
            m_FinishOnAnimationEnd = true;
            m_UseRootMotion = true;
            m_UseRotationRootMotion = true;
            m_UseVerticalRootMotion = true;
        }
    }
}