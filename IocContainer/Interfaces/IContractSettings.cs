namespace CCLLC.Core
{
    public interface IContractSettings
    {
        /// <summary>
        /// Register the implementation as a single instance. The container will issue to same instance on each contract resolution.
        /// </summary>
        /// <returns></returns>
        IContractSettings AsSingleInstance();

        /// <summary>
        /// Overwrite any existing implementation registration for the contract.
        /// </summary>
        /// <returns></returns>
        IContractSettings WithOverwrite();
    }
}
