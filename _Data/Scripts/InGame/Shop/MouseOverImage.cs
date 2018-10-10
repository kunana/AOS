using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseOverImage : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IScrollHandler {

    private Image myImage;
    private Color myColor;
    private Color mouseoverColor;

    private ScrollRect SearchScroll;
	// Use this for initialization
	void Start () {
        myImage = GetComponent<Image>();
        myColor = myImage.color;

        mouseoverColor = new Color(43f/255f, 96f/255f, 93f/255f, 60f/255f);

        SearchScroll = GameObject.FindGameObjectWithTag("SearchView").GetComponent<ScrollRect>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PointerEnter()
    {
        myImage.color = mouseoverColor;
    }

    public void PointerExit()
    {
        myImage.color = myColor;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        SearchScroll.OnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        SearchScroll.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        SearchScroll.OnEndDrag(eventData);
    }

    public void OnScroll(PointerEventData eventData)
    {
        SearchScroll.OnScroll(eventData);
    }
}
