using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatWindow : MonoBehaviour
{
    public GameObject displayedUnit;
    Unit unit;
    Stats stats;

    [SerializeField] GameObject window;
    [SerializeField] Image unitSprite;
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Text hp;
    [SerializeField] Text mp;
    [SerializeField] Text str;
    [SerializeField] Text mag;
    [SerializeField] Text def;
    [SerializeField] Text res;
    [SerializeField] Text agi;
    [SerializeField] Text move;
    //equipment and abilities later
    

    public void ShowStatWindow(GameObject character)
    {
        window.SetActive(true); //may want to make happen after values have changed if necessary

        displayedUnit = character;
        unit = displayedUnit.GetComponent<Unit>();
        stats = displayedUnit.GetComponent<Stats>();

        unitSprite.sprite = displayedUnit.GetComponent<SpriteRenderer>().sprite;
        nameText.text = unit.unitName;
        levelText.text = "Level: " + stats.level.ToString();
        hp.text = "HP: " + stats.currentHP.ToString() + "/" + stats.maxHP.ToString();
        mp.text = "MP: " + stats.currentMP.ToString() + "/" + stats.maxMP.ToString();

        str.text = "Strength: " + stats.str.ToString();
        mag.text = "Magic: " + stats.mag.ToString();
        def.text = "Defense: " + stats.def.ToString();
        res.text = "Resistance: " + stats.res.ToString();
        agi.text = "Agility: " + stats.agi.ToString();
        move.text = "Move: " + unit.moveDistance.ToString();

    }
    
    public void CloseStatWindow()
    {
        BattleUIHandler.Instance.inUI = false;
        window.SetActive(false);
    }
}
