
namespace CCLCC.Core
{
    public interface IIocContainer
    {
        void Register<TContract, TImplementation>() where TImplementation : TContract;

        void RegisterAsSingleInstance<TContract, TImplementation>() where TImplementation : TContract;

        T Resolve<T>();

        bool IsRegistered<TContract>();

        int Count { get; }
    }
}
