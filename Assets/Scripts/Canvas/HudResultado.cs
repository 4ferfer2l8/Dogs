using TMPro;
using UnityEngine;

public class HudResultado : MonoBehaviour
{
    [SerializeField] private ProgressoNaCorrida progresso;
    [SerializeField] private GameObject painel;      // fundo escuro + texto
    [SerializeField] private TMP_Text textoResultado;

    private GerenciadorDeCorrida corrida;

    private void Start()
    {
        painel.SetActive(false);

        corrida = FindFirstObjectByType<GerenciadorDeCorrida>();
        if (corrida != null)
            corrida.AoDefinirResultado += Mostrar;
    }

    private void OnDestroy()
    {
        if (corrida != null)
            corrida.AoDefinirResultado -= Mostrar;
    }

    private void Mostrar(ProgressoNaCorrida quem, bool venceu)
    {
        // Só reage ao resultado do SEU jogador
        if (quem != progresso) return;

        painel.SetActive(true);
        textoResultado.text = venceu ? "VITÓRIA" : "DERROTA";
    }
}