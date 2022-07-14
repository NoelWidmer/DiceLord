using UnityEngine;

public abstract class Singleton<TSelf, TInterface> : MonoBehaviour where TSelf : Singleton<TSelf, TInterface>, TInterface
{
    public static TInterface Instance { get; private set; }
    protected static TSelf ConcreteInstance { get; private set; }

    protected virtual DisableBehaviour DisableBehaviour
        => DisableBehaviour.DisableOnAwake;

    protected void Awake()
    {
        if (ConcreteInstance != null)
        {
            if (ReferenceEquals(this, ConcreteInstance))
            {
                Debug.LogWarning($"A {GetType().Name}'s awake method was called multiple times.");
            }
            else
            {
                Debug.LogWarning($"A duplicate {GetType().Name} was found. The duplicate will be destroyed immediatly.");
                Destroy(this);
            }
        }
        else if (this is TSelf self)
        {
            ConcreteInstance = self;
            Instance = self;

            if (DisableBehaviour == DisableBehaviour.DisableOnAwake)
            {
                enabled = false;
            }

            OnAwake();
        }
        else
        {
            Debug.LogError($"Singleton of type {GetType().Name} claims to be of type {typeof(TSelf).Name} but isn't. It will be destroyed immediatly.");
            Destroy(this);
        }
    }

    protected virtual void OnAwake()
    { }
}