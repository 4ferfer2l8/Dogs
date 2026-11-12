using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LargadaDaCorrida : MonoBehaviour
{
    [SerializeField] private int segundos = 3;
    [SerializeField] private int jogadoresParaComecar = 2;

    private readonly List<CarPlayerInput> pilotos = new();
    private bool contagemIniciada;

    public event System.Action<string> AoAtualizarContagem;

    public void RegistrarJogador(PlayerInput jogador)
    {
        var piloto = jogador.GetComponentInChildren<CarPlayerInput>();
        Debug.Log($"[Largada] RegistrarJogador chamado | piloto encontrado? {piloto != null} | pilotos até agora: {pilotos.Count}");
        if (piloto == null) return;

        piloto.PodeDirigir = false;
        pilotos.Add(piloto);

        // Quando todos os jogadores esperados entrarem, começa a contagem
        if (!contagemIniciada && pilotos.Count >= jogadoresParaComecar)
        {
            contagemIniciada = true;
            StartCoroutine(Contagem());
        }
    }

    private IEnumerator Contagem()
    {
        Debug.Log("[Largada] contagem iniciada");

        // Deixa os HUDs dos karts recém-criados se inscreverem antes de contar
        yield return null;

        for (int i = segundos; i > 0; i--)
        {
            Debug.Log($"[Largada] disparando contagem: '{i}'");
            AoAtualizarContagem?.Invoke(i.ToString());
            yield return new WaitForSeconds(1f);
        }

        AoAtualizarContagem?.Invoke("VAI!");

        foreach (var piloto in pilotos)
            piloto.PodeDirigir = true;

        yield return new WaitForSeconds(1f);
        AoAtualizarContagem?.Invoke("");
    }
}