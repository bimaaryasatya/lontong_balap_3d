using UnityEngine;

public class mobil : MonoBehaviour
{
    [Header("Joystick")]
    public FixedJoystick joystick;

    private float horizontalInput, verticalInput;
    private float currentSteerAngle, currentbreakForce;
    private bool isBreaking;

    [Header("Settings")]
    [SerializeField] private float motorForce;
    [SerializeField] private float breakForce;
    [SerializeField] private float maxSteerAngle;

    [Header("Wheel Colliders")]
    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;
    [SerializeField] private WheelCollider rearRightWheelCollider;

    [Header("Wheel Meshes")]
    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;

    private void FixedUpdate()
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
    }

    [Header("Debug")]
    [SerializeField] private bool debug = true;
    [SerializeField] private bool debugDetailed = true;
    [SerializeField] private bool driveAllWheelsForDebug = false;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning("mobil: Rigidbody not found on the car GameObject. WheelColliders need a Rigidbody on the root.");
        }
        else if (rb.isKinematic)
        {
            Debug.LogWarning("mobil: Rigidbody is kinematic. Set isKinematic = false to allow physics movement.");
        }
        // check assigned colliders
        if (frontLeftWheelCollider == null || frontRightWheelCollider == null || rearLeftWheelCollider == null || rearRightWheelCollider == null)
        {
            Debug.LogWarning("mobil: One or more WheelCollider references are not assigned in the Inspector.");
        }
        // Auto-assign joystick if not set in Inspector
        if (joystick == null)
        {
            var found = FindObjectOfType<FixedJoystick>();
            if (found != null)
            {
                joystick = found;
                Debug.Log("mobil: Auto-assigned FixedJoystick from scene.");
            }
            else
            {
                Debug.LogWarning("mobil: FixedJoystick not assigned and none found in scene. Assign joystick in Inspector.");
            }
        }
    }

    private void GetInput()
    {
        // Use joystick if assigned, otherwise fallback to keyboard axes for quick testing in Editor
        if (joystick != null)
        {
            horizontalInput = joystick.Horizontal;
            verticalInput = joystick.Vertical;
        }
        else
        {
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");
        }

        // braking: Space key for quick test
        isBreaking = Input.GetKey(KeyCode.Space);
    }

    private void HandleMotor()
    {
        // safety checks
        if (frontLeftWheelCollider == null || frontRightWheelCollider == null)
        {
            if (debug) Debug.LogWarning("mobil: Front wheel colliders not assigned. Cannot apply motor torque.");
            return;
        }

        float appliedTorque = verticalInput * motorForce;
        frontLeftWheelCollider.motorTorque = appliedTorque;
        frontRightWheelCollider.motorTorque = appliedTorque;
        if (driveAllWheelsForDebug)
        {
            if (rearLeftWheelCollider != null) rearLeftWheelCollider.motorTorque = appliedTorque;
            if (rearRightWheelCollider != null) rearRightWheelCollider.motorTorque = appliedTorque;
        }

        currentbreakForce = isBreaking ? breakForce : 0f;

        if (debug)
        {
            Debug.Log($"mobil: input V={verticalInput:F2} H={horizontalInput:F2} torque={appliedTorque:F1} break={(isBreaking?"ON":"OFF")}");
            if (rb == null)
            {
                Debug.Log("mobil: No Rigidbody detected on car root — physics may not move the vehicle.");
            }
        }

        if (debugDetailed && rb != null)
        {
            Debug.Log($"mobil: Rigidbody mass={rb.mass} kinematic={rb.isKinematic} velocity={rb.linearVelocity.magnitude:F2}");
            WheelHit hit;
            if (frontLeftWheelCollider != null && frontLeftWheelCollider.GetGroundHit(out hit))
            {
                Debug.Log($"mobil: FL rpm={frontLeftWheelCollider.rpm:F1} grounded forwardSlip={hit.forwardSlip:F2} sidewaysSlip={hit.sidewaysSlip:F2}");
            }
            else if (frontLeftWheelCollider != null)
            {
                Debug.Log($"mobil: FL not grounded rpm={frontLeftWheelCollider.rpm:F1}");
            }

            if (frontRightWheelCollider != null && frontRightWheelCollider.GetGroundHit(out hit))
            {
                Debug.Log($"mobil: FR rpm={frontRightWheelCollider.rpm:F1} grounded forwardSlip={hit.forwardSlip:F2} sidewaysSlip={hit.sidewaysSlip:F2}");
            }
            else if (frontRightWheelCollider != null)
            {
                Debug.Log($"mobil: FR not grounded rpm={frontRightWheelCollider.rpm:F1}");
            }

            if (rearLeftWheelCollider != null && rearLeftWheelCollider.GetGroundHit(out hit))
            {
                Debug.Log($"mobil: RL rpm={rearLeftWheelCollider.rpm:F1} grounded forwardSlip={hit.forwardSlip:F2} sidewaysSlip={hit.sidewaysSlip:F2}");
            }
            else if (rearLeftWheelCollider != null)
            {
                Debug.Log($"mobil: RL not grounded rpm={rearLeftWheelCollider.rpm:F1}");
            }

            if (rearRightWheelCollider != null && rearRightWheelCollider.GetGroundHit(out hit))
            {
                Debug.Log($"mobil: RR rpm={rearRightWheelCollider.rpm:F1} grounded forwardSlip={hit.forwardSlip:F2} sidewaysSlip={hit.sidewaysSlip:F2}");
            }
            else if (rearRightWheelCollider != null)
            {
                Debug.Log($"mobil: RR not grounded rpm={rearRightWheelCollider.rpm:F1}");
            }
        }

        ApplyBreaking();
    }

    private void ApplyBreaking()
    {
        frontRightWheelCollider.brakeTorque = currentbreakForce;
        frontLeftWheelCollider.brakeTorque = currentbreakForce;
        rearLeftWheelCollider.brakeTorque = currentbreakForce;
        rearRightWheelCollider.brakeTorque = currentbreakForce;
    }

    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput;

        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;

        wheelCollider.GetWorldPose(out pos, out rot);

        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }
}