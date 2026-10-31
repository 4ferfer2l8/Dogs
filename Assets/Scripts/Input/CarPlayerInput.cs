using UnityEngine;
using UnityEngine.InputSystem;

// Só lê o input e manda pro carro. Nada de física aqui.
[RequireComponent(typeof(PlayerInput))]
public class CarPlayerInput : MonoBehaviour
{
    [SerializeField] private CarController car;

    public bool PodeDirigir { get; set; } = false;
    public bool DriftSegurado { get; private set; }

    private InputAction steerAction;
    private InputAction throttleAction;
    private InputAction driftAction;
    private InputAction respawnAction;   
private KartRespawn respawn; 

    private void Awake()
{
    var playerInput = GetComponent<PlayerInput>();

    // Lista tudo que o asset tem ANTES de procurar, pra aparecer mesmo se faltar alguma
    foreach (var a in playerInput.actions)
        Debug.Log($"Action no asset: '{a.name}'");

    steerAction = playerInput.actions["Steer"];
    throttleAction = playerInput.actions["Throttle"];
    driftAction = playerInput.actions["Drift"];
    respawnAction = playerInput.actions["Respawn"];  
    respawn = GetComponent<KartRespawn>();   
}
    private void Update()
    {
        if (driftAction.WasPressedThisFrame()) Debug.Log("Drift apertado | PodeDirigir = " + PodeDirigir);
        if (!PodeDirigir)
        {
            DriftSegurado = false;
            car.SetInput(0f, 0f);
            car.SetDrift(false);
            return;
        }
        if (respawnAction.WasPressedThisFrame()) respawn.VoltarAoCheckpoint();

        float steer = steerAction.ReadValue<float>();
        float throttle = throttleAction.ReadValue<float>();
        DriftSegurado = driftAction.IsPressed();

        car.SetInput(throttle, steer);   // mesma ordem de antes: (move, steer)
        car.SetDrift(DriftSegurado);
    }
    private void Start()
{
    Debug.Log("CarPlayerInput rodando em: " + name, this);
}
}