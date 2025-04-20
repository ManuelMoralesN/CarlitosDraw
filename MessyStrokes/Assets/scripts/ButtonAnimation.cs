using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject button;

    // Función para cuando el puntero entra al área del botón
    public void OnPointerEnter(PointerEventData eventData)
    {
        LeanTween.scale(button, Vector3.one * 1.4f, 0.2f).setEase(LeanTweenType.easeOutBack);
    }

    // Función para cuando el puntero sale del área del botón
    public void OnPointerExit(PointerEventData eventData)
    {
        LeanTween.scale(button, Vector3.one, 0.2f).setEase(LeanTweenType.easeInBack);        
    }
}
