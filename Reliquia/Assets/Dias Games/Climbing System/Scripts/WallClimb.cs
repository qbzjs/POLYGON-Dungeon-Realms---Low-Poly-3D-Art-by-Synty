using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiasGames.ThirdPersonSystem.ClimbingSystem
{
    public class WallClimb : ThirdPersonAbility
    {
        [SerializeField] private LayerMask m_ClimbableMask = 1 << 26;
        [SerializeField] private float m_MaxDistanceToFindWall = 1.5f;
        [SerializeField] private float m_HorizontalOfssetFromWall = 0.32f;

        [SerializeField] private float m_NormalizedTimeToStop = 0.40f;

        [SerializeField] private string m_ClimbUpState = "Climb Up.Brace Climb Up";

        private Vector3 targetPosition;
        private Quaternion targetRotation;
        private float step;

        private ClimbJump m_ClimbJump;
        private Vector3 GrabPosition { get { return transform.position + m_System.CapsuleOriginalHeight * Vector3.up; } }
        private Collider CurrentCollider;
        private Transform wallReference;

        protected void Awake()
        {
            m_ClimbJump = GetComponent<ClimbJump>();

            if (wallReference == null)
                wallReference = new GameObject("Wall Climb Reference Object").transform;
        }

        private void OnEnable()
        {
            m_ClimbJump.OnExitEvent += ResetCollider;
        }
        private void OnDisable()
        {
            m_ClimbJump.OnExitEvent -= ResetCollider;
        }

        public void ResetCollider()
        {
            CurrentCollider = null;
        }

        public override bool TryEnterAbility()
        {
            if (m_System.IsGrounded)
            {
                ResetCollider();
                return false;
            }

            RaycastHit bottom, top;
            if (FoundWall(out bottom, out top))
            {
                SetCharacterPosition(bottom);
                if(bottom.collider != CurrentCollider)
                    return true;
            }

            return base.TryEnterAbility();
        }

        public override bool TryExitAbility()
        {
            return m_System.IsGrounded || (m_InputManager.dropButton.WasPressed && !m_System.IsCoroutinePlaying);
        }

        public override void OnEnterAbility()
        {
            base.OnEnterAbility();

            m_System.IsCoroutinePlaying = true;
            step = Vector3.Distance(transform.position, targetPosition) / 0.05f;

            m_System.m_Rigidbody.useGravity = false;
            m_AnimatorManager.SetAnimationMultiplierParameter(0,0);
        }

        public override void OnExitAbility()
        {
            base.OnExitAbility();

            m_System.m_Rigidbody.useGravity = true;
            m_System.m_Capsule.enabled = true;
        }

        public override void FixedUpdateAbility()
        {
            base.FixedUpdateAbility();

            // Set Position
            if (m_System.IsCoroutinePlaying)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, step * Time.deltaTime);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.2f);

                if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
                    m_System.IsCoroutinePlaying = false;
                return;
            }
            
            float verticalInput = 0;

            if (m_InputManager.Move.y > 0.2f)
                verticalInput = 1;
            if (m_InputManager.Move.y < -0.2f)
                verticalInput = -1;

            if (Mathf.Approximately(verticalInput, 0))
            {
                if (Mathf.Abs(m_AnimatorManager.GetNormalizedTime(0,true) - m_NormalizedTimeToStop) > 0.1f &&
                    Mathf.Abs(m_AnimatorManager.GetNormalizedTime(0,true)) > 0.05f)
                    verticalInput = m_AnimatorManager.GetAnimationMultiplierParameter();
            }

            RaycastHit bottom, top;
            if (FoundWall(out bottom, out top))
            {
                SetCharacterPosition(bottom, true);         // Set character position on wall
                CurrentCollider = bottom.collider;
            }
            else
            {
                if (top.collider == null && verticalInput > 0)
                {
                    if (FreeAbove())
                    {
                        // Climb up
                        SetState(m_ClimbUpState);
                        m_System.m_Capsule.enabled = false;
                        return;
                    }

                    verticalInput = 0;

                    // Hop Up
                    if (m_InputManager.jumpButton.WasPressed)
                        m_ClimbJump.StartHopUp(GrabPosition, GrabPosition, true);
                }

                // Stop Movement
                if (bottom.collider == null && verticalInput < 0)
                    verticalInput = 0;
            }

            m_AnimatorManager.SetAnimationMultiplierParameter(verticalInput, 0);


            if (verticalInput == 0)
            {
                if (m_InputManager.jumpButton.WasPressed)
                {
                    if (m_InputManager.Move.x > 0.2f)
                        m_ClimbJump.StartClimbJump(ClimbJumpType.Right, transform.right, GrabPosition, m_System.CapsuleOriginalHeight, true);
                    if (m_InputManager.Move.x < -0.2f)
                        m_ClimbJump.StartClimbJump(ClimbJumpType.Left, -transform.right, GrabPosition, m_System.CapsuleOriginalHeight, true);
                }
            }
        }

        private void SetCharacterPosition(RaycastHit bottomHit, bool setToTarget = false)
        {
            targetPosition = bottomHit.point + bottomHit.normal * m_HorizontalOfssetFromWall;
            targetPosition.y = transform.position.y;
            targetRotation = GetRotationFromDirection(-bottomHit.normal);

            if(setToTarget)
            {
                transform.position = targetPosition;
                transform.rotation = targetRotation;
            }
        }

        private bool FreeAbove()
        {
            Vector3 Start = transform.position + Vector3.up * (m_System.m_Capsule.height + 0.5f);
            RaycastHit hit;
            if (!Physics.SphereCast(Start, 0.25f, transform.forward, out hit, m_MaxDistanceToFindWall, Physics.AllLayers, QueryTriggerInteraction.Ignore))
                return true;

            return false;
        }

        private bool FoundWall(out RaycastHit hitFromBottom, out RaycastHit hitTop)
        {
            Vector3 StartPointTop = transform.position + Vector3.up * (m_System.m_Capsule.height) - transform.forward * m_System.m_Capsule.radius;
            Vector3 StartPointBottom = transform.position - transform.forward * m_System.m_Capsule.radius;

            Vector3 direction = transform.forward;

            // Overlap ledges around
            // It checks if character overlapped a ledge on his side or on his back, to allow him to climb
            // Useful in situations that character jump to back from a climb and has a ledge on side.
            if (!Active || CurrentStatePlaying != m_EnterState)
            {
                Vector3 overlapPoint1 = transform.position + Vector3.up * (m_System.m_Capsule.height - m_System.m_Capsule.radius);

                Collider[] overlappedLedges = Physics.OverlapSphere(overlapPoint1, m_System.m_Capsule.radius * 2, m_ClimbableMask, QueryTriggerInteraction.Collide);

                if (overlappedLedges.Length > 0)
                {
                    Vector3 playerClimbReference = transform.position + Vector3.up * (m_System.m_Capsule.height - m_System.m_Capsule.radius);
                    Vector3 closestPoint = overlappedLedges[0].ClosestPoint(playerClimbReference);

                    // Chose the closest ledge to player
                    foreach (Collider coll in overlappedLedges)
                    {
                        if (coll.transform.position.y > playerClimbReference.y)
                            continue;

                        Vector3 point = coll.ClosestPoint(playerClimbReference);
                        if (Vector3.Distance(playerClimbReference, point) < Vector3.Distance(playerClimbReference, closestPoint))
                            closestPoint = point;
                    }

                    closestPoint.y = playerClimbReference.y;
                    direction = closestPoint - playerClimbReference;

                    float angle = Mathf.Abs(Vector3.SignedAngle(direction, transform.forward, Vector3.up));
                    if (angle > 100)
                        direction = transform.forward;
                }
            }

            // Cast for top and bottom positions
            bool bFoundTop = Physics.Raycast(StartPointTop, direction, out hitTop, m_MaxDistanceToFindWall, m_ClimbableMask);
            bool bFoundBottom = Physics.Raycast(StartPointBottom, direction, out hitFromBottom, m_MaxDistanceToFindWall, m_ClimbableMask);
            
            Debug.DrawRay(StartPointBottom, direction * m_MaxDistanceToFindWall, Color.blue);
            Debug.DrawRay(StartPointTop, direction * m_MaxDistanceToFindWall, Color.blue);

            if (bFoundBottom && bFoundBottom)
            {
                wallReference.position = hitFromBottom.point;
                wallReference.forward = hitFromBottom.normal;

                RaycastHit right, left;
                if (Physics.Raycast(StartPointBottom + wallReference.right * 0.5f, direction, out right, Mathf.Infinity, m_ClimbableMask) &&
                   Physics.Raycast(StartPointBottom - wallReference.right * 0.5f, direction, out left, Mathf.Infinity, m_ClimbableMask))
                {
                    if (Vector3.Dot(hitFromBottom.normal, right.normal) < 0.75f)
                        return false;

                    if (Vector3.Dot(hitFromBottom.normal, left.normal) < 0.75f)
                        return false;
                }
                else
                    return false;
            }

            return (bFoundTop && bFoundBottom); // Return true only if both hits found ladder
        }
        
        private void Reset()
        {
            m_EnterState = "Climb.Wall";
            m_TransitionDuration = 0.2f;
            m_FinishOnAnimationEnd = true;
            m_UseRootMotion = true;
            m_UseRotationRootMotion = false;
            m_UseVerticalRootMotion = true;
        }
    }
}