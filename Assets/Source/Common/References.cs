using UnityEngine;

public interface IReferences
{
    AudioClip[] SwordAttackSounds { get; }

    AudioClip[] BowChargeSounds { get; }
    AudioClip[] BowImpactSounds { get; }
    AudioClip[] AoEAttackSounds { get; }
    AudioClip[] PlayerMoveSounds { get; }

    AudioClip[] TakeDamageSounds { get; }
    AudioClip[] DeathScreamSounds { get; }

    AudioClip[] CubeManTakeDamageSounds { get; }
    AudioClip[] CubeManScreamSounds { get; }

    AudioClip[] EyeTakeDamageSounds { get; }
    AudioClip[] EyeScreamSounds { get; }

    AudioClip[] SlimeTakeDamageSounds { get; }
    AudioClip[] SlimeScreamSounds { get; }

    AudioClip[] SlimeSmallTakeDamageSounds { get; }
    AudioClip[] SlimeSmallScreamSounds { get; }

    AudioClip DirectionalArrowHover { get; }
    AudioClip DirectionalArrowClick { get; }


    AudioClip MoveBlockedSound { get; }
}

public class References : Singleton<References, IReferences>, IReferences
{
    [Header("Attack")]
    [SerializeField]
    private AudioClip[] _swordAttackSounds;
    public AudioClip[] SwordAttackSounds => _swordAttackSounds;

    [Header("Bow")]
    [SerializeField]
    private AudioClip[] _bowChargeSounds;
    public AudioClip[] BowChargeSounds => _bowChargeSounds;

    [SerializeField]
    private AudioClip[] _bowImpactSounds;
    public AudioClip[] BowImpactSounds => _bowImpactSounds;

    [Header("AoE")]
    [SerializeField]
    private AudioClip[] _aoeAttackSounds;
    public AudioClip[] AoEAttackSounds => _aoeAttackSounds;

    [Header("Move")]
    [SerializeField]
    private AudioClip[] _playerMoveSounds;
    public AudioClip[] PlayerMoveSounds => _playerMoveSounds;

    [Header("Player Character")]
    [SerializeField]
    private AudioClip[] _takeDamageSounds;
    public AudioClip[] TakeDamageSounds => _takeDamageSounds;

    [SerializeField]
    private AudioClip[] _deathScreamSounds;
    public AudioClip[] DeathScreamSounds => _deathScreamSounds;

    [Header("Dice Knight")]
    [SerializeField]
    private AudioClip[] _cubeManTakeDamageSounds;
    public AudioClip[] CubeManTakeDamageSounds => _cubeManTakeDamageSounds;

    [SerializeField]
    private AudioClip[] _cubeManScreamSounds;
    public AudioClip[] CubeManScreamSounds => _cubeManScreamSounds;

    [Header("Eye")]
    [SerializeField]
    private AudioClip[] _eyeTakeDamageSounds;
    public AudioClip[] EyeTakeDamageSounds => _eyeTakeDamageSounds;

    [SerializeField]
    private AudioClip[] _eyeScreamSounds;
    public AudioClip[] EyeScreamSounds => _eyeScreamSounds;

    [Header("Big Slime")]
    [SerializeField]
    private AudioClip[] _slimeDamageSounds;
    public AudioClip[] SlimeTakeDamageSounds => _slimeDamageSounds;

    [SerializeField]
    private AudioClip[] _slimeScreamSounds;
    public AudioClip[] SlimeScreamSounds => _slimeScreamSounds;

    [Header("Small Slime")]
    [SerializeField]
    private AudioClip[] _slimeSmallDamageSounds;
    public AudioClip[] SlimeSmallTakeDamageSounds => _slimeSmallDamageSounds;

    [SerializeField]
    private AudioClip[] _slimeSmallScreamSounds;
    public AudioClip[] SlimeSmallScreamSounds => _slimeSmallScreamSounds;

    [Header("Direction Arrows")]
    [SerializeField]
    private AudioClip _directionalArrowHover;
    public AudioClip DirectionalArrowHover => _directionalArrowHover;

    [SerializeField]
    private AudioClip _directionalArrowClick;
    public AudioClip DirectionalArrowClick => _directionalArrowClick;

    [SerializeField]
    private AudioClip _moveBlockedSound;
    public AudioClip MoveBlockedSound => _moveBlockedSound;
}
