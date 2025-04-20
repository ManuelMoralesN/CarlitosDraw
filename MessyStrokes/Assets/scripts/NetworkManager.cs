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

    public static NetworkManager Instance;


    void Awake()
    {
        // Solo un NetworkManager activo
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
        // Inicia la conexi�n usando los ajustes que configuraste en el PUN Wizard
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(" Conectado a Photon Master");
        // Esto permite que cuando llames a PhotonNetwork.LoadLevel,
        // todos los clientes carguen la misma escena autom�ticamente:
        PhotonNetwork.AutomaticallySyncScene = true;
        // Aqu� podr�as habilitar tus botones de crear/unirse, si antes estaban desactivados.
    }

    public void CreateRoom()
    {
        string name = roomNameInput.text;
        if (string.IsNullOrEmpty(name)) return;
        PhotonNetwork.CreateRoom(
            name,
            new RoomOptions { MaxPlayers = 4 }, // l�mite de jugadores
            TypedLobby.Default
        );
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Entr� a la sala: " + PhotonNetwork.CurrentRoom.Name);
        Debug.Log(
            $"Jugadores en sala: " +
            $"{PhotonNetwork.CurrentRoom.PlayerCount}/" +
            $"{PhotonNetwork.CurrentRoom.MaxPlayers}"
        );
        pantallaManager.IrALobby();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"Crear sala fall�: {message}");
        // Aqu� puedes mostrar un mensaje de error al jugador
    }

    // Llamado por el bot�n �Unirse�
    public void JoinRoom()
    {
        string name = joinRoomInput.text;
        if (string.IsNullOrEmpty(name)) return;
        PhotonNetwork.JoinRoom(name);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"Fall� unirse: {message}");
        // Puedes avisar �Sala no existe� o �Est� llena�
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Carga la escena �LocalGame� en todos los jugadores
            PhotonNetwork.LoadLevel("LocalGame");
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(
            $" {newPlayer.NickName} se uni�. " +
            $"Jugadores ahora: {PhotonNetwork.CurrentRoom.PlayerCount}/" +
            $"{PhotonNetwork.CurrentRoom.MaxPlayers}"
        );
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log(
            $" {otherPlayer.NickName} sali�. " +
            $"Jugadores ahora: {PhotonNetwork.CurrentRoom.PlayerCount}/" +
            $"{PhotonNetwork.CurrentRoom.MaxPlayers}"
        );
    }
}
