using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class LargadaDaCorrida : MonoBehaviour
{
    [SerializeField] private int segundos = 3;

    private readonly List<CarPlayerInput> pilotos = new();

    public event System.Action<string> AoAtualizarContagem;

    public void RegistrarJogador(PlayerInput jogador)
    {
        var piloto = jogador.GetComponentInChildren<CarPlayerInput>();
        if (piloto == null) return;

        piloto.PodeDirigir = false;
        pilotos.Add(piloto);

        // Com os dois dentro, começa a contagem
        if (pilotos.Count == 2)
            StartCoroutine(Contagem());
    }

    private IEnumerator Contagem()
    {

         // Deixa os HUDs dos karts recém-criados se inscreverem antes de contar
        yield return null;

        for (int i = segundos; i > 0; i--)
        {
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