using UnityEngine;

public interface IReferences
{
    AudioClip[] SwordAttackSounds { get; }
    AudioClip[] RangedSounds { get; }
    AudioClip[] PlayerMoveSounds { get; }
    AudioClip[] TakeDamageSounds { get; }
}

public class References : Singleton<References, IReferences>, IReferences
{
    [SerializeField]
    private AudioClip[] _swordAttackSounds;
    public AudioClip[] SwordAttackSounds => _swordAttackSounds;

    [SerializeField]
    private AudioClip[] _rangedSounds;
    public AudioClip[] RangedSounds => _rangedSounds;

    [SerializeField]
    private AudioClip[] _playerMoveSounds;
    public AudioClip[] PlayerMoveSounds => _playerMoveSounds;

    [SerializeField]
    private AudioClip[] _takeDamageSounds;
    public AudioClip[] TakeDamageSounds => _takeDamageSounds;
}
