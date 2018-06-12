using System.Collections.Generic;

namespace CCLLC.Telemetry.Context
{
    /// <summary>
    /// Provides context tags for a generic implementation that includes data key context information 
    /// that is not supported by Microsoft Application Insights.
    /// </summary>
    public class ContextTagKeys : IContextTagKeys
    {
        public IReadOnlyDictionary<string, int> TagSizeLimits { get; private set; }
        public string ComponentName { get; set; }
        public string ComponentVersion { get; set; }
        public string DataRecordId { get; set; }
        public string DataRecordType { get; set; }
        public string DataRecordSource { get; set; }
        public string DataRecordAltKeyName { get; set; }
        public string DataRecordAltKeyValue { get; set; }
        public string DeviceId { get; set; }
        public string DeviceLocale { get; set; }
        public string DeviceModel { get; set; }
        public string DeviceOEMName { get; set; }
        public string DeviceOSVersion { get; set; }
        public string DeviceType { get; set; }
        public string LocationIp { get; set; }
        public string LocationCountry { get; set; }
        public string LocationProvince { get; set; }
        public string LocationCity { get; set; }
        public string OperationId { get; set; }
        public string OperationName { get; set; }
        public string OperationParentId { get; set; }
        public string OperationSyntheticSource { get; set; }
        public string OperationCorrelationVector { get; set; }
        public string SessionId { get; set; }
        public string SessionIsFirst { get; set; }
        public string UserAccountId { get; set; }
        public string UserId { get; set; }
        public string UserAuthUserId { get; set; }
        public string UserAgent { get; set; }
        public string CloudRole { get; set; }
        public string CloudRoleInstance { get; set; }
        public string InternalSdkVersion { get; set; }
        public string InternalAgentVersion { get; set; }
        public string InternalNodeName { get; set; }

        public ContextTagKeys()
        {
            ComponentVersion = "component.ver";
            ComponentName = "component.name";
            DeviceId = "device.id";
            DataRecordAltKeyName = "data.altkeyname";
            DataRecordAltKeyValue = "data.altkeyvalue";
            DataRecordId = "data.recordid";
            DataRecordSource = "data.recordsource";
            DataRecordType = "data.recordtype";
            DeviceLocale = "device.locale";
            DeviceModel = "device.model";
            DeviceOEMName = "device.oemName";
            DeviceOSVersion = "device.osVersion";
            DeviceType = "device.type";
            LocationIp = "location.ip";
            LocationCountry = "location.country";
            LocationProvince = "location.province";
            LocationCity = "location.city";
            OperationId = "operation.id";
            OperationName = "operation.name";
            OperationParentId = "operation.parentId";
            OperationSyntheticSource = "operation.syntheticSource";
            OperationCorrelationVector = "operation.correlationVector";
            SessionId = "session.id";
            SessionIsFirst = "session.isFirst";
            UserAccountId = "user.accountId";
            UserId = "user.id";
            UserAuthUserId = "user.authUserId";
            CloudRole = "cloud.role";
            CloudRoleInstance = "cloud.roleInstance";
            InternalSdkVersion = "internal.sdkVersion";
            InternalAgentVersion = "internal.agentVersion";
            InternalNodeName = "internal.nodeName";

            this.TagSizeLimits = new Dictionary<string, int>(){
                { ComponentVersion, 256 },
                { ComponentName, 256 },
                { DataRecordAltKeyName, 256 },
                { DataRecordAltKeyValue, 1024 },
                { DataRecordId, 256 },
                { DataRecordSource, 256 },
                { DataRecordType, 256 },
                { DeviceId, 1024 },
                { DeviceModel, 256 },
                { DeviceOEMName, 256 },
                { DeviceOSVersion, 256 },
                { DeviceType, 64 },
                { LocationIp, 45 },
                { OperationId, 128 },
                { OperationName, 1024 },
                { OperationParentId, 128 },
                { OperationSyntheticSource, 1024 },
                { OperationCorrelationVector, 64 },
                { SessionId, 64 },
                { UserId, 128 },
                { UserAccountId, 1024 },
                { UserAuthUserId, 1024 },
                { CloudRole, 256 },
                { CloudRoleInstance, 256 },
                { InternalSdkVersion, 64 },
                { InternalAgentVersion, 64 },
                { InternalNodeName, 256 }
            };
        }

    }
}
