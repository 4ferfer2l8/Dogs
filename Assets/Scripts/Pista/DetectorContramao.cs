using System;
using UnityEngine;

public class DetectorContramao : MonoBehaviour
{
    // Arraste aqui o CaminhoDaPista (ou qualquer MonoBehaviour que implemente IDirecaoDaPista)
    [SerializeField] private MonoBehaviour fontePista;
    [SerializeField] private CarPlayerInput piloto;
    [SerializeField] private float tempoParaAvisar = 1.5f;
    [SerializeField] private float limiarAlinhamento = -0.2f;

    private IDirecaoDaPista pista;
    private float tempoNaContramao = 0f;
    private bool avisando = false;

    public event Action<bool> AoMudarEstado;

    public void DefinirPista(IDirecaoDaPista novaPista) => pista = novaPista;

    private void Awake()
    {
        if (pista == null) pista = fontePista as IDirecaoDaPista;
    }

    private void OnValidate()
    {
        if (fontePista != null && fontePista is not IDirecaoDaPista)
        {
            Debug.LogWarning($"{fontePista.name} não implementa IDirecaoDaPista.", this);
            fontePista = null;
        }
    }

    private void Update()
    {
        if (pista == null) return;

        if (piloto == null || !piloto.PodeDirigir)
        {
            DefinirAviso(false);
            tempoNaContramao = 0f;
            return;
        }

        // Direção do traçado no ponto da pista mais próximo do kart
        if (!pista.TentarObterDirecao(transform.position, out Vector3 direcaoDaPista)) return;
        direcaoDaPista.y = 0f;

        Vector3 frenteDoKart = transform.forward;
        frenteDoKart.y = 0f;

        float alinhamento = Vector3.Dot(frenteDoKart.normalized, direcaoDaPista.normalized);

        if (alinhamento < limiarAlinhamento)
            tempoNaContramao += Time.deltaTime;
        else
            tempoNaContramao = 0f;

        DefinirAviso(tempoNaContramao >= tempoParaAvisar);
    }

    private void DefinirAviso(bool valor)
    {
        if (valor == avisando) return;
        avisando = valor;
        AoMudarEstado?.Invoke(avisando);
    }
}