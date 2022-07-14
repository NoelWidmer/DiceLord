public interface IReferences
{ }

public class References : Singleton<References, IReferences>, IReferences
{
    // add global references here.
    // then you can access them from anywhere using References.Instance.[Property].
}
