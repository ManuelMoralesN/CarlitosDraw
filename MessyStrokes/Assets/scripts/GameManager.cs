using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Configuraci�n de Dibujo")]
    public RectTransform drawingArea;      // �rea de dibujo UI (por ejemplo, una Image de fondo)
    public GameObject uiLinePrefab;        // Prefab que contiene el componente UILineRenderer
    public float minDistance = 5f;         // Distancia m�nima (en p�xeles) para agregar un nuevo punto

    private UILineRenderer currentLine;    // La stroke actual
    private List<Vector2> currentPoints = new List<Vector2>();

    void Update()
    {
        ProcessDrawing();
    }

    void ProcessDrawing()
    {
        // Procesa la entrada solo si el puntero est� dentro del �rea de dibujo
        if (!RectTransformUtility.RectangleContainsScreenPoint(drawingArea, Input.mousePosition, null))
        {
            // Si el puntero sale, se reinicia la stroke actual para evitar conectar trazos
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
                // Convertir la posici�n de pantalla a coordenadas locales del drawingArea
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
        GameObject lineObj = Instantiate(uiLinePrefab, drawingArea);
        currentLine = lineObj.GetComponent<UILineRenderer>();
        if (currentLine == null)
        {
            Debug.LogError("El prefab no tiene el componente UILineRenderer");
            return;
        }
        currentLine.Points = new List<Vector2>();
    }
}
