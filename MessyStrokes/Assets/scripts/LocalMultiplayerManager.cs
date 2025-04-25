using System.Collections;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// Gestiona las rondas de dibujo y orquesta la votación final de manera sincronizada.
/// Solo el MasterClient controla el flujo completo.
/// </summary>
public class LocalMultiplayerManager : MonoBehaviourPunCallbacks
{
    [Header("Duraciones (segundos)")]
    public float viewDuration = 30f;   // Tiempo para ver la referencia en cada ronda
    public float drawDuration = 30f;   // Tiempo para dibujar en cada ronda
    public int totalRounds = 3;        // Número de rondas antes de votar

    [Header("Referencias UI")]
    public TextMeshProUGUI feedbackText;    // Mensajes de feedback y rondas
    public TextMeshProUGUI timerText;       // Contador de tiempo
    public GameObject referenceImage;       // Imagen de referencia a dibujar
    public GameObject drawingCoverPanel;    // Panel que cubre la UI durante dibujo

    [Header("Dibujo")]
    public GameManager drawingController;   // Controlador de entrada de dibujo
    public RectTransform drawingArea;       // Área del Canvas donde se dibuja

    [Header("Contenedores de votación")]
    public RectTransform player1Container;  // Guarda trazos del Jugador 1
    public RectTransform player2Container;  // Guarda trazos del Jugador 2

    [Header("Gestor de votación")]
    public VotingScreenManager votingScreenManager; // Maneja la UI de votación (debe tener PhotonView)

    void Start()
    {
        // Solo el MasterClient inicia el flujo de juego completo
        if (PhotonNetwork.IsMasterClient)
            photonView.RPC(nameof(RPC_StartGame), RpcTarget.All);
    }

    [PunRPC]
    void RPC_StartGame()
    {
        StartCoroutine(GameFlow());
    }

    /// <summary>
    /// Ejecuta las rondas de referencia y dibujo.
    /// </summary>
    IEnumerator GameFlow()
    {
        for (int round = 1; round <= totalRounds; round++)
        {
            // 1) Mostrar marcador de ronda
            feedbackText.text = $"Ronda {round}/{totalRounds}";
            timerText.text = "";
            yield return new WaitForSeconds(1f);

            // 2) Fase de referencia
            referenceImage.SetActive(true);
            drawingCoverPanel.SetActive(false);
            drawingController.enabled = false;
            float t = viewDuration;
            while (t > 0f)
            {
                feedbackText.text = $"Mira la referencia ({Mathf.Ceil(t)}s)";
                timerText.text = $"{Mathf.Ceil(t)} s";
                t -= Time.deltaTime;
                yield return null;
            }
            referenceImage.SetActive(false);

            // 3) Fase de dibujo
            drawingCoverPanel.SetActive(true);
            drawingController.enabled = true;
            t = drawDuration;
            while (t > 0f)
            {
                feedbackText.text = $"Dibuja! ({Mathf.Ceil(t)}s)";
                timerText.text = $"{Mathf.Ceil(t)} s";
                t -= Time.deltaTime;
                yield return null;
            }
            drawingController.enabled = false;
            drawingCoverPanel.SetActive(false);

            // 4) Pausa antes de la siguiente ronda
            feedbackText.text = "Fin de ronda";
            timerText.text = "";
            yield return new WaitForSeconds(2f);
        }

        // Al terminar todas las rondas, orquestar votación (solo Master)
        if (PhotonNetwork.IsMasterClient)
            StartCoroutine(VotingOrchestrator());
    }

    /// <summary>
    /// Controla la votación por RPCs para que todos vean el mismo contenedor.
    /// </summary>
    IEnumerator VotingOrchestrator()
    {
        // 1) Reparentar todos los trazos a los contenedores adecuados
        for (int i = drawingArea.childCount - 1; i >= 0; i--)
        {
            Transform stroke = drawingArea.GetChild(i);
            var pv = stroke.GetComponent<PhotonView>();
            if (pv != null && pv.Owner != null && pv.Owner.ActorNumber == 2)
                stroke.SetParent(player2Container, false);
            else
                stroke.SetParent(player1Container, false);
        }

        // 2) Votación Jugador 1
        votingScreenManager.photonView.RPC(
            nameof(votingScreenManager.RPC_ShowVoteContainer),
            RpcTarget.All, 1);
        yield return new WaitForSeconds(votingScreenManager.votingTime + 0.5f);

        // 3) Votación Jugador 2
        votingScreenManager.photonView.RPC(
            nameof(votingScreenManager.RPC_ShowVoteContainer),
            RpcTarget.All, 2);
        yield return new WaitForSeconds(votingScreenManager.votingTime + 0.5f);

        // 4) Mostrar resultado final
        votingScreenManager.photonView.RPC(
            nameof(votingScreenManager.RPC_ShowVoteResult),
            RpcTarget.All,
            votingScreenManager.player1Points,
            votingScreenManager.player2Points);
    }
}

