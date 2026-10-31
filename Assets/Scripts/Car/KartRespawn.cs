using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class KartRespawn : MonoBehaviour
{
    [SerializeField] private float alturaExtra = 0.3f;   // evita nascer enterrado no chão
    [SerializeField] private float cooldown = 1f;        // evita spam do botão

    private Rigidbody rb;
    private bool temPonto;
    private Vector3 pontoPos;
    private Quaternion pontoRot;
    private float proximoUso;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Guarda a pose do kart toda vez que ele toca um checkpoint
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<Checkpoint>() == null) return;

        pontoPos = rb.position;
        pontoRot = Quaternion.Euler(0f, transform.eulerAngles.y, 0f); // só a direção, sem inclinação
        temPonto = true;
    }

    public void VoltarAoCheckpoint()
    {
        if (!temPonto || Time.time < proximoUso) return;
        proximoUso = Time.time + cooldown;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.position = pontoPos + Vector3.up * alturaExtra;
        rb.rotation = pontoRot;
    }
}