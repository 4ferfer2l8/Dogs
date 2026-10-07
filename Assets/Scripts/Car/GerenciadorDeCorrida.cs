using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GerenciadorDeCorrida : MonoBehaviour
{
    private readonly List<ProgressoNaCorrida> participantes = new();
    private bool corridaEncerrada = false;

    // true = venceu, false = perdeu
    public event Action<ProgressoNaCorrida, bool> AoDefinirResultado;

    public void RegistrarJogador(PlayerInput jogador)
    {
        var progresso = jogador.GetComponentInChildren<ProgressoNaCorrida>();
        if (progresso == null) return;

        participantes.Add(progresso);
        progresso.AoTerminarCorrida += QuandoAlguemTerminar;
    }

    private void QuandoAlguemTerminar(ProgressoNaCorrida vencedor)
    {
        if (corridaEncerrada) return;
        corridaEncerrada = true;

        foreach (var p in participantes)
            AoDefinirResultado?.Invoke(p, p == vencedor);
    }

    private void OnDestroy()
    {
        foreach (var p in participantes)
        {
            var piloto = p.GetComponentInChildren<CarPlayerInput>();
            if (piloto != null) piloto.PodeDirigir = false;
        }
    }
}