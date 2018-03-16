using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

using LockingPolicy = Thalmic.Myo.LockingPolicy;
using Pose = Thalmic.Myo.Pose;
using UnlockType = Thalmic.Myo.UnlockType;
using VibrationType = Thalmic.Myo.VibrationType;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof (Rigidbody))]
    [RequireComponent(typeof (CapsuleCollider))]
    public class RigidbodyFirstPersonController : MonoBehaviour
    {

		/// ///////////////////////////
		// Myo game object to connect with.
		// This object must have a ThalmicMyo script attached.
		public GameObject myo = null;

		// A rotation that compensates for the Myo armband's orientation parallel to the ground, i.e. yaw.
		// Once set, the direction the Myo armband is facing becomes "forward" within the program.
		// Set by making the fingers spread pose or pressing "r".
		private Quaternion _antiYaw = Quaternion.identity;

		// A reference angle representing how the armband is rotated about the wearer's arm, i.e. roll.
		// Set by making the fingers spread pose or pressing "r".
		private float _referenceRoll = 0.0f;

		// The pose from the last update. This is used to determine if the pose has changed
		// so that actions are only performed upon making them rather than every frame during
		// which they are active.
		private Pose _lastPose = Pose.Unknown;

		///////////////////////////////////

        [Serializable]
        public class MovementSettings
        {
            public float ForwardSpeed = 8.0f;   // Speed when walking forward
            public float BackwardSpeed = 4.0f;  // Speed when walking backwards
            public float StrafeSpeed = 4.0f;    // Speed when walking sideways
            public float RunMultiplier = 2.0f;   // Speed when sprinting
	        public KeyCode RunKey = KeyCode.LeftShift;
            public float JumpForce = 30f;
            public AnimationCurve SlopeCurveModifier = new AnimationCurve(new Keyframe(-90.0f, 1.0f), new Keyframe(0.0f, 1.0f), new Keyframe(90.0f, 0.0f));
            [HideInInspector] public float CurrentTargetSpeed = 8f;

#if !MOBILE_INPUT
            private bool m_Running;
#endif

            public void UpdateDesiredTargetSpeed(Vector2 input)
            {
	            if (input == Vector2.zero) return;
				if (input.x > 0 || input.x < 0)
				{
					//strafe
					CurrentTargetSpeed = StrafeSpeed;
				}
				if (input.y < 0)
				{
					//backwards
					CurrentTargetSpeed = BackwardSpeed;
				}
				if (input.y > 0)
				{
					//forwards
					//handled last as if strafing and moving forward at the same time forwards speed should take precedence
					CurrentTargetSpeed = ForwardSpeed;
				}
#if !MOBILE_INPUT
	            if (Input.GetKey(RunKey))
	            {
		            CurrentTargetSpeed *= RunMultiplier;
		            m_Running = true;
	            }
	            else
	            {
		            m_Running = false;
	            }
#endif
            }

#if !MOBILE_INPUT
            public bool Running
            {
                get { return m_Running; }
            }
#endif
        }


        [Serializable]
        public class AdvancedSettings
        {
            public float groundCheckDistance = 0.01f; // distance for checking if the controller is grounded ( 0.01f seems to work best for this )
            public float stickToGroundHelperDistance = 0.5f; // stops the character
            public float slowDownRate = 20f; // rate at which the controller comes to a stop when there is no input
            public bool airControl; // can the user control the direction that is being moved in the air
            [Tooltip("set it to 0.1 or more if you get stuck in wall")]
            public float shellOffset; //reduce the radius by that ratio to avoid getting stuck in wall (a value of 0.1f is nice)
        }


        public Camera cam;
        public MovementSettings movementSettings = new MovementSettings();
        public MouseLook mouseLook = new MouseLook();
        public AdvancedSettings advancedSettings = new AdvancedSettings();


        private Rigidbody m_RigidBody;
        private CapsuleCollider m_Capsule;
        private float m_YRotation;
        private Vector3 m_GroundContactNormal;
        private bool m_Jump, m_PreviouslyGrounded, m_Jumping, m_IsGrounded;
		private int moveLeftRight;
		private int moveFrontBack;
		private double myoUp;
		private double myoLeft;


        public Vector3 Velocity
        {
            get { return m_RigidBody.velocity; }
        }

        public bool Grounded
        {
            get { return m_IsGrounded; }
        }

        public bool Jumping
        {
            get { return m_Jumping; }
        }

        public bool Running
        {
            get
            {
 #if !MOBILE_INPUT
				return movementSettings.Running;
#else
	            return false;
#endif
            }
        }


        private void Start()
        {
            m_RigidBody = GetComponent<Rigidbody>();
            m_Capsule = GetComponent<CapsuleCollider>();
            mouseLook.Init (transform, cam.transform);
            CrossPlatformInputManager.SwitchActiveInputMethod(CrossPlatformInputManager.ActiveInputMethod.Hardware);
        }


        private void Update()
        {
            RotateView();

            if (CrossPlatformInputManager.GetButtonDown("Jump") && !m_Jump)
            {
                m_Jump = true;
            }
			////////////////////////////////////////////////
			// Access the ThalmicMyo component attached to the Myo object.
//			myo = null;
			ThalmicMyo thalmicMyo = myo.GetComponent<ThalmicMyo> ();

			// Update references when the pose becomes fingers spread or the q key is pressed.
/*			bool updateReference = false;
			if (thalmicMyo.pose != _lastPose) {
				_lastPose = thalmicMyo.pose;

				if (thalmicMyo.pose == Pose.FingersSpread) {
					updateReference = true;

					ExtendUnlockAndNotifyUserAction(thalmicMyo);
				}
			}
			if (Input.GetKeyDown ("r")) {
				updateReference = true;
			}
/*
			// Update references. This anchors the joint on-screen such that it faces forward away
			// from the viewer when the Myo armband is oriented the way it is when these references are taken.
/*			if (updateReference) {
				// _antiYaw represents a rotation of the Myo armband about the Y axis (up) which aligns the forward
				// vector of the rotation with Z = 1 when the wearer's arm is pointing in the reference direction.
				_antiYaw = Quaternion.FromToRotation (
					new Vector3 (myo.transform.forward.x, 0, myo.transform.forward.z),
					new Vector3 (0, 0, 1)
				);

				// _referenceRoll represents how many degrees the Myo armband is rotated clockwise
				// about its forward axis (when looking down the wearer's arm towards their hand) from the reference zero
				// roll direction. This direction is calculated and explained below. When this reference is
				// taken, the joint will be rotated about its forward axis such that it faces upwards when
				// the roll value matches the reference.
				Vector3 referenceZeroRoll = computeZeroRollVector (myo.transform.forward);
				_referenceRoll = rollFromZero (referenceZeroRoll, myo.transform.forward, myo.transform.up);
			}
*/
			// Current zero roll vector and roll value.
//			Vector3 zeroRoll = computeZeroRollVector (myo.transform.forward);
//			float roll = rollFromZero (zeroRoll, myo.transform.forward, myo.transform.up);

			// The relative roll is simply how much the current roll has changed relative to the reference roll.
			// adjustAngle simply keeps the resultant value within -180 to 180 degrees.
//			float relativeRoll = normalizeAngle (roll - _referenceRoll);

			// antiRoll represents a rotation about the myo Armband's forward axis adjusting for reference roll.
//			Quaternion antiRoll = Quaternion.AngleAxis (relativeRoll, myo.transform.forward);

			// Here the anti-roll and yaw rotations are applied to the myo Armband's forward direction to yield
			// the orientation of the joint.
//			transform.rotation = _antiYaw * antiRoll * Quaternion.LookRotation (myo.transform.forward);

			// The above calculations were done assuming the Myo armbands's +x direction, in its own coordinate system,
			// was facing toward the wearer's elbow. If the Myo armband is worn with its +x direction facing the other way,
			// the rotation needs to be updated to compensate.
/*			if (thalmicMyo.xDirection == Thalmic.Myo.XDirection.TowardWrist) {
				// Mirror the rotation around the XZ plane in Unity's coordinate system (XY plane in Myo's coordinate
				// system). This makes the rotation reflect the arm's orientation, rather than that of the Myo armband.
				transform.rotation = new Quaternion(transform.localRotation.x,
					-transform.localRotation.y,
					transform.localRotation.z,
					-transform.localRotation.w);
		}
		*/				/////////////////////////////////////////////////
        }

		/// <summary>
		/// //////////////////////////////////////////////////////
		/// </summary>
		// Compute the angle of rotation clockwise about the forward axis relative to the provided zero roll direction.
		// As the armband is rotated about the forward axis this value will change, regardless of which way the
		// forward vector of the Myo is pointing. The returned value will be between -180 and 180 degrees.
/*		float rollFromZero (Vector3 zeroRoll, Vector3 forward, Vector3 up)
		{
			// The cosine of the angle between the up vector and the zero roll vector. Since both are
			// orthogonal to the forward vector, this tells us how far the Myo has been turned around the
			// forward axis relative to the zero roll vector, but we need to determine separately whether the
			// Myo has been rolled clockwise or counterclockwise.
			float cosine = Vector3.Dot (up, zeroRoll);

			// To determine the sign of the roll, we take the cross product of the up vector and the zero
			// roll vector. This cross product will either be the same or opposite direction as the forward
			// vector depending on whether up is clockwise or counter-clockwise from zero roll.
			// Thus the sign of the dot product of forward and it yields the sign of our roll value.
			Vector3 cp = Vector3.Cross (up, zeroRoll);
			float directionCosine = Vector3.Dot (forward, cp);
			float sign = directionCosine < 0.0f ? 1.0f : -1.0f;

			// Return the angle of roll (in degrees) from the cosine and the sign.
			return sign * Mathf.Rad2Deg * Mathf.Acos (cosine);
		}

*/
		// Compute a vector that points perpendicular to the forward direction,
		// minimizing angular distance from world up (positive Y axis).
		// This represents the direction of no rotation about its forward axis.
	Vector3 computeZeroRollVector (Vector3 forward)
		{
			Vector3 antigravity = Vector3.up;
			Vector3 m = Vector3.Cross (myo.transform.forward, antigravity);
			Vector3 roll = Vector3.Cross (m, myo.transform.forward);

			return roll.normalized;
		}

		// Adjust the provided angle to be within a -180 to 180.
		float normalizeAngle (float angle)
		{
			if (angle > 180.0f) {
				return angle - 360.0f;
			}
			if (angle < -180.0f) {
				return angle + 360.0f;
			}
			return angle;
		}

		// Extend the unlock if ThalmcHub's locking policy is standard, and notifies the given myo that a user action was
		// recognized.
		void ExtendUnlockAndNotifyUserAction (ThalmicMyo myo)
		{
			ThalmicHub hub = ThalmicHub.instance;

			if (hub.lockingPolicy == LockingPolicy.Standard) {
				myo.Unlock (UnlockType.Timed);
			}

			myo.NotifyUserAction ();
		}

		////////////////////////////////////////
        private void FixedUpdate()
        {
            GroundCheck();

	//		int moveHorizontal = 0;
	//		Debug.Log(myo.transform.up.z);
			myoLeft = myo.transform.right.y;
			myoUp = myo.transform.up.z;

			// Horisontal control left/right
			if (myoLeft <= 0.15 && myoLeft >= -0.15 ) {
				moveLeftRight = 0;
				//			Debug.Log(moveLeftRight);
			}
			if (myoLeft >= 0.15 ) {
				moveLeftRight = -1;
				//			Debug.Log(moveLeftRight);
			}

			if(myoLeft <= -0.15 ) {
				moveLeftRight = 1;
				//			Debug.Log(moveLeftRight);
			}

			Debug.Log (myoUp);
			// Horisontal control front/back
			if (myoUp <= 0.15 && myoUp >= -0.15) {
				moveFrontBack = 0;
//				Debug.Log(moveFrontBack);
			}

			if (myoUp >= 0.15 ) {
				moveFrontBack = 1;
//				Debug.Log(moveFrontBack);
			}

			if(myoUp <= -0.15 ) {
				moveFrontBack = -1;
//				Debug.Log(moveFrontBack);
			}

			Vector2 input = new Vector3 (moveLeftRight, moveFrontBack);
//			Vector2 input2 = GetInput ();
			//Vector2 input = new Vector3 (myo.transform.right.x, myo.transform.up.z);
			movementSettings.UpdateDesiredTargetSpeed(input);

            if ((Mathf.Abs(input.x) > float.Epsilon || Mathf.Abs(input.y) > float.Epsilon) && (advancedSettings.airControl || m_IsGrounded))
            {
                // always move along the camera forward as it is the direction that it being aimed at
                Vector3 desiredMove = cam.transform.forward*input.y + cam.transform.right*input.x;
                desiredMove = Vector3.ProjectOnPlane(desiredMove, m_GroundContactNormal).normalized;

              desiredMove.x = desiredMove.x*movementSettings.CurrentTargetSpeed;
                desiredMove.z = desiredMove.z*movementSettings.CurrentTargetSpeed;
            desiredMove.y = desiredMove.y*movementSettings.CurrentTargetSpeed;


                if (m_RigidBody.velocity.sqrMagnitude <
                    (movementSettings.CurrentTargetSpeed*movementSettings.CurrentTargetSpeed))
                {
                    m_RigidBody.AddForce(desiredMove*SlopeMultiplier(), ForceMode.Impulse);
                }
            }
				
            if (m_IsGrounded)
            {
                m_RigidBody.drag = 5f;

                if (m_Jump)
                {
                    m_RigidBody.drag = 0f;
                    m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x, 0f, m_RigidBody.velocity.z);
                    m_RigidBody.AddForce(new Vector3(0f, movementSettings.JumpForce, 0f), ForceMode.Impulse);
                    m_Jumping = true;
                }

                if (!m_Jumping && Mathf.Abs(input.x) < float.Epsilon && Mathf.Abs(input.y) < float.Epsilon && m_RigidBody.velocity.magnitude < 1f)
                {
                    m_RigidBody.Sleep();
                }
            }
            else
            {
                m_RigidBody.drag = 0f;
                if (m_PreviouslyGrounded && !m_Jumping)
                {
                    StickToGroundHelper();
                }
            }
            m_Jump = false;
        }

		private Vector2 GetInput()
		{

			Vector2 input = new Vector2
			{
				x = CrossPlatformInputManager.GetAxis("Horizontal"),
				y = CrossPlatformInputManager.GetAxis("Vertical")
			};
			movementSettings.UpdateDesiredTargetSpeed(input);
			return input;
		}

        private float SlopeMultiplier()
        {
            float angle = Vector3.Angle(m_GroundContactNormal, Vector3.up);
            return movementSettings.SlopeCurveModifier.Evaluate(angle);
        }


        private void StickToGroundHelper()
        {
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, m_Capsule.radius * (1.0f - advancedSettings.shellOffset), Vector3.down, out hitInfo,
                                   ((m_Capsule.height/2f) - m_Capsule.radius) +
                                   advancedSettings.stickToGroundHelperDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                if (Mathf.Abs(Vector3.Angle(hitInfo.normal, Vector3.up)) < 85f)
                {
                    m_RigidBody.velocity = Vector3.ProjectOnPlane(m_RigidBody.velocity, hitInfo.normal);
                }
            }
        }

        private void RotateView()
        {
            //avoids the mouse looking if the game is effectively paused
            if (Mathf.Abs(Time.timeScale) < float.Epsilon) return;

            // get the rotation before it's changed
            float oldYRotation = transform.eulerAngles.y;

            mouseLook.LookRotation (transform, cam.transform);

            if (m_IsGrounded || advancedSettings.airControl)
            {
                // Rotate the rigidbody velocity to match the new direction that the character is looking
                Quaternion velRotation = Quaternion.AngleAxis(transform.eulerAngles.y - oldYRotation, Vector3.up);
                m_RigidBody.velocity = velRotation*m_RigidBody.velocity;
            }
        }

        /// sphere cast down just beyond the bottom of the capsule to see if the capsule is colliding round the bottom
        private void GroundCheck()
        {
            m_PreviouslyGrounded = m_IsGrounded;
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, m_Capsule.radius * (1.0f - advancedSettings.shellOffset), Vector3.down, out hitInfo,
                                   ((m_Capsule.height/2f) - m_Capsule.radius) + advancedSettings.groundCheckDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                m_IsGrounded = true;
                m_GroundContactNormal = hitInfo.normal;
            }
            else
            {
                m_IsGrounded = false;
                m_GroundContactNormal = Vector3.up;
            }
            if (!m_PreviouslyGrounded && m_IsGrounded && m_Jumping)
            {
                m_Jumping = false;
            }
        }
    }
}
