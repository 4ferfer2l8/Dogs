using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Checkpoint : MonoBehaviour
{
    [SerializeField] private int indice; 

    public int Indice => indice;

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        var progresso = other.GetComponentInParent<ProgressoNaCorrida>();
        if (progresso != null)
            progresso.PassouPeloCheckpoint(indice);
    }
}