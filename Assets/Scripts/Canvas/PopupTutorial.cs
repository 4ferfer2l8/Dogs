using UnityEngine;

public class PopupTutorial : MonoBehaviour
{
    [SerializeField] private GameObject popup;
    [SerializeField] private LargadaDaCorrida largada;

    private void Start()
    {
        popup.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Iniciar()
    {
        popup.SetActive(false);
        largada.LiberarLargada();
    }
}