using System.Collections;
using UnityEngine;
using TMPro;

public class LocalMultiplayerManager : MonoBehaviour
{
    [Header("Duraciones (segundos)")]
    public float viewDuration = 30f;       // Tiempo para ver la imagen y el container
    public float drawDuration = 30f;       // Tiempo para dibujar
    public float turnEndDuration = 2f;     // Tiempo de mensaje de fin de turno
    public float playerTime = 180f;        // Tiempo asignado por jugador (p.ej., 3 minutos)

    [Header("Referencias UI")]
    public TextMeshProUGUI feedbackText;   // Texto para mensajes de feedback
    public TextMeshProUGUI timerText;      // Texto para mostrar el contador principal
    public GameObject referenceImage;      // Imagen de referencia (se muestra en la fase de vista)
    public GameObject drawingCoverPanel;   // Panel que cubre la pantalla durante la fase de dibujo

    [Header("Control de Dibujo")]
    public GameManager drawingController;   // Componente que habilita la entrada para dibujar
    public RectTransform drawingArea;               // Área de dibujo (contenedor temporal de los trazos)

    [Header("Contenedores de Progreso")]
    public RectTransform player1Container; // Container para guardar los trazos del jugador 1
    public RectTransform player2Container; // Container para guardar los trazos del jugador 2

    [Header("Votación")]
    public VotingScreenManager votingScreenManager;  // Referencia al gestor de votación

    private int currentPlayer = 1;
    private float elapsedPlayer1 = 0f;
    private float elapsedPlayer2 = 0f;

    void Start()
    {
        StartCoroutine(RunGame());
    }

    IEnumerator RunGame()
    {
        // Se repite mientras al menos uno de los jugadores tenga tiempo restante.
        while (elapsedPlayer1 < playerTime || elapsedPlayer2 < playerTime)
        {
            if (currentPlayer == 1)
            {
                if (elapsedPlayer1 < playerTime)
                {
                    yield return StartCoroutine(RunTurn(1, player1Container));
                }
                currentPlayer = 2;
            }
            else // currentPlayer == 2
            {
                if (elapsedPlayer2 < playerTime)
                {
                    yield return StartCoroutine(RunTurn(2, player2Container));
                }
                currentPlayer = 1;
            }
        }

        // Fin de la partida: se muestran los dibujos para votar.
        feedbackText.text = "¡Partida terminada!";
        timerText.text = "";
        referenceImage.SetActive(true);
        drawingCoverPanel.SetActive(false);

        // Llama al gestor de votación.
        votingScreenManager.ShowVotingScreen();
    }

    IEnumerator RunTurn(int playerNumber, RectTransform playerContainer)
    {
        // --- Fase de Vista ---
        // Mostrar la imagen de referencia y el container del jugador (para que vea lo que ha pintado)
        playerContainer.gameObject.SetActive(true);
        referenceImage.SetActive(true);
        drawingCoverPanel.SetActive(false);
        drawingController.enabled = false;

        float viewTimeLeft = viewDuration;
        while (viewTimeLeft > 0f)
        {
            feedbackText.text = "Jugador " + playerNumber + ": Tienes " + Mathf.Ceil(viewTimeLeft).ToString() + " s para ver la imagen y tus trazos.";
            timerText.text = Mathf.Ceil(viewTimeLeft).ToString() + " s";
            viewTimeLeft -= Time.deltaTime;
            yield return null;
        }
        if (playerNumber == 1)
            elapsedPlayer1 += viewDuration;
        else
            elapsedPlayer2 += viewDuration;

        // Ocultar la imagen y el container al terminar la fase de vista.
        referenceImage.SetActive(false);
        playerContainer.gameObject.SetActive(false);

        // --- Fase de Dibujo ---
        // Se activa el panel que cubre la pantalla y se habilita el dibujo.
        drawingCoverPanel.SetActive(true);
        drawingController.enabled = true;

        float drawTimeLeft = drawDuration;
        while (drawTimeLeft > 0f)
        {
            feedbackText.text = "Jugador " + playerNumber + " Dibuja!";
            timerText.text = Mathf.Ceil(drawTimeLeft).ToString() + " s";
            drawTimeLeft -= Time.deltaTime;
            yield return null;
        }
        if (playerNumber == 1)
            elapsedPlayer1 += drawDuration;
        else
            elapsedPlayer2 += drawDuration;

        // Finaliza el turno: deshabilita el dibujo, guarda los trazos en el container correspondiente y oculta el container.
        drawingController.enabled = false;
        ReparentChildren(drawingArea, playerContainer);
        playerContainer.gameObject.SetActive(false);

        feedbackText.text = "Turno finalizado para Jugador " + playerNumber;
        yield return new WaitForSeconds(turnEndDuration);
        if (playerNumber == 1)
            elapsedPlayer1 += turnEndDuration;
        else
            elapsedPlayer2 += turnEndDuration;
    }

    // Mueve todos los hijos (trazos) de 'drawingArea' al container 'to'
    void ReparentChildren(RectTransform from, RectTransform to)
    {
        int count = from.childCount;
        for (int i = count - 1; i >= 0; i--)
        {
            Transform child = from.GetChild(i);
            child.SetParent(to, false);
        }
    }
}
