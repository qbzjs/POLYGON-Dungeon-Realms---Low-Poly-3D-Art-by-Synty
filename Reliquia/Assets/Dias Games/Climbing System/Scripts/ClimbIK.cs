/*------------------------------------------------------------------------------*/

// Developed by Rodrigo Dias
// Contact: rodrigoaadias@hotmail.com

/*-----------------------------------------------------------------------------*/

using UnityEngine;

namespace DiasGames.ThirdPersonSystem.ClimbingSystem
{
    public class ClimbIK : Modifier
    {
        // -------------------------------------------- MASTER CONTROL --------------------------------------------- //

        [Tooltip("How far must cast to find ledge")] [SerializeField] private float m_DistanceToCast = 1f;

        private LayerMask m_HandLayers; // Which layer must cast
        public bool DebugCasts = true; // Should Debug the cast on Editor?

        // ------------------------------------------------------------------------------------------------------- //
        
        #region Hand
        // -------------------------------------------- HAND IK PARAMETERS ----------------------------------------------- //

        [Header("Hand IK Parameters")]
        [Tooltip("Should it run hand ik when character is climbing?")] [SerializeField] private bool m_RunHandIK = true;
        [Space]
        [SerializeField] Vector3 m_HandOffset = new Vector3(0, 1.5f, 0);
        [SerializeField] Vector3 m_HandOffsetOnHang = new Vector3(0, 1.5f, 0);
        [Tooltip("How fast should hand start IK position. Lower values result in smoother positioning")]
        [SerializeField] private float m_HandIKSmooth = 3f;

        [Tooltip("Capsule cast radius for hands")] private float m_HandCapsuleRadius = 0.01f;
        [Tooltip("Capsule cast height for hands")] private float m_HandCapsuleHeight = 0.75f;
        

        // Hands references position to start cast
        private Vector3 RightHandReference;
        private Vector3 LeftHandReference;
        
        // Internal vars to control IK behaviour
        private float handWeight = 0;
        private float rightHandWeight = 1;

        #endregion

        #region Foot

        private Vector3 RightFootPosReference;
        private Vector3 LeftFootPosReference;

        private float rFootDelta = 0, lFootDelta = 0;

        [Header("Foot IK Parameters")]
        [Tooltip("Should it run foot ik when character is climbing?")] [SerializeField] private bool m_RunFootIK = true;
        [Space]
        public LayerMask m_FeetLayers = (1 << 0) | (1 << 14) | (1 << 16) | (1 << 17) | (1 << 18) | (1 << 19) | (1 << 20);
        [SerializeField] private float m_RightFootWallOfsset = 0.48f;
        [SerializeField] private float m_LeftFootWallOfsset = 0.48f;

        #endregion

        private bool onHang = false;

        public bool OnHang { get { return onHang; } set { onHang = value; } }

        // Components
        private Animator m_Animator;
        private float rDeltaY = 0, lDeltaY = 0;

        public bool ApplyLeftHandIK { get; set; } = true;
        public bool ApplyRightHandIK { get; set; } = true;

        private RaycastHit m_TopHit;

        private void Awake()
        {
            m_Animator = GetComponent<Animator>();
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (layerIndex != 0)
                return;

            // -------------------------------- HAND IK ------------------------------ //

            if (m_System.ActiveAbility is ClimbingAbility)
            {
                handWeight = Mathf.Lerp(handWeight, 1, m_HandIKSmooth * Time.fixedDeltaTime);

                // --------------------------- FOOT IK --------------------------------- //

                if (m_RunFootIK && !onHang)
                {
                    m_Animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
                    m_Animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);

                    SetFootIKPos(AvatarIKGoal.RightFoot, rFootDelta);
                    SetFootIKPos(AvatarIKGoal.LeftFoot, lFootDelta);
                }

                // -------------------------------------------------------------------- //
            }
            else
            {
                handWeight = Mathf.Lerp(handWeight, 0, 5 * Time.fixedDeltaTime);

                if (Mathf.Approximately(handWeight, 0))
                    return;
            }


            if (m_RunHandIK)
            {
                // Set right hand ik weight
                if (ApplyRightHandIK)
                {
                    m_Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, handWeight * rightHandWeight);

                    // Set hands IK position and rotation
                    SetIK(AvatarIKGoal.RightHand, rDeltaY);
                    rightHandWeight = Mathf.Lerp(rightHandWeight, 1, m_HandIKSmooth * Time.fixedDeltaTime);
                }
                else
                    rightHandWeight = Mathf.Lerp(rightHandWeight, 0, m_HandIKSmooth * Time.fixedDeltaTime);


                if (ApplyLeftHandIK)
                {
                    // Set left hand ik weight
                    m_Animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, handWeight);

                    // Set hands IK position and rotation
                    SetIK(AvatarIKGoal.LeftHand, lDeltaY);
                }
            }
        }




        /// <summary>
        /// It must be called by climbing component in every frame to calculate hand position (Think this method like an Update)
        /// </summary>
        /// <param name="topHit"></param>
        /// <param name="climbingMask"></param>
        /// <param name="CurrentLedge"></param>
        public void RunIK(RaycastHit topHit, LayerMask climbingMask, Transform CurrentLedge)
        {
            // ------------------------------------------------ HAND ------------------------------------------------------------- //

            SetHandReference(topHit, ref RightHandReference, HumanBodyBones.RightHand); // Set right hand start position for cast
            SetHandReference(topHit, ref LeftHandReference, HumanBodyBones.LeftHand); // Set left hand start position for cast

            m_HandLayers = climbingMask;

            // Start Casting
            CastHand(RightHandReference, ref rDeltaY, CurrentLedge); // Cast for right hand
            CastHand(LeftHandReference, ref lDeltaY, CurrentLedge); // Cast for left hand


            // ---------------------------------------------------------------------------------------------------------------- //



            // ------------------------------------------------ FOOT ------------------------------------------------------------- //

            SetFootReferenceStartPos(HumanBodyBones.RightFoot, ref RightFootPosReference);
            SetFootReferenceStartPos(HumanBodyBones.LeftFoot, ref LeftFootPosReference);

            CastFeet();

            // ---------------------------------------------------------------------------------------------------------------- //

        }


        #region Hand Methods

        /// <summary>
        /// Calculate the start point of cast a ledge for hands
        /// </summary>
        /// <param name="topHit"></param>
        /// <param name="handPosRef"></param>
        /// <param name="hand"></param>
        private void SetHandReference(RaycastHit topHit, ref Vector3 handPosRef, HumanBodyBones hand)
        {
            // Sets hand position to start raycasting
            handPosRef = m_Animator.GetBoneTransform(hand).transform.position;
            handPosRef.y = topHit.point.y; // Set y pos

            handPosRef = transform.InverseTransformPoint(handPosRef);
            handPosRef.z = 0; // Set to same local z pos of character
            handPosRef = transform.TransformPoint(handPosRef);
        }








        /// <summary>
        /// Cast ledge for a specific hand
        /// </summary>
        /// <param name="handReference"></param>
        /// <param name="handIKPos"></param>
        /// <param name="handIKRot"></param>
        /// <param name="handTransform"></param>
        void CastHand(Vector3 handReference, ref float handDelta, Transform currentLedge)
        {
            // Set Capsule points to cast front ledge
            Vector3 cp1 = handReference + Vector3.down * m_HandCapsuleHeight * 0.5f;// + transform.right * 0.02f * directionDelta * i;
            Vector3 cp2 = cp1 + Vector3.up * m_HandCapsuleHeight;

            Debug.DrawLine(cp1, cp1 + transform.forward * m_DistanceToCast, Color.blue);
            Debug.DrawLine(cp2, cp2 + transform.forward * m_DistanceToCast, Color.blue);

            RaycastHit[] frontHandHits = Physics.CapsuleCastAll(cp1, cp2, m_HandCapsuleRadius, transform.forward, m_DistanceToCast, m_HandLayers);
            // Cast ledge from front
            foreach (RaycastHit handHit in frontHandHits)
            {
                if (handHit.transform == currentLedge)
                {
                    // Sets new capsule points
                    Vector3 p1 = handHit.point + Vector3.up * (m_DistanceToCast) - transform.forward * m_HandCapsuleHeight * 0.5f;
                    Vector3 p2 = p1 + transform.forward * m_HandCapsuleHeight;

                    ////////////////////////////////////////////////////
                    // Debug on Editor both castings
                    if (DebugCasts)
                    {
                        Debug.DrawLine(p1, p1 + Vector3.down * m_DistanceToCast * 1.5f, Color.blue);
                        Debug.DrawLine(p2, p2 + Vector3.down * m_DistanceToCast * 1.5f, Color.blue);
                    }
                    ////////////////////////////////////////////////

                    // Cast from top and ignore hits not equals to front hit
                    RaycastHit[] topsHits = Physics.CapsuleCastAll(p1, p2, m_HandCapsuleRadius * 0.1f, Vector3.down, m_DistanceToCast * 1.5f, m_HandLayers);
                    foreach (RaycastHit handTopHit in topsHits)
                    {
                        if (handTopHit.transform == handHit.transform)
                        {
                            // Set hand ik pos and rot to be used in OnAnimatorIK
                            Vector3 handIKPos = new Vector3(m_TopHit.point.x, handTopHit.point.y, m_TopHit.point.z);
                            handDelta = handIKPos.y - transform.position.y - (onHang ? m_HandOffsetOnHang : m_HandOffset).y;
                            return;
                        }
                    }
                }
            }
        }





        /// <summary>
        /// Set hand IK position and rotation to desired position found on casting
        /// </summary>
        /// <param name="hand"></param>
        /// <param name="handIKPos"></param>
        /// <param name="handIKRot"></param>
        /// <param name="animParam"></param>
        private void SetIK(AvatarIKGoal hand, float deltaY)
        {
            Vector3 desiredPos = m_Animator.GetIKPosition(hand); // Get hand current position
            Vector3 offset = (onHang) ? m_HandOffsetOnHang : m_HandOffset;

            desiredPos = transform.InverseTransformPoint(desiredPos); // Transform to local position
            
            desiredPos.x += (hand == AvatarIKGoal.RightHand) ? -offset.x : offset.x; // Put hand on center
            desiredPos.z += offset.z;
            desiredPos.y += deltaY; // Keep y position fixed

            desiredPos = transform.TransformPoint(desiredPos);// Transform to world position
            desiredPos += m_System.DeltaPos;

            // Set position and rotation of hand
            m_Animator.SetIKPosition(hand, desiredPos);
        }

        #endregion

        #region Foot IK methods

        private void SetFootIKPos(AvatarIKGoal foot, float footDelta)
        {
            Vector3 desiredPos = m_Animator.GetIKPosition(foot);

            desiredPos = transform.InverseTransformPoint(desiredPos);

            desiredPos.z += footDelta;

            desiredPos = transform.TransformPoint(desiredPos);

            desiredPos += m_System.DeltaPos;

            m_Animator.SetIKPosition(foot, desiredPos);
        }







        /// <summary>
        /// Cast forward to player to find wall and set feet position
        /// </summary>
        private void CastFeet()
        {
            RaycastHit footHit;
            Vector3 castDirection = transform.forward;
            castDirection.y = 0;

            if (Physics.SphereCast(RightFootPosReference, 0.1f, castDirection, out footHit, m_DistanceToCast, m_FeetLayers, QueryTriggerInteraction.Collide))
            {
                Vector3 rFootIKPos = footHit.point + footHit.normal * m_RightFootWallOfsset;
                rFootDelta = transform.InverseTransformPoint(rFootIKPos).z;
            }

            if (Physics.SphereCast(LeftFootPosReference, 0.1f, castDirection, out footHit, m_DistanceToCast, m_FeetLayers, QueryTriggerInteraction.Collide))
            {
                Vector3 lFootIKPos = footHit.point + footHit.normal * m_LeftFootWallOfsset;
                lFootDelta = transform.InverseTransformPoint(lFootIKPos).z;
            }
        }









        /// <summary>
        /// Set foot cast start position
        /// </summary>
        /// <param name="foot"></param>
        /// <param name="footReferencePos"></param>
        private void SetFootReferenceStartPos(HumanBodyBones foot, ref Vector3 footReferencePos)
        {
            footReferencePos = m_Animator.GetBoneTransform(foot).position;

            footReferencePos = transform.InverseTransformPoint(footReferencePos);
            footReferencePos.z = 0;
            footReferencePos = transform.TransformPoint(footReferencePos);
        }

        #endregion
    }
}
