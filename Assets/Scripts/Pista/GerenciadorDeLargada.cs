using UnityEngine;
using UnityEngine.InputSystem;

public class GerenciadorDeLargada : MonoBehaviour
{
    [SerializeField] private Transform[] pontosDeLargada;

    private int proximoPonto = 0;

    public void AoEntrarJogador(PlayerInput jogador)
    {
        if (proximoPonto >= pontosDeLargada.Length) return;

        Transform ponto = pontosDeLargada[proximoPonto];
        jogador.transform.SetPositionAndRotation(ponto.position, ponto.rotation);

        // Zera o impulso pra ele não nascer voando
        if (jogador.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        proximoPonto++;
    }
}