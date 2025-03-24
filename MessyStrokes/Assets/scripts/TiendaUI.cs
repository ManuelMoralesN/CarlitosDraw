using UnityEngine;

public class TiendaUI : MonoBehaviour
{
    public GameObject panelTienda;

    private bool abierta = false;

    public void ToggleTienda()
    {
        abierta = !abierta;
        panelTienda.SetActive(abierta);
    }
}
