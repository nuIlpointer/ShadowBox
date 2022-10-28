using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LayerIndicatorChange : MonoBehaviour
{
    // Start is called before the first frame update
    public Sprite imageOutsideBlock; //0
    public Sprite imageOutsideWall;  //1
    public Sprite imageInsideBlock;  //2
    public Sprite imageInsideWall;   //3
    public Text indicatorTextObj;
    private Image indicator;
    private int indicatorIndex = 0;
    private int oldIndicatorIndex;
    private float mouseWheel;

    void Start()
    {
        indicator = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        oldIndicatorIndex = indicatorIndex;
        mouseWheel = Input.GetAxis("Mouse ScrollWheel");
        if (mouseWheel > 0)
            indicatorIndex = (++indicatorIndex) % 4;
        else if (mouseWheel < 0)
            indicatorIndex = (--indicatorIndex) % 4;
        if (indicatorIndex < 0) indicatorIndex += 4;
        if(indicatorIndex != oldIndicatorIndex) {
            if (indicatorIndex == 0) {
                indicatorTextObj.text = "外ブロック";
                indicator.sprite = imageOutsideBlock;
            }
            if (indicatorIndex == 1) {
                indicatorTextObj.text = "外カベ";
                indicator.sprite = imageOutsideWall;
            }
            if (indicatorIndex == 2) {
                indicatorTextObj.text = "内ブロック";
                indicator.sprite = imageInsideBlock;
            }
            if (indicatorIndex == 3) {
                indicatorTextObj.text = "内カベ";
                indicator.sprite = imageInsideWall;
            }
        }
    }
}
