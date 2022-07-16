using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionIcon : MonoBehaviour
{
    private GameMode.PlayerAction _action;

    public Sprite moveIcon;
    public Sprite meleeIcon;
    public Sprite rangedIcon;
    public Sprite aoeIcon;
    public Sprite dodgeIcon;
    public Sprite pushIcon;
    public Sprite healIcon;
    public Sprite nopIcon;

    public void SetAction(GameMode.PlayerAction action)
    {
        _action = action;
        Sprite sprite = nopIcon;

        switch(action)
        {
            case GameMode.PlayerAction.NOP:
                sprite = nopIcon;
                break;

            case GameMode.PlayerAction.Move:
                sprite = moveIcon;
                break;

            case GameMode.PlayerAction.Melee:
                sprite = meleeIcon;
                break;

            case GameMode.PlayerAction.Ranged:
                sprite = rangedIcon;
                break;

            case GameMode.PlayerAction.AOE:
                sprite = aoeIcon;
                break;

            case GameMode.PlayerAction.Dodge:
                sprite = dodgeIcon;
                break;

            case GameMode.PlayerAction.Push:
                sprite = pushIcon;
                break;

            case GameMode.PlayerAction.Heal:
                sprite = healIcon;
                break;
        }

        GetComponentInParent<Image>().sprite = sprite;
    }

    public GameMode.PlayerAction GetAction() => _action;
}
