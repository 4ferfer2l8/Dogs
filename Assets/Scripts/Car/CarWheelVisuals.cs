using UnityEngine;

public class CarWheelVisuals : MonoBehaviour
{
    [System.Serializable]
    public class Wheel
    {
        public Transform mesh;       // a roda em si (não o EixoRodas)
        public bool isFrontWheel;    // só as da frente esterçam

        [HideInInspector] public Quaternion startLocalRot;   // rotação inicial relativa ao carro
        [HideInInspector] public Vector3 startCenterLocal;   // centro visual da roda relativo ao carro
        [HideInInspector] public Vector3 pivotToCenter;      // do pivô até o centro, no espaço da roda
        [HideInInspector] public float spinAngle;
    }

    [SerializeField] private CarController car;
    [SerializeField] private Wheel[] wheels;   // MESMA ordem dos rayPoints: FL, FR, RL, RR
    [SerializeField] private float maxSteerAngle = 30f;
    [SerializeField] private float steerSmoothing = 10f;

    private float currentSteerAngle;

    private void Start()
    {
        foreach (Wheel w in wheels)
        {
            if (w.mesh == null) continue;

            w.startLocalRot = Quaternion.Inverse(car.transform.rotation) * w.mesh.rotation;

            // Centro real da roda (independe de onde está o pivô do modelo)
            Renderer r = w.mesh.GetComponentInChildren<Renderer>();
            Vector3 centerWorld = r != null ? r.bounds.center : w.mesh.position;

            w.startCenterLocal = car.transform.InverseTransformPoint(centerWorld);
            w.pivotToCenter = Quaternion.Inverse(w.mesh.rotation) * (centerWorld - w.mesh.position);
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
            if (w.mesh == null) continue;

            // Suspensão: mola mais curta que o repouso = roda sobe
            float offset = car.RestLength - car.GetSpringLength(i);

            float steer = w.isFrontWheel ? currentSteerAngle : 0f;
            w.spinAngle += spinDelta;

            Quaternion rot = car.transform.rotation
                             * Quaternion.Euler(0f, steer, 0f)
                             * Quaternion.Euler(w.spinAngle, 0f, 0f)
                             * w.startLocalRot;

            Vector3 centerPos = car.transform.TransformPoint(w.startCenterLocal)
                                + car.transform.up * offset;

            w.mesh.rotation = rot;
            w.mesh.position = centerPos - rot * w.pivotToCenter;
        }
    }
}