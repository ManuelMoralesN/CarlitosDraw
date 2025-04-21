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

    [Header("Votacion")]
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
            feedbackText.text = $"Ronda {round}/{totalRounds}";
            timerText.text = "";
            yield return new WaitForSeconds(1f);

            // Fase de Referencia
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

            // Fase de Dibujo
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

            // Guardar los trazos de esta ronda en ambos contenedores
            drawingController.enabled = false;
            ReparentChildren(drawingArea, myContainer); // ¡Aquí se hace la magia!
            drawingCoverPanel.SetActive(false);

            feedbackText.text = $"Fin de ronda {round}";
            timerText.text = "";
            yield return new WaitForSeconds(2f);
        }

        // Al terminar todas las rondas, iniciar votación
        if (PhotonNetwork.IsMasterClient)
            photonView.RPC(nameof(RPC_StartVoting), RpcTarget.All);
    }

    [PunRPC]
    void RPC_StartVoting()
    {
        votingScreenManager.ShowVotingScreen();
    }

    // 👇 NUEVA VERSIÓN: Reparte trazos según dueño
    void ReparentChildren(RectTransform from, RectTransform fallbackContainer)
    {
        for (int i = from.childCount - 1; i >= 0; i--)
        {
            Transform child = from.GetChild(i);
            PhotonView pv = child.GetComponent<PhotonView>();

            if (pv != null && pv.Owner != null)
            {
                int actor = pv.Owner.ActorNumber;

                if (actor == 1)
                    child.SetParent(player1Container, false);
                else if (actor == 2)
                    child.SetParent(player2Container, false);
                else
                    child.SetParent(fallbackContainer, false);
            }
            else
            {
                child.SetParent(fallbackContainer, false);
            }
        }
    }
}
