using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Rigidbody carRB;
    [SerializeField] private Transform[] rayPoints;
    [SerializeField] private LayerMask drivable;
    [SerializeField] private Transform accelerationpoint;
    [SerializeField] private Vector3 centerOfMass = new Vector3(0f, -0.2f, 0f);

    [Header("Suspension Settings")]
    [SerializeField] private float springStiffness = 750f;
    [SerializeField] private float damperStiffness = 450f;
    [SerializeField] private float restLength = 0.3f;
    [SerializeField] private float springTravel = 0.2f;
    [SerializeField] private float wheelRadius = 0.33f;

    [Header("Car Settings")]
    [SerializeField] private float acceleration = 14f;
    [SerializeField] private float maxSpeed = 25f;
    [SerializeField] private float deceleration = 6f;
    [SerializeField] private float steerStrength = 8f;
    [SerializeField] private AnimationCurve turningCurve = new AnimationCurve(
        new Keyframe(0f, 0f), new Keyframe(0.1f, 1f), new Keyframe(1f, 0.7f));
    [SerializeField] private float dragCoefficient = 4f;

    // Input (preenchido de fora, via SetInput)
    private float moveInput = 0f;
    private float steerInput = 0f;

    private Vector3 currentCarLocalVelocity = Vector3.zero;
    private float carVelocityRatio = 0f;

    private int[] wheelsIsGrounded;
    private float[] springLengths;
    private bool isGrounded = false;

    #region Public API (input + dados para as rodas)

    public float SteerInput => steerInput;
    public float ForwardSpeed => currentCarLocalVelocity.z;
    public float WheelRadius => wheelRadius;
    public float RestLength => restLength;

    public float GetSpringLength(int i) => springLengths[i];

    // Quem controla o carro (jogador, IA, replay...) chama isto.
    public void SetInput(float move, float steer)
    {
        moveInput = Mathf.Clamp(move, -1f, 1f);
        steerInput = Mathf.Clamp(steer, -1f, 1f);
    }

    #endregion

    private void Start()
    {
        if (carRB == null) carRB = GetComponent<Rigidbody>();
        carRB.centerOfMass = centerOfMass;

        wheelsIsGrounded = new int[rayPoints.Length];
        springLengths = new float[rayPoints.Length];
        for (int i = 0; i < springLengths.Length; i++)
            springLengths[i] = restLength + springTravel;
    }

    private void FixedUpdate()
    {
        Suspension();
        GroundCheck();
        CalculatorCarVelocity();
        Movment();
    }

    #region Movment

    private void Movment()
    {
        if (isGrounded)
        {
            Acceleration();
            Deceleration();
            Turn();
            SidewaysDrag();
        }
        else
        {
            carRB.AddForceAtPosition(acceleration * moveInput * transform.forward, accelerationpoint.position, ForceMode.Acceleration);
        }
    }

    private void Acceleration()
    {
        // Só acelera se ainda não chegou na velocidade máxima
        // (ou se o jogador pede o sentido contrário ao que o carro anda).
        bool underMaxSpeed = Mathf.Abs(currentCarLocalVelocity.z) < maxSpeed;
        bool opposingMotion = Mathf.Sign(moveInput) != Mathf.Sign(currentCarLocalVelocity.z);

        if (underMaxSpeed || opposingMotion)
        {
            carRB.AddForceAtPosition(acceleration * moveInput * transform.forward, accelerationpoint.position, ForceMode.Acceleration);
        }
    }

    private void Deceleration()
    {
        // Só freia sozinho quando o jogador não está acelerando.
        if (Mathf.Abs(moveInput) > 0.01f) return;
        if (Mathf.Abs(currentCarLocalVelocity.z) < 0.1f) return; // evita tremedeira parado

        float direction = -Mathf.Sign(currentCarLocalVelocity.z);
        carRB.AddForceAtPosition(deceleration * direction * transform.forward, accelerationpoint.position, ForceMode.Acceleration);
    }

    private void Turn()
    {
        // Abs: a curva não tem valores negativos, então usamos a velocidade em módulo.
        // Mathf.Sign inverte o giro na ré.
        float curve = turningCurve.Evaluate(Mathf.Abs(carVelocityRatio));
        carRB.AddTorque(steerStrength * steerInput * curve * Mathf.Sign(carVelocityRatio) * transform.up, ForceMode.Acceleration);
    }

    private void SidewaysDrag()
    {
        float currentSidewaysSpeed = currentCarLocalVelocity.x;

        float dragMagnitude = -currentSidewaysSpeed * dragCoefficient;

        Vector3 dragForce = transform.right * dragMagnitude;

        carRB.AddForceAtPosition(dragForce, carRB.worldCenterOfMass, ForceMode.Acceleration);
    }

    #endregion

    #region Car Status Check

    private void GroundCheck()
    {
        int tempGroundedWheels = 0;

        for (int i = 0; i < wheelsIsGrounded.Length; i++)
        {
            tempGroundedWheels += wheelsIsGrounded[i];
        }

        isGrounded = tempGroundedWheels > 1;
    }

    private void CalculatorCarVelocity()
    {
        currentCarLocalVelocity = transform.InverseTransformDirection(carRB.linearVelocity);
        carVelocityRatio = currentCarLocalVelocity.z / maxSpeed;
    }

    private void Suspension()
    {
        for (int i = 0; i < rayPoints.Length; i++)
        {
            RaycastHit hit;
            float maxLength = restLength + springTravel;

            if (Physics.Raycast(rayPoints[i].position, -rayPoints[i].up, out hit, maxLength + wheelRadius, drivable))
            {
                wheelsIsGrounded[i] = 1;

                float currentSpringLength = hit.distance - wheelRadius;
                springLengths[i] = currentSpringLength;

                float springCompression = (restLength - currentSpringLength) / springTravel;

                float springVelocity = Vector3.Dot(carRB.GetPointVelocity(rayPoints[i].position), rayPoints[i].up);
                float damperForce = damperStiffness * springVelocity;

                float springForce = springStiffness * springCompression;

                float netForce = springForce - damperForce;

                carRB.AddForceAtPosition(netForce * rayPoints[i].up, rayPoints[i].position);

                Debug.DrawLine(rayPoints[i].position, hit.point, Color.red);
            }
            else
            {
                wheelsIsGrounded[i] = 0;
                springLengths[i] = maxLength;

                Debug.DrawLine(rayPoints[i].position, rayPoints[i].position + (wheelRadius + maxLength) * -rayPoints[i].up, Color.green);
            }
        }
    }

    #endregion
}