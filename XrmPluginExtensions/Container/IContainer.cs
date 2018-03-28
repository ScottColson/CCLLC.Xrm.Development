
namespace CCLCC.XrmPluginExtensions.Container
{
    public interface IContainer
    {
        void Register<TContract, TImplementation>() where TImplementation : TContract;

        void RegisterAsSingleInstance<TContract, TImplementation>() where TImplementation : TContract;

        T Resolve<T>();

        bool IsRegistered<TContract>();
    }
}
