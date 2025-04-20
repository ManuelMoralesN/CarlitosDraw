using System.Collections;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class LocalMultiplayerManager : MonoBehaviourPunCallbacks
{
    [Header("Duraciones (segundos)")]
    public float viewDuration = 30f;
    public float drawDuration = 30f;
    public int totalRounds = 3;

    [Header("UI")]
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI timerText;
    public GameObject referenceImage;
    public GameObject drawingCoverPanel;

    [Header("Dibujo")]
    public GameManager drawingController;
    public RectTransform drawingArea;

    [Header("Contenedores")]
    public RectTransform player1Container;
    public RectTransform player2Container;

    [Header("Votación")]
    public VotingScreenManager votingScreenManager;

    private RectTransform myContainer;

    void Start()
    {
        // Determinar el contenedor propio según ActorNumber
        int actor = PhotonNetwork.LocalPlayer.ActorNumber;
        myContainer = (actor == 1) ? player1Container : player2Container;
        player1Container.gameObject.SetActive(false);
        player2Container.gameObject.SetActive(false);

        // Solo el MasterClient inicia la partida en todos
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
            // Mostrar número de ronda
            feedbackText.text = $"Ronda {round}/{totalRounds}";
            timerText.text = "";
            yield return new WaitForSeconds(1f);

            // --- Fase de Referencia ---
            referenceImage.SetActive(true);
            drawingCoverPanel.SetActive(false);
            drawingController.enabled = false;
            myContainer.gameObject.SetActive(true);

            float t = viewDuration;
            while (t > 0f)
            {
                feedbackText.text = $"Ronda {round}: Mira la referencia ({Mathf.Ceil(t)}s)";
                timerText.text = $"{Mathf.Ceil(t)} s";
                t -= Time.deltaTime;
                yield return null;
            }
            referenceImage.SetActive(false);

            // --- Fase de Dibujo ---
            drawingCoverPanel.SetActive(true);
            drawingController.enabled = true;

            t = drawDuration;
            while (t > 0f)
            {
                feedbackText.text = $"Ronda {round}: Dibuja! ({Mathf.Ceil(t)}s)";
                timerText.text = $"{Mathf.Ceil(t)} s";
                t -= Time.deltaTime;
                yield return null;
            }

            // Guardar los trazos de esta ronda
            drawingController.enabled = false;
            ReparentChildren(drawingArea, myContainer);
            drawingCoverPanel.SetActive(false);

            // Pausa antes de siguiente ronda
            feedbackText.text = $"Fin de ronda {round}";
            timerText.text = "";
            yield return new WaitForSeconds(2f);
        }

        // Iniciar votación tras todas las rondas
        if (PhotonNetwork.IsMasterClient)
            photonView.RPC(nameof(RPC_StartVoting), RpcTarget.All);
    }

    [PunRPC]
    void RPC_StartVoting()
    {
        votingScreenManager.ShowVotingScreen();
    }

    void ReparentChildren(RectTransform from, RectTransform to)
    {
        for (int i = from.childCount - 1; i >= 0; i--)
            from.GetChild(i).SetParent(to, false);
    }
}
