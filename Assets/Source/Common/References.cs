using UnityEngine;

public interface IReferences
{
    AudioClip[] SwordAttackSounds { get; }
    AudioClip[] PlayerMoveSounds { get; }
}

public class References : Singleton<References, IReferences>, IReferences
{
    [SerializeField]
    private AudioClip[] _swordAttackSounds;
    public AudioClip[] SwordAttackSounds => _swordAttackSounds;

    [SerializeField]
    private AudioClip[] _playerMoveSounds;
    public AudioClip[] PlayerMoveSounds => _playerMoveSounds;
}
