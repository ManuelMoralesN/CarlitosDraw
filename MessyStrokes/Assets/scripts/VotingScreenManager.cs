using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class VotingScreenManager : MonoBehaviour
{
    [Header("UI for Voting")]
    public GameObject votingPanel;             // Panel de votación (se activa al finalizar la partida)
    public RectTransform player1Container;     // Container con el dibujo del Jugador 1
    public RectTransform player2Container;     // Container con el dibujo del Jugador 2
    public TextMeshProUGUI votingFeedbackText;   // Texto para mensajes y contador durante la votación
    public Button voteMaloButton;              // Botón "malo" (0 pts)
    public Button voteMehButton;               // Botón "meh" (1 pt)
    public Button voteWowButton;               // Botón "wow" (3 pts)

    [Header("Result UI")]
    public TextMeshProUGUI resultText;         // Texto final con el ganador y feedback
    public TextMeshProUGUI scoreTableText;     // Tabla de puntajes

    [Header("Reference Image")]
    public GameObject referenceImageVoting;    // Nueva sección para mostrar la imagen de referencia en votación

    [Header("Voting Duration")]
    public float votingTime = 30f;             // Tiempo para votar por cada dibujo (en segundos)

    // Variables para almacenar los votos
    private int player1Points = 0;
    private int player2Points = 0;

    public void ShowVotingScreen()
    {
        votingPanel.SetActive(true);
        // Activamos la imagen de referencia para votación (ubicada en su propio apartado)
        if (referenceImageVoting != null)
            referenceImageVoting.SetActive(true);

        // Aseguramos que al comenzar solo se vea el feedback de votación
        votingFeedbackText.gameObject.SetActive(true);
        resultText.gameObject.SetActive(false);
        scoreTableText.gameObject.SetActive(false);
        // Ocultamos ambos containers para activarlos de forma secuencial
        player1Container.gameObject.SetActive(false);
        player2Container.gameObject.SetActive(false);
        StartCoroutine(VotingSequence());
    }

    IEnumerator VotingSequence()
    {
        // Votación para Jugador 1
        player1Container.gameObject.SetActive(true);
        player2Container.gameObject.SetActive(false);
        yield return StartCoroutine(VoteForPlayer(1));
        player1Container.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);

        // Votación para Jugador 2
        player2Container.gameObject.SetActive(true);
        yield return StartCoroutine(VoteForPlayer(2));
        player2Container.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);

        // Mostrar resultados finales:
        // Ocultamos el texto de votación
        votingFeedbackText.gameObject.SetActive(false);

        string winnerMsg = "";
        if (player1Points > player2Points)
            winnerMsg = "¡Gana el Jugador 1!";
        else if (player2Points > player1Points)
            winnerMsg = "¡Gana el Jugador 2!";
        else
            winnerMsg = "¡Empate!";

        resultText.text = winnerMsg;
        scoreTableText.text = "Puntajes:\nJugador 1: " + player1Points + "\nJugador 2: " + player2Points;

        // Mostrar el dibujo ganador junto con la imagen de referencia para comparar.
        if (player1Points > player2Points)
        {
            player1Container.gameObject.SetActive(true);
            player2Container.gameObject.SetActive(false);
        }
        else if (player2Points > player1Points)
        {
            player2Container.gameObject.SetActive(true);
            player1Container.gameObject.SetActive(false);
        }
        else
        {
            // En empate, se muestran ambos en una disposición horizontal (puedes ajustar su posición en el Canvas)
            player1Container.gameObject.SetActive(true);
            player2Container.gameObject.SetActive(true);
        }

        resultText.gameObject.SetActive(true);
        scoreTableText.gameObject.SetActive(true);

        // La imagen de referencia permanece activa para facilitar la comparación.
        yield return new WaitForSeconds(10f);
        votingPanel.SetActive(false);
    }

    IEnumerator VoteForPlayer(int playerNumber)
    {
        int vote = -1;
        bool voteReceived = false;

        // Configurar listeners para los botones
        voteMaloButton.onClick.AddListener(() => { vote = 0; voteReceived = true; });
        voteMehButton.onClick.AddListener(() => { vote = 1; voteReceived = true; });
        voteWowButton.onClick.AddListener(() => { vote = 3; voteReceived = true; });

        float timeLeft = votingTime;
        while (timeLeft > 0f && !voteReceived)
        {
            votingFeedbackText.text = "Vota el dibujo del Jugador " + playerNumber + ": " + Mathf.Ceil(timeLeft).ToString() + " s restantes";
            timeLeft -= Time.deltaTime;
            yield return null;
        }

        // Quitar los listeners para evitar duplicados
        voteMaloButton.onClick.RemoveAllListeners();
        voteMehButton.onClick.RemoveAllListeners();
        voteWowButton.onClick.RemoveAllListeners();

        if (!voteReceived)
        {
            vote = 0; // Voto por defecto si no se votó
        }

        if (playerNumber == 1)
            player1Points = vote;
        else if (playerNumber == 2)
            player2Points = vote;

        yield break;
    }
}
