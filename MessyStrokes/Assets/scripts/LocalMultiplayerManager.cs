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

    [HideInInspector] public int player1Points = 0;
    [HideInInspector] public int player2Points = 0;

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
        // 1) Reparent en todos los clientes usando los containers del VotingScreenManager
        photonView.RPC(nameof(RPC_ReparentLines), RpcTarget.AllBuffered);
        yield return new WaitForSeconds(0.5f);

        // 2) Votación Jugador 1
        photonView.RPC(nameof(RPC_ShowVoteContainer), RpcTarget.All, 1);
        yield return new WaitForSeconds(votingScreenManager.votingTime + 0.5f);

        // 3) Votación Jugador 2
        photonView.RPC(nameof(RPC_ShowVoteContainer), RpcTarget.All, 2);
        yield return new WaitForSeconds(votingScreenManager.votingTime + 0.5f);

        // 4) Mostrar resultado
        photonView.RPC(nameof(RPC_ShowVoteResult), RpcTarget.All,
            player1Points, player2Points);
    }
    [PunRPC]
    void RPC_ReparentLines()
    {
        for (int i = drawingArea.childCount - 1; i >= 0; i--)
        {
            Transform stroke = drawingArea.GetChild(i);
            var pv = stroke.GetComponent<PhotonView>();

            // Mover al container correcto del VotingScreenManager
            if (pv != null && pv.Owner != null && pv.Owner.ActorNumber == 2)
                stroke.SetParent(votingScreenManager.player2Container, false);
            else
                stroke.SetParent(votingScreenManager.player1Container, false);

            // ¡Y vuelve a mostrarlos!
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
        if (owner == 1) player1Points = voteValue;
        else player2Points = voteValue;
    }

    [PunRPC]
    void RPC_ShowVoteResult(int p1, int p2)
    {
        votingScreenManager.ShowVoteResult(p1, p2);
    }
}
