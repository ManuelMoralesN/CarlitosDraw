using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UILineRenderer : Graphic
{
    [Tooltip("Lista de puntos en coordenadas locales (del RectTransform) para dibujar la línea")]
    public List<Vector2> Points = new List<Vector2>();

    [Tooltip("Grosor de la línea (en píxeles)")]
    public float Thickness = 5f;


    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        if (Points == null || Points.Count < 2)
            return;

        // Recorre cada segmento entre dos puntos consecutivos
        for (int i = 0; i < Points.Count - 1; i++)
        {
            Vector2 start = Points[i];
            Vector2 end = Points[i + 1];
            Vector2 direction = (end - start).normalized;
            // Vector perpendicular para darle grosor
            Vector2 normal = new Vector2(-direction.y, direction.x) * (Thickness / 2f);

            // Calcula los cuatro vértices del segmento
            Vector2 v1 = start - normal;
            Vector2 v2 = start + normal;
            Vector2 v3 = end + normal;
            Vector2 v4 = end - normal;

            int index = vh.currentVertCount;
            vh.AddVert(v1, color, Vector2.zero);
            vh.AddVert(v2, color, Vector2.up);
            vh.AddVert(v3, color, Vector2.one);
            vh.AddVert(v4, color, Vector2.right);

            vh.AddTriangle(index, index + 1, index + 2);
            vh.AddTriangle(index, index + 2, index + 3);
        }
    }
}
