using System.Collections.Generic;

namespace CCLCC.Telemetry
{
    public interface IContextTagKeys
    {
        IReadOnlyDictionary<string,int> TagSizeLimits { get; }
        string ComponentName { get; set; }
        string ComponentVersion { get; set; }
        string DataRecordId { get; set; }
        string DataRecordType { get; set; }
        string DataRecordSource { get; set; }
        string DataRecordAltKeyName { get; set; }
        string DataRecordAltKeyValue { get; set; }

        string DeviceId { get; set; }
        string DeviceLocale { get; set; }
        string DeviceModel { get; set; }
        string DeviceOEMName { get; set; }
        string DeviceOSVersion { get; set; }
        string DeviceType { get; set; }
        string LocationIp { get; set; }
        string LocationCountry { get; set; }
        string LocationProvince { get; set; }
        string LocationCity { get; set; }
        string OperationId { get; set; }
        string OperationName { get; set; }
        string OperationParentId { get; set; }
        string OperationSyntheticSource { get; set; }
        string OperationCorrelationVector { get; set; }
        string SessionId { get; set; }
        string SessionIsFirst { get; set; }
        string UserAccountId { get; set; }
        string UserId { get; set; }
        string UserAuthUserId { get; set; }
        string UserAgent { get; set; }
        string CloudRole { get; set; }
        string CloudRoleInstance { get; set; }
        string InternalSdkVersion { get; set; }
        string InternalAgentVersion { get; set; }
        string InternalNodeName { get; set; }
    }
}
