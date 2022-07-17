using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IReconfigureController
{

}

public class ReconfigureController : Singleton<ReconfigureController, IReconfigureController>, IReconfigureController, IDragDrop, IUnityInputSystemMessages
{
    public List<GameMode.PlayerAction> availableActions;
    public GameObject ActionIconPrefab;

    private GameObject _table;
    private GameObject _tray;
    private Rect _trayRect;
    private List<GameObject> _slots;

    private Vector2 _mousePosition;
    // Start is called before the first frame update
    protected override void OnAwake()
    {
        // init canvas
        _table = transform.Find("Table").gameObject;

        // init tray
        _tray = _table.transform.Find("Tray").gameObject;
        _trayRect = _tray.GetComponent<RectTransform>().rect;

        // init slots
        _slots = new();
        foreach(Transform slot in _table.transform.Find("Net"))
        {
            _slots.Add(slot.gameObject);
        }

        PopulateTray(availableActions);
    }
    
    public void Drop(GameObject actionIcon)
    {
        Vector3 localMouse;
        foreach (var slot in _slots)
        {
            Rect slotRect = slot.transform.GetComponent<RectTransform>().rect;
            localMouse = slot.transform.GetComponent<RectTransform>().InverseTransformPoint(_mousePosition);

            Bounds slotBounds = new(new(0f, 0f, 0f), slotRect.size * 1.2f);
            if (slotBounds.Contains(localMouse))
            {
                AddToSlot(slot, actionIcon);
                return;
            }
        }

        Rect actionIconRect = actionIcon.transform.GetComponent<RectTransform>().rect;
        localMouse = _tray.transform.GetComponent<RectTransform>().InverseTransformPoint(_mousePosition);
        Bounds trayBounds = new(new(0f, 0f, 0f), _trayRect.size - actionIconRect.size);
        if (trayBounds.Contains(localMouse))
        {
            return;
        }
        PlaceInTray(actionIcon);
    }

    public void PopulateTray(List<GameMode.PlayerAction> actions)
    {
        foreach (var action in actions)
        {
            GameObject actionIcon = Instantiate(ActionIconPrefab, transform);
            actionIcon.name = action.ToString();
            actionIcon.GetComponent<ActionIcon>().SetAction(action);
            PlaceInTray(actionIcon);
        }
    }
    public void PlaceInTray(GameObject actionIcon)
    {
        GameObject other = _tray.transform.Find(actionIcon.name)?.gameObject;
        if (other && !actionIcon.Equals(other))
        {
            Destroy(actionIcon);
            return;
        }

        actionIcon.transform.SetParent(_tray.transform);
        Rect actionRect = actionIcon.GetComponent<RectTransform>().rect;
        float xSpread = (_trayRect.width / 2) - (actionRect.width / 3);
        float ySpread = (_trayRect.height / 2) - (actionRect.height / 3);
        actionIcon.GetComponent<RectTransform>().localPosition = new(Random.Range(-xSpread, xSpread), Random.Range(-ySpread, ySpread));
    }

    public void AddToSlot(GameObject slot, GameObject actionIcon)
    {
        GameObject iconClone = Instantiate(actionIcon, transform);
        iconClone.name = actionIcon.name;

        if (slot.transform.childCount > 0)
        {
            DisplaceFromSlot(slot);
        }
        actionIcon.transform.SetParent(slot.transform);
        actionIcon.GetComponent<RectTransform>().localPosition = new(0f, 0f);

        PlaceInTray(iconClone);
    }

    public void DisplaceFromSlot(GameObject slot)
    {
        GameObject actionIcon = slot.transform.GetChild(0).gameObject;
        PlaceInTray(actionIcon);
    }

    public List<GameMode.PlayerAction> GetSelectedActions()
    {
        List<GameMode.PlayerAction> actions = new();

        foreach (var slot in _slots)
        {
            if (slot.transform.childCount > 0)
            {
                actions.Add(slot.transform.GetChild(0).GetComponent<ActionIcon>().GetAction());
            }
        }

        return actions;
    }

    public void OnConfirmButton()
    {
        // TODO: pass selected actions
    }

    public void OnRoll(InputValue inputValue)
    { }

    public void OnMouse(InputValue inputValue)
    {
        _mousePosition = inputValue.Get<Vector2>();
    }

    public void OnClick(InputValue inputValue)
    { }

    public Vector2 GetMousePosition() => _mousePosition;
}
