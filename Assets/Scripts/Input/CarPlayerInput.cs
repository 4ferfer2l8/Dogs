using UnityEngine;
using UnityEngine.InputSystem;

// Só lê o input e manda pro carro. Nada de física aqui.
[RequireComponent(typeof(PlayerInput))]
public class CarPlayerInput : MonoBehaviour
{
    [SerializeField] private CarController car;

    public bool PodeDirigir { get; set; } = false;

    private InputAction moveAction;

    private void Awake()
    {
        // Pega a action da instância DESTE jogador, já pareada com o dispositivo dele
        var playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
    }

    private void Update()
    {

        if (!PodeDirigir)
        {
            car.SetInput(0f, 0f);  
            return;
        }

        Vector2 move = moveAction.ReadValue<Vector2>();
        car.SetInput(move.y, move.x);
    }
}