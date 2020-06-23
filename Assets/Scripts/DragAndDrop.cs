using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class DragAndDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler,IDropHandler
{
    [SerializeField]
    private RectTransform rt;
    [SerializeField]
    private Canvas canvas;
    private CanvasGroup canvasgroup;
    public Vector3 basepos,localpos;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        canvas = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Canvas>();
        canvasgroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasgroup.alpha = .6f;
        canvasgroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rt.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasgroup.alpha = 1f;
        canvasgroup.blocksRaycasts = true ;
        if (gameObject.transform.parent.tag.Equals("Player")) {
            Debug.Log("Did not move");
            eventData.pointerDrag.GetComponent<RectTransform>().transform.position = eventData.pointerDrag.GetComponent<DragAndDrop>().basepos;
        }

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        basepos = gameObject.GetComponent<RectTransform>().transform.position;
        localpos = gameObject.GetComponent<RectTransform>().transform.localPosition;
        //Debug.Log("OnPointerDown On "+gameObject.GetComponent<CardValue>().card.Cardname);
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("Dropped");
    }
}
