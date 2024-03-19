using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenResizeing : MonoBehaviour
{
    [SerializeField] private RectTransform canvas;
    private RectTransform rtransform;

    // Start is called before the first frame update
    void Start()
    {
        rtransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        float f;
        //幅基準でリサイズした場合高さがはみ出さないか
        if((rtransform.sizeDelta / (rtransform.sizeDelta.x / canvas.sizeDelta.x)).y < canvas.sizeDelta.y) {

            //はみ出さない
            if (Mathf.Abs((f = rtransform.sizeDelta.x / canvas.sizeDelta.x) - 1) > 0.001f) {
                rtransform.sizeDelta = rtransform.sizeDelta / f;
            }
        } else {

            //はみ出す
            if (Mathf.Abs((f = rtransform.sizeDelta.y / canvas.sizeDelta.y) - 1) > 0.001f) {
                rtransform.sizeDelta = rtransform.sizeDelta / f;
            }
        }

        




        
    }
}
