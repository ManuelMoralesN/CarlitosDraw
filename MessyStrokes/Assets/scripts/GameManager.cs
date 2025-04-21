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
        // Instanciamos la línea en red para que todos los clientes la vean
        GameObject lineObj = PhotonNetwork.InstantiateRoomObject(
            "LinesPrefab",       // Debe coincidir con el nombre del prefab en Resources
            Vector3.zero,
            Quaternion.identity,
            0                        // Grupo de interés (usualmente 0)
        );
        // Lo parentamos dentro de drawingArea para que aparezca en UI
        lineObj.transform.SetParent(drawingArea, false);

        currentLine = lineObj.GetComponent<UILineRenderer>();
        if (currentLine == null)
        {
            Debug.LogError("El prefab no tiene el componente UILineRenderer");
            return;
        }
        currentLine.Points = new List<Vector2>();
    }
}
