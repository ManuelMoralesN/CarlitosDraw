using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimacionesUI : MonoBehaviour
{
    [SerializeField] private GameObject logo;
    [SerializeField] private GameObject logo1;
    [SerializeField] private GameObject inicioGrupo;

    private void Start()
    {
        LeanTween.moveX(logo.GetComponent<RectTransform>(), 116, 1.5f)
            .setEase(LeanTweenType.easeOutBounce)
            .setDelay(2.5f)
            .setOnComplete(BajarAlpha);

            LeanTween.moveX(logo1.GetComponent<RectTransform>(), 0, 1.5f)
            .setEase(LeanTweenType.easeOutBounce)
            .setDelay(2.5f)
            .setOnComplete(BajarAlpha);
    }

    private void BajarAlpha()
    {
        LeanTween.alpha(inicioGrupo.GetComponent<RectTransform>(), 0f, 1f)
            .setDelay(0.5f);
        inicioGrupo.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }
}
