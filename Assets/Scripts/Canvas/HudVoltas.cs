using TMPro;
using UnityEngine;

public class HudVoltas : MonoBehaviour
{
    [SerializeField] private ProgressoNaCorrida progresso;
    [SerializeField] private TMP_Text textoVoltas;
    [SerializeField] private int totalDeVoltas = 3;

    private void OnEnable()
    {
        progresso.AoCompletarVolta += AtualizarTexto;
        MostrarVolta(progresso.VoltasCompletas);
    }

    private void OnDisable()
    {
        progresso.AoCompletarVolta -= AtualizarTexto;
    }

    // Assinatura do evento: (ProgressoNaCorrida, int)
    private void AtualizarTexto(ProgressoNaCorrida quem, int voltas)
    {
        MostrarVolta(voltas);
    }

    private void MostrarVolta(int voltasCompletas)
    {
        // Enquanto corre a 1ª volta, mostra "1/3"
        int voltaAtual = Mathf.Min(voltasCompletas + 1, totalDeVoltas);
        textoVoltas.text = $"VOLTA {voltaAtual}/{totalDeVoltas}";
    }
}