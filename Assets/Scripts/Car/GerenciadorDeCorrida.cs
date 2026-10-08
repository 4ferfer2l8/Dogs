using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GerenciadorDeCorrida : MonoBehaviour
{
    private readonly List<ProgressoNaCorrida> participantes = new();
    private bool corridaEncerrada = false;
    private bool largadaLiberada = false;

    // true = venceu, false = perdeu
    public event Action<ProgressoNaCorrida, bool> AoDefinirResultado;

    // Teste: libera sozinho após 3s. Troque pela sua contagem quando tiver uma.
    private void Start()
    {
        Invoke(nameof(LiberarLargada), 3f);
    }

    public void RegistrarJogador(PlayerInput jogador)
    {
        var progresso = jogador.GetComponentInChildren<ProgressoNaCorrida>();
        if (progresso == null) return;

        participantes.Add(progresso);
        progresso.AoTerminarCorrida += QuandoAlguemTerminar;

        // Quem entra depois da largada já nasce liberado
        DefinirPodeDirigir(progresso, largadaLiberada && !corridaEncerrada);
    }

    public void LiberarLargada()
    {
        largadaLiberada = true;
        foreach (var p in participantes)
            DefinirPodeDirigir(p, true);
    }

    private void QuandoAlguemTerminar(ProgressoNaCorrida vencedor)
    {
        if (corridaEncerrada) return;
        corridaEncerrada = true;

        foreach (var p in participantes)
        {
            if (p == null) continue; // kart já destruído
            AoDefinirResultado?.Invoke(p, p == vencedor);
        }
    }

    private void DefinirPodeDirigir(ProgressoNaCorrida p, bool valor)
    {
        if (p == null) return;
        var piloto = p.GetComponentInChildren<CarPlayerInput>();
        if (piloto != null) piloto.PodeDirigir = valor;
    }

    private void OnDestroy()
    {
        foreach (var p in participantes)
        {
            if (p == null) continue;
            p.AoTerminarCorrida -= QuandoAlguemTerminar;
            DefinirPodeDirigir(p, false);
        }
    }
}