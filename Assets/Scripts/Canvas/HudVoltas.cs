using UnityEngine;
using UnityEngine.UI;

public class HudVoltas : MonoBehaviour
{
    [SerializeField] private ProgressoNaCorrida progresso;

    [Header("Imagens dos números (ficam por cima do background)")]
    [SerializeField] private Image imagemVoltaAtual;
    [SerializeField] private Image imagemTotalVoltas; // opcional

    [Header("Sprites (índice = número: 0, 1, 2, 3...)")]
    [SerializeField] private Sprite[] spritesNumeros;

    [SerializeField] private int totalDeVoltas = 3;

    private void OnEnable()
    {
        progresso.AoCompletarVolta += AtualizarHud;
        MostrarVolta(progresso.VoltasCompletas);
    }

    private void OnDisable()
    {
        progresso.AoCompletarVolta -= AtualizarHud;
    }

    // Assinatura do evento: (ProgressoNaCorrida, int)
    private void AtualizarHud(ProgressoNaCorrida quem, int voltas)
    {
        MostrarVolta(voltas);
    }

    private void MostrarVolta(int voltasCompletas)
    {
        // Enquanto corre a 1ª volta, mostra "1/3"
        int voltaAtual = Mathf.Min(voltasCompletas + 1, totalDeVoltas);

        DefinirSprite(imagemVoltaAtual, voltaAtual);
        DefinirSprite(imagemTotalVoltas, totalDeVoltas);
    }

    private void DefinirSprite(Image alvo, int numero)
    {
        if (alvo == null || spritesNumeros == null || spritesNumeros.Length == 0) return;

        numero = Mathf.Clamp(numero, 0, spritesNumeros.Length - 1);
        alvo.sprite = spritesNumeros[numero];
    }
}