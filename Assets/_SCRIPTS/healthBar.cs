using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class healthBar : MonoBehaviour {

    public float fill = 1.0f;
    private Image barImg;
    public Text counter;

    private bool isCritical;

    void Awake()
    {
        barImg = gameObject.GetComponent<Image>();
    }

 

    public void updateFill(int curr, int max)
    {
        fill = (curr * 1.0f / max * 1.0f);
		barImg.transform.localScale = new Vector3((fill*0.115f), barImg.transform.localScale.y, barImg.transform.localScale.z);
        counter.text = curr + "/" + max;
    }
}  
