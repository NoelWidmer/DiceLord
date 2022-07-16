using UnityEngine;

public interface IDirectionalArrowButton
{
    GridDirection Direction { get; }
    void OnCursorEnter();
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
            _src.clip = References.Instance.DeathScreamSounds.GetRandomItem();
        }
    }

    public void OnCursorEnter()
    {
        _src.Play();
    }
}
