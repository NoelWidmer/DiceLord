using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICanvasController
{

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

        _slots = new();
        for (int i = 0; i < transform.GetComponentInParent<GameMode>().number_of_dice; i++) //TODO
        {
            var slot = Instantiate(SlotPrefab, transform);
            slot.name = "Slot " + i;
            slot.transform.SetParent(_slotsArea.transform);
            float width = slot.GetComponent<RectTransform>().rect.width;
            float offset = _slotsAreaRect.width / 2 - width / 2;
            slot.GetComponent<RectTransform>().localPosition = new(100f * i - offset, 0f);
            _slots.Add(slot);
        }
    }

    public void PopulateTray(List<GameMode.PlayerAction> rolls)
    { 
        foreach(var roll in rolls)
        {
            var actionIcon = Instantiate(ActionIconPrefab, transform);
            actionIcon.name = roll.ToString();
            actionIcon.GetComponent<ActionIcon>().SetAction(roll);
            actionIcon.transform.SetParent(_tray.transform);

            Rect actionRect = actionIcon.GetComponent<RectTransform>().rect;
            float xSpread = (_trayRect.width / 2) - (actionRect.width / 2);
            float ySpread = (_trayRect.height / 2) - (actionRect.height / 2);
            actionIcon.GetComponent<RectTransform>().localPosition = new(Random.value * xSpread, Random.value * ySpread);
        }
    }

    public void ClearTray()
    {
        while(_tray.transform.childCount > 0)
        {
            DestroyImmediate(_tray.transform.GetChild(0).gameObject);
        }
    }

    public void AddToSlot(int idx, GameObject actionIcon)
    {
        GameObject slot = _slots[idx];
        actionIcon.transform.SetParent(slot.transform);
        actionIcon.GetComponent<RectTransform>().localPosition = new(0f, 0f);
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

    public GameObject GetTray() => _tray;
}
