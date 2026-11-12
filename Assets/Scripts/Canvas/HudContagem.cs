using UnityEngine;
using UnityEngine.UI;

public class HudContagem : MonoBehaviour
{
    [SerializeField] private Image imagem;

    [Header("Sprites (índice = número: 0, 1, 2, 3...)")]
    [SerializeField] private Sprite[] spritesNumeros;

    [Header("Sprite do 'VAI!' (usado para qualquer texto que não seja número)")]
    [SerializeField] private Sprite spriteVai;

    private LargadaDaCorrida largada;

    private void Start()
    {
        imagem.enabled = false;
        largada = FindFirstObjectByType<LargadaDaCorrida>();
         Debug.Log($"[HudContagem] largada encontrada? {largada != null}");
        if (largada != null) largada.AoAtualizarContagem += Mostrar;
    }

    private void OnDestroy()
    {
        if (largada != null) largada.AoAtualizarContagem -= Mostrar;
    }

    private void Mostrar(string valor)
    {
         Debug.Log($"[HudContagem] recebeu: '{valor}'");
        if (string.IsNullOrWhiteSpace(valor))
        {
            imagem.enabled = false;
            return;
        }

        Sprite sprite;
        if (int.TryParse(valor.Trim(), out int n) && n >= 0 && n < spritesNumeros.Length)
            sprite = spritesNumeros[n];
        else
            sprite = spriteVai;
              Debug.Log($"[HudContagem] sprite escolhido: {(sprite != null ? sprite.name : "NULL")}");

        imagem.sprite = sprite;
        imagem.enabled = sprite != null;
    }
}