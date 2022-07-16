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
    private GameObject _slotsArea;
    private List<GameObject> _slots;

    protected override void OnAwake()
    {
        // init canvas
        _canvas = transform.Find("Canvas").gameObject;


        // init tray
        _tray = _canvas.transform.Find("Tray").gameObject;

        // init slots
        _slotsArea = _canvas.transform.Find("SlotsArea").gameObject;
        _slots = new();
        float areaWidth = _slotsArea.GetComponent<RectTransform>().rect.width;
        for (int i = 0; i < transform.GetComponentInParent<GameMode>().number_of_dice; i++) //TODO
        {
            var slot = Instantiate(SlotPrefab, transform);
            slot.name = "Slot " + i;
            slot.transform.SetParent(_slotsArea.transform);
            float width = slot.GetComponent<RectTransform>().rect.width;
            float offset = areaWidth / 2 - width / 2;
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
            actionIcon.GetComponent<ActionIcon>().action = roll;
            actionIcon.transform.SetParent(_tray.transform);
            actionIcon.GetComponent<RectTransform>().localPosition = new(0f, 0f);
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
        Debug.Log("Added actionIcon " + actionIcon + "(" + actionIcon.GetComponent<ActionIcon>().action + ") to slot " + idx);
    }

    public List<GameMode.PlayerAction> GetSelectedActions()
    {
        List<GameMode.PlayerAction> actions = new();

        foreach(var slot in _slots)
        {
            Debug.Log(slot + " has " + slot.transform.childCount + " child(ren)");
            if(slot.transform.childCount > 0)
            {
                GameObject child = slot.transform.GetChild(0).gameObject;
                Debug.Log("Found child " + child);
                actions.Add(slot.transform.GetChild(0).GetComponent<ActionIcon>().action);
            }
        }


        Debug.Log("Returning selected actions:");
        foreach (var action in actions)
        {
            Debug.Log(action);
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
