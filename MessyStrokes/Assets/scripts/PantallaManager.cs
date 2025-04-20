using UnityEngine;
using UnityEngine.SceneManagement;

public class PantallaManager : MonoBehaviour
{
    public GameObject panelMenu;
    public GameObject panelUnirse;
    public GameObject panelCrear;
    public GameObject panelLobby;
    public GameObject panelTienda;
    public GameObject panelGameplay;

    public AudioSource musicAudioSource;

    void Start()
    {
        MostrarSolo(panelMenu); // Mostrar solo el menú al inicio
    }

    public void MostrarSolo(GameObject panel)
    {
        panelMenu.SetActive(false);
        panelUnirse.SetActive(false);
        panelCrear.SetActive(false);
        panelLobby.SetActive(false);

        panel.SetActive(true);
    }

    // Métodos para los botones
    public void IrACrearSala() => MostrarSolo(panelCrear);
    public void IrAUnirseSala() => MostrarSolo(panelUnirse);
    public void IrALobby() => MostrarSolo(panelLobby);
    public void IrAMenu() => MostrarSolo(panelMenu);
    public void IrATienda() => MostrarSolo(panelTienda);
    public void IrCanvaGameplay() => MostrarSolo(panelGameplay);
    public void IrAlGameplay() => SceneManager.LoadScene("LocalGame");

    public void SalirApp() => Application.Quit();

    // Método para alternar el silencio de la música
    public void ToggleMusicMute()
    {
        if (musicAudioSource != null)
        {
            musicAudioSource.mute = !musicAudioSource.mute;
        }
        else
        {
            Debug.LogWarning("AudioSource de música no asignado en PantallaManager.");
        }
    }
}
