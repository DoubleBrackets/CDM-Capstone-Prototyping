using System.Collections.Generic;
using Cinemachine;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

public class FloatyBoi : MonoBehaviour
{
    [SerializeField] private HoverbikeInputProvider player1InputProvider;
    [SerializeField] private HoverbikeInputProvider player2InputProvider;

    [Header("Moving"), SerializeField]
    private float speed;

    [SerializeField]
    private float accel;

    [Header("Gear Switch"), SerializeField]
    private float gearChangeSpeed;

    [SerializeField]
    private Vector2 turningSpeedMod;

    [SerializeField]
    private Vector2 speedMod;

    [Header("Turning"), SerializeField]
    private float turningSpeed;

    [SerializeField] private float turnRoll;
    [SerializeField] private float stabilizeSpeed;
    [SerializeField] private float fullStabilizeHeightThreshold;

    [Header("Hovering"), SerializeField]
    private float hoverForce;

    [SerializeField]
    private float damping;

    [SerializeField]
    private float targetHoverHeight;

    [SerializeField]
    private float maxHoverHeight;

    [SerializeField]
    private float maxHoverAngle;

    [SerializeField]
    private Rigidbody rb;

    [SerializeField]
    private LayerMask castMask;

    [SerializeField]
    private SphereCollider caster;

    [Header("Camera"), SerializeField]
    private List<CinemachineVirtualCamera> cameras;

    [SerializeField] private float fovMultiplier;

    private Vector3 normal;
    private Vector3 tangent;
    private Vector3 bitangent;
    private bool isAirborne;
    private float currentHoverHeight;

    private bool isParkMode;


    // 0 to 1
    public float Gear { get; private set; }

    private void Awake()
    {
        Gear = 0;
    }

    private void Update()
    {
        ApplyThrust();
        ApplyRotation();
        UpdateEngineStatus();
        UpdateCamera();
    }

    private void ApplyThrust()
    {
        // Calculate speed
        var speedBonus = Mathf.Lerp(speedMod.x, speedMod.y, Gear);

        // Calculate forward acceleration
        // Simple thrust in the straight forward direction
        var moveInput = player1InputProvider.moveInput;
        moveInput.y = player1InputProvider.accelerateInput ? 1 : 0;

        var targetVel = Quaternion.LookRotation(transform.forward, Vector3.up)
                        * new Vector3(0, 0, moveInput.y)
                        * (speed + speedBonus);

        var vel = rb.velocity;
        vel.y = 0;
        vel = Vector3.MoveTowards(vel, targetVel, accel * Time.deltaTime);

        vel.y = rb.velocity.y;
        rb.velocity = vel;
    }

    private void ApplyRotation()
    {
        var turnSpeedBonus = Mathf.Lerp(turningSpeedMod.x, turningSpeedMod.y, Gear);

        // Create ground basis
        bitangent = Vector3.Cross(normal, Vector3.forward);
        tangent = Vector3.Cross(bitangent, normal);

        var groundTransform = Matrix4x4.Rotate(Quaternion.LookRotation(tangent, normal));
        var groundTransformInv = groundTransform.inverse;

        // Get the current rotation relative to the normal
        var relativeToGroundRotEuler = (groundTransformInv.rotation * transform.rotation).eulerAngles;

        // Apply steering
        var currentY = relativeToGroundRotEuler.y;
        var targetY = currentY + player1InputProvider.moveInput.x * (turningSpeed + turnSpeedBonus) * Time.deltaTime;

        var newY = Mathf.MoveTowardsAngle(currentY,
            targetY,
            (turningSpeed + turnSpeedBonus) * Time.deltaTime);

        // Apply steering roll effect
        var currentZ = relativeToGroundRotEuler.z;
        var targetZ = player1InputProvider.moveInput.x * -turnRoll;
        var steeringRollT = 1 - Mathf.Pow(0.01f, Time.deltaTime);
        var newZ = Mathf.Lerp(currentZ, currentZ + Mathf.DeltaAngle(currentZ, targetZ), steeringRollT);

        // Apply stabilizing (align to ground normal) if grounded
        var currentX = relativeToGroundRotEuler.x;
        var targetX = 0;
        var delta = Mathf.DeltaAngle(currentX, targetX);

        // Multiply by the distance to get a spring like damping effect
        // Also multiply by the hover height ratio, to smoothly stabilize when falling
        var stabilizeDelta = Mathf.Abs(delta) *
                             stabilizeSpeed * Time.deltaTime;
        if (!isAirborne)
        {
            stabilizeDelta *= 1 - Mathf.InverseLerp(fullStabilizeHeightThreshold, maxHoverHeight, currentHoverHeight);
        }

        var newX = Mathf.MoveTowards(currentX, currentX + delta, stabilizeDelta);

        // Transform it back
        var finalAngle = groundTransform.rotation * Quaternion.Euler(newX, newY, newZ);

        transform.rotation = finalAngle;
    }

    private void UpdateEngineStatus()
    {
        if (player2InputProvider.accelerateInput)
        {
            Gear += gearChangeSpeed * Time.deltaTime;
        }

        if (player2InputProvider.deaccelerateInput)
        {
            Gear -= gearChangeSpeed * Time.deltaTime;
        }

        Gear = Mathf.Clamp01(Gear);
    }

    private void UpdateCamera()
    {
        foreach (var cam in cameras)
        {
            cam.m_Lens.FieldOfView = 60 + Vector3.ProjectOnPlane(rb.velocity, Vector3.up).magnitude * fovMultiplier;
        }
    }

    private void FixedUpdate()
    {
        var center = caster.bounds.center;
        // Raycast down

        var raycastHit = Physics.Raycast(
            center,
            Vector3.down,
            out var finalHit,
            maxHoverHeight,
            castMask);

        var isStable = Vector3.Angle(finalHit.normal, Vector3.up) <= maxHoverAngle;
        if (raycastHit && isStable)
        {
            normal = finalHit.normal;
            currentHoverHeight = finalHit.distance;
            isAirborne = false;
            var dist = targetHoverHeight - finalHit.distance;
            if (dist > -2f)
            {
                rb.AwakeAsync();
                rb.AddForce(Vector3.up * (dist * hoverForce), ForceMode.Acceleration);

                var vel = rb.velocity;
                vel.y = Mathf.MoveTowards(vel.y, 0, Mathf.Abs(vel.y) * damping * Time.fixedDeltaTime);
                rb.velocity = vel;
            }
        }
        else
        {
            isAirborne = true;
            currentHoverHeight = maxHoverHeight;

            // Use the player's velocity as a reference normal
            var velDir = rb.velocity.normalized;
            var velTangent = Vector3.Cross(velDir, Vector3.up);
            normal = Vector3.Cross(velTangent, velDir);
        }
    }

    private void OnDrawGizmos()
    {
        // Draw ground basis
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + tangent * 10);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + bitangent * 10);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + normal * 10);


        // Draw hover height
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(caster.bounds.center + Vector3.down * targetHoverHeight, 0.3f);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(caster.bounds.center + Vector3.down * maxHoverHeight, 0.3f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(caster.bounds.center + Vector3.down * fullStabilizeHeightThreshold, 0.3f);
    }
}