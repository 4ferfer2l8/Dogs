using UnityEngine;

// Versão simples: não precisa de WheelPivot nem de hierarquia especial.
// Cada roda fica onde está no modelo; o script só soma um deslocamento
// (suspensão), um esterço e um giro em cima da posição original.
public class CarWheelVisuals : MonoBehaviour
{
    [System.Serializable]
    public class Wheel
    {
        public Transform mesh;       // o modelo da roda (ex: Wheel_Front_Left)
        public bool isFrontWheel;    // só as da frente esterçam

        [HideInInspector] public Vector3 startLocalPos;      // posição relativa ao carro
        [HideInInspector] public Quaternion startLocalRot;   // rotação relativa ao carro
        [HideInInspector] public float spinAngle;
    }

    [SerializeField] private CarController car;
    [SerializeField] private Wheel[] wheels;   // MESMA ordem do array rayPoints: FL, FR, RL, RR
    [SerializeField] private float maxSteerAngle = 30f;
    [SerializeField] private float steerSmoothing = 10f;

    private float currentSteerAngle;

    private void Start()
    {
        // Guarda onde cada roda está no começo, em relação ao carro.
        foreach (Wheel w in wheels)
        {
            w.startLocalPos = car.transform.InverseTransformPoint(w.mesh.position);
            w.startLocalRot = Quaternion.Inverse(car.transform.rotation) * w.mesh.rotation;
        }
    }

    private void LateUpdate()
    {
        float targetSteer = car.SteerInput * maxSteerAngle;
        currentSteerAngle = Mathf.Lerp(currentSteerAngle, targetSteer, steerSmoothing * Time.deltaTime);

        float spinDelta = (car.ForwardSpeed / car.WheelRadius) * Mathf.Rad2Deg * Time.deltaTime;

        for (int i = 0; i < wheels.Length; i++)
        {
            Wheel w = wheels[i];

            // Suspensão: mola mais curta que o repouso = roda sobe em relação ao corpo.
            float offset = car.RestLength - car.GetSpringLength(i);
            Vector3 basePos = car.transform.TransformPoint(w.startLocalPos);
            w.mesh.position = basePos + car.transform.up * offset;

            // Esterço (só frente) + giro
            float steer = w.isFrontWheel ? currentSteerAngle : 0f;
            w.spinAngle += spinDelta;

            w.mesh.rotation = car.transform.rotation
                              * Quaternion.Euler(0f, steer, 0f)
                              * Quaternion.Euler(w.spinAngle, 0f, 0f)
                              * w.startLocalRot;
        }
    }
}