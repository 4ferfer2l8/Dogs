using UnityEngine;
using UnityEngine.InputSystem;

public class EntradaAutomatica : MonoBehaviour
{
    public void EntrarComOsDoisJogadores()
    {
        var manager = PlayerInputManager.instance;

        // Jogador 1: teclado
        if (Keyboard.current != null)
            manager.JoinPlayer(playerIndex: 0, pairWithDevice: Keyboard.current);

        // Jogador 2: primeiro gamepad conectado
        if (Gamepad.current != null)
            manager.JoinPlayer(playerIndex: 1, pairWithDevice: Gamepad.current);
    }

    public void Start()
    {
        EntrarComOsDoisJogadores();
    }
}   