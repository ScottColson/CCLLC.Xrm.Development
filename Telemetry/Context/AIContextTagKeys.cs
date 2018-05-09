using System.Collections.Generic;

namespace CCLCC.Telemetry.Context
{
    public class AIContextTagKeys : IContextTagKeys
    {
        public IReadOnlyDictionary<string,int> TagSizeLimits { get; private set; }
        public string ComponentName { get; set; }
        public string ComponentVersion { get; set; }
        public string DataRecordId { get; set; }
        public string DataRecordType { get; set; }
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

        public AIContextTagKeys()
        {
            ComponentVersion = "ai.application.ver";
            DeviceId = "ai.device.id";
            DeviceLocale = "ai.device.locale";
            DeviceModel = "ai.device.model";
            DeviceOEMName = "ai.device.oemName";
            DeviceOSVersion = "ai.device.osVersion";
            DeviceType = "ai.device.type";
            LocationIp = "ai.location.ip";
            LocationCountry = "ai.location.country";
            LocationProvince = "ai.location.province";
            LocationCity = "ai.location.city";
            OperationId = "ai.operation.id";
            OperationName = "ai.operation.name";
            OperationParentId = "ai.operation.parentId";
            OperationSyntheticSource = "ai.operation.syntheticSource";
            OperationCorrelationVector = "ai.operation.correlationVector";
            SessionId = "ai.session.id";
            SessionIsFirst = "ai.session.isFirst";
            UserAccountId = "ai.user.accountId";
            UserId = "ai.user.id";
            UserAuthUserId = "ai.user.authUserId";
            CloudRole = "ai.cloud.role";
            CloudRoleInstance = "ai.cloud.roleInstance";
            InternalSdkVersion = "ai.internal.sdkVersion";
            InternalAgentVersion = "ai.internal.agentVersion";
            InternalNodeName = "ai.internal.nodeName";

            this.TagSizeLimits = new Dictionary<string, int>(){
                { ComponentVersion, 1024 },
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
