using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Drift estilo Mario Kart: começa com um pulinho, trava o lado da curva,
/// carrega em níveis (azul, laranja, roxo) e dá um mini-turbo ao soltar o botão.
///
/// Este script NÃO lê input e NÃO mexe no Rigidbody.
/// Ele só recebe valores (Processar) e devolve valores
/// (esterço final e multiplicador de velocidade).
/// </summary>
public class DriftController : MonoBehaviour
{
    [Header("Para começar o drift")]
    [SerializeField] private float velocidadeMinima = 8f;
    [SerializeField, Range(0f, 1f)] private float esterceMinimo = 0.3f;

    [Header("Esterço durante o drift")]
    [Tooltip("Multiplicador quando o jogador vira PARA FORA da curva (curva mais aberta)")]
    [SerializeField] private float esterceFora = 0.3f;
    [Tooltip("Multiplicador quando o jogador vira PARA DENTRO da curva (curva mais fechada)")]
    [SerializeField] private float esterceDentro = 1.0f;

    [Header("Mini-turbo")]
    [Tooltip("Segundos de drift necessários para atingir cada nível")]
    [SerializeField] private float[] temposDosNiveis = { 0.5f, 1.3f, 2.2f };
    [Tooltip("Duração do turbo de cada nível (mesmo tamanho do array acima)")]
    [SerializeField] private float[] duracaoDoTurbo = { 0.5f, 0.9f, 1.4f };
    [SerializeField] private float multiplicadorDoTurbo = 1.4f;

    [Header("Visual (opcional)")]
    [Tooltip("Objeto filho que contém o modelo do kart. Só ele gira/pula, o kart em si não.")]
    [SerializeField] private Transform modeloVisual;
    [SerializeField] private float anguloVisual = 10f;
    [SerializeField] private float suavizacaoVisual = 10f;
    [SerializeField] private float alturaDoPulo = 0.25f;
    [SerializeField] private float duracaoDoPulo = 0.25f;
    [SerializeField] private ParticleSystem[] faiscas;
    [Tooltip("Cor das faíscas assim que o drift começa (antes do nível 1)")]
    [SerializeField] private Color corInicialFaiscas = new Color(1f, 0.95f, 0.7f);
    [SerializeField] private Color[] coresDosNiveis =
    {
        new Color(0.2f, 0.6f, 1f),   // nível 1: azul
        new Color(1f, 0.55f, 0.1f),  // nível 2: laranja
        new Color(0.8f, 0.3f, 1f)    // nível 3: roxo
    };
    [Tooltip("Faíscas por segundo = base + (por nível x nível atual)")]
    [SerializeField] private float faiscasBase = 40f;
    [SerializeField] private float faiscasPorNivel = 60f;

    [Header("Eventos (use para som, câmera, efeitos...)")]
    public UnityEvent AoIniciarDrift;
    public UnityEvent<int> AoMudarNivel;
    public UnityEvent<int> AoIniciarTurbo;
    public UnityEvent AoTerminarDrift;

    // ---------- Estado público (somente leitura) ----------
    public bool EstaDerrapando { get; private set; }
    public int DirecaoDrift { get; private set; }   // -1 = esquerda, +1 = direita, 0 = nenhum
    public int NivelCarga { get; private set; }     // 0 = sem carga, 1..3
    public bool EmTurbo => tempoTurbo > 0f;
    public float MultiplicadorVelocidade => EmTurbo ? multiplicadorDoTurbo : 1f;

    // ---------- Estado interno ----------
    private float tempoDrift;
    private float tempoTurbo;
    private float tempoPulo = 999f;
    private bool segurandoAnterior;
    private bool armado;

    private float anguloAtual;
    private Vector3 posicaoInicial;
    private Quaternion rotacaoInicial = Quaternion.identity;

    private void Awake()
    {
        if (modeloVisual != null)
        {
            posicaoInicial = modeloVisual.localPosition;
            rotacaoInicial = modeloVisual.localRotation;
        }
    }

    private void Start()
    {
        // Garante que as faíscas começam paradas, mesmo com Play On Awake ligado.
        PararFaiscas();
    }

    /// <summary>
    /// Chame uma vez por FixedUpdate no seu CarController.
    /// Devolve o esterço que o carro deve usar de verdade.
    /// </summary>
    /// <param name="esterce">-1 (esquerda) a +1 (direita)</param>
    /// <param name="segurandoDrift">botão de drift está pressionado</param>
    /// <param name="velocidadeFrente">velocidade do kart para a frente (negativa se de ré)</param>
    /// <param name="noChao">kart está tocando o chão (raycasts acertando)</param>
    public float Processar(float esterce, bool segurandoDrift, float velocidadeFrente, bool noChao)
    {
        float dt = Time.deltaTime;

        if (tempoTurbo > 0f) tempoTurbo -= dt;

        // Aperta o botão (borda de subida) -> "arma" o drift se estiver rápido o bastante.
        if (segurandoDrift && !segurandoAnterior)
            armado = velocidadeFrente >= velocidadeMinima;
        if (!segurandoDrift)
            armado = false;
        segurandoAnterior = segurandoDrift;

        // Armado + esterçando + no chão -> começa a derrapar.
        if (!EstaDerrapando && armado && noChao && Mathf.Abs(esterce) >= esterceMinimo)
            IniciarDrift(esterce > 0f ? 1 : -1);

        if (!EstaDerrapando)
            return esterce;

        // Ficou lento demais: cancela sem turbo.
        if (velocidadeFrente < velocidadeMinima * 0.5f)
        {
            EncerrarDrift(false);
            return esterce;
        }

        // Soltou o botão: termina e entrega o turbo (se carregou).
        if (!segurandoDrift)
        {
            EncerrarDrift(true);
            return esterce;
        }

        // Carrega o drift (só conta enquanto estiver no chão).
        if (noChao)
        {
            tempoDrift += dt;
            AtualizarNivel();
        }

        // Remapeia o esterço: nunca inverte o lado, só abre ou fecha a curva.
        // t = 0 (esterçando pra fora), 0.5 (reto), 1 (esterçando pra dentro)
        float t = (esterce * DirecaoDrift + 1f) * 0.5f;
        return DirecaoDrift * Mathf.Lerp(esterceFora, esterceDentro, t);
    }

    // ---------- Lógica ----------

    private void IniciarDrift(int direcao)
    {
        EstaDerrapando = true;
        DirecaoDrift = direcao;
        tempoDrift = 0f;
        NivelCarga = 0;
        tempoPulo = 0f;
        IniciarFaiscas(); // faíscas já no começo do drift, antes do nível 1
        AoIniciarDrift?.Invoke();
    }

    private void AtualizarNivel()
    {
        int novoNivel = 0;
        for (int i = 0; i < temposDosNiveis.Length; i++)
        {
            if (tempoDrift >= temposDosNiveis[i]) novoNivel = i + 1;
        }

        if (novoNivel != NivelCarga)
        {
            NivelCarga = novoNivel;
            AtualizarFaiscas();
            AoMudarNivel?.Invoke(NivelCarga);
        }
    }

    private void EncerrarDrift(bool comTurbo)
    {
        if (comTurbo && NivelCarga > 0 && duracaoDoTurbo.Length > 0)
        {
            int indice = Mathf.Min(NivelCarga, duracaoDoTurbo.Length) - 1;
            tempoTurbo = duracaoDoTurbo[indice];
            AoIniciarTurbo?.Invoke(NivelCarga);
        }

        EstaDerrapando = false;
        DirecaoDrift = 0;
        NivelCarga = 0;
        tempoDrift = 0f;
        armado = false;

        PararFaiscas();
        AoTerminarDrift?.Invoke();
    }

    // ---------- Visual ----------

    private void LateUpdate()
    {
        if (modeloVisual == null) return;

        // Gira o modelo para o lado da curva (suavizado).
        float alvo = EstaDerrapando ? DirecaoDrift * anguloVisual : 0f;
        anguloAtual = Mathf.Lerp(anguloAtual, alvo, 1f - Mathf.Exp(-suavizacaoVisual * Time.deltaTime));

        // Pulinho no começo do drift.
        float altura = 0f;
        if (tempoPulo < duracaoDoPulo)
        {
            tempoPulo += Time.deltaTime;
            altura = Mathf.Sin(Mathf.Clamp01(tempoPulo / duracaoDoPulo) * Mathf.PI) * alturaDoPulo;
        }

        modeloVisual.localRotation = rotacaoInicial * Quaternion.Euler(0f, anguloAtual, 0f);
        modeloVisual.localPosition = posicaoInicial + Vector3.up * altura;
    }

    // Faíscas fracas e claras assim que o drift começa.
    private void IniciarFaiscas()
    {
        AplicarFaiscas(corInicialFaiscas, faiscasBase);
    }

    // A cada nível: muda a cor e solta mais faíscas.
    private void AtualizarFaiscas()
    {
        if (NivelCarga <= 0 || coresDosNiveis.Length == 0) return;

        Color cor = coresDosNiveis[Mathf.Clamp(NivelCarga, 1, coresDosNiveis.Length) - 1];
        AplicarFaiscas(cor, faiscasBase + faiscasPorNivel * NivelCarga);
    }

    private void AplicarFaiscas(Color cor, float taxa)
    {
        if (faiscas == null) return;

        foreach (ParticleSystem ps in faiscas)
        {
            if (ps == null) continue;

            var main = ps.main;
            main.startColor = cor;

            var emission = ps.emission;
            emission.rateOverTime = taxa;

            if (!ps.isPlaying) ps.Play();
        }
    }

    private void PararFaiscas()
    {
        if (faiscas == null) return;
        foreach (ParticleSystem ps in faiscas)
        {
            if (ps != null) ps.Stop();
        }
    }
}