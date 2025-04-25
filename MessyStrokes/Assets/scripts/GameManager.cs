using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPun
{
    public static GameManager Instance;

    [Header("Configuración de Dibujo")]
    public RectTransform drawingArea;
    public GameObject uiLinePrefab;
    public float minDistance = 5f;

    private UILineRenderer currentLine;
    private List<Vector2> currentPoints = new List<Vector2>();

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        ProcessDrawing();
    }

    void ProcessDrawing()
    {
        if (!RectTransformUtility.RectangleContainsScreenPoint(drawingArea, Input.mousePosition, null))
        {
            currentLine = null;
            currentPoints.Clear();
            return;
        }

        if (Input.GetMouseButtonDown(0))
            StartNewLine();

        if (Input.GetMouseButton(0) && currentLine != null)
        {
            Vector2 screenPos = Input.mousePosition;
            if (currentPoints.Count == 0 ||
                Vector2.Distance(currentPoints[currentPoints.Count - 1], screenPos) >= minDistance)
            {
                currentPoints.Add(screenPos);

                // Convertir a coordenadas locales y añadir punto
                Vector2 localPoint;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    drawingArea, screenPos, null, out localPoint);
                currentLine.Points.Add(localPoint);
                currentLine.SetVerticesDirty();

                // Propagar al resto de clientes
                currentLine.GetComponent<PhotonView>()
                           .RPC("RPC_AddPoint",
                                RpcTarget.OthersBuffered,
                                localPoint);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            currentLine = null;
            currentPoints.Clear();
        }
    }

    void StartNewLine()
    {
        // Instancia la línea en todos los clientes
        GameObject lineObj = PhotonNetwork.Instantiate(
            uiLinePrefab.name,
            Vector3.zero,
            Quaternion.identity);

        // No hace falta SetParent aquí, lo hará OnPhotonInstantiate()
        currentLine = lineObj.GetComponent<UILineRenderer>();
        currentPoints.Clear();
    }
}
