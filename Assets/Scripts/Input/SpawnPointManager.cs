using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnPointManager : MonoBehaviour
{
    [SerializeField] private Transform[] pontosDeLargada;

    private int proximoPonto = 0;

    public void AoEntrarJogador(PlayerInput jogador)
    {
        if (proximoPonto >= pontosDeLargada.Length) return;

        Transform ponto = pontosDeLargada[proximoPonto];
        jogador.transform.SetPositionAndRotation(ponto.position, ponto.rotation);
        proximoPonto++;
    }
}