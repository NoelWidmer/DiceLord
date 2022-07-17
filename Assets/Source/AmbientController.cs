public interface IAmbientController
{ }

public class AmbientController : Singleton<AmbientController, IAmbientController>, IAmbientController
{
    // empty on purpose
}
