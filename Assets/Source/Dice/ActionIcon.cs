using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ActionIcon : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
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

    private bool _isDragging;
    private Vector3 _initialScaling;

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

    public void Update()
    {
        if(_isDragging)
        {
            Vector2 position = transform.GetComponentInParent<IDragDrop>().GetMousePosition();
            transform.position = position;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _initialScaling = transform.GetComponent<RectTransform>().localScale;
        _isDragging = true;
        transform.GetComponent<RectTransform>().localScale = _initialScaling * .9f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isDragging = false;
        transform.GetComponent<RectTransform>().localScale = _initialScaling;

        transform.GetComponentInParent<IDragDrop>().Drop(this.gameObject);
    }
}
