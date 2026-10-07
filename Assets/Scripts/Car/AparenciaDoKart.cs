using UnityEngine;
using UnityEngine.InputSystem;

public class AparenciaDoKart : MonoBehaviour
{
    [SerializeField] private GameObject[] modelos;  // 0 = kart do P1, 1 = kart do P2

    private void Start()
    {
        int indice = GetComponent<PlayerInput>().playerIndex;

        for (int i = 0; i < modelos.Length; i++)
            modelos[i].SetActive(i == indice);
    }
}