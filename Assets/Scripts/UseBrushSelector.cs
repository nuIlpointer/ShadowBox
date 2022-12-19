using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseBrushSelector : MonoBehaviour
{
    public UseBrushViewer ubv;
    public int modification;

    public void Click_Action() {
        ubv.SetBrush(modification);
    }

    

}
