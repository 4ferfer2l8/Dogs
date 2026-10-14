using UnityEngine;

/// <summary>
/// Qualquer fonte que saiba dizer "para onde a pista aponta" perto de uma posição.
/// </summary>
public interface IDirecaoDaPista
{
    bool TentarObterDirecao(Vector3 posicao, out Vector3 direcao);
}

/// <summary>
/// Caminho da pista formado por waypoints (Transforms) em ordem, no sentido da corrida.
/// Os waypoints podem ser os próprios checkpoints + pontos extras nas curvas.
/// </summary>
public class CaminhoDaPista : MonoBehaviour, IDirecaoDaPista
{
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private bool circuitoFechado = true;
    [SerializeField] private Color corGizmo = Color.cyan;

    public bool TentarObterDirecao(Vector3 posicao, out Vector3 direcao)
    {
        direcao = Vector3.zero;
        if (waypoints == null || waypoints.Length < 2) return false;

        int segmentos = circuitoFechado ? waypoints.Length : waypoints.Length - 1;
        float menorDistancia2 = float.MaxValue;
        bool achou = false;

        Vector3 p = posicao;
        p.y = 0f;

        for (int i = 0; i < segmentos; i++)
        {
            Transform a = waypoints[i];
            Transform b = waypoints[(i + 1) % waypoints.Length];
            if (a == null || b == null) continue;

            Vector3 pa = a.position; pa.y = 0f;
            Vector3 pb = b.position; pb.y = 0f;

            Vector3 ab = pb - pa;
            float tamanho2 = ab.sqrMagnitude;
            if (tamanho2 < 0.0001f) continue;

            // Ponto mais próximo do kart dentro do segmento A→B
            float t = Mathf.Clamp01(Vector3.Dot(p - pa, ab) / tamanho2);
            float distancia2 = (pa + ab * t - p).sqrMagnitude;

            if (distancia2 < menorDistancia2)
            {
                menorDistancia2 = distancia2;
                direcao = ab.normalized;
                achou = true;
            }
        }

        return achou;
    }

    [ContextMenu("Preencher com os filhos")]
    private void PreencherComFilhos()
    {
        waypoints = new Transform[transform.childCount];
        for (int i = 0; i < waypoints.Length; i++)
            waypoints[i] = transform.GetChild(i);
    }

    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length < 2) return;

        Gizmos.color = corGizmo;
        int segmentos = circuitoFechado ? waypoints.Length : waypoints.Length - 1;

        for (int i = 0; i < segmentos; i++)
        {
            Transform a = waypoints[i];
            Transform b = waypoints[(i + 1) % waypoints.Length];
            if (a == null || b == null) continue;

            Gizmos.DrawLine(a.position, b.position);
            Gizmos.DrawSphere(a.position, 0.6f);

            // Seta no meio do segmento mostrando o sentido
            Vector3 meio = (a.position + b.position) * 0.5f;
            Vector3 dir = (b.position - a.position).normalized;
            Vector3 lado = Vector3.Cross(Vector3.up, dir);
            Gizmos.DrawLine(meio, meio - dir * 1.5f + lado * 0.8f);
            Gizmos.DrawLine(meio, meio - dir * 1.5f - lado * 0.8f);

#if UNITY_EDITOR
            UnityEditor.Handles.Label(a.position + Vector3.up, i.ToString());
#endif
        }
    }
}