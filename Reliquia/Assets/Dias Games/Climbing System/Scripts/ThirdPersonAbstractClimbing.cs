/*------------------------------------------------------------------------------*/

// Developed by Rodrigo Dias
// Contact: rodrigoaadias@hotmail.com

/*-----------------------------------------------------------------------------*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiasGames.ThirdPersonSystem.ClimbingSystem
{
    public abstract class ThirdPersonAbstractClimbing : ThirdPersonAbility
    {
        [Tooltip("Layers that player can cast ledge and climb")] [SerializeField] protected LayerMask m_ClimbableMask;
        [Tooltip("Set layers that should be treated as obstacles to avoid undesired climbings")] [SerializeField] protected LayerMask m_ObstacleMask = ~(1 << 15 | 1 << 13 | 1 << 1);

        // ------------------------------------------------------------------
        // Physics Parameters
        // ------------------------------------------------------------------

        [Tooltip("Should this ability calculate a launch trajectory?")]
        [SerializeField] protected bool m_UseLaunchMath = true;

        // Position as reference for player grab a ledge
        public Vector3 GrabPosition { get { return transform.position + Vector3.up * (m_VerticalLinecastStartPoint + m_VerticalLinecastEndPoint) * 0.5f + transform.forward * characterOffset.z; } }

        // -------------------------------------------------------------------
        // Public getters
        // -------------------------------------------------------------------
        public bool UseLaunchMath { get { return m_UseLaunchMath; } }

        [Tooltip("Capsule radius to be used in the Capsule Cast")] [SerializeField] protected float m_CastCapsuleRadius = 0.3f;
        [Tooltip("How many iteration engines should do after find a ledge")] [SerializeField] protected int m_Iterations = 10;

        [SerializeField] protected float m_VerticalLinecastStartPoint = 2f;
        [SerializeField] protected float m_VerticalLinecastEndPoint = 1.5f;
        [Tooltip("If true, updates linecast range by velocity in y axis")] public bool m_UpdateCastByVerticalSpeed = false;
        [Tooltip("Max distance to cast a ledge from current position")] [SerializeField] protected float m_MaxDistanceToFindLedge = 1f;


        [Tooltip("Ofsset from ledge to set character position")] [SerializeField] protected Vector3 m_CharacterOffsetFromLedge = Vector3.zero;
        [Tooltip("Time to set character position on ledge")] [SerializeField] protected float m_PositioningSmoothnessTime = 0.1f;

        [SerializeField] private bool drawGizmos = false;
        [SerializeField] private Color gizmoColor = Color.red; // The color that editor must draw on Scene

        protected RaycastHit frontHit, topHit;
        protected Transform m_CurrentLedgeTransform;
        protected Transform hitReference;
        protected Vector3 characterOffset = Vector3.zero;

        /// //////////////////////////////////////////////////////////////////////////////////
        /// Public getters for parameters of climbing
        /// //////////////////////////////////////////////////////////////////////////////////

        //public Transform LastLedgeTransform { get { return lastLedgeTransform; } }
        public Transform CurrentLedgeTransform { get { return m_CurrentLedgeTransform; } }
        public LayerMask ClimbingMask { get { return m_ClimbableMask; } set { m_ClimbableMask = value; } }

        public Vector3 CharacterOffset { get { return m_CharacterOffsetFromLedge; } }

        public float LinecastStartPoint { get { return m_VerticalLinecastStartPoint; } }
        public float LinecastEndPoint { get { return m_VerticalLinecastEndPoint; } }

        /// /////////////////////////////////////////////////////////////////////////////////

        private LayerMask m_LadderMask = (1 << 20);

        protected virtual void Awake()
        {
            LadderAbility climbLadderAbility = GetComponent<LadderAbility>();
            if (climbLadderAbility != null)
                m_LadderMask = climbLadderAbility.LadderMask;

            characterOffset = m_CharacterOffsetFromLedge;
        }

        public override void OnEnterAbility()
        {
            m_System.m_Rigidbody.isKinematic = true; // Stop movement from rigidbody
            m_System.m_Rigidbody.useGravity = false; // Stop gravity applying during a climb
            m_UpdatePosition = true; // Update character position to fit ledge

            base.OnEnterAbility();
        }


        public override void OnExitAbility()
        {
            m_System.m_Capsule.enabled = true;
            m_System.m_Rigidbody.useGravity = true;
            base.OnExitAbility();
        }

        public override void FixedUpdateAbility()
        {
            base.FixedUpdateAbility();
            UpdateCharPosition();
        }


        /// <summary>
        /// First step of casting. It casts a Capsule in the direction of character movement trying to find a ledge around
        /// </summary>
        /// <param name="capsuleHit"></param>
        /// <returns></returns>
        protected bool HasFoundLedge(out RaycastHit capsuleHit, bool m_IsAlreadyClimbing = true)
        {
            ////////////////////////////////////////
            // Capsule Cast is used to check an existing ledge in front of player
            ///////////////////////////////////////

            capsuleHit = new RaycastHit();

            //if (character.CharState == CharacterState.Blocked) { return false; }

            float endCast = m_VerticalLinecastEndPoint;
            if (m_UpdateCastByVerticalSpeed)
            {
                // Adjust linecast size according velocity of jump
                endCast = m_VerticalLinecastEndPoint + m_System.m_Rigidbody.velocity.y / 10f;
                endCast = Mathf.Clamp(endCast, 0, m_VerticalLinecastEndPoint);
            }

            Vector3 castDirection = transform.forward;

            // Overlap ledges around
            // It checks if character overlapped a ledge on his side or on his back, to allow him to climb
            // Useful in situations that character jump to back from a climb and has a ledge on side.
            if (!m_IsAlreadyClimbing || m_FinishOnAnimationEnd)
            {
                Vector3 overlapPoint1 = transform.position + Vector3.up * m_VerticalLinecastStartPoint;
                Vector3 overlapPoint2 = transform.position + Vector3.up * endCast;

                Collider[] overlappedLedges = Physics.OverlapCapsule(overlapPoint1, overlapPoint2, m_System.m_Capsule.radius * 2, m_ClimbableMask, QueryTriggerInteraction.Collide);

                if (overlappedLedges.Length > 0)
                {
                    Vector3 playerClimbReference = transform.position + m_VerticalLinecastStartPoint * Vector3.up;
                    Vector3 closestPoint = overlappedLedges[0].ClosestPoint(playerClimbReference);

                    // Chose the closest ledge to player
                    foreach (Collider coll in overlappedLedges)
                    {
                        if (coll.transform.position.y > playerClimbReference.y)
                            continue;

                        Vector3 point = coll.ClosestPoint(playerClimbReference);
                        if (Vector3.Distance(playerClimbReference, point) < Vector3.Distance(playerClimbReference, closestPoint))
                        {
                            closestPoint = point;
                        }
                    }

                    closestPoint.y = playerClimbReference.y;
                    castDirection = Vector3.Lerp(closestPoint - playerClimbReference, transform.forward, 0.5f);

                    float angle = Mathf.Abs(Vector3.SignedAngle(castDirection, transform.forward, Vector3.up));
                    if (angle > 100)
                        castDirection = transform.forward;
                }
            }

            //Set capsule points
            Vector3 capsulePoint1 = transform.position + Vector3.up * (m_VerticalLinecastStartPoint - m_CastCapsuleRadius) - castDirection * m_CastCapsuleRadius * 2;
            Vector3 capsulePoint2 = transform.position + Vector3.up * (endCast + m_CastCapsuleRadius) - castDirection * m_CastCapsuleRadius * 2;

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ///// Debug cast on Editor
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (drawGizmos)
            {
                Debug.DrawLine(capsulePoint1 + Vector3.up * m_CastCapsuleRadius, capsulePoint1 + castDirection * (m_MaxDistanceToFindLedge + m_CastCapsuleRadius) + Vector3.up * m_CastCapsuleRadius, Color.green);
                Debug.DrawLine(capsulePoint2 - Vector3.up * m_CastCapsuleRadius, capsulePoint2 + castDirection * (m_MaxDistanceToFindLedge + m_CastCapsuleRadius) - Vector3.up * m_CastCapsuleRadius, Color.black);
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ///// List of edges found during Capsule Cast. 
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////

            List<RaycastHit> top = new List<RaycastHit>();
            List<RaycastHit> front = new List<RaycastHit>();

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////

            /////////////////////////////////////////////////
            // Cast all existing ledges in player movement and add to the list
            ////////////////////////////////////////////////

            RaycastHit[] hits = Physics.CapsuleCastAll(capsulePoint1, capsulePoint2, m_CastCapsuleRadius, castDirection, m_MaxDistanceToFindLedge + m_CastCapsuleRadius, m_ClimbableMask);
            for (int i = 0; i < hits.Length; i++)
            {
                if (m_IsAlreadyClimbing)
                    AddHitsToList(hits[i], ref front, ref top);
                else
                {
                    if (hitReference == null)
                    {
                        GameObject reference = new GameObject("Climbing Ledge Reference");
                        hitReference = reference.transform;
                    }

                    hitReference.transform.forward = -hits[i].normal;
                    Vector3 cp1 = hits[i].point + hits[i].normal * 0.06f + hitReference.right * m_CastCapsuleRadius;
                    Vector3 cp2 = cp1 - hitReference.right * m_CastCapsuleRadius * 2;

                    // Check if point is free
                    if (Physics.CheckCapsule(cp1, cp2, 0.04f, m_ObstacleMask))
                        continue;

                    // Check side length
                    // Check if its corner
                    Vector3 rightOrigin = hits[i].point + hitReference.right * (m_System.m_Capsule.radius * 1.5f);
                    Vector3 lefttOrigin = hits[i].point - hitReference.right * (m_System.m_Capsule.radius * 1.5f);
                    float rightAngle = 0;
                    float leftAngle = 0;
                    bool hasRight = FoundSide(rightOrigin, hits[i], ref rightAngle);
                    bool hasLeft = FoundSide(lefttOrigin, hits[i], ref leftAngle);

                    // function
                    if (hasRight && hasLeft)
                    {
                        if (rightAngle + leftAngle >= 45)
                        {
                            AddHitsToList(hits[i], ref front, ref top);
                            continue;
                        }
                    }


                    float delta = (m_System.m_Capsule.radius * 3) / (m_Iterations * 2);
                    Vector3 left = Vector3.zero;
                    Vector3 right = Vector3.zero;
                    for (int j = 0; j < m_Iterations * 2; j++)
                    {
                        Vector3 checkpoint = hits[i].point + hitReference.transform.right * (-m_System.m_Capsule.radius * 1.5f + delta * j);

                        Vector3 cap01 = checkpoint + Vector3.up * 0.5f + hits[i].normal * characterOffset.z;
                        Vector3 cap02 = checkpoint + Vector3.down * 0.5f + hits[i].normal * characterOffset.z;

                        RaycastHit sideCheckHit;
                        if (Physics.CapsuleCast(cap01, cap02, 0.02f, -hits[i].normal, out sideCheckHit, characterOffset.z * 1.5f, m_ClimbableMask))
                        {
                            if (Vector3.Angle(hits[i].normal, sideCheckHit.normal) < 45f)
                            {
                                if (hits[i].collider == sideCheckHit.collider)
                                {
                                    left = (left == Vector3.zero) ? checkpoint : left;
                                    right = checkpoint;
                                }
                            }
                        }
                    }

                    if (Vector3.Distance(left, right) >= m_System.m_Capsule.radius * 1.5f)
                    {
                        hits[i].point = (left + right) / 2;
                        AddHitsToList(hits[i], ref front, ref top);
                    }
                }

            }


            ////////////////////////////////////////////////////////////////


            /////////////////////////////////////////////////
            // Choose the ledge with the lowest y position
            ////////////////////////////////////////////////

            for (int i = 0; i < top.Count; i++)
            {
                if (i == 0 || top[i].point.y < topHit.point.y)
                {
                    capsuleHit = front[i];
                    topHit = top[i];
                    if (hitReference != null)
                        hitReference.transform.forward = -capsuleHit.normal;
                }
            }

            if (top.Count != 0) { return true; }

            ///////////////////////////////////////////////

            return false; // Return false if find nothing
        }

        public void AddHitsToList(RaycastHit hit, ref List<RaycastHit> front, ref List<RaycastHit> top)
        {
            if (Physics.CheckSphere(hit.point, 0.05f, m_LadderMask))
                return;

            if (Mathf.Abs(hit.normal.y) < 0.3f && hit.point.y > transform.position.y)
            {
                if (CastLedgeFromTop(hit, out topHit)) // Ledge found
                {
                    // Add current ledge to the list of ledges
                    top.Add(topHit);
                    front.Add(hit);
                }
            }
        }

        private bool FoundSide(Vector3 originPoint, RaycastHit hit, ref float angle)
        {
            Vector3 cap01 = originPoint + Vector3.up * 0.5f + hit.normal * characterOffset.z;
            Vector3 cap02 = originPoint + Vector3.down * 0.5f + hit.normal * characterOffset.z;

            RaycastHit sideCheckHit;
            if (Physics.CapsuleCast(cap01, cap02, 0.05f, -hit.normal, out sideCheckHit, Mathf.Infinity, m_ClimbableMask))
            {
                if (hit.collider == sideCheckHit.collider)
                {
                    angle = Vector3.Angle(hit.normal, sideCheckHit.normal);
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// After find a ledge, cast from top to check height of ledge
        /// </summary>
        /// <param name="frontHit"> Hit from capsule cast</param>
        /// <param name="topHit"> Hit to get information from linecast</param>
        /// <returns></returns>
        protected bool CastLedgeFromTop(RaycastHit frontHit, out RaycastHit topHit)
        {
            Vector3 directionToCheckLedge = (frontHit.point - transform.position); // Get direction from player to point that found ledge
            directionToCheckLedge.y = 0;

            topHit = new RaycastHit(); // Initialize topHit
            float step = m_MaxDistanceToFindLedge / m_Iterations;

            List<RaycastHit> hits = new List<RaycastHit>();

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

                if (drawGizmos)
                    Debug.DrawLine(Start, End, Color.red);

                hits.AddRange(Physics.RaycastAll(Start, Vector3.down, Start.y - End.y, m_ClimbableMask));
            }

            foreach (RaycastHit hit in hits)
            {
                if (hit.transform == frontHit.transform && hit.normal.y > 0.8f) // Cast ledge
                {
                    if (Physics.OverlapSphere(hit.point + Vector3.up * 0.02f, 0.01f, m_ObstacleMask).Length > 0)
                        return false;

                    topHit = hit;
                    return true;
                }
            }

            return false;
        }









        /// <summary>
        /// Set character position after find ledge
        /// </summary>
        /// <param name="frontHit">Hit from capsule cast</param>
        /// <param name="topHit">Hit from line cast</param>
        protected virtual void SetCharacterPositionOnLedge(bool setPosition = true, bool setRotation = true)
        {
            if (setPosition)
                transform.position = CalculatePositionOnLedge();
            if (setRotation)
                transform.rotation = CalculateRotation();

        }




        private Vector3 CalculatePositionOnLedge()
        {
            Vector3 horizontalVelocity = m_System.m_Rigidbody.velocity;
            horizontalVelocity.y = 0;

            Vector3 newPos = frontHit.point + frontHit.normal * (characterOffset.z + horizontalVelocity.magnitude * Time.fixedDeltaTime); // Set horizontal position on ledge
            newPos.y = topHit.point.y - characterOffset.y; // Set vertical position on ledge

            return newPos;
        }

        protected Vector3 CalculatePositionOnLedge(RaycastHit desiredFrontHit)
        {
            Vector3 horizontalVelocity = m_System.m_Rigidbody.velocity;
            horizontalVelocity.y = 0;

            Vector3 newPos = desiredFrontHit.point + desiredFrontHit.normal * (characterOffset.z + horizontalVelocity.magnitude * Time.fixedDeltaTime); // Set horizontal position on ledge
            newPos.y = transform.position.y; // Set vertical position on ledge

            return newPos;
        }

        private Quaternion CalculateRotation()
        {
            Vector3 direction = frontHit.normal; // Get direction of ledge
            return GetRotationFromDirection(direction, 180f); // Rotate character to face ledge
        }



        protected bool m_UpdatePosition = false;
        protected bool m_IsPositionating = false;

        private float positionStep = 0;
        private float timeCounter = 0;

        Vector3 desiredPosition = Vector3.zero;
        Quaternion desiredRot = Quaternion.identity;

        private void UpdateCharPosition()
        {
            if (!m_UpdatePosition)
                return;

            if (!m_IsPositionating)
            {
                desiredPosition = CalculatePositionOnLedge();
                desiredRot = CalculateRotation();

                positionStep = Vector3.Distance(transform.position, desiredPosition) / m_PositioningSmoothnessTime;
                timeCounter = 0;
            }

            m_IsPositionating = true;
            m_System.IsCoroutinePlaying = true;
            desiredPosition += m_System.DeltaPos;

            transform.position = Vector3.MoveTowards(transform.position, desiredPosition, positionStep * Time.deltaTime); // Set position
            transform.rotation = Quaternion.Lerp(transform.rotation, desiredRot, 0.1f);
            timeCounter += Time.fixedDeltaTime;

            if (Mathf.Approximately(Vector3.Distance(transform.position, desiredPosition), 0) || timeCounter > m_PositioningSmoothnessTime)
            {
                m_IsPositionating = false;
                m_UpdatePosition = false;
                m_System.m_Rigidbody.isKinematic = false;
                m_System.IsCoroutinePlaying = false;
            }
        }


        /// <summary>
        /// Check obstacles above the ledge to allow or not climb up
        /// </summary>
        /// <returns></returns>
        protected bool FreeAboveLedge()
        {
            //Set capsule points
            Vector3 capsulePoint1 = topHit.point + Vector3.up * (m_CastCapsuleRadius + 0.1f) - transform.forward * m_CastCapsuleRadius * 2;
            Vector3 capsulePoint2 = capsulePoint1 + Vector3.up * (m_System.m_Capsule.height * 0.5f - m_CastCapsuleRadius * 2);

            Vector3 groundCheckStart = topHit.point + Vector3.up + transform.forward * m_System.m_Capsule.radius * 2f;
            bool hasGround = Physics.Raycast(groundCheckStart, Vector3.down, 1.5f, Physics.AllLayers, QueryTriggerInteraction.Ignore);

            return !Physics.CapsuleCast(capsulePoint1, capsulePoint2, m_CastCapsuleRadius, -frontHit.normal, m_CastCapsuleRadius * 4, m_ObstacleMask) && hasGround;
        }



        private void OnDrawGizmosSelected()
        {
            if (!drawGizmos)
                return;

            Gizmos.color = gizmoColor;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * (m_VerticalLinecastEndPoint + m_CastCapsuleRadius) + transform.forward * m_MaxDistanceToFindLedge, m_CastCapsuleRadius);
            Gizmos.DrawWireSphere(transform.position + Vector3.up * (m_VerticalLinecastStartPoint - m_CastCapsuleRadius) + transform.forward * m_MaxDistanceToFindLedge, m_CastCapsuleRadius);
        }

    }
}
