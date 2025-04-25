using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(UILineRenderer))]
public class LineSync : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    private UILineRenderer line;

    void Awake()
    {
        line = GetComponent<UILineRenderer>();
    }

    // Se dispara en CADA cliente cuando PhotonNetwork.Instantiate crea la línea.
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        // 1) Parent bajo el canvas de dibujo local
        if (GameManager.Instance != null && GameManager.Instance.drawingArea != null)
            transform.SetParent(GameManager.Instance.drawingArea, false);

        // 2) Inicializa puntos
        line.Points = new List<Vector2>();
        line.SetVerticesDirty();

        // 3) Oculta los trazos remotos durante la fase de dibujo
        if (photonView.Owner != PhotonNetwork.LocalPlayer)
            gameObject.SetActive(false);
    }

    [PunRPC]
    public void RPC_AddPoint(Vector2 point)
    {
        line.Points.Add(point);
        line.SetVerticesDirty();
    }
}
