using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;  // Necesario para PhotonNetwork

public class GameManager : MonoBehaviourPun
{
    [Header("Configuración de Dibujo")]
    public RectTransform drawingArea;      // Área de dibujo UI (por ejemplo, una Image de fondo)
    public GameObject LinesPrefab;        // Prefab (en Resources/) con UILineRenderer + PhotonView
    public float minDistance = 5f;         // Distancia mínima (en pixeles) para agregar un nuevo punto

    private UILineRenderer currentLine;    // La stroke actual
    private List<Vector2> currentPoints = new List<Vector2>();

    void Update()
    {
        ProcessDrawing();
    }

    void ProcessDrawing()
    {
        // Procesa la entrada solo si el puntero está dentro del área de dibujo
        if (!RectTransformUtility.RectangleContainsScreenPoint(drawingArea, Input.mousePosition, null))
        {
            // Si el puntero sale, se reinicia la stroke actual
            currentLine = null;
            currentPoints.Clear();
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            StartNewLine();
        }
        if (Input.GetMouseButton(0) && currentLine != null)
        {
            Vector2 pos = Input.mousePosition;
            if (currentPoints.Count == 0 || Vector2.Distance(currentPoints[currentPoints.Count - 1], pos) >= minDistance)
            {
                currentPoints.Add(pos);
                // Convertir la posición de pantalla a coordenadas locales del drawingArea
                Vector2 localPoint;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(drawingArea, pos, null, out localPoint);
                currentLine.Points.Add(localPoint);
                currentLine.SetVerticesDirty(); // Marca la malla para actualizarse
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            // Finaliza la stroke actual
            currentLine = null;
            currentPoints.Clear();
        }
    }

    void StartNewLine()
    {
     // Verificación básica
    if (drawingArea == null)
    {
        Debug.LogError("drawingArea no asignado en GameManager.");
        return;
    }

    if (!PhotonNetwork.IsConnectedAndReady)
    {
        Debug.LogWarning("No estás conectado a Photon. Abortando instanciación.");
        return;
    }

    Debug.Log("Llamando PhotonNetwork.Instantiate(\"LinesPrefab\")");

    // Instancia el objeto en red
    GameObject lineObj = PhotonNetwork.Instantiate(
        "LinesPrefab",             // Asegúrate que este nombre sea exacto
        Vector3.zero,
        Quaternion.identity
    );

    if (lineObj == null)
    {
        Debug.LogError("PhotonNetwork.Instantiate devolvió null");
        return;
    }

    // 🔥 Aseguramos que el objeto se parenta a drawingArea, en todos los clientes
    lineObj.transform.SetParent(drawingArea, false);

    // Obtén el componente de línea
    currentLine = lineObj.GetComponent<UILineRenderer>();
    if (currentLine == null)
    {
        Debug.LogError("El prefab 'LinesPrefab' no tiene componente UILineRenderer.");
        return;
    }

    currentPoints.Clear();
    currentLine.Points = new List<Vector2>();

    Debug.Log("Línea preparada para dibujar.");
    }
    
}
