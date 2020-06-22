using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardSlots : MonoBehaviour, IDropHandler
{
    public string cardType;
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null && (cardType.Equals("") || cardType.Equals(eventData.pointerDrag.GetComponent<CardValue>().card.Cardname)))
        {
            //eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
            cardType = eventData.pointerDrag.GetComponent<CardValue>().card.Cardname;
            eventData.pointerDrag.GetComponent<RectTransform>().SetParent(transform);
            gameObject.tag = "Player Cards";
        }
        else {
            eventData.pointerDrag.GetComponent<RectTransform>().transform.position = eventData.pointerDrag.GetComponent<DragAndDrop>().basepos;
        }
    }
}
