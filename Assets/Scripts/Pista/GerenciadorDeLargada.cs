using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class GerenciadorDeLargada : MonoBehaviour
{
    [SerializeField] private Transform[] pontosDeLargada;

    private int proximoPonto = 0;
    private Transform[] checkpointsOrdenados;

    private void Awake()
    {
        // Busca os checkpoints da cena e ordena pelo índice que cada um declara
        checkpointsOrdenados = FindObjectsByType<Checkpoint>(FindObjectsSortMode.None)
            .OrderBy(c => c.Indice)
            .Select(c => c.transform)
            .ToArray();
    }

    public void AoEntrarJogador(PlayerInput jogador)
    {
        var detector = jogador.GetComponentInChildren<DetectorContramao>();
        if (detector != null)
            detector.DefinirPista(FindFirstObjectByType<CaminhoDaPista>());

        if (proximoPonto >= pontosDeLargada.Length) return;

        Transform ponto = pontosDeLargada[proximoPonto];
        jogador.transform.SetPositionAndRotation(ponto.position, ponto.rotation);

        if (jogador.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        proximoPonto++;
    }
}