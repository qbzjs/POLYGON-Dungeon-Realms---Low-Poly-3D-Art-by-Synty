using System;
using System.Collections.Generic;
using UnityEngine;

namespace DiasGames.ThirdPersonSystem.ClimbingSystem
{
    public enum ClimbJumpType
    {
        Up, Right, Left, Back, FromGroundToHang, FromGroundToLower, Drop, Noone
    }

    [System.Serializable]
    public struct JumpParameters
    {
        public string m_AnimationState;
        public float m_MinJumpHeight;
        public float m_MaxJumpHeight;
        public float HorizontalSpeed;
        [ReadOnly] public float clipDuration;

        public float VerticalSpeed { get { return Mathf.Sqrt(-2 * Physics.gravity.y * m_MaxJumpHeight); } }

        public JumpParameters(string state, float minJump, float maxJump, float speed)
        {
            m_AnimationState = state;
            m_MinJumpHeight = minJump;
            m_MaxJumpHeight = maxJump;
            HorizontalSpeed = speed;
            clipDuration = -1;
        }
    }

    public class ClimbJump : ThirdPersonAbility
    {
        #region Animations States

        [SerializeField] private string m_JumpToHang = "Air.Jump To Hang";
        [SerializeField] [ReadOnly] private float jumpHangClipDuration = 0.1f;
        [SerializeField] private string m_JumpToLowerClimb = "Air.Jump To Lower";
        [SerializeField] [ReadOnly] private float jumpLowerClipDuration = 0.1f;

        private Dictionary<string, float> statesDuration = new Dictionary<string, float>();

        #endregion

        // ------------------------------------------------------------------
        // Physics Parameters
        // ------------------------------------------------------------------

        [Tooltip("Should system calculate a jump trajectory to the target ledge?")]
        [SerializeField] private bool m_UseLaunchMath = true;

        [Tooltip("Layers to cast and find a ledge")]
        [SerializeField] private LayerMask m_ClimbablesLayers = (1 << 17) | (1 << 18) | (1 << 19) | (1 << 20);

        [Tooltip("The maximum degree between character forward direction and direction to get ledge that character can predict a jump. 0 = character looking direct to ledge, 90 = ledge on the right or left side of character, 180 = ledge behing character")]
        [SerializeField] private float m_MaxAngle = 60f;

        // -------------------------------------------------------------------


        // -------------------------------------------------------------------
        // Debug parameters. (Draw the jump curve to grab ledge)
        // -------------------------------------------------------------------

        [Header("Debug")]
        [Tooltip("Should Editor draw jump trajectory curve?")]
        [SerializeField] private bool debugTrajectory = true;
        [SerializeField] private int m_Resolution = 30;

        // -------------------------------------------------------------------

        // -------------------------------------------------------------------
        // Private parameters
        // -------------------------------------------------------------------

        protected float m_Gravity { get { return Physics.gravity.y; } }
        private bool m_ForceEnterAbility = false;
        private float m_JumpTimeToTarget = 1f;

        Quaternion m_CharacterDesiredRotation;
        Vector3 m_CharacterDesiredVelocity;

        private ClimbJumpType m_JumpType = ClimbJumpType.Back;
        private float m_DotResult = 0;

        // -------------------------------------------------------------------

        // -------------------------------------------------------------------
        // Move parameters
        // -------------------------------------------------------------------

        protected float VerticalSpeed { get { return Mathf.Sqrt(-2 * m_Gravity * NormalJumpParameters.m_MaxJumpHeight); } }
        protected float MaxHorizontalDistance
        {
            get
            {
                Vector2 vel = new Vector2(NormalJumpParameters.HorizontalSpeed, VerticalSpeed);
                float angle = Mathf.Asin(VerticalSpeed / vel.magnitude);

                return Mathf.Abs(vel.magnitude * vel.magnitude * Mathf.Sin(2 * angle) / m_Gravity);
            }
        }
        protected Vector3 MoveDirection
        {
            get
            {
                Vector3 moveDirection = m_System.InputManager.RelativeInput;
                if (Mathf.Approximately(moveDirection.magnitude, 0))
                    moveDirection = transform.forward;

                return moveDirection;
            }
        }

        // -------------------------------------------------------------------
        // Public getters
        // -------------------------------------------------------------------

        public bool UsePredictionMath { get { return m_UseLaunchMath; } }
        public ClimbJumpType JumpType { get { return m_JumpType; } }
        public float JumpTimeToTarget { get { return m_JumpTimeToTarget; } }

        // -------------------------------------------------------------------

        // -------------------------------------------------------------------
        // Jump and Hop Parameters
        // -------------------------------------------------------------------

        [SerializeField] private JumpParameters NormalJumpParameters = new JumpParameters(string.Empty, 0.6f, 1.4f, 7f);
        [SerializeField] private JumpParameters HopRightParameters = new JumpParameters("Climb Right.Hop Air", 0.1f, 0.5f, 6f);
        [SerializeField] private JumpParameters HopLeftParameters = new JumpParameters("Climb Left.Hop Air", 0.1f, 0.5f, 6f);
        [SerializeField] private JumpParameters HopUpParameters = new JumpParameters("Climb Up.Hop Up Air", 0.3f, 1.5f, 0f);
        [SerializeField] private JumpParameters JumpBackParameters = new JumpParameters("Climb Down.Jump Back End", 0.5f, 1f, 6f);

        // -------------------------------------------------------------------

        public event Action OnExitEvent;
        
        // Launch Data for no launch calculation
        LaunchData GetLaunchFromParameters(JumpParameters jumpParameters, Vector3 horizontalDir)
        {
            Vector3 targetVelocity = jumpParameters.HorizontalSpeed * horizontalDir + jumpParameters.VerticalSpeed * Vector3.up;
            return new LaunchData(targetVelocity, Vector3.zero, 0, false);
        }

        protected void Awake()
        {
            statesDuration = new Dictionary<string, float>();
        }

        private void Start()
        {
            statesDuration[m_JumpToHang] = jumpHangClipDuration;
            statesDuration[m_JumpToLowerClimb] = jumpLowerClipDuration;
            statesDuration[JumpBackParameters.m_AnimationState] = JumpBackParameters.clipDuration;
            statesDuration[HopRightParameters.m_AnimationState] = HopRightParameters.clipDuration;
            statesDuration[HopLeftParameters.m_AnimationState] = HopLeftParameters.clipDuration;
            statesDuration[HopUpParameters.m_AnimationState] = HopUpParameters.clipDuration;
        }

        public override string GetEnterState()
        {
            // Select animation state based on JumpDirection
            switch (m_JumpType)
            {
                case ClimbJumpType.Up:
                    return HopUpParameters.m_AnimationState;
                case ClimbJumpType.Right:
                    return m_DotResult > 0.9f ? HopRightParameters.m_AnimationState : m_JumpToHang;
                case ClimbJumpType.Left:
                    return m_DotResult > 0.9f ? HopLeftParameters.m_AnimationState : m_JumpToHang;
                case ClimbJumpType.Back:
                    return JumpBackParameters.m_AnimationState;
                case ClimbJumpType.FromGroundToHang:
                    return m_JumpToHang;
                case ClimbJumpType.FromGroundToLower:
                    return m_JumpToLowerClimb;
                default:
                    return m_JumpToHang;
            }
        }

        public override bool TryEnterAbility()
        {
            if ((m_System.IsGrounded || m_System.GroundCheckDistance < 0.05f)
                && UsePredictionMath)
            {
                if (!Mathf.Approximately(m_System.InputManager.RelativeInput.magnitude, 0))
                {
                    Vector3 direction = m_System.InputManager.RelativeInput.normalized;

                    // Get all climbing abilities attached to character
                    List<ThirdPersonAbstractClimbing> climbingAttached = new List<ThirdPersonAbstractClimbing>();
                    climbingAttached.AddRange(GetComponents<ThirdPersonAbstractClimbing>());

                    // Loop trough all climbing abilities
                    foreach (ThirdPersonAbstractClimbing climbing in climbingAttached)
                    {
                        // Ignore abilities that no use math
                        if (!climbing.UseLaunchMath)
                            continue;

                        // Get a launch data
                        LaunchData data = GetLaunchData(climbing.GrabPosition, climbing.LinecastStartPoint, NormalJumpParameters,
                            direction, m_MaxAngle, Vector3.zero, climbing.ClimbingMask);

                        if (IsLaunchDataOnlyWay(data, direction)) // If launch data is the unique way to reach point
                        {
                            SetLaunchParameters(data, (climbing is LowerClimbAbility) ? ClimbJumpType.FromGroundToLower :
                                ClimbJumpType.FromGroundToHang);

                            return true;
                        }
                    }

                    // Check for ladders
                    LadderAbility ladder = GetComponent<LadderAbility>();
                    if (ladder != null)
                    {
                        // Check if ladder wants to use math for launch
                        if (ladder.CalculateLaunchParameters)
                        {
                            LaunchData data = GetLaunchData(ladder.GrabPosition, m_System.m_Capsule.height, NormalJumpParameters, direction, m_MaxAngle, Vector3.up, ladder.LadderMask);
                            if (IsLaunchDataOnlyWay(data, direction))
                            {
                                SetLaunchParameters(data, ClimbJumpType.FromGroundToHang);
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public override bool ForceEnterAbility()
        {
            return m_ForceEnterAbility;
        }

        public override void OnEnterAbility()
        {
            base.OnEnterAbility();

            // Change parameters to allow jumping
            m_System.GroundCheckDistance = 0.01f;
            m_System.IsGrounded = false;

            m_System.m_Rigidbody.velocity = m_CharacterDesiredVelocity;                 // Set character velocity
            m_ForceEnterAbility = false;                                                // Reset force enter parameter

            // Calculate animation speed to match jump time
            float speed = 1;
            if (m_JumpTimeToTarget > 0)
                speed = statesDuration[m_CurrentStatePlaying] / m_JumpTimeToTarget;

            m_AnimatorManager.SetAnimationMultiplierParameter(speed, 0);                 // Set animation speed
            m_FinishOnAnimationEnd = true;
        }

        public override void FixedUpdateAbility()
        {
            base.FixedUpdateAbility();

            // Update rotation
            transform.rotation = Quaternion.Lerp(transform.rotation, m_CharacterDesiredRotation, 0.2f);
        }

        public override bool TryExitAbility()
        {
            if (m_System.IsGrounded)
                return true;

            return base.TryExitAbility();
        }

        public override void OnExitAbility()
        {
            base.OnExitAbility();
            OnExitEvent?.Invoke();
        }

        /// <summary>
        /// Star a climb jump
        /// </summary>
        /// <param name="jumpType">Desired jump type</param>
        /// <param name="directionVector">Direction to jump</param>
        /// <param name="GrabPosition">Grab position on character</param>
        /// <param name="LinecastStartHeight">Point that starts cast</param>
        /// <returns>Time to reach the target point</returns>
        public float StartClimbJump(ClimbJumpType jumpType, Vector3 directionVector, Vector3 GrabPosition, float LinecastStartHeight, bool abilityUseMath, Collider ignoreCollider = null)
        {
            m_JumpType = jumpType;
            if (m_UseLaunchMath && abilityUseMath)
                LaunchCharacter(directionVector, GrabPosition, LinecastStartHeight, ignoreCollider);
            else
            {
                JumpParameters parameter = new JumpParameters();

                // Choose jump parameters based on Jump Type
                switch (m_JumpType)
                {
                    case ClimbJumpType.Right:
                        parameter = HopRightParameters;
                        break;
                    case ClimbJumpType.Left:
                        parameter = HopLeftParameters;
                        break;
                    case ClimbJumpType.Back:
                        parameter = JumpBackParameters;
                        break;
                }

                SetLaunchParameters(GetLaunchFromParameters(parameter, directionVector), m_JumpType);
                if (m_JumpType != ClimbJumpType.Back)
                    m_CharacterDesiredRotation = transform.rotation;
            }

            m_ForceEnterAbility = true;
            m_DotResult = Quaternion.Dot(transform.rotation, m_CharacterDesiredRotation);

            return m_JumpTimeToTarget;
        }

        /// <summary>
        /// Called by climbing to hop up
        /// </summary>
        /// <param name="GrabPosition"></param>
        /// <param name="topHitPoint"></param>
        public float StartHopUp(Vector3 GrabPosition, Vector3 topHitPoint, bool abilityUseMath)
        {
            if (m_UseLaunchMath && abilityUseMath)
            {
                LaunchData hopLaunch = GetLaunchHopUp(topHitPoint, GrabPosition);
                SetLaunchParameters(hopLaunch, ClimbJumpType.Up);
            }
            else
                SetLaunchParameters(GetLaunchFromParameters(HopUpParameters, Vector3.zero), ClimbJumpType.Up);

            m_ForceEnterAbility = true;

            return m_JumpTimeToTarget;
        }


        /// <summary>
        /// Set parameters to jump
        /// </summary>
        /// <param name="data">launch data</param>
        /// <param name="targetJumpType">Jump Type that character will jump</param>
        private void SetLaunchParameters(LaunchData data, ClimbJumpType targetJumpType)
        {
            m_CharacterDesiredVelocity = data.initialVelocity;                                              // Use data velocity
            m_CharacterDesiredRotation = (targetJumpType == ClimbJumpType.Up) ?
                transform.rotation : GetRotationFromDirection(m_CharacterDesiredVelocity.normalized);      // Get rotation to face to point

            m_JumpTimeToTarget = data.timeToTarget;                                                        // Get total time to reach the target
            m_JumpType = targetJumpType;                                                                    // Set desired jump direction
        }

        /// <summary>
        /// Check if launch data is the only way to reach the point
        /// </summary>
        /// <param name="data">Launch data</param>
        /// <returns>true: launch data is the only way to reach target</returns>
        private bool IsLaunchDataOnlyWay(LaunchData data, Vector3 direction)
        {
            if (data.foundSolution) // If found a solution
            {
                // Check if character can reach the point with a normal jump
                LaunchData normalJump = CalculateLaunchData(transform.position, data.target, NormalJumpParameters);

                if (!normalJump.foundSolution)
                    return true;
                else
                {
                    Vector3 cp1 = data.target + Vector3.up * m_System.m_Capsule.radius * 2.1f;
                    Vector3 cp2 = data.target + Vector3.up * (m_System.m_Capsule.height - m_System.m_Capsule.radius*2) + direction * m_System.m_Capsule.radius;
                    
                    if (Physics.OverlapCapsule(cp1, cp2, m_System.m_Capsule.radius*2, Physics.AllLayers, QueryTriggerInteraction.Ignore).Length > 0)
                        return true;
                }
            }
            return false;
        }

        #region Math

        /// <summary>
        /// Search ledges around, calculate possible trajectories and choose the best one
        /// </summary>
        /// <returns>Best possible launch data</returns>
        public LaunchData GetLaunchData(Vector3 launchOriginPoint, float LinecastStartPoint, JumpParameters parameter, Vector3 moveDirection, float maxAngle, Vector3 offset, LayerMask climbableMask, Collider ignoreCollider = null)
        {
            List<Collider> m_LedgesFound;
            // Check ledges around
            if (FoundLedgeToGrab(out m_LedgesFound, moveDirection, LinecastStartPoint, climbableMask, ignoreCollider))
            {
                // Start a list of launches
                List<LaunchData> launches = new List<LaunchData>();

                // Get all possible target points
                List<Vector3> points = GetTargetPoints(m_LedgesFound, launchOriginPoint, moveDirection, offset);

                // Loop trough all points
                foreach (Vector3 point in points)
                {
                    // Angle between desired direction and target point
                    float angle = Vector3.Angle(moveDirection, Vector3.Scale(point - launchOriginPoint, new Vector3(1, 0, 1)).normalized);

                    // Check angle between character and target point
                    if (angle < maxAngle)
                    {
                        LaunchData data = CalculateLaunchData(launchOriginPoint, point, parameter);
                        if (data.foundSolution) // Is this launch possible?
                            launches.Add(data); // Add this launch to the list
                    }
                }

                // Found at least one launch
                if (launches.Count > 0)
                {
                    LaunchData bestLaunch = ChooseBestLaunchData(launchOriginPoint, launches, moveDirection); // Choose best launch in all possible launches

                    if (debugTrajectory)
                        DrawPath(bestLaunch, launchOriginPoint);

                    return bestLaunch;
                }

            }

            return new LaunchData(); // Return a new empty launch data that means the character did not find any launch
        }

        /// <summary>
        /// Overlap around character to find ledges
        /// </summary>
        /// <returns>true: found ledge and it's reachable</returns>
        private bool FoundLedgeToGrab(out List<Collider> ledges, Vector3 direction, float LinecastStartPoint, LayerMask climbableMask, Collider ignoreCollider)
        {
            float radius = MaxHorizontalDistance * 3;

            // Pontos da capsula
            Vector3 capsulePointUp = transform.position + Vector3.up * LinecastStartPoint + direction * radius;
            Vector3 capsulePointDown = capsulePointUp + Vector3.down * 3f;

            // Find all ledges and store all in a list
            ledges = new List<Collider>();
            ledges.AddRange(Physics.OverlapCapsule(capsulePointUp, capsulePointDown, radius, climbableMask, QueryTriggerInteraction.Collide));

            // Remove the ledge that character is grounded
            if (m_System.GroundHitInfo.collider != null && m_System.IsGrounded)
            {
                if (ledges.Contains(m_System.GroundHitInfo.collider))
                    ledges.Remove(m_System.GroundHitInfo.collider);
            }

            if (ledges.Contains(ignoreCollider))
                ledges.Remove(ignoreCollider);

            for(int i=0; i < ledges.Count; i++)
            {
                if(ledges[i] is MeshCollider)
                {
                    if(!(ledges[i] as MeshCollider).convex)
                    {
                        ledges.RemoveAt(i);
                        i--;
                    }
                }
            }

            return ledges.Count > 0;
        }


        /// <summary>
        /// Get target points around character
        /// </summary>
        /// <returns>A List of points</returns>
        private List<Vector3> GetTargetPoints(List<Collider> ledges, Vector3 launcOriginPoint, Vector3 targetDirection, Vector3 offset)
        {
            // Start a new list of points
            List<Vector3> points = new List<Vector3>();

            foreach (Collider coll in ledges)
            {
                Vector3 origin = launcOriginPoint;
                float verticalPoint = coll.bounds.center.y + coll.bounds.extents.y;

                // Check if this coll is a ladder
                if (coll.gameObject.layer == 20)
                {
                    // Get closest point on ladder
                    Vector3 ladderClosestPoint = coll.ClosestPoint(origin);

                    // ---------------------------------------------------------------------------
                    // Set point to the center of the ladder (On X axis)
                    // ---------------------------------------------------------------------------

                    ladderClosestPoint = coll.transform.InverseTransformPoint(ladderClosestPoint);
                    ladderClosestPoint.x = 0;
                    ladderClosestPoint = coll.transform.TransformPoint(ladderClosestPoint);

                    // ---------------------------------------------------------------------------

                    float minVertical = coll.bounds.center.y - coll.bounds.extents.y;
                    // Clamp vertical position
                    ladderClosestPoint.y = transform.position.y + m_System.m_Capsule.height * 0.5f;
                    ladderClosestPoint.y = Mathf.Clamp(ladderClosestPoint.y, minVertical + m_System.m_Capsule.height, verticalPoint);

                    if (IsPointFreeToGrab(ladderClosestPoint + Vector3.Scale(offset, Vector3.up), launcOriginPoint, coll))
                        points.Add(ladderClosestPoint + Vector3.Scale(offset, Vector3.up)); // Add this point to the list

                    continue;                       // Go to the next collider
                }

                origin.y = verticalPoint;                               // Vertical point is always on top of the ledge,
                                                                        // because it is where character grab a ledge
                Vector3 closestPoint = coll.ClosestPoint(origin);       // Get closest point

                Vector3 position = launcOriginPoint;
                position.y = closestPoint.y;

                if (IsPointFreeToGrab(closestPoint, launcOriginPoint, coll))
                {
                    // Check distance to fix side jumps detection
                    if (Vector3.Distance(closestPoint, position) < 0.45f && (m_JumpType == ClimbJumpType.Right || m_JumpType == ClimbJumpType.Left))
                    {
                        closestPoint += targetDirection * 0.5f;
                        if (coll.bounds.Contains(closestPoint))
                            points.Add(closestPoint + offset);
                    }
                    else
                        points.Add(closestPoint + offset);                      // Add the closest point to the list
                }

                // Find a point in the move direction
                RaycastHit hit;
                if (Physics.SphereCast(origin, 0.5f, targetDirection, out hit, m_ClimbablesLayers))
                {
                    if (hit.collider == coll && IsPointFreeToGrab(hit.point, launcOriginPoint, coll))
                        points.Add(hit.point + offset);
                }

                // Try finding a point between forward direction and closest point direction
                Vector3 betweenDirection = Vector3.Lerp(targetDirection, (closestPoint - origin).normalized, 0.5f);
                RaycastHit averageHit;
                if (Physics.SphereCast(origin, 0.5f, betweenDirection, out averageHit, m_ClimbablesLayers))
                {
                    if (averageHit.collider == coll && IsPointFreeToGrab(averageHit.point, launcOriginPoint, coll))
                        points.Add(averageHit.point + offset);
                }

            }

            return points;
        }

        /// <summary>
        /// Check if a target point can be used as possible point
        /// </summary>
        /// <param name="point"></param>
        /// <param name="launchOrigin"></param>
        /// <param name="coll"></param>
        /// <returns></returns>
        private bool IsPointFreeToGrab(Vector3 point, Vector3 launchOrigin, Collider coll)
        {
            Vector3 direction = point - launchOrigin;
            RaycastHit hit;
            if (Physics.Raycast(launchOrigin, direction.normalized, out hit, direction.magnitude - 0.05f, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider != coll)
                    return false;
            }

            Vector3 horDirection = Vector3.Scale(direction, new Vector3(1, 0, 1));
            Vector3 cp1 = point - horDirection;
            Vector3 cp2 = cp1 + Vector3.down;

            if (Physics.CheckCapsule(cp1, cp2, 0.3f, LayerMaskManager.IgnoreOnlyPlayer, QueryTriggerInteraction.Ignore))
            {
                cp1 = point - transform.forward;
                cp2 = cp1 + Vector3.down;

                if (m_JumpType == ClimbJumpType.Back)
                    return true;

                if (!Physics.CheckCapsule(cp1, cp2, 0.3f, LayerMaskManager.IgnoreOnlyPlayer, QueryTriggerInteraction.Ignore))
                    return true;

                return false;
            }

            return true;
        }

        /// <summary>
        /// Calculate velocity to reach desired point
        /// </summary>
        /// <returns>Data for the launch</returns>
        public LaunchData CalculateLaunchData(Vector3 startPoint, Vector3 targetPoint, JumpParameters parameter)
        {
            LaunchData nullData = new LaunchData(Vector3.zero, targetPoint, -1, false);

            // Full displacement
            Vector3 Displacement = targetPoint - startPoint;

            // Organize by vertical and horizontal displacements
            float displacementY = Displacement.y;
            Vector3 displacementXZ = new Vector3(Displacement.x, 0, Displacement.z);

            // Check if target point is too high
            // When target point is higher than character maximum jump height, it means that point is not reachable
            if (displacementY - parameter.m_MaxJumpHeight > 0)
                return nullData;

            // Get a jump height if target point is between min height and maximum height
            float m_JumpHeight = Mathf.Clamp(displacementY, parameter.m_MinJumpHeight, parameter.m_MaxJumpHeight);

            // Time to reach point
            // time: Time using the maximum height jump
            float time = Mathf.Sqrt(-2 * parameter.m_MaxJumpHeight / m_Gravity) +
                Mathf.Sqrt(2 * (displacementY - parameter.m_MaxJumpHeight) / m_Gravity);

            // timeLower: Time using height of the ledge
            float timeLower = Mathf.Sqrt(-2 * m_JumpHeight / m_Gravity) +
                Mathf.Sqrt(2 * (displacementY - m_JumpHeight) / m_Gravity);

            // Velocities for each time calculated
            Vector3 velocity = displacementXZ / time;
            Vector3 velocityLower = displacementXZ / timeLower;

            // If velocity is greater than maximum horizontal speed, means that this launch is not possible
            if (velocity.magnitude > parameter.HorizontalSpeed)
                return nullData;

            // Check which launch to use
            bool useLower = timeLower < time && velocityLower.magnitude <= parameter.HorizontalSpeed;

            // Set vertical speed
            float vy = Mathf.Sqrt(-2 * m_Gravity * (useLower ? m_JumpHeight : parameter.m_MaxJumpHeight));

            // Get final velocity
            Vector3 finalVelocity = (useLower) ? velocityLower : velocity;
            finalVelocity.y = vy * -Mathf.Sign(m_Gravity);

            return new LaunchData(finalVelocity, targetPoint, (useLower) ? timeLower : time, true);
        }


        /// <summary>
        /// Choose between possible launches which is the best
        /// </summary>
        /// <param name="allData">all launches that solves the math</param>
        /// <returns>Best Launch Data</returns>
        private LaunchData ChooseBestLaunchData(Vector3 launchOrigin, List<LaunchData> allData, Vector3 targetDirection)
        {
            // Start using the first launch in the list
            LaunchData highestLaunch = allData[0];

            // Loop trough all data to choose the highest one
            foreach (LaunchData data in allData)
            {
                if (data.target.y > highestLaunch.target.y)
                    highestLaunch = data;
            }

            // Start a list for possible launches data
            List<LaunchData> possibleLaunches = new List<LaunchData>();

            // Loop trough all data and choose those that are in an acceptable vertical range
            foreach (LaunchData data in allData)
            {
                if (Mathf.Abs(data.target.y - highestLaunch.target.y) < 0.2f)
                    possibleLaunches.Add(data);
            }

            // Start the best launch data in the first index
            LaunchData bestLaunch = highestLaunch;

            launchOrigin.y = bestLaunch.target.y;                                     // Equalize the height to get best direction
            Vector3 direction = (bestLaunch.target - launchOrigin);        // Get direction between target and launch origin  
            direction.y = 0;
            direction.Normalize();
            float DotResultInUse = Vector3.Dot(targetDirection, direction);           // Dot result for directions vectors

            // Loop trough all possible launches to choose the one that is closer 
            // to character desired move direction
            foreach (LaunchData data in possibleLaunches)
            {
                launchOrigin.y = data.target.y;
                direction = (data.target - launchOrigin).normalized;
                direction.y = 0;
                direction.Normalize();
                float dot = Vector3.Dot(targetDirection, direction);

                if (dot > DotResultInUse)
                {
                    DotResultInUse = dot;
                    bestLaunch = data;
                }
            }

            return bestLaunch;
        }

        /// <summary>
        /// Draw Jump Trajectory
        /// </summary>
        protected void DrawPath(LaunchData launchData, Vector3 grabPosition)
        {
            Vector3 previousPosition = grabPosition;

            if (launchData.foundSolution == false)
                return;

            for (int i = 1; i <= m_Resolution; i++)
            {
                float simulationTime = i / (float)m_Resolution * launchData.timeToTarget;

                Vector3 displacement = (launchData.initialVelocity * simulationTime) +
                    Vector3.up * m_Gravity * Mathf.Pow(simulationTime, 2) / 2f;

                Vector3 drawPoint = grabPosition + displacement;

                Debug.DrawLine(previousPosition, drawPoint, Color.blue, 3f);
                previousPosition = drawPoint;
            }

        }

        #endregion

        private void LaunchCharacter(Vector3 m_JumpDirectionVector, Vector3 GrabPosition, float LinecastStart, Collider ignoreCollider = null)
        {
            Vector3 offset = Vector3.zero;      // Start without offset

            JumpParameters parameter = new JumpParameters();

            // Choose jump parameters based on Jump Type
            switch (m_JumpType)
            {
                case ClimbJumpType.Up:
                    parameter = HopUpParameters;
                    break;
                case ClimbJumpType.Right:
                    parameter = HopRightParameters;
                    offset = transform.right * m_System.m_Capsule.radius;   // Add a small offset to the right to avoid grab on corners
                    break;
                case ClimbJumpType.Left:
                    parameter = HopLeftParameters;
                    offset = -transform.right * m_System.m_Capsule.radius;   // Add a small offset to the left to avoid grab on corners
                    break;
                case ClimbJumpType.Back:
                    parameter = JumpBackParameters;
                    break;
            }

            // Set initial velocity and rotation considering that no data will be found
            m_CharacterDesiredRotation = GetRotationFromDirection(m_JumpDirectionVector);
            m_CharacterDesiredVelocity = m_JumpDirectionVector * parameter.HorizontalSpeed + Vector3.up * parameter.VerticalSpeed;
            
            // Try find a possible launch
            LaunchData launch = GetLaunchData(GrabPosition, LinecastStart, parameter,
                m_JumpDirectionVector, m_MaxAngle, offset, m_ClimbablesLayers, ignoreCollider);
            
            if (IsLaunchDataOnlyWay(launch, m_JumpDirectionVector))
            {
                // Found a launch solution
                SetLaunchParameters(launch, m_JumpType);

                // Check if character is jumping right or left
                // After, cast a ray to the target point, if find a ledge and 
                // its normal is not perpendicular to character forward, don't change character rotation
                if (m_JumpType == ClimbJumpType.Right || m_JumpType == ClimbJumpType.Left)
                {
                    Vector3 startRay = launch.target - transform.forward;
                    Vector3 direction = (launch.target - startRay).normalized;

                    RaycastHit hit;
                    if (Physics.SphereCast(startRay, 0.1f, direction, out hit, 3f, m_ClimbablesLayers, QueryTriggerInteraction.Collide))
                    {
                        if (Vector3.Dot(transform.forward, -hit.normal) > 0.5f)
                            m_CharacterDesiredRotation = transform.rotation;
                    }
                }
            }
        }

        /// <summary>
        /// Get Launch Data when character try hop up
        /// </summary>
        /// <param name="maxJumpHeight"></param>
        /// <param name="topHitPoint"></param>
        /// <returns></returns>
        public LaunchData GetLaunchHopUp(Vector3 topHitPoint, Vector3 GrabPosition)
        {
            Vector3 origin = topHitPoint + (HopUpParameters.m_MaxJumpHeight + 0.3f) * Vector3.up;

            RaycastHit[] hits = Physics.BoxCastAll(origin, Vector3.one * 0.25f, Vector3.down, Quaternion.identity, HopUpParameters.m_MaxJumpHeight, m_ClimbablesLayers, QueryTriggerInteraction.Collide);
            Vector3 target = origin;

            foreach (RaycastHit hit in hits)
            {
                if (hit.point == Vector3.zero)
                    continue;

                if (Mathf.Abs(hit.transform.position.y - GrabPosition.y) > 0.1f)
                {
                    if (hit.point.y < target.y)
                        target = hit.point;
                }
            }

            Debug.DrawRay(target, -transform.forward, Color.magenta, 3f);

            float totalHeight = target.y - GrabPosition.y;
            totalHeight = Mathf.Clamp(totalHeight, HopUpParameters.m_MaxJumpHeight * 0.2f, HopUpParameters.m_MaxJumpHeight + 0.5f);
            float speed = Mathf.Sqrt(-2 * m_Gravity * totalHeight);

            float time = Mathf.Sqrt(-2 * totalHeight / m_Gravity);

            return new LaunchData(Vector3.up * speed, target, time, true);
        }

        private void Reset()
        {
            m_UseRootMotion = false;

            m_UseInputStateToEnter = InputEnterType.ButtonDown;
            InputButton = InputReference.Jump;
        }
    }


    /// <summary>
    /// Struct that stores launch properties
    /// </summary>
    public struct LaunchData
    {
        public readonly Vector3 initialVelocity;
        public readonly Vector3 target;
        public readonly float timeToTarget;
        public readonly bool foundSolution;

        public LaunchData(Vector3 launchVelocity, Vector3 targetPoint, float timeToReach, bool found)
        {
            initialVelocity = launchVelocity;
            target = targetPoint;
            timeToTarget = timeToReach;
            foundSolution = found;
        }
    }
}