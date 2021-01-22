using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiasGames.ThirdPersonSystem.ClimbingSystem
{
    public enum WallDirection { Left, Right}

    public class WallRun : ThirdPersonAbility
    {
        [SerializeField] private LayerMask m_WallMask = 1 << 25;
        [SerializeField] private float m_MaxDistanceToRunOnWall = 1.25f;
        [SerializeField] private float m_CharOffsetOnWal = 0.65f;

        [SerializeField] private string m_WallRunRightState = "Wall Run Right";

        private RaycastHit wallHit;
        private WallDirection wallDirection = WallDirection.Left;

        public WallDirection CharWallDirection { get { return wallDirection; } }

        private ClimbJump m_ClimbJump;

        private Transform wallReference;

        protected void Awake()
        {
            m_ClimbJump = GetComponent<ClimbJump>();

            if (wallReference == null)
                wallReference = new GameObject("Wall Reference Object").transform;
        }

        /// <summary>
        /// Cast three rays against wall to detect wall to run
        /// </summary>
        /// <param name="hit">Hit result from cast</param>
        /// <param name="direction">Direction to cast</param>
        /// <returns></returns>
        private bool CastSide(out RaycastHit hit, Vector3 direction, Vector3 aidDirection)
        {
            RaycastHit front, back;

            Vector3 startCenter = transform.position;
            Vector3 startFront = transform.position + aidDirection * 0.5f;
            Vector3 startBack = transform.position - aidDirection * 0.5f;

            if (Physics.SphereCast(startCenter, 0.1f, direction, out hit, m_MaxDistanceToRunOnWall, m_WallMask))
            {
                if(Physics.SphereCast(startFront, 0.1f, direction, out front, m_MaxDistanceToRunOnWall, m_WallMask) &&
                Physics.SphereCast(startBack, 0.1f, direction, out back, m_MaxDistanceToRunOnWall, m_WallMask))
                {
                    if(Vector3.Dot(hit.normal, front.normal) > 0.8f && 
                        Vector3.Dot(hit.normal, back.normal) > 0.8f)
                        return true;
                    
                }
            }

            return false;
        }

        /// <summary>
        /// Cast three rays against wall to detect wall to run
        /// </summary>
        /// <param name="hit">Hit result from cast</param>
        /// <param name="direction">Direction to cast</param>
        /// <returns></returns>
        private bool CastSide(out RaycastHit hit, Vector3 direction)
        {
            return CastSide(out hit, direction, transform.forward);
        }

        /// <summary>
        /// Casts a capsule to find a wall on right or left
        /// </summary>
        /// <returns></returns>
        private bool FoundWall()
        {
            if (m_System.ActiveAbility == m_ClimbJump)// && (m_Engine.LastAbility is ThirdPersonAbstractClimbing || m_Engine.LastAbility is LadderAbility))
            {
                if (m_ClimbJump.JumpType == ClimbJumpType.Right ||
                m_ClimbJump.JumpType == ClimbJumpType.Left)
                {
                    RaycastHit forwardHit;
                    if (CastSide(out forwardHit, transform.forward, transform.right))
                    {
                        wallHit = forwardHit;
                        wallDirection = m_ClimbJump.JumpType == ClimbJumpType.Right ?
                            WallDirection.Left : WallDirection.Right;

                        return true;
                    }
                }
                else
                    return false;
            }

            RaycastHit leftHit;
            if (CastSide(out leftHit, -transform.right))
            {
                wallHit = leftHit;
                if (!Active)
                    wallDirection = WallDirection.Left;
                return true;
            }

            RaycastHit rightHit;
            if (CastSide(out rightHit, transform.right))
            {
                wallHit = rightHit;
                if (!Active)
                    wallDirection = WallDirection.Right;
                return true;
            }

            return false;
        }

        public override string GetEnterState()
        {
            if (wallDirection == WallDirection.Right)
                return m_WallRunRightState;

            return base.GetEnterState();
        }

        public override bool TryEnterAbility()
        {
            if (m_System.IsGrounded || m_System.m_Rigidbody.velocity.y < 1)
                return false;

            if (FoundWall())
                return true;

            return base.TryEnterAbility();
        }


        public override void FixedUpdateAbility()
        {
            base.FixedUpdateAbility();

            FoundWall();

            wallReference.forward = wallHit.normal;
            wallReference.position = wallHit.point;

            Vector3 target = wallReference.InverseTransformPoint(transform.position);
            if (target.z > m_CharOffsetOnWal)
                m_System.m_Rigidbody.AddForce(-wallHit.normal, ForceMode.Impulse);
            else
            {
                target.z = m_CharOffsetOnWal;
                target = wallReference.TransformPoint(target);
                transform.position = target;
            }

            float angleDelta = 90f;
            if (wallDirection == WallDirection.Left)
                angleDelta = -90f;

            Quaternion rot = GetRotationFromDirection(wallHit.normal);
            rot = Quaternion.Euler(0, rot.eulerAngles.y + angleDelta, 0);

            transform.rotation = Quaternion.Lerp(transform.rotation, rot, 0.1f);

        }

        public override void OnExitAbility()
        {
            base.OnExitAbility();

            m_System.m_Rigidbody.AddForce(-wallHit.normal * 10, ForceMode.Force);
        }

        private void Reset()
        {
            m_EnterState = "Wall Run Left";
            m_FinishOnAnimationEnd = true;
            m_UseVerticalRootMotion = true;
            m_RootMotionMultiplier = new Vector3(0, 1.25f, 2.25f);
        }
    }
}