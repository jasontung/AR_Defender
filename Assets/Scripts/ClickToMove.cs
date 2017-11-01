using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class ClickToMove : MonoBehaviour
{
    public ParticleSystem clickEffect;
    public HeroController heroController;
    public void OnPointClick(BaseEventData eventData)
    {
        PointerEventData pData = (PointerEventData)eventData;
        var clickPos = pData.pointerCurrentRaycast.worldPosition;
        heroController.Move(clickPos);
        clickEffect.transform.position = clickPos + Vector3.one * 0.1f;
        clickEffect.Play();
    }
}
