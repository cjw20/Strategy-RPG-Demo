using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoverWindow : MonoBehaviour
{
    [SerializeField] GameObject window;
    GameObject unit;
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Text xpText;
    [SerializeField] Text hpText;
    [SerializeField] Text mpText;

    public void ShowHover(GameObject character)
    {
        window.SetActive(true);
        Stats unitStats = character.GetComponent<Stats>();
        Unit unit = character.GetComponent<Unit>();
        nameText.text = unit.unitName;
        levelText.text = "Level: " + unitStats.level.ToString();
        xpText.text = "EXP: " + unitStats.currentXP.ToString() + " / " + ((unitStats.level + 1) * 100).ToString();
        hpText.text = "HP: " + unitStats.currentHP.ToString() + " / " + unitStats.maxHP.ToString();
        mpText.text = "MP: " + unitStats.currentMP.ToString() + " / " + unitStats.maxMP.ToString();

        RectTransform rect = window.GetComponent<RectTransform>();
        rect.position = Input.mousePosition + new Vector3(110,110, 110);
    }

    public void HideHover()
    {
        window.SetActive(false);
    }
}
