using UnityEngine;

public interface IReferences
{
    AudioClip[] SwordAttackSounds { get; }
}

public class References : Singleton<References, IReferences>, IReferences
{
    // add global references here.
    // then you can access them from anywhere using References.Instance.[Property].
    [SerializeField]
    private AudioClip[] _swordAttackSounds;
    public AudioClip[] SwordAttackSounds => _swordAttackSounds;
}
