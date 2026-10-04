using System;
using UnityEngine;

public class ProgressoNaCorrida : MonoBehaviour
{
    [SerializeField] private int totalDeCheckpoints = 4;
    [SerializeField] private int voltasParaVencer = 3;

    private int proximoCheckpoint = 0;
    private int voltasCompletas = 0;

    private bool jaDeuAVoltaCompleta = false;

    public int VoltasCompletas => voltasCompletas;

    public event Action<ProgressoNaCorrida, int> AoCompletarVolta;
    public event Action<ProgressoNaCorrida> AoTerminarCorrida;

    public void PassouPeloCheckpoint(int indice)
    {
        // Ignora checkpoint fora de ordem (evita atalho e trapaça)
        if (indice != proximoCheckpoint) return;

        // Cruzou a linha de largada tendo passado por todos os outros? Volta completa.
        if (indice == 0 && jaDeuAVoltaCompleta)
        {
            voltasCompletas++;
            AoCompletarVolta?.Invoke(this, voltasCompletas);

            if (voltasCompletas >= voltasParaVencer)
                AoTerminarCorrida?.Invoke(this);
        }

        // Avança para o próximo, voltando ao 0 depois do último
        proximoCheckpoint = (indice + 1) % totalDeCheckpoints;

        // A partir do primeiro checkpoint do percurso, a próxima passagem pelo 0 vale volta
        if (indice == totalDeCheckpoints - 1)
            jaDeuAVoltaCompleta = true;

        Debug.Log($"{name} passou pelo checkpoint {indice}. Próximo: {proximoCheckpoint}. Voltas completas: {voltasCompletas}");
    }
}