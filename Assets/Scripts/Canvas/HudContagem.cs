using TMPro;
using UnityEngine;

public class HudContagem : MonoBehaviour
{
    [SerializeField] private TMP_Text texto;

    private LargadaDaCorrida largada;

    private void Start()
    {
        texto.text = "";
        largada = FindFirstObjectByType<LargadaDaCorrida>();
        if (largada != null) largada.AoAtualizarContagem += Mostrar;
    }

    private void OnDestroy()
    {
        if (largada != null) largada.AoAtualizarContagem -= Mostrar;
    }

    private void Mostrar(string valor) => texto.text = valor;
}