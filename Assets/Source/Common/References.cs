using UnityEngine;

public interface IReferences
{
    AudioClip[] SwordAttackSounds { get; }
    AudioClip[] RangedSounds { get; }
    AudioClip[] PlayerMoveSounds { get; }
    AudioClip[] TakeDamageSounds { get; }
    AudioClip[] DeathScreamSounds { get; }
    AudioClip DirectionalArrowHover { get; }
    AudioClip DirectionalArrowClick { get; }
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

    [SerializeField]
    private AudioClip[] _deathScreamSounds;
    public AudioClip[] DeathScreamSounds => _deathScreamSounds;

    [SerializeField]
    private AudioClip _directionalArrowHover;
    public AudioClip DirectionalArrowHover => _directionalArrowHover;

    [SerializeField]
    private AudioClip _directionalArrowClick;
    public AudioClip DirectionalArrowClick => _directionalArrowClick;
}
