using UnityEngine;
using UnityEngine.InputSystem;

// Só lê o input e manda pro carro. Nada de física aqui.
[RequireComponent(typeof(PlayerInput))]
public class CarPlayerInput : MonoBehaviour
{
    [SerializeField] private CarController car;

    public bool PodeDirigir { get; set; } = false;
    public bool DriftSegurado { get; private set; }

    private InputAction moveAction;
    private InputAction driftAction;

    private void Awake()
    {
        // Pega as actions da instância DESTE jogador, já pareadas com o dispositivo dele.
        // Os nomes precisam ser iguais aos do Input Actions ("Move" e "Drift").
        var playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        driftAction = playerInput.actions["Drift"];
    }

    private void Update()
    {
        if (driftAction.WasPressedThisFrame()) Debug.Log("Espaço apertado | PodeDirigir = " + PodeDirigir);
        if (!PodeDirigir)
        {
            DriftSegurado = false;
            car.SetInput(0f, 0f);
            car.SetDrift(false);
            return;
        }

        Vector2 move = moveAction.ReadValue<Vector2>();
        DriftSegurado = driftAction.IsPressed();

        car.SetInput(move.y, move.x);
        car.SetDrift(DriftSegurado);
    }
}