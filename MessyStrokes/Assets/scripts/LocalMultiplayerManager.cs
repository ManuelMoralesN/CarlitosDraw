// LocalMultiplayerManager.cs
using System.Collections;
using UnityEngine;
using TMPro;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class LocalMultiplayerManager : MonoBehaviourPunCallbacks
{
    public static LocalMultiplayerManager Instance;

    [Header("Duraciones (segundos)")]
    public float viewDuration = 30f;
    public float drawDuration = 30f;
    public int totalRounds = 3;

    [Header("Referencias UI")]
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI timerText;
    public GameObject referenceImage;
    public GameObject drawingCoverPanel;

    [Header("Dibujo")]
    public GameManager drawingController;
    public RectTransform drawingArea;

    [Header("Gestor de votación")]
    public VotingScreenManager votingScreenManager;

    // Contadores de puntos acumulados
    private int totalVotesPlayer1;
    private int totalVotesPlayer2;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
            photonView.RPC(nameof(RPC_StartGame), RpcTarget.All);
    }

    [PunRPC]
    void RPC_StartGame()
    {
        StartCoroutine(GameFlow());
    }

    IEnumerator GameFlow()
    {
        for (int round = 1; round <= totalRounds; round++)
        {
            feedbackText.text = $"Ronda {round}/{totalRounds}";
            timerText.text = "";
            yield return new WaitForSeconds(1f);

            // Fase de referencia
            referenceImage.SetActive(true);
            drawingCoverPanel.SetActive(false);
            drawingController.enabled = false;
            yield return Countdown(viewDuration, "Mira la referencia");
            referenceImage.SetActive(false);

            // Fase de dibujo
            drawingCoverPanel.SetActive(true);
            drawingController.enabled = true;
            yield return Countdown(drawDuration, "Dibuja!");
            drawingController.enabled = false;
            drawingCoverPanel.SetActive(false);

            feedbackText.text = "Fin de ronda";
            timerText.text = "";
            yield return new WaitForSeconds(2f);
        }

        if (PhotonNetwork.IsMasterClient)
            StartCoroutine(VotingOrchestrator());
    }

    IEnumerator Countdown(float duration, string prefix)
    {
        float t = duration;
        while (t > 0f)
        {
            feedbackText.text = $"{prefix} ({Mathf.Ceil(t)}s)";
            timerText.text = $"{Mathf.Ceil(t)} s";
            t -= Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator VotingOrchestrator()
    {
        // 1) Reiniciar contadores
        totalVotesPlayer1 = 0;
        totalVotesPlayer2 = 0;

        // 2) Reparent y mostrar trazos en votación
        photonView.RPC(nameof(RPC_ReparentLines), RpcTarget.AllBuffered);
        yield return new WaitForSeconds(0.5f);

        // 3) Votación Jugador 1
        photonView.RPC(nameof(RPC_ShowVoteContainer), RpcTarget.All, 1);
        yield return new WaitForSeconds(votingScreenManager.votingTime + 0.5f);

        // 4) Votación Jugador 2
        photonView.RPC(nameof(RPC_ShowVoteContainer), RpcTarget.All, 2);
        yield return new WaitForSeconds(votingScreenManager.votingTime + 0.5f);

        // 5) Mostrar resultado final con los totales acumulados
        photonView.RPC(
            nameof(RPC_ShowVoteResult),
            RpcTarget.All,
            totalVotesPlayer1,
            totalVotesPlayer2
        );
    }

    [PunRPC]
    void RPC_ReparentLines()
    {
        for (int i = drawingArea.childCount - 1; i >= 0; i--)
        {
            Transform stroke = drawingArea.GetChild(i);
            var pv = stroke.GetComponent<PhotonView>();
            if (pv != null && pv.Owner != null && pv.Owner.ActorNumber == 2)
                stroke.SetParent(votingScreenManager.player2Container, false);
            else
                stroke.SetParent(votingScreenManager.player1Container, false);

            stroke.gameObject.SetActive(true);
        }
    }

    [PunRPC]
    void RPC_ShowVoteContainer(int owner)
    {
        votingScreenManager.ShowVoteContainer(owner);
    }

    [PunRPC]
    public void RPC_SubmitVote(int owner, int voteValue, PhotonMessageInfo info)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        // Acumula en lugar de sobrescribir
        if (owner == 1)
            totalVotesPlayer1 += voteValue;
        else
            totalVotesPlayer2 += voteValue;
    }

    [PunRPC]
    void RPC_ShowVoteResult(int p1Total, int p2Total)
    {
        votingScreenManager.ShowVoteResult(p1Total, p2Total);
    }
}
