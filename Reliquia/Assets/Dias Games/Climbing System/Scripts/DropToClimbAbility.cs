/*------------------------------------------------------------------------------*/

// Developed by Rodrigo Dias
// Contact: rodrigoaadias@hotmail.com

/*-----------------------------------------------------------------------------*/

using System.Collections;
using UnityEngine;

namespace DiasGames.ThirdPersonSystem.ClimbingSystem
{
    public class DropToClimbAbility : ThirdPersonAbility
    {
        [SerializeField] private LayerMask m_DroppableLayer; // Layers that character can drop bellow
        [SerializeField] private LayerMask m_ObstacleMask = ~(1 << 15 | 1 << 13 | 1 << 1); // Layers that are considered as obstacles
        [SerializeField] private float m_DistanceToCast = 0.3f; // Distance in forward direction to start cast a ledge bellow
        [SerializeField] private float m_MaxHeightBellow = 0.1f; // Height bellow character position to cast

        [SerializeField] private float m_OffseFromLedge = 0.2f; // Distance to put character from ledge after find a ledge bellow

        [Tooltip("If character move speed is greater than this value, character will not drop")] [SerializeField] private float m_MaxAllowedDropSpeed = 5f;

        private Vector3 m_PositionFromLedge; // Desired position after find ledge
        private Quaternion m_RotationOnLedge;
        private Transform m_TargetGround = null;

        public LayerMask DroppableLayer { get { return m_DroppableLayer; } set { m_DroppableLayer = value; } }

        /// <summary>
        /// Cast a ledge bellow character feet
        /// </summary>
        /// <param name="hitBellow"></param>
        /// <returns></returns>
        bool CastBellow(out RaycastHit hitBellow)
        {
            Vector3 start = transform.position + transform.forward * m_DistanceToCast + Vector3.down * m_MaxHeightBellow; // Start cast position

            if (Physics.Raycast(start, -transform.forward, out hitBellow, m_DistanceToCast, m_DroppableLayer)) // Try cast layer
            {
                Vector3 cp1 = hitBellow.point + Vector3.down * 0.1f + hitBellow.normal * 0.15f;
                Vector3 cp2 = cp1 + Vector3.down * (m_System.CapsuleOriginalHeight - 0.15f);

                if (Physics.OverlapCapsule(cp1,cp2, 0.1f, m_ObstacleMask, QueryTriggerInteraction.Ignore).Length > 0)
                    return false;

                if (hitBellow.normal.y < 0.1f) // Only drop if the ledge bellow is not a slope
                    return true;
            }

            return false;
        }

        public override bool TryEnterAbility()
        {
            if (m_System.m_Rigidbody.velocity.magnitude > m_MaxAllowedDropSpeed)
                return false;

            RaycastHit hit; // Hit information
            if (CastBellow(out hit)) // Check for a ledge bellow
            {
                // Calculate position to start drop
                m_PositionFromLedge = hit.point - hit.normal * m_OffseFromLedge;
                m_PositionFromLedge.y = transform.position.y;

                // Calculate rotation to drop
                m_RotationOnLedge = Quaternion.FromToRotation(transform.forward, hit.normal);
                m_RotationOnLedge.x = 0;
                m_RotationOnLedge.z = 0;
                transform.rotation *= m_RotationOnLedge;

                return true;
            }

            return base.TryEnterAbility();
        }

        public override bool ForceEnterAbility()
        {
            if(m_UseInputStateToEnter == InputEnterType.Noone)
                return TryEnterAbility();

            return base.ForceEnterAbility();
        }

        public override void OnEnterAbility()
        {
            base.OnEnterAbility();

            m_System.m_Rigidbody.velocity = Vector3.zero;
            transform.position = m_PositionFromLedge;

            m_TargetGround = m_System.GroundHitInfo.transform;

            m_System.m_Capsule.enabled = false;
            m_System.m_Rigidbody.useGravity = false;
        }

        public override void OnExitAbility()
        {
            base.OnExitAbility();
            m_System.m_Capsule.enabled = true;
            m_System.m_Rigidbody.useGravity = true;
            m_System.m_Rigidbody.velocity = new Vector3(0, m_System.m_Rigidbody.velocity.y, 0); 
            m_TargetGround = null;
        }


        public override void FixedUpdateAbility()
        {
            base.FixedUpdateAbility();

            m_System.UpdatePositionOnMovableObject(m_TargetGround);
            m_PositionFromLedge += m_System.DeltaPos;
            if(AbilityEnterFixedTime + 0.1f > Time.fixedTime)
                transform.position = m_PositionFromLedge;
        }

        private void Reset()
        {
            m_EnterState = "Climb.Drop to Ledge Bellow";
            m_TransitionDuration = 0.2f;
            m_FinishOnAnimationEnd = true;
            m_UseRootMotion = true;
            m_UseRotationRootMotion = true;
            m_UseVerticalRootMotion = true;
        }

    }
}
