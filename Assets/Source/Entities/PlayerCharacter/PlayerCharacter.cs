using UnityEngine;

public interface IPlayerCharacter : IEntity
{
}

public class PlayerCharacter : Entity, IPlayerCharacter
{
}
