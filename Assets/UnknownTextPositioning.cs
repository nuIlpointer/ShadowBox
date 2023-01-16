using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnknownTextPositioning : MonoBehaviour
{
    public RectTransform rectTransform;
    public Transform block;
    
    public Canvas canvas;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = transform.parent.GetComponent<RectTransform>();
        canvas = GetComponent<Graphic>().canvas;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 bSPos = Camera.main.WorldToScreenPoint(block.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, bSPos, null, out Vector2 uiPos);
        transform.localPosition = uiPos;
    }
}
