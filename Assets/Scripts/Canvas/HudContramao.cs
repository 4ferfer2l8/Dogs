using UnityEngine;

public class HudContramao : MonoBehaviour
{
    [SerializeField] private DetectorContramao detector;
    [SerializeField] private GameObject aviso;  

    private void Start()
    {
        aviso.SetActive(false);
        detector.AoMudarEstado += Mostrar;
    }

    private void OnDestroy()
    {
        detector.AoMudarEstado -= Mostrar;
    }

    private void Mostrar(bool mostrar) => aviso.SetActive(mostrar);
}