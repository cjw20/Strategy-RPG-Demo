using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatForecast : MonoBehaviour
{
    [SerializeField] GameObject forecastWindow;
    [SerializeField] Text allyName;
    [SerializeField] Text targetName;
    [SerializeField] Text attackName;
    [SerializeField] Text attackPower;
    [SerializeField] Text results;
    [SerializeField] Text enemyRes;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace)) //functions the same as cancel button on ui
        {
            if (AllyController.Instance.waitingForConfirm)
            {
                AllyController.Instance.SetConfirm(true);
            }
            
        }
    }

    public void ShowForecast(GameObject ally, GameObject target, Ability ability)
    {
        
        forecastWindow.SetActive(true);
        allyName.text = ally.GetComponent<Unit>().unitName;
        targetName.text = target.GetComponent<Unit>().unitName;
        attackName.text = ability.abilityName;
        attackPower.text = "Power: " + ability.CalculatePower(ally).ToString();

        if (!ability.support)
        {
            results.text = target.GetComponent<Stats>().currentHP.ToString() + " -> " + (target.GetComponent<Stats>().currentHP - ability.CalculateResult(ability.CalculatePower(ally), target)).ToString();
        }
        else
        {
            int finalHP = target.GetComponent<Stats>().currentHP + ability.CalculateResult(ability.CalculatePower(ally), target);
            if(finalHP > target.GetComponent<Stats>().maxHP)
            {
                finalHP = target.GetComponent<Stats>().maxHP;
            }
            results.text = target.GetComponent<Stats>().currentHP.ToString() + " -> " + finalHP.ToString();
        }
        
    }

    public void HideForecast()
    {
        BattleUIHandler.Instance.state = UIState.NULL;
        BattleUIHandler.Instance.inUI = false;
        forecastWindow.SetActive(false);
        
    }
}
