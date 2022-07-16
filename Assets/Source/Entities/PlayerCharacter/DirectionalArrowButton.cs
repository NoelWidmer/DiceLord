using UnityEngine;

public interface IDirectionalArrowButton
{
    GridDirection Direction { get; }
    void OnCursorEnter();
    void OnCursorExit();
    void OnClick();
}

public class DirectionalArrowButton : MonoBehaviour, IDirectionalArrowButton
{
    [SerializeField]
    private GridDirection _direction;
    public GridDirection Direction => _direction;

    [SerializeField]
    private Sprite _light;
    private Sprite _regular;

    private AudioSource _src;
    private SpriteRenderer _renderer;

    private void Start()
    {
        enabled = false;

        // add collider
        {
            var collider = gameObject.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
        }

        _src = gameObject.AddComponent<AudioSource>();
        
        // sprites
        {
            _renderer = GetComponent<SpriteRenderer>();
            _regular = _renderer.sprite;
        }
    }

    public void OnCursorEnter()
    {
        // play sound
        {
            _src.clip = References.Instance.DirectionalArrowHover;
            _src.Play();
        }

        // switch sprites
        {
            _renderer.sprite = _light;
        }
    }

    public void OnCursorExit()
    {
        _renderer.sprite = _regular;
    }

    public void OnClick()
    {
        _src.clip = References.Instance.DirectionalArrowClick;
        _src.Play();
    }
}
