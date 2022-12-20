using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class kentiku : MonoBehaviour {
    [SerializeField] private GameObject expandButton;
    private bool expanded = true;

    public void OnClick() {
        if(expanded) {
            expandButton.transform.Find("Arrow").GetComponent<TextMeshProUGUI>().text = "↑";
            transform.position -= new Vector3(0, 300, 0);
        } else {
            expandButton.transform.Find("Arrow").GetComponent<TextMeshProUGUI>().text = "↓";
            transform.position += new Vector3(0, 300, 0);
        }
        expanded = !expanded;
    }
}
