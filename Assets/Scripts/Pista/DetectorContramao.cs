using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class DetectorContramao : MonoBehaviour
{
    [SerializeField] private SplineContainer pista;
    [SerializeField] private CarPlayerInput piloto;
    [SerializeField] private float tempoParaAvisar = 1.5f;
    [SerializeField] private float limiarAlinhamento = -0.2f;

    private float tempoNaContramao = 0f;
    private bool avisando = false;

    public event Action<bool> AoMudarEstado;

    public void DefinirPista(SplineContainer container) => pista = container;

    private void Update()
    {
        if (pista == null) return;

        if (piloto == null || !piloto.PodeDirigir)
        {
            DefinirAviso(false);
            tempoNaContramao = 0f;
            return;
        }

        // Ponto da spline mais próximo do kart; t = posição ao longo dela (0 a 1)
        SplineUtility.GetNearestPoint(
            pista.Spline,
            pista.transform.InverseTransformPoint(transform.position),
            out float3 _,
            out float t);

        // Tangente = direção do traçado naquele ponto
        float3 tangenteLocal = pista.Spline.EvaluateTangent(t);
        Vector3 direcaoDaPista = pista.transform.TransformDirection((Vector3)tangenteLocal);
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