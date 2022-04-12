using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum UIState { UNIT, ABILITY, ITEM, RANGE, STAT, FORECAST, PAUSE, NULL}
public class BattleUIHandler : MonoBehaviour
{
    private static BattleUIHandler instance;
    public static BattleUIHandler Instance { get { return instance; } }

    public UIState state;
    [SerializeField] GameObject pauseWindow;
    [SerializeField] GameObject actionWindow;
    [SerializeField] StatWindow statWindow;
    [SerializeField] Text nameText;
    [SerializeField] Text turnText;

    [SerializeField] CombatForecast forecast;

    [SerializeField] GameObject skillWindow;
    [SerializeField] GameObject abilityButtonPrefab;
    [SerializeField] HoverWindow hoverWindow;
    List<GameObject> buttons;
    [SerializeField] float buttonWGap;
    [SerializeField] float buttonHGap;

    public GameObject currentUnit;

    public GameObject moveButton;
    public GameObject[] actionButtons; //skill attack item buttons to be greyed out when units action has been used
    
    public bool inUI; //true when in ui so that tiles cant be clicked when active
    void Awake()
    {
        instance = this;
        buttons = new List<GameObject>();
        state = UIState.NULL;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            Back();
            //add other keys later
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (BattleSystem.Instance.gameState == GameState.ALLYTURN && state != UIState.STAT)
            {
                OpenPause();
            }
        }
    }
    public void Back()
    {
        if (AllyController.Instance.actionPlaying)
        {
            return; //dont cancel when
        }
        switch (state) //this could probably use cleaning up. and maybe make it so it doesnt always just go back to default window
        {
            case UIState.UNIT:
                {
                    actionWindow.SetActive(false);
                    inUI = false;
                    state = UIState.NULL;
                    break;
                }
            case UIState.ABILITY:
                {
                    AllyController.Instance.DeSelectUnit();
                    HideAbilities();
                    AllyController.Instance.SelectUnit(currentUnit);
                    break;
                }
            case UIState.FORECAST:
                {
                    //doesnt work because in action is true
                    forecast.HideForecast();
                    //in ui and state null set in above function
                    AllyController.Instance.DeSelectUnit();
                    AllyController.Instance.SelectUnit(currentUnit);
                    break;
                }
            case UIState.RANGE:
                {
                    AllyController.Instance.DeSelectUnit();
                    HideAbilities();
                    AllyController.Instance.SelectUnit(currentUnit);
                    break;
                }
            case UIState.ITEM:
                {
                    HideItems();
                    AllyController.Instance.DeSelectUnit();
                    AllyController.Instance.SelectUnit(currentUnit);
                    break;
                }
            case UIState.STAT:
                {
                    statWindow.CloseStatWindow();

                    break;
                }
            case UIState.PAUSE:
                {
                    ClosePause();
                    break;
                }
            case UIState.NULL:
                {
                    break;
                }

        }
    }
    public void OpenPause()
    {
        pauseWindow.SetActive(true);
        inUI = true;
        state = UIState.PAUSE;
    }

    public void ClosePause()
    {
        pauseWindow.SetActive(false);
        inUI = false;
        state = UIState.NULL;
    }

    public void EndTurn()
    {
        ClosePause();
        AllyController.Instance.EndTurn();
    }
    public void DisplayAbilities(GameObject character)
    {

        inUI = true;
        state = UIState.ABILITY;
        RectTransform bRect = abilityButtonPrefab.GetComponent<RectTransform>();
        float bWidth = bRect.sizeDelta.x;
        float bHeight = bRect.sizeDelta.y; //dimensions of ability button
        RectTransform windowRect = skillWindow.GetComponent<RectTransform>();
        float sWidth = windowRect.sizeDelta.x;
        float sHeight = windowRect.sizeDelta.y; //dimensions of skill window

        actionWindow.SetActive(false);        
        skillWindow.SetActive(true);
        
        Stats unitStats = character.GetComponent<Stats>();

      

        int a = 0; //index for list of abilities
        for(int x = 0; x < (sWidth / (bWidth + buttonWGap)) - 1; x++) //nested for loop to instantiate buttons in correct format
        {
            for(int y = 0; y < (sHeight / (bHeight + buttonHGap)) - 1; y++)
            {
                if(a < unitStats.abilities.Count)
                {
                    if (unitStats.abilities[a].unlockLevel <= unitStats.level) //ignores abilities that havent been unlocked yet, make sure abilites are in order of unlock level so that their arent gaps in the buttons
                    {
                        GameObject btn = Instantiate(abilityButtonPrefab, skillWindow.transform);
                        buttons.Add(btn);
                        btn.GetComponentInChildren<Text>().text = unitStats.abilities[a].abilityName;
                        Ability thisAbility = unitStats.abilities[a];
                        btn.GetComponent<Button>().onClick.AddListener(delegate { SelectAbility(thisAbility); });
                        if (unitStats.abilities[a].mpCost > unitStats.currentMP)
                        {
                            btn.GetComponent<Button>().interactable = false; //make sure this greys out button to avoid confusion
                        }
                        btn.GetComponent<RectTransform>().anchoredPosition = new Vector3((x * (bWidth + buttonWGap)) + buttonWGap, -y * (bHeight + buttonHGap) - buttonWGap, 0);
                    }
                    a++;
                }

                else
                {
                    break; //breaks out of loop once there are no more abilities to show
                }
                
                
            }
        }
    }

    public void HideAbilities()
    {
        foreach(GameObject btn in buttons)
        {
            Destroy(btn); //make sure this doesnt mess up loop somehow
        }
        skillWindow.SetActive(false);
        inUI = false;
        state = UIState.NULL;
    }
    void SelectAbility(Ability ability)
    {
        AllyController.Instance.ShowAbilityRange(ability);        
        HideAbilities();
        state = UIState.RANGE;
    }
    public void ShowItems()
    {
        state = UIState.ITEM;
    }

    public void HideItems()
    {
        state = UIState.NULL;
    }    
    

    public void SelectUnit(GameObject character)
    {

        currentUnit = character;
        inUI = true;
        Unit unitInfo = character.GetComponent<Unit>();
        actionWindow.SetActive(true);
        state = UIState.UNIT;
        nameText.text = unitInfo.unitName;

        //hides unusable buttons
        if (!unitInfo.hasAction)
        {
            foreach(GameObject btn in actionButtons)
            {
                btn.GetComponent<Button>().interactable = false;
            }
        }
        else
        {
            foreach (GameObject btn in actionButtons)
            {
                btn.GetComponent<Button>().interactable = true;
            }
        }


        if (!unitInfo.hasMove)
        {
            moveButton.GetComponent<Button>().interactable = false;
        }
        else
        {
            moveButton.GetComponent<Button>().interactable = true;
        }

    }

    public void HideActionWindow()
    {
        inUI = false;
        actionWindow.SetActive(false);
        state = UIState.NULL;
    }
    
    public void ShowAttackWindow(GameObject ally, GameObject target, Ability ability)
    {
        inUI = true;
        state = UIState.FORECAST;
        forecast.ShowForecast(ally, target, ability);
        
    }
    public void HideAttackWindow()
    {
        inUI = false;
        forecast.HideForecast();
        state = UIState.NULL;
    }
    public void ShowStatWindow(GameObject unit)
    {
        inUI = true;
        state = UIState.STAT;
        statWindow.ShowStatWindow(unit);
    }

    public void ShowHoverWindow(GameObject character)
    {
        hoverWindow.ShowHover(character);
    }

    public void HideHoverWindow()
    {
        hoverWindow.HideHover();
    }
    public void AllyTurn()
    {
        turnText.text = "Ally Turn";
    }

    public void EnemyTurn()
    {
        turnText.text = "Enemy Turn";
    }
}
