using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToggleImage : MonoBehaviour {
    [SerializeField] private Sprite[] instructImages;
    [SerializeField] private GameObject imageTargetObj;
    [SerializeField] private GameObject imageIndicatorObj;
     
    private TextMeshProUGUI indicator;
    private Image image;
    private int index = 0;
    // Start is called before the first frame update
    void Start() {
        image = imageTargetObj.GetComponent<Image>();
        indicator = imageIndicatorObj.GetComponent<TextMeshProUGUI>();
        Debug.Log($"{instructImages.Length} images loaded.");
        image.sprite = instructImages[index];
        image.preserveAspect = true;
        indicator.text = GenerateIndicatorText(index, instructImages.Length);
    }

    public void IncrementImage() {
        if (index < instructImages.Length - 1) {
            index++;
            image.sprite = instructImages[index];
            image.preserveAspect = true;
            indicator.text = GenerateIndicatorText(index, instructImages.Length);
        }
    }

    public void DecrementImage() {
        if (index > 0) {
            index--;
            image.sprite = instructImages[index];
            image.preserveAspect = true;
            indicator.text = GenerateIndicatorText(index, instructImages.Length);
        }
    }

    private string GenerateIndicatorText(int index, int length) {
        var resText = "";
        for (int i = 0; i < length; i++)
            resText += (i == index ? "●" : "○");
        return resText;
    }
}
