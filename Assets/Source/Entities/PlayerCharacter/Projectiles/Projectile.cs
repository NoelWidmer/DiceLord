using System;
using System.Collections;
using UnityEngine;

public interface IProjectile
{
    void Charge(int distance, float durationPerField, GridDirection direction);
    void Launch();
}

public class Projectile : MonoBehaviour, IProjectile
{
    public Sprite NeSprite;
    public Sprite SeSprite;
    public Sprite SwSprite;
    public Sprite NwSprite;

    private SpriteRenderer _renderer;

    private void Awake()
    {
        enabled = false;
        _renderer = GetComponentInChildren<SpriteRenderer>();
    }

    private float _remainingDistance;
    private float _speed;
    private Vector2 _direction;

    public void Charge(int distance, float durationPerField, GridDirection direction)
    {
        _remainingDistance = distance * GridVector.DistanceBetweenFields;
        var duration = durationPerField * distance;
        _speed = distance / duration;
        _direction = direction.GetVector();

        _renderer.sprite = direction switch
        {
            GridDirection.NorthEast => NeSprite,
            GridDirection.SouthEast => SeSprite,
            GridDirection.SouthWest => SwSprite,
            GridDirection.NorthWest => NwSprite,
            _ => throw new NotImplementedException(),
        };



        transform.rotation = Quaternion.Euler(0f, 0f, direction switch
        {
            GridDirection.NorthEast => -14f,
            GridDirection.SouthEast => 16f,
            GridDirection.SouthWest => -14f,
            GridDirection.NorthWest => 16f,
            _ => throw new NotImplementedException(),
        });
    }

    public void Launch() => enabled = true;

    private void Update()
    {
        var delta = _direction * _speed * Time.deltaTime;
        _remainingDistance -= delta.magnitude;

        if (_remainingDistance > 0)
        {
            transform.position += new Vector3(delta.x, delta.y, 0f);
        }
        else
        {
            enabled = false;
            StartCoroutine(DelayDestroy());

            IEnumerator DelayDestroy()
            {
                yield return new WaitForSeconds(1f);
                Destroy(gameObject);
            }
        }
    }
}
