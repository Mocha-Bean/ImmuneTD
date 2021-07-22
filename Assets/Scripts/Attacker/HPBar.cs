using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBar : MonoBehaviour
{

    private RectTransform barBackgroundRect, barForegroundRect;
    private Attacker attacker;
    private Vector2 barDimensions;
    // Start is called before the first frame update
    void Start()
    {
        // Get references to the UI elements
        barBackgroundRect = this.transform.GetChild(0).GetComponent<RectTransform>();
        barForegroundRect = this.transform.GetChild(0).transform.GetChild(0).GetComponent<RectTransform>();
        // Get reference to oparent Attacker object
        attacker = GetComponentInParent<Attacker>();
        barDimensions = barBackgroundRect.sizeDelta;
    }

    // Update is called once per frame
    void Update()
    {
        float hpPortion = attacker.health / attacker.healthMax;
        if(hpPortion < 1.0F) {
            barForegroundRect.sizeDelta = new Vector2(barDimensions.x * hpPortion, barBackgroundRect.sizeDelta.y);
            barBackgroundRect.sizeDelta = barDimensions;
        } else {
            barForegroundRect.sizeDelta = Vector2.zero;
            barBackgroundRect.sizeDelta = Vector2.zero;
        }
    }
}
