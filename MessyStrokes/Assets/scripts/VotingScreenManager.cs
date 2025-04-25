using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

/// <summary>
/// Maneja la UI de votación: muestra dibujos y recoge votos por turnos.
/// Solo permite votar al jugador contrario al dueño del dibujo.
/// </summary>
public class VotingScreenManager : MonoBehaviour
{
    [Header("UI de votación")]
    public GameObject votingPanel;
    public RectTransform player1Container;
    public RectTransform player2Container;
    public TextMeshProUGUI votingFeedbackText;
    public Button voteMaloButton, voteMehButton, voteWowButton;

    [Header("UI de resultados")]
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI scoreTableText;

    [Header("Imagen Referencia")]
    public GameObject referenceImageVoting;

    [Header("Configuración")]
    public float votingTime = 30f;

    /// <summary>
    /// Muestra el contenedor de votación para el jugador indicado en todos los clientes.
    /// </summary>
    public void ShowVoteContainer(int owner)
    {
        votingPanel.SetActive(true);
        referenceImageVoting?.SetActive(true);

        player1Container.gameObject.SetActive(owner == 1);
        player2Container.gameObject.SetActive(owner == 2);

        bool canVote = PhotonNetwork.LocalPlayer.ActorNumber != owner;
        voteMaloButton.gameObject.SetActive(canVote);
        voteMehButton.gameObject.SetActive(canVote);
        voteWowButton.gameObject.SetActive(canVote);

        StartCoroutine(VoteFor(owner));
    }

    /// <summary>
    /// Rutina de votación local, envía voto al MasterClient.
    /// </summary>
    IEnumerator VoteFor(int owner)
    {
        votingFeedbackText.gameObject.SetActive(true);
        resultText.gameObject.SetActive(false);
        scoreTableText.gameObject.SetActive(false);

        int vote = 0;
        bool received = false;
        voteMaloButton.onClick.AddListener(() => { vote = 0; received = true; });
        voteMehButton.onClick.AddListener(() => { vote = 1; received = true; });
        voteWowButton.onClick.AddListener(() => { vote = 3; received = true; });

        float t = votingTime;
        while (t > 0f && !received)
        {
            votingFeedbackText.text = $"Vota dibujo Jugador {owner}: {Mathf.Ceil(t)}s";
            t -= Time.deltaTime;
            yield return null;
        }

        // Enviar voto al MasterClient a través del PhotonView de LocalMultiplayerManager
        LocalMultiplayerManager.Instance.photonView.RPC(
            nameof(LocalMultiplayerManager.RPC_SubmitVote),
            RpcTarget.MasterClient, owner, vote);

        voteMaloButton.onClick.RemoveAllListeners();
        voteMehButton.onClick.RemoveAllListeners();
        voteWowButton.onClick.RemoveAllListeners();
        votingFeedbackText.gameObject.SetActive(false);
        votingPanel.SetActive(false);
    }

    /// <summary>
    /// Muestra el resultado final y el dibujo ganador.
    /// </summary>
    public void ShowVoteResult(int p1, int p2)
    {
        votingPanel.SetActive(true);
        referenceImageVoting?.SetActive(true);

        bool p1Win = p1 > p2;
        bool p2Win = p2 > p1;
        resultText.text = p1Win ? "¡Gana Jugador 1!"
                         : p2Win ? "¡Gana Jugador 2!"
                         : "¡Empate!";

        player1Container.gameObject.SetActive(p1Win || (!p1Win && !p2Win));
        player2Container.gameObject.SetActive(p2Win || (!p1Win && !p2Win));

        scoreTableText.text = $"Jugador 1: {p1}\nJugador 2: {p2}";
        resultText.gameObject.SetActive(true);
        scoreTableText.gameObject.SetActive(true);

        Debug.Log("Votación finalizada.");
    }
}
