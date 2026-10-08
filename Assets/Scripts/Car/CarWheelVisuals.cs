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
    [SerializeField] private Wheel[] wheels;   // MESMA ordem dos rayPoints: FE, FD, TE, TD
    [SerializeField] private float maxSteerAngle = 30f;
    [SerializeField] private float steerSmoothing = 10f;

    [Header("Giro")]
    [SerializeField] private bool inverterGiro = true;   // o kart tem a frente em -Z, então a ForwardSpeed vem negativa

    [Header("Debug")]
    [SerializeField] private bool logDebug = false;      // ligue só quando precisar investigar

    private float currentSteerAngle;
    private bool pronto;

    private void Awake()
    {
        // Se o campo ficou vazio (ex.: kart instanciado pelo PlayerInputManager), tenta achar sozinho
        if (car == null) car = GetComponentInParent<CarController>();
    }

    private void Start()
    {
        if (car == null)
        {
            Debug.LogError($"{name}: CarWheelVisuals sem CarController. Atribua o campo Car.", this);
            return;
        }

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

        // Diagnóstico: mostra quais rodas o script está controlando
        for (int i = 0; i < wheels.Length; i++)
        {
            Transform m = wheels[i].mesh;
            Debug.Log(m == null
                ? $"Roda {i}: SEM MESH"
                : $"Roda {i}: {m.name} | ativa={m.gameObject.activeInHierarchy} | caminho={Caminho(m)}", this);
        }

        pronto = true;
    }

    private static string Caminho(Transform t)
    {
        string p = t.name;
        while (t.parent != null) { t = t.parent; p = t.name + "/" + p; }
        return p;
    }

    private void LateUpdate()
    {
        if (!pronto) return;

        // Valores do carro, protegidos contra NaN/Infinity (um único frame ruim travaria o giro para sempre)
        float steerInput = Sanear(car.SteerInput);
        float forwardSpeed = Sanear(car.ForwardSpeed);
        float raio = car.WheelRadius;

        float targetSteer = steerInput * maxSteerAngle;
        currentSteerAngle = Mathf.Lerp(currentSteerAngle, targetSteer, steerSmoothing * Time.deltaTime);

        float sinal = inverterGiro ? -1f : 1f;
        float spinDelta = raio > 0.001f
            ? sinal * (forwardSpeed / raio) * Mathf.Rad2Deg * Time.deltaTime
            : 0f;

        if (logDebug && Time.frameCount % 30 == 0)
        {
            Debug.Log($"[Rodas] speed={forwardSpeed:F2} steer={steerInput:F2} raio={raio:F2} " +
                      $"rest={car.RestLength:F3} mola0={car.GetSpringLength(0):F3} spinDelta={spinDelta:F2}", this);
        }

        for (int i = 0; i < wheels.Length; i++)
        {
            Wheel w = wheels[i];
            if (w.mesh == null) continue;

            // Suspensão: mola mais curta que o repouso = roda sobe
            float offset = Sanear(car.RestLength - car.GetSpringLength(i));

            float steer = w.isFrontWheel ? currentSteerAngle : 0f;
            w.spinAngle = Mathf.Repeat(Sanear(w.spinAngle) + spinDelta, 360f);

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

    private static float Sanear(float v)
    {
        return (float.IsNaN(v) || float.IsInfinity(v)) ? 0f : v;
    }
}