using System.Collections;
using UnityEngine;

public interface IProjectile
{
    void Prepell(int distance, float durationPerField, Vector3 direction);
}

public class Projectile : MonoBehaviour, IProjectile
{
    private void Awake()
    {
        enabled = false;
    }

    private float _remainingDistance;
    private float _speed;
    private Vector2 _direction;

    public void Prepell(int distance, float durationPerField, Vector3 direction)
    {
        _remainingDistance = distance * GridVector.DistanceBetweenFields;
        var duration = durationPerField * distance;
        _speed = distance / duration;
        _direction = direction;
        enabled = true;
    }

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
