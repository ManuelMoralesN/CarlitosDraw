using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField roomNameInput;
    public TMP_InputField joinRoomInput;
    public PantallaManager pantallaManager;
    public TextMeshProUGUI errorMessage;

    // Imágenes para mostrar en el lobby
    public Image hostImage;   // Imagen del jugador host
    public Image guestImage;  // Imagen del jugador invitado

    public Sprite hostSprite; // Asignar imagen del host
    public Sprite guestSprite; // Asignar imagen del invitado

    public static NetworkManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Conectado a Photon Master");
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void CreateRoom()
    {
        string name = roomNameInput.text;
        if (string.IsNullOrEmpty(name)) return;
        PhotonNetwork.CreateRoom(
            name,
            new RoomOptions { MaxPlayers = 4 },
            TypedLobby.Default
        );
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Entró a la sala: " + PhotonNetwork.CurrentRoom.Name);
        Debug.Log(
            $"Jugadores en sala: {PhotonNetwork.CurrentRoom.PlayerCount}/" +
            $"{PhotonNetwork.CurrentRoom.MaxPlayers}"
        );
        pantallaManager.IrALobby();

        // Muestra la imagen del host
        hostImage.gameObject.SetActive(true);
        
        // Si ya hay 2 jugadores, muestra la imagen del invitado
        if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            guestImage.gameObject.SetActive(true);
            guestImage.sprite = guestSprite; // Asigna la imagen del invitado
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"Crear sala falló: {message}");
    }

    public void JoinRoom()
    {
        string name = joinRoomInput.text;
        if (string.IsNullOrEmpty(name)) return;
        PhotonNetwork.JoinRoom(name);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"Falló unirse: {message}");

        if (errorMessage != null)
        {
            errorMessage.gameObject.SetActive(true);
            errorMessage.text = "No se pudo unir: " + message;
            Invoke(nameof(HideErrorMessage), 3f); // Ocultar después de 3 segundos
        }
    }

    private void HideErrorMessage()
    {
        if (errorMessage != null)
        {
            errorMessage.gameObject.SetActive(false);
            errorMessage.text = "";
        }
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("LocalGame");
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(
            $"{newPlayer.NickName} se unió. " +
            $"Jugadores ahora: {PhotonNetwork.CurrentRoom.PlayerCount}/" +
            $"{PhotonNetwork.CurrentRoom.MaxPlayers}"
        );

        // Si es el invitado, muestra su imagen
        if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            guestImage.gameObject.SetActive(true);
            guestImage.sprite = guestSprite; // Asigna la imagen del invitado
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log(
            $"{otherPlayer.NickName} salió. " +
            $"Jugadores ahora: {PhotonNetwork.CurrentRoom.PlayerCount}/" +
            $"{PhotonNetwork.CurrentRoom.MaxPlayers}"
        );

        // Cuando el jugador se va, ocultamos la imagen del invitado
        guestImage.gameObject.SetActive(false);
    }
}
