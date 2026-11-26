using UnityEngine;
using UnityEngine.UI;

public class HudResultado : MonoBehaviour
{
    [SerializeField] private ProgressoNaCorrida progresso;
    [SerializeField] private GameObject painel;
    [SerializeField] private Image imagemResultado;
    [SerializeField] private Sprite spriteVitoria;
    [SerializeField] private Sprite spriteDerrota;

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
        if (quem != progresso) return;

        painel.SetActive(true);
        imagemResultado.sprite = venceu ? spriteVitoria : spriteDerrota;
    }
}