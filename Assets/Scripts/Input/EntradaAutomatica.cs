using UnityEngine;
using UnityEngine.InputSystem;

public class EntradaAutomatica : MonoBehaviour
{
    public void EntrarComOsDoisJogadores()
    {
        var manager = PlayerInputManager.instance;

        manager.JoinPlayer(0, -1, "TecladoEsquerda", Keyboard.current);

        if (Gamepad.current != null)
            manager.JoinPlayer(1, -1, "Gamepad", Gamepad.current);
        else
            manager.JoinPlayer(1, -1, "TecladoDireita", Keyboard.current);
    }

    public void Start()
    {
        EntrarComOsDoisJogadores();
    }
}   