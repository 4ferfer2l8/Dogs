using UnityEngine;
using UnityEngine.InputSystem;

// Só lê o input e manda pro carro. Nada de física aqui.
public class CarPlayerInput : MonoBehaviour
{
    [SerializeField] private CarController car;
    [SerializeField] private InputActionReference moveAction; // Vector2: x = direção, y = acelerar/ré

    private void OnEnable()  => moveAction.action.Enable();
    private void OnDisable() => moveAction.action.Disable();

    private void Update()
    {
        Vector2 move = moveAction.action.ReadValue<Vector2>();
        car.SetInput(move.y, move.x);
    }
}