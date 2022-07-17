using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectile
{
    void Charge(int distance, float durationPerField, GridDirection direction);
    void Launch();
    void OnLastFieldHit();
}

public class Projectile : MonoBehaviour, IProjectile
{
    public Sprite NeSprite;
    public Sprite NeSpriteBroken;

    public Sprite SeSprite;
    public Sprite SeSpriteBroken;

    public Sprite SwSprite;
    public Sprite SwSpriteBroken;

    public Sprite NwSprite;
    public Sprite NwSpriteBroken;

    private SpriteRenderer _renderer;

    private void Awake()
    {
        enabled = false;
        _renderer = GetComponentInChildren<SpriteRenderer>();
    }

    private float _remainingDistance;
    private float _speed;
    private GridDirection _direction;

    public void Charge(int distance, float durationPerField, GridDirection direction)
    {
        _remainingDistance = distance * GridVector.DistanceBetweenFields;
        var duration = durationPerField * distance;
        _speed = distance / duration;
        _direction = direction;

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

        this.PlayParallelSound(ref _sources, References.Instance.BowChargeSounds.GetRandomItem());
    }

    public void Launch() => enabled = true;

    private List<AudioSource> _sources = new();

    public void OnLastFieldHit()
    {
        _renderer.sprite = _direction switch
        {
            GridDirection.NorthEast => NeSpriteBroken,
            GridDirection.SouthEast => SeSpriteBroken,
            GridDirection.SouthWest => SwSpriteBroken,
            GridDirection.NorthWest => NwSpriteBroken,
            _ => throw new NotImplementedException(),
        };

        this.PlayParallelSound(ref _sources, References.Instance.BowImpactSounds.GetRandomItem());
    }

    private void Update()
    {
        var delta = _speed * Time.deltaTime * _direction.GetVector();
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
