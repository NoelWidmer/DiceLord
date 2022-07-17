using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface ICanvasController
{
    void OnMouseMoved(Vector2 position);
}

public class CanvasController : Singleton<CanvasController, ICanvasController>, ICanvasController
{
    public GameObject SlotPrefab;
    public GameObject ActionIconPrefab;

    private GameObject _canvas;
    private GameObject _tray;
    private Rect _trayRect;
    private GameObject _slotsArea;
    private Rect _slotsAreaRect;
    private List<GameObject> _slots;
    private GameObject _confirmButton;
    private GameObject _rollButton;
    private GameMode _gameMode;

    public AudioClip ConfirmSound;
    public AudioClip DropSound;

    /*********************************
     * Init
     *********************************/
    protected override void OnAwake()
    {
        // init canvas
        _canvas = transform.Find("Canvas").gameObject;

        // init tray
        _tray = _canvas.transform.Find("Tray").gameObject;
        _trayRect = _tray.GetComponent<RectTransform>().rect;

        // init slots
        _slotsArea = _canvas.transform.Find("SlotsArea").gameObject;
        _slotsAreaRect = _slotsArea.GetComponent<RectTransform>().rect;

        _gameMode = transform.GetComponentInParent<GameMode>();

        _slots = new();
        for (int i = 0; i < _gameMode.number_of_dice; i++)
        {
            var slot = Instantiate(SlotPrefab, transform);
            slot.name = "Slot " + i;
            slot.transform.SetParent(_slotsArea.transform);
            float width = slot.GetComponent<RectTransform>().rect.width;
            float spacing = (_slotsAreaRect.width + width) / (_gameMode.number_of_dice + 1);
            float offset = -(_slotsAreaRect.width / 2) - (width / 2) + spacing;
            slot.GetComponent<RectTransform>().localPosition = new(offset + (i * spacing), 0f);
            _slots.Add(slot);
        }

        // init buttons
        _confirmButton = _canvas.transform.Find("ConfirmButton").gameObject;
        _rollButton = _canvas.transform.Find("Table").Find("RollButton").gameObject;
        EnableConfirmButton(false);
        EnableRollButton(false);
    }

    /*********************************
     * Mouse Position
     *********************************/
    private Vector2 _mousePosition;

    public Vector2 GetMousePosition() => _mousePosition;

    public void OnMouseMoved(Vector2 position)
    {
        _mousePosition = position;
    }

    /*********************************
     * Confirm Button
     *********************************/
    public void EnableConfirmButton(bool enabled)
    {
        _confirmButton.GetComponent<Button>().interactable = enabled;
    }

    private List<AudioSource> _sources = new();

    public void OnConfirmButton()
    {
        if (GameMode.Instance.TurnState == TurnState.Choose)
        {
            this.PlayParallelSound(ref _sources, ConfirmSound, false);
            _gameMode.OnConfirmSelection();
        }
    }

    /*********************************
     * Roll Button
     *********************************/
    public void EnableRollButton(bool enabled)
    {
        _rollButton.GetComponent<Button>().interactable = enabled;
    }

    public void OnRollButton()
    {
        _gameMode.OnRollDice();
    }

    /*********************************
     * Drag & Drop
     *********************************/
    public void Drop(GameObject actionIcon)
    {
        Vector3 localMouse;
        foreach (var slot in _slots)
        {
            Rect slotRect = slot.transform.GetComponent<RectTransform>().rect;
            localMouse = slot.transform.GetComponent<RectTransform>().InverseTransformPoint(_mousePosition);

            Bounds slotBounds = new(new(0f,0f,0f), slotRect.size * 1.2f);
            if(slotBounds.Contains(localMouse))
            {
                AddToSlot(slot, actionIcon);
                this.PlayParallelSound(ref _sources, DropSound, false);
                return;
            }
        }

        Rect actionIconRect = actionIcon.transform.GetComponent<RectTransform>().rect;
        localMouse = _tray.transform.GetComponent<RectTransform>().InverseTransformPoint(_mousePosition);
        Bounds trayBounds = new(new(0f, 0f, 0f), _trayRect.size-actionIconRect.size);
        if(trayBounds.Contains(localMouse))
        {
            return;
        }
        PlaceInTray(actionIcon);
    }

    /*********************************
     * Tray
     *********************************/
    public void PopulateTray(List<GameMode.PlayerAction> rolls)
    { 
        foreach(var roll in rolls)
        {
            GameObject actionIcon = Instantiate(ActionIconPrefab, transform);
            actionIcon.name = roll.ToString();
            actionIcon.GetComponent<ActionIcon>().SetAction(roll);
            PlaceInTray(actionIcon);
        }
    }

    public void PlaceInTray(GameObject actionIcon)
    {
        actionIcon.transform.SetParent(_tray.transform);
        Rect actionRect = actionIcon.GetComponent<RectTransform>().rect;
        float xSpread = (_trayRect.width / 2) - (actionRect.width / 3);
        float ySpread = (_trayRect.height / 2) - (actionRect.height / 3);
        actionIcon.GetComponent<RectTransform>().localPosition = new(Random.Range(-xSpread, xSpread), Random.Range(-ySpread, ySpread));
    }

    public void ClearTray()
    {
        while(_tray.transform.childCount > 0)
        {
            DestroyImmediate(_tray.transform.GetChild(0).gameObject);
        }
    }

    public GameObject GetTray() => _tray;

    /*********************************
     * Slot
     *********************************/
    public void AddToSlot(int idx, GameObject actionIcon)
    {
        GameObject slot = _slots[idx];
        AddToSlot(slot, actionIcon);
    }

    public void AddToSlot(GameObject slot, GameObject actionIcon)
    {
        if(slot.transform.childCount > 0)
        {
            DisplaceFromSlot(slot);
        }
        actionIcon.transform.SetParent(slot.transform);
        actionIcon.GetComponent<RectTransform>().localPosition = new(0f, 0f);
    }

    public void DisplaceFromSlot(GameObject slot)
    {
        GameObject actionIcon = slot.transform.GetChild(0).gameObject;
        PlaceInTray(actionIcon);
    }

    public List<GameMode.PlayerAction> GetSelectedActions()
    {
        List<GameMode.PlayerAction> actions = new();

        foreach(var slot in _slots)
        {
            if(slot.transform.childCount > 0)
            {
                GameObject child = slot.transform.GetChild(0).gameObject;
                actions.Add(slot.transform.GetChild(0).GetComponent<ActionIcon>().GetAction());
            }
        }

        return actions;
    }

    public void ClearSlots()
    {
        foreach(var slot in _slots)
        {
            while(slot.transform.childCount > 0)
            {
                DestroyImmediate(slot.transform.GetChild(0).gameObject);
            }
        }
    }
}
