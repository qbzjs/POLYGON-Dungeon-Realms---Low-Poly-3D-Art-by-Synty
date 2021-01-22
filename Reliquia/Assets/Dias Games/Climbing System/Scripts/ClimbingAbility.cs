/*------------------------------------------------------------------------------*/

// Developed by Rodrigo Dias
// Contact: rodrigoaadias@hotmail.com

/*-----------------------------------------------------------------------------*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiasGames.ThirdPersonSystem.ClimbingSystem
{
    public enum MovementInputType { Relative, Absolute}
    public class ClimbingAbility : ThirdPersonAbstractClimbing
    {
        [Tooltip("Offset from ledge  to be applied when character is hanging")]
        [SerializeField] private Vector3 m_CharacterOffsetOnHang = new Vector3(0, 1.5f, 0.3f);

        // --------------------------- STATES USED DURING CLIMBING ----------------------------- //

        [SerializeField] private string m_BraceGrabTopState = "Climb.Brace From Top";
        [SerializeField] private string m_HangGrabState = "Climb.Begin Hang";

        [SerializeField] private string m_BraceClimbUpState = "Climb Up.Brace Climb Up";
        [SerializeField] private string m_HangClimbUpState = "Climb Up.Hang Climb Up";
        [Header("Hop")]
        [SerializeField] private string m_HopUpStartState = "Climb Up.Hop Up Start";
        [SerializeField] private string m_HopUpEndState = "Climb Up.Hop Up End";

        [SerializeField] private string m_BraceDropDownState = "Climb Down.Brace Drop";
        [SerializeField] private string m_HangDropDownState = "Climb Down.Hang Drop";
        [SerializeField] private string m_LookBackState = "Climb Down.Look Back";
        [SerializeField] private string m_JumpBackState = "Climb Down.Jump Back Start";

        [SerializeField] private string m_BraceIdleState = "Climb.Braced Idle";
        [SerializeField] private string m_HangIdleState = "Climb.Hang Idle";
        [Header("Change Between Hang and Brace")]
        [SerializeField] private string m_HangToBraceIdleState = "Climb.Hang to Braced";
        [SerializeField] private string m_BraceToHangIdleState = "Climb.Braced to Hang";


        [SerializeField] private string m_RightSubState = "Climb Right";
        [SerializeField] private string m_LeftSubState = "Climb Left";

        [SerializeField] private string m_BraceShimmyState = "Braced Shimmy";
        [SerializeField] private string m_BraceFastShimmyState = "Braced Fast Shimmy";
        [SerializeField] private string m_FreehangShimmyState = "Shimmy";

        [SerializeField] private string m_BraceCornerOutState = "Brace Corner Out";
        [SerializeField] private string m_BraceCornerInState = "Brace Corner In";
        [SerializeField] private string m_HangCornerOutState = "Hang Corner Out";
        [SerializeField] private string m_HangCornerInState = "Hang Corner In";

        [SerializeField] private string m_LookSideState = "Look Side";
        [SerializeField] private string m_HopSideStartState = "Hop Start";
        [SerializeField] private string m_HopSideEndState = "Hop End";

        // ------------------------------------------------------------------------------------ //

        [SerializeField] private MovementInputType m_MovementInput = MovementInputType.Absolute;
        private Vector3 InputMove
        {
            get
            {
                return (m_MovementInput == MovementInputType.Absolute) ? m_InputManager.Move : 
                    new Vector3(m_System.FreeMoveDirection.x, m_System.FreeMoveDirection.z);
            }
        }

        // ---------------------------------------------------- SIDE CLIMBING PARAMETERS ------------------------------------------------ //

        [Tooltip("Value must be less than Side Distance From Char Origin to cast works fine")] [SerializeField] private float m_SideCapsuleRadius = 0.1f;
        [SerializeField] private float m_SideCapsuleHeight = 0.75f;
        [Tooltip("Max Distance from character position to check a ledge")] [SerializeField] private float m_SideMaxDistanceToCast = 1f;
        [Tooltip("Distance from ledge limit to set new position for character")] [SerializeField] private float m_CharacterOffsetOnSide = 0.25f;

        private bool m_SideAdjustment = false;

        // -------------------------------------------------------------------------------------------------------------------------- //

        private string startState = string.Empty;
        private bool bWallOnFoot = false; // Is there wall in front of feet?

        // Components
        private ClimbIK m_ClimbIK; // Controls climbing IK
        private ClimbJump m_ClimbJump;
        private WallRun m_WallRun;

        // --------------------- FAST MOVE ON CLIMBING -------------------------- //

        private float m_MoveMultiplier = 1f;
        private float m_MultiplierLastTime = 0;

        // --------------------------------------------------------------------- //
        
        private float timeWithoutFoundLedge = 0;        // Control how much time character stay trying climbing but don't find ledge

        // ------------------------------------------------------------------------------------- //

        /// <summary>
        /// Every time character starts a jump from a ledge, it is set to true. Variable is reset when character grab a different ledge
        /// or when character lands. Can also be reset by a method called ResetJumpLedge
        /// </summary>
        private bool m_HasJumpFromLedge = false;

        /// <summary>
        /// Desired direction of the jump in a ledge jump
        /// </summary>
        private Vector3 m_JumpDirection = Vector3.zero;

        /// <summary>
        /// Desired climb jump type
        /// </summary>
        private ClimbJumpType m_ClimbJumpType = ClimbJumpType.Back;


        protected override void Awake()
        {
            base.Awake();
            m_ClimbIK = GetComponent<ClimbIK>();
        }

        public override void Initialize(ThirdPersonSystem mainSystem, AnimatorManager animatorManager, UnityInputManager inputManager)
        {
            base.Initialize(mainSystem, animatorManager, inputManager);

            m_ClimbJump = m_System.CharacterAbilities.Find(x => x is ClimbJump) as ClimbJump;
            m_WallRun = m_System.CharacterAbilities.Find(x => x is WallRun) as WallRun;
        }

        // Reset ledge every time ClimbJump ends
        private void OnEnable()
        {
            m_ClimbJump.OnExitEvent += ResetJumpLedge;
        }
        private void OnDisable()
        {
            m_ClimbJump.OnExitEvent -= ResetJumpLedge;
        }

        /// <summary>
        /// Reset Jump from ledge parameter to allow climb again the same ledge
        /// </summary>
        private void ResetJumpLedge()
        {
            m_HasJumpFromLedge = false;
        }
        
        /// <summary>
        /// Get a state by side checking Relative Horizontal Input
        /// </summary>
        /// <param name="state">Target state that exists in right and left sides</param>
        /// <returns>Full path to desired state with the side set correctly</returns>
        private string GetSideState(string state)
        {
            string subState = (InputMove.x > 0) ? m_RightSubState : m_LeftSubState;
            return string.Join(".", subState, state);
        }

        /// <summary>
        /// Get a side state
        /// </summary>
        /// <param name="sideSubState">Can be right or left</param>
        /// <param name="state">Target state</param>
        /// <returns>Full path to desired state</returns>
        private string GetSideState(string sideSubState, string state)
        {
            return string.Join(".", sideSubState, state);
        }

        /// <summary>
        /// Check conditions to choose the right enter state for climbing
        /// </summary>
        /// <returns>Correct enter state to climb</returns>
        public override string GetEnterState()
        {
            // Check if character is above ledge
            bool IsPlayerAboveLedge = (transform.position.y + characterOffset.y) > topHit.point.y && m_System.m_Rigidbody.velocity.y < -2f;

            WallOnFeet();
            // Check if there is a wall to place feet
            if (bWallOnFoot)
            {
                float rightDot = Vector3.Dot(frontHit.normal, transform.right);
                float leftDot = Vector3.Dot(frontHit.normal, -transform.right);

                if (m_System.LastAbility == m_ClimbJump && m_ClimbJump.JumpType == ClimbJumpType.Right || rightDot > 0.5f)
                    return GetSideState(m_RightSubState, m_HopSideEndState);

                if (m_System.LastAbility == m_ClimbJump && m_ClimbJump.JumpType == ClimbJumpType.Left || leftDot > 0.5f)
                    return GetSideState(m_LeftSubState, m_HopSideEndState);

                if (m_ClimbJump.JumpType == ClimbJumpType.Up && m_System.LastAbility == m_ClimbJump)
                    return m_HopUpEndState;


                if (m_System.LastAbility == m_WallRun)
                {
                    return m_WallRun.CharWallDirection == WallDirection.Right ?
                         GetSideState(m_RightSubState, m_HopSideEndState) : GetSideState(m_LeftSubState, m_HopSideEndState);
                }

                if (IsPlayerAboveLedge || (m_System.LastAbility == m_ClimbJump && m_ClimbJump.JumpType == ClimbJumpType.Back) || m_System.LastAbility is FallAbility)
                    return m_BraceGrabTopState;
            }
            else
                return m_HangGrabState;

            return base.GetEnterState();
        }

        public override bool TryEnterAbility()
        {
            if (m_System.IsGrounded)
            {
                m_HasJumpFromLedge = false;
                return false;
            }

            //Check if climb jump is active
            if (m_ClimbJump.Active)
            {
                float multiplier = (m_ClimbJump.JumpType == ClimbJumpType.Right || m_ClimbJump.JumpType == ClimbJumpType.Left) ?
                    0.7f : 0.9f;
                if (m_ClimbJump.JumpTimeToTarget * multiplier + m_ClimbJump.AbilityEnterFixedTime > Time.fixedTime)
                    return false;
            }

            if (HasFoundLedge(out frontHit, false))
            {
                if (m_HasJumpFromLedge && m_CurrentLedgeTransform == frontHit.transform)
                    return false;

                WallOnFeet();
                return true;
            }

            return base.TryEnterAbility();
        }


        public override void OnEnterAbility()
        {
            base.OnEnterAbility();

            m_FinishOnAnimationEnd = false;
            startState = GetEnterState();

            if (m_ClimbIK != null)
                m_ClimbIK.OnHang = !bWallOnFoot;
            
            bool right = CanClimbSide(1, false);
            bool left = CanClimbSide(-1, false);

            // Adjust position if character grab on the limits of the ledge
            if (!right && left)
                frontHit.point -= hitReference.right * m_CharacterOffsetOnSide * 0.5f;
            if (right && !left)
                frontHit.point += hitReference.right * m_CharacterOffsetOnSide * 0.5f;

            m_HasJumpFromLedge = false;
        }


        public override bool TryExitAbility()
        {
            // if system don't find ledge for a time, exit ability
            if (timeWithoutFoundLedge >= 0.1f)
            {
                timeWithoutFoundLedge = 0;
                return true;
            }

            return base.TryExitAbility();
        }


        public override void OnExitAbility()
        {
            if (m_ClimbIK != null)
                m_ClimbIK.OnHang = false;

            if (m_HasJumpFromLedge && m_ClimbJumpType != ClimbJumpType.Drop)
            {
                if (m_ClimbJumpType == ClimbJumpType.Up)
                    m_ClimbJump.StartHopUp(GrabPosition, transform.position + transform.forward * characterOffset.z +
                        Vector3.up * m_VerticalLinecastStartPoint, UseLaunchMath);
                else
                    m_ClimbJump.StartClimbJump(m_ClimbJumpType, m_JumpDirection, GrabPosition, m_VerticalLinecastStartPoint,
                        UseLaunchMath, CurrentLedgeTransform.GetComponent<Collider>());
            }

            m_FinishOnAnimationEnd = false;
            m_SideAdjustment = false;
            m_JumpDirection = Vector3.zero;
            m_ClimbJumpType = ClimbJumpType.Back;

            m_System.UpdatePositionOnMovableObject(null);

            base.OnExitAbility();
        }

        /// <summary>
        /// Check if exists wall in front of feets
        /// </summary>
        private void WallOnFeet()
        {
            Vector3 Start = transform.position - transform.forward * m_CastCapsuleRadius * 2; // Set Start position to cast

            RaycastHit wallHit;
            Vector3 direction = (frontHit.point - transform.position).normalized; // Get direction to cast
            direction.y = 0;

            // Cast both feet
            bool rightWall = Physics.SphereCast(Start + Vector3.right * 0.25f, m_CastCapsuleRadius*0.15f, direction, out wallHit, m_MaxDistanceToFindLedge + m_CastCapsuleRadius * 2 + 0.5f, m_System.GroundMask);
            bool leftWall = Physics.SphereCast(Start + Vector3.left * 0.25f, m_CastCapsuleRadius*0.15f, direction, out wallHit, m_MaxDistanceToFindLedge + m_CastCapsuleRadius * 2 + 0.5f, m_System.GroundMask);

            // Only set bWallOnFoot bool if both feet have the same value
            if (rightWall && leftWall)
                bWallOnFoot = true;

            if (!rightWall && !leftWall)
                bWallOnFoot = false;

            if(Time.fixedTime - AbilityEnterFixedTime < 0.1f)
            {
                if (!rightWall || !leftWall)
                    bWallOnFoot = false;
            }
        }
        
        public override void FixedUpdateAbility()
        {
            base.FixedUpdateAbility();

            if (m_ClimbIK != null)
                m_ClimbIK.OnHang = !bWallOnFoot;

            characterOffset = (bWallOnFoot) ? m_CharacterOffsetFromLedge : m_CharacterOffsetOnHang;

            m_System.UpdatePositionOnMovableObject(m_CurrentLedgeTransform);

            if (m_FinishOnAnimationEnd)
                return;

            if (m_CurrentStatePlaying == m_LookBackState)
                m_ClimbIK.ApplyRightHandIK = false;
            else
            {
                if (!m_System.m_Animator.IsInTransition(0) &&
                m_ClimbIK.ApplyRightHandIK == false)
                    m_ClimbIK.ApplyRightHandIK = true;

            }

            // Check if start animation has ended
            if (m_CurrentStatePlaying == startState)
            {
                if (m_AnimatorManager.HasFinishedAnimation(startState) || Time.fixedTime - AbilityEnterFixedTime > 1)
                { 
                    string state = (bWallOnFoot) ? m_BraceIdleState : m_HangIdleState;
                    SetState(state, m_TransitionDuration);
                }
            }

            CheckToTurn();

            // constantly update found ledge
            if (HasFoundLedge(out frontHit, true))
            {
                WallOnFeet();
                timeWithoutFoundLedge = 0;

                // ------------------------------ CHANGE BETWEEN HANG AND BRACED CLIMBING -------------------------------- //

                //  Check shimmy conditions by state tag
                if (bWallOnFoot)
                {
                    if (m_System.m_Animator.GetCurrentAnimatorStateInfo(AnimatorManager.BaseLayerIndex).IsTag("Hang"))
                        StartCoroutine(ChangeClimbType(ClimbType.Braced));
                }
                else
                {
                    if (m_System.m_Animator.GetCurrentAnimatorStateInfo(AnimatorManager.BaseLayerIndex).IsTag("Brace"))
                        StartCoroutine(ChangeClimbType(ClimbType.Hang));
                }

                // ----------------------------------------------------------------------------------------------------- //

                m_CurrentLedgeTransform = topHit.transform; // Set current ledge as the ledge that character is holding
                if (!m_System.IsCoroutinePlaying)
                {
                    SetCharacterPositionOnLedge();
                    if (m_ClimbIK != null && !m_SideAdjustment)
                        m_ClimbIK.RunIK(topHit, m_ClimbableMask, m_CurrentLedgeTransform);
                }


            }
            else
                timeWithoutFoundLedge += Time.deltaTime; // Count time without finding ledge
            
            SetAnimationsStates();
        }

        protected override void SetCharacterPositionOnLedge(bool setPosition = true, bool setRotation = true)
        {
            base.SetCharacterPositionOnLedge(setPosition, setRotation);

            if (m_FixingSidePosition)
            {
                if (m_FixingTime < 0.1f)
                {
                    m_SideTargetFixPos += m_System.DeltaPos;
                    transform.position = Vector3.MoveTowards(transform.position, m_SideTargetFixPos, m_FixingStep * Time.fixedDeltaTime);
                    m_FixingTime += Time.fixedDeltaTime;
                }
                else
                    m_FixingSidePosition = false;
            }
        }

        public override void UpdateAbility()
        {
            base.UpdateAbility();

            InputControl();
        }


        private void SetAnimationsStates()
        {
            if (m_CurrentStatePlaying == startState || m_FinishOnAnimationEnd || m_System.IsCoroutinePlaying)
                return;
            
            // Look Side
            if (!CanClimbSide(InputMove.x, true))
            {
                SetState(bWallOnFoot ? GetSideState(m_LookSideState) : m_HangIdleState);
                return;
            }

            if (m_SideAdjustment)
                return;

            // Look Back
            if (InputMove.y < -0.85f && Mathf.Abs(InputMove.x) < 0.3f && bWallOnFoot)
            {
                SetState(m_LookBackState);
                return;
            }

            // Idle and Shimmy states
            if (Mathf.Abs(InputMove.x) < 0.2f)
            {
                SetState(bWallOnFoot ? m_BraceIdleState : m_HangIdleState);
            }
            else
            {
                if (Mathf.Abs(InputMove.y) < 0.5f)
                {
                    if (m_MoveMultiplier > 1.5f && bWallOnFoot)
                        SetState(GetSideState(m_BraceFastShimmyState));
                    else
                        SetState(bWallOnFoot ? GetSideState(m_BraceShimmyState) : GetSideState(m_FreehangShimmyState));
                }
            }
        }

        private void InputControl()
        {
            if (m_SideAdjustment)
                return;

            if (m_CurrentStatePlaying == startState || m_FinishOnAnimationEnd || m_System.IsCoroutinePlaying)
                return;

            // Fast move
            if (m_InputManager.jumpButton.WasPressed)
            {
                m_MoveMultiplier = 2f;
                m_MultiplierLastTime = Time.fixedTime + 0.5f;
            }

            if (m_MultiplierLastTime <= Time.fixedTime)
                m_MoveMultiplier = 1f;

            // ----------------------- DROP ------------------------------------------ //
            if (m_InputManager.dropButton.WasPressed)
            {
                string state = (bWallOnFoot) ? m_BraceDropDownState : m_HangDropDownState;
                SetState(state);
                PerformClimbJump(Vector3.zero, ClimbJumpType.Drop);
                return;
            }

            //Climb up
            if (InputMove.y > 0.5f && Mathf.Abs(InputMove.x) < 0.3f)
            {
                if (FreeAboveLedge())
                {
                    // -------------- CLIMB UP --------------- //

                    string state = (bWallOnFoot) ? m_BraceClimbUpState : m_HangClimbUpState;
                    m_FinishOnAnimationEnd = true;
                    m_System.m_Capsule.enabled = false;
                    SetState(state);
                    return;

                    // ------------------------------------- //
                }
                else
                {
                    if (m_InputManager.jumpButton.WasPressed && bWallOnFoot)
                    {
                        // --------------- HOP UP ---------------- //

                        PerformClimbJump(transform.forward, ClimbJumpType.Up);
                        SetState(m_HopUpStartState);
                        return;

                        // ------------------------------------- //
                    }
                }
            }
            
            // ------------------------------------------------- SIDE JUMP --------------------------------------------------------------------------- //

            if (m_AnimatorManager.IsPlayingState(GetSideState(m_LookSideState), AnimatorManager.BaseLayerIndex))
            {
                if (m_InputManager.jumpButton.WasPressed)
                {
                    bool IsRight = InputMove.x > 0;
                    SetState(GetSideState(m_HopSideStartState));
                    PerformClimbJump(IsRight ? transform.right : -transform.right,
                        IsRight ? ClimbJumpType.Right : ClimbJumpType.Left);
                    return;
                }
            }

            // ------------------------------------------------------------------------------------------------------------------------------------- //



            // ------------------------------------------------- BACK JUMP --------------------------------------------------------------------------- //

            if (m_AnimatorManager.IsPlayingState(m_LookBackState, AnimatorManager.BaseLayerIndex))
            {
                if (m_InputManager.jumpButton.WasPressed)
                {
                    SetState(m_JumpBackState);
                    PerformClimbJump(-transform.forward, ClimbJumpType.Back);
                    return;
                }
            }

            // ------------------------------------------------------------------------------------------------------------------------------------- /

            SetAnimationsStates();
        }

        /// <summary>
        /// Called to do a climb jump
        /// </summary>
        private void PerformClimbJump(Vector3 targetDirection, ClimbJumpType targetClimbJump)
        {
            m_FinishOnAnimationEnd = true;
            m_HasJumpFromLedge = true;
            m_JumpDirection = targetDirection;
            m_ClimbJumpType = targetClimbJump;
        }

        /// <summary>
        /// This method cast ledge on side to check if character can shimmy or not. If character get the limit of a ledge, it automatcally fix character position
        /// </summary>
        /// <param name="hordirection"></param>
        /// <param name="canTurn"></param>
        /// <returns></returns>
        private bool CanClimbSide(float hordirection, bool canTurn)
        {
            if (Mathf.Approximately(hordirection, 0))
                return true;

            Vector3 HandReference = transform.position + Vector3.up * 1.5f;
            float direction = (hordirection > 0) ? 1 : -1;

            ////////////////////////////////////////////////////////////////////////////////////////////
            // Set capsule points to cast
            ////////////////////////////////////////////////////////////////////////////////////////////

            Vector3 cp1 = transform.position + transform.right * direction * (m_CharacterOffsetOnSide + 0.2f);

            if (!canTurn)
            {
                GameObject getRotationGameObject = new GameObject();
                getRotationGameObject.transform.position = topHit.point;
                getRotationGameObject.transform.forward = -frontHit.normal;
                cp1 = topHit.point + getRotationGameObject.transform.right * direction * (m_CharacterOffsetOnSide + 0.2f) + frontHit.normal * m_SideMaxDistanceToCast * 0.5f;


                Destroy(getRotationGameObject.gameObject);
            }

            cp1.y = HandReference.y - m_SideCapsuleHeight * 0.5f;

            Vector3 cp2 = cp1 + Vector3.up * m_SideCapsuleHeight;

            ////////////////////////////////////////////////////////////////////////////////////////////



            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // First cast: Cast on side, in direction of movement. If find a ledge, return true to allow character keeping moving
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            RaycastHit hit;
            if (Physics.CapsuleCast(cp1, cp2, m_SideCapsuleRadius, -frontHit.normal, out hit, m_SideMaxDistanceToCast, ClimbingMask | (1 << 21)))
            {
                if (hit.transform == topHit.transform)
                    return true;
            }

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////





            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Second cast: this cast check the ledge limit and set character position on the limit
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            cp1 += transform.forward * characterOffset.z;
            cp2 += transform.forward * characterOffset.z;

            Vector3 newPos = Vector3.zero;
            RaycastHit[] hitsFromSide = Physics.CapsuleCastAll(cp1, cp2, m_SideCapsuleRadius, -transform.right * direction, m_SideMaxDistanceToCast, ClimbingMask);
            foreach (RaycastHit hitSide in hitsFromSide)
            {
                if (hitSide.transform == topHit.transform)
                {
                    if (hitSide.point == Vector3.zero) { return false; }

                    newPos = transform.InverseTransformPoint(hitSide.point);

                    newPos.z = 0;
                    newPos.y = 0;
                    newPos.x -= m_CharacterOffsetOnSide * direction;

                    newPos = transform.TransformPoint(newPos);

                    break;
                }
            }

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Third cast: this cast check condition to character turn a ledge only if ledge is big enough to allow this. 
            // First, it overlaps the region to not allow turn if a obstacle is in there.
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (canTurn)
            {
                Vector3 cp3 = cp1 + transform.forward * (m_SideCapsuleRadius + m_System.m_Capsule.radius);
                Vector3 cp4 = cp2 + transform.forward * (m_SideCapsuleRadius + m_System.m_Capsule.radius);

                if (!Physics.CheckCapsule(cp3, cp4, m_SideCapsuleRadius, m_ObstacleMask, QueryTriggerInteraction.Ignore))
                {
                    if (Physics.CapsuleCast(cp3, cp4, m_SideCapsuleRadius, -transform.right * direction, out hit, m_SideMaxDistanceToCast, ClimbingMask))
                    {
                        if (hit.transform == topHit.transform)
                        {
                            StartCoroutine(TurnCharacterOnLedge(direction));
                            return true;
                        }
                    }
                }
            }

            if (newPos != Vector3.zero)
                FixCharacterPositionOnSide(newPos);

            return false;
        }




        /// <summary>
        ///  Enumarator called to turn a character in a corner
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        IEnumerator TurnCharacterOnLedge(float direction)
        {
            Vector3 startDirection = transform.forward;
            if (!m_SideAdjustment)
            {
                m_SideAdjustment = true;

                string sideSubState = (direction > 0) ? m_RightSubState : m_LeftSubState;
                string cornerState = (bWallOnFoot) ? m_BraceCornerOutState : m_HangCornerOutState;

                SetState(GetSideState(sideSubState, cornerState));

                while (!m_AnimatorManager.HasFinishedAnimation(GetSideState(sideSubState, cornerState)))
                    yield return null;

                SetState(bWallOnFoot ? m_BraceIdleState : m_HangIdleState);

                m_SideAdjustment = false;
            }
        }

        private bool m_FixingSidePosition = false;
        private float m_FixingTime = 0;
        private float m_FixingStep = 0;
        private Vector3 m_SideTargetFixPos = Vector3.zero;

        /// <summary>
        /// Fix character side position on the end of the ledge.
        /// </summary>
        /// <param name="target"></param>
        private void FixCharacterPositionOnSide(Vector3 target)
        {
            if (!m_FinishOnAnimationEnd && !m_FixingSidePosition)
            {
                m_FixingSidePosition = true;
                m_FixingStep = Vector3.Distance(target, transform.position) / 0.1f;
                m_FixingTime = 0;
                m_SideTargetFixPos = target;
            }
        }


        private enum ClimbType { Braced, Hang }
        /// <summary>
        /// Change between braced and hang climb
        /// </summary>
        /// <param name="m_ClimbType"></param>
        /// <returns></returns>
        IEnumerator ChangeClimbType(ClimbType m_ClimbType)
        {
            if (!m_System.IsCoroutinePlaying && !m_SideAdjustment)
            {
                m_System.IsCoroutinePlaying = true;

                string state = (m_ClimbType == ClimbType.Braced) ? m_HangToBraceIdleState : m_BraceToHangIdleState;

                if (m_ClimbType == ClimbType.Braced)
                {
                    if (m_ClimbIK != null)
                        m_ClimbIK.OnHang = false;
                }

                SetState(state);

                while (m_CurrentStatePlaying != state)
                    yield return null;

                while (!m_AnimatorManager.HasFinishedAnimation(state))
                    yield return null;

                state = (m_ClimbType == ClimbType.Braced) ? m_BraceIdleState : m_HangIdleState;

                if (m_ClimbType == ClimbType.Hang)
                {
                    if (m_ClimbIK != null)
                        m_ClimbIK.OnHang = true;
                }

                yield return new WaitForSeconds(Time.fixedDeltaTime);

                SetState(state, 0.05f);
                m_System.IsCoroutinePlaying = false;

            }
        }

        /// <summary>
        /// Cast ledge sides to make character turn
        /// </summary>
        private void CheckToTurn()
        {
            if (Time.fixedTime - AbilityEnterFixedTime < 1f)
                return;

            Vector3 capsulePoint1 = transform.position + Vector3.up * (m_VerticalLinecastStartPoint - m_CastCapsuleRadius);
            Vector3 capsulePoint2 = transform.position + Vector3.up * (m_VerticalLinecastEndPoint + m_CastCapsuleRadius);

            float castDirection = InputMove.x;
            RaycastHit sideHit;
            if (Physics.CapsuleCast(capsulePoint1, capsulePoint2, m_CastCapsuleRadius, transform.right * castDirection, out sideHit, 0.6f, m_ClimbableMask))
            {
                Vector3 directionToCheckLedge = (sideHit.point - transform.position); // Get direction from player to point that found ledge
                directionToCheckLedge.y = 0;

                float step = 0.6f / m_Iterations;

                for (int i = 0; i < m_Iterations; i++)
                {
                    float endCast = m_VerticalLinecastEndPoint;

                    if (m_UpdateCastByVerticalSpeed)
                    {
                        // Adjust linecast size according velocity of jump
                        endCast = m_VerticalLinecastEndPoint + m_System.m_Rigidbody.velocity.y / 10f;
                        endCast = Mathf.Clamp(endCast, 0, m_VerticalLinecastEndPoint);
                    }

                    //Sets start point and endpoint of linecast
                    Vector3 Start = transform.position + Vector3.up * m_VerticalLinecastStartPoint + directionToCheckLedge.normalized * step * i;
                    Vector3 End = transform.position + Vector3.up * endCast + directionToCheckLedge.normalized * step * i;

                    if (Physics.Raycast(Start, Vector3.down, Start.y - End.y, m_ClimbableMask))
                    {
                        hitReference.forward = sideHit.normal;
                        float angleDelta = transform.rotation.eulerAngles.y - hitReference.eulerAngles.y;

                        string subState = castDirection > 0 ? m_RightSubState : m_LeftSubState;
                        string cornerState = (bWallOnFoot) ? m_BraceCornerInState : m_HangCornerInState;

                        string state = GetSideState(cornerState);

                        StartCoroutine(InternalTurn(angleDelta, state, CalculatePositionOnLedge(sideHit) - transform.forward*0.6f + sideHit.normal * 0.05f));
                    }
                }
            }
        }

        IEnumerator InternalTurn(float angleDelta, string state, Vector3 targetPosition)
        {
            if (!m_System.IsCoroutinePlaying)
            {
                bool initialrootMotion = m_UseRootMotion;
                m_UseRootMotion = false;

                m_System.IsCoroutinePlaying = true;
                SetState(state);
                while (!m_System.m_Animator.GetCurrentAnimatorStateInfo(AnimatorManager.BaseLayerIndex).IsName(state))
                    yield return null;

                float clipDuration = m_System.m_Animator.GetCurrentAnimatorStateInfo(AnimatorManager.BaseLayerIndex).length;

                Quaternion desiredRot = Quaternion.Euler(0, transform.rotation.eulerAngles.y + angleDelta, 0);
                bool updatePos = bWallOnFoot;

                while (m_System.m_Animator.GetCurrentAnimatorStateInfo(AnimatorManager.BaseLayerIndex).normalizedTime < 0.9f)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, desiredRot, 0.08f / clipDuration);

                    if (updatePos)
                    {
                        transform.position = Vector3.Lerp(transform.position, targetPosition, 0.08f);

                        if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
                            updatePos = false;
                    }
                    yield return null;
                }

                SetState(bWallOnFoot ? m_BraceIdleState : m_HangIdleState);
                m_System.IsCoroutinePlaying = false;
                m_UseRootMotion = initialrootMotion;
            }
        }
        
        private void Reset()
        {
            m_ClimbableMask = (1 << 18) | (1 << 19);
            m_EnterState = "Climb.Brace From Down";
            m_TransitionDuration = 0.2f;
            m_Iterations = 25;
            m_UpdateCastByVerticalSpeed = true;
            m_FinishOnAnimationEnd = false;
            m_UseRootMotion = true;
            m_UseRotationRootMotion = true;
            m_UseVerticalRootMotion = true;

            m_CastCapsuleRadius = 0.1f;
            m_VerticalLinecastStartPoint = 1.8f;
            m_VerticalLinecastEndPoint = 1.1f;
            m_MaxDistanceToFindLedge = 1f;
            m_CharacterOffsetFromLedge = new Vector3(0, 1.5f, 0.3f);
            m_PositioningSmoothnessTime = 0.1f;
        }

    }
}
