using UnityEngine;

public interface IDirectionalArrowButton
{
    GridDirection Direction { get; }
    void OnCursorEnter();
    void OnClick();
}

public class DirectionalArrowButton : MonoBehaviour, IDirectionalArrowButton
{
    [SerializeField]
    private GridDirection _direction;
    public GridDirection Direction => _direction;

    private AudioSource _src;

    private void Start()
    {
        enabled = false;

        // add collider
        {
            var collider = gameObject.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
        }

        // add audio source
        {
            _src = gameObject.AddComponent<AudioSource>();
        }
    }

    public void OnCursorEnter()
    {
        _src.clip = References.Instance.DirectionalArrowHover;
        _src.Play();
    }

    public void OnClick()
    {
        _src.clip = References.Instance.DirectionalArrowClick;
        _src.Play();
    }
}
