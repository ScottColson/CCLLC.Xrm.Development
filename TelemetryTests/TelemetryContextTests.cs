using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CCLCC.Telemetry;
using CCLCC.Telemetry.Context;

namespace TelemetryTests
{
    [TestClass]
    public class TelemetryContextTests
    {
        private DataKeyTelemetryContext generateInitiatedDataKeyTelemetryContext()
        {
            var context = new DataKeyTelemetryContext();

            context.Cloud.RoleInstance = "cloud.roleinstance";
            context.Cloud.RoleName = "cloud.rolename";

            context.Component.Name = "component.name";
            context.Component.Version = "component.version";

            context.Data.RecordId = "data.recordid";
            context.Data.RecordType = "data.recordtype";

            context.Device.Id = "device.id";
            context.Device.Model = "device.model";
            context.Device.OemName = "device.oemname";
            context.Device.OperatingSystem = "device.operatingsytem";
            context.Device.Type = "device.type";

            context.InstrumentationKey = "key";

            context.Internal.AgentVersion = "internal.agentversion";
            context.Internal.NodeName = "internal.nodename";
            context.Internal.SdkVersion = "internal.sdkversion";

            context.Location.Ip = "location.ip";

            context.Operation.CorrelationVector = "operation.vector";
            context.Operation.Id = "operation.id";
            context.Operation.Name = "operation.name";
            context.Operation.ParentId = "operation.parentid";


            context.Session.Id = "session.id";
            context.Session.IsFirst = true;

            context.User.AccountId = "user.accountid";
            context.User.AuthenticatedUserId = "user.authenticateduserid";
            context.User.Id = "user.id";
            context.User.UserAgent = "user.agent";

            context.Properties.Add("custom-1", "value-1");
            context.Properties.Add("custom-2", "value-2");

            return context;
        }

        private TelemetryContext generateInitiatedTelemetryContext()
        {
            var context = new TelemetryContext();

            context.Cloud.RoleInstance = "cloud.roleinstance";
            context.Cloud.RoleName = "cloud.rolename";

            context.Component.Name = "component.name";
            context.Component.Version = "component.version";

            context.Device.Id = "device.id";
            context.Device.Model = "device.model";
            context.Device.OemName = "device.oemname";
            context.Device.OperatingSystem = "device.operatingsytem";
            context.Device.Type = "device.type";

            context.InstrumentationKey = "key";

            context.Internal.AgentVersion = "internal.agentversion";
            context.Internal.NodeName = "internal.nodename";
            context.Internal.SdkVersion = "internal.sdkversion";

            context.Location.Ip = "location.ip";

            context.Operation.CorrelationVector = "operation.vector";
            context.Operation.Id = "operation.id";
            context.Operation.Name = "operation.name";
            context.Operation.ParentId = "operation.parentid";


            context.Session.Id = "session.id";
            context.Session.IsFirst = true;

            context.User.AccountId = "user.accountid";
            context.User.AuthenticatedUserId = "user.authenticateduserid";
            context.User.Id = "user.id";
            context.User.UserAgent = "user.agent";

            context.Properties.Add("custom-1", "value-1");
            context.Properties.Add("custom-2", "value-2");

            return context;
        }


        [TestMethod]
        public void CanCreateTelemetryContext()
        {
            var context = new TelemetryContext();

            //implements ITelemetryContext
            Assert.IsInstanceOfType(context, typeof(ITelemetryContext));

            //does not implement ISupportDataKeyContext
            Assert.IsNotInstanceOfType(context, typeof(ISupportDataKeyContext));

        }

        [TestMethod]
        public void CanCreateDataKeyTelemetryContext()
        {
            ITelemetryContext context = new DataKeyTelemetryContext();

            //should be instance of TelemetryContext
            Assert.IsInstanceOfType(context, typeof(TelemetryContext));

            //implements ITelemetryContext
            Assert.IsInstanceOfType(context, typeof(ITelemetryContext));

            //implements ISupportDataKeyContext
            Assert.IsInstanceOfType(context, typeof(ISupportDataKeyContext));
        }


        [TestMethod]
        public void NewTelemetryContextIsNotInitialized()
        {
            var context = new TelemetryContext();

            Assert.IsNull(context.Cloud.RoleInstance);
            Assert.IsNull(context.Cloud.RoleName);

            Assert.IsNull(context.Component.Name);
            Assert.IsNull(context.Component.Version);

            Assert.IsNull(context.Device.Id);
            Assert.IsNull(context.Device.Model);
            Assert.IsNull(context.Device.OemName);
            Assert.IsNull(context.Device.OperatingSystem);
            Assert.IsNull(context.Device.Type);

            Assert.IsNull(context.InstrumentationKey);

            Assert.IsNull(context.Internal.AgentVersion);
            Assert.IsNull(context.Internal.NodeName);
            Assert.IsNull(context.Internal.SdkVersion);

            Assert.IsNull(context.Location.Ip);

            Assert.IsNull(context.Operation.CorrelationVector);
            Assert.IsNull(context.Operation.Id);
            Assert.IsNull(context.Operation.Name);
            Assert.IsNull(context.Operation.ParentId);

            Assert.AreEqual(0, context.Properties.Count);

            Assert.IsNull(context.Session.Id);
            Assert.IsNull(context.Session.IsFirst);

            Assert.IsNull(context.User.AccountId);
            Assert.IsNull(context.User.AuthenticatedUserId);
            Assert.IsNull(context.User.Id);
            Assert.IsNull(context.User.UserAgent);
        }

        [TestMethod]
        public void NewDataKeyTelemetryContextIsNotInitialized()
        {
            var context = new DataKeyTelemetryContext();

            Assert.IsNull(context.Cloud.RoleInstance);
            Assert.IsNull(context.Cloud.RoleName);

            Assert.IsNull(context.Component.Name);
            Assert.IsNull(context.Component.Version);

            Assert.IsNull(context.Data.RecordId);
            Assert.IsNull(context.Data.RecordType);

            Assert.IsNull(context.Device.Id);
            Assert.IsNull(context.Device.Model);
            Assert.IsNull(context.Device.OemName);
            Assert.IsNull(context.Device.OperatingSystem);
            Assert.IsNull(context.Device.Type);

            Assert.IsNull(context.InstrumentationKey);

            Assert.IsNull(context.Internal.AgentVersion);
            Assert.IsNull(context.Internal.NodeName);
            Assert.IsNull(context.Internal.SdkVersion);

            Assert.IsNull(context.Location.Ip);

            Assert.IsNull(context.Operation.CorrelationVector);
            Assert.IsNull(context.Operation.Id);
            Assert.IsNull(context.Operation.Name);
            Assert.IsNull(context.Operation.ParentId);

            Assert.AreEqual(0, context.Properties.Count);

            Assert.IsNull(context.Session.Id);
            Assert.IsNull(context.Session.IsFirst);

            Assert.IsNull(context.User.AccountId);
            Assert.IsNull(context.User.AuthenticatedUserId);
            Assert.IsNull(context.User.Id);
            Assert.IsNull(context.User.UserAgent);
        }


        [TestMethod]
        public void CanCloneUninitializedTelemetryContext()
        {
            var context = new TelemetryContext();

            var clone = context.DeepClone();

            Assert.IsInstanceOfType(clone, typeof(TelemetryContext));
            Assert.AreNotSame(context, clone);
            Assert.AreNotSame(context.Cloud, clone.Cloud);
            Assert.AreNotSame(context.Component, clone.Component);
            Assert.AreNotSame(context.Device, clone.Device);
            Assert.AreNotSame(context.Internal, clone.Internal);
            Assert.AreNotSame(context.Location, clone.Location);
            Assert.AreNotSame(context.Operation, clone.Operation);
            Assert.AreNotSame(context.Properties, clone.Properties);
            Assert.AreNotSame(context.Session, clone.Session);
            Assert.AreNotSame(context.User, clone.User);


        }


        [TestMethod]
        public void CanCloneUninitializedDataKeyTelemetryContext()
        {
            var context = new DataKeyTelemetryContext();

            var clone = context.DeepClone();

            Assert.IsInstanceOfType(clone, typeof(DataKeyTelemetryContext));
            Assert.AreNotSame(context, clone);
            Assert.AreNotSame(context.Cloud, clone.Cloud);
            Assert.AreNotSame(context.Component, clone.Component);
            Assert.AreNotSame(context.Data, (clone as ISupportDataKeyContext).Data);
            Assert.AreNotSame(context.Device, clone.Device);
            Assert.AreNotSame(context.Internal, clone.Internal);
            Assert.AreNotSame(context.Location, clone.Location);
            Assert.AreNotSame(context.Operation, clone.Operation);
            Assert.AreNotSame(context.Properties, clone.Properties);
            Assert.AreNotSame(context.Session, clone.Session);
            Assert.AreNotSame(context.User, clone.User);


        }


        [TestMethod]
        public void CanCloneTelemetryContext()
        {
            var context = generateInitiatedTelemetryContext();

            var clone = context.DeepClone();

            Assert.IsInstanceOfType(clone, typeof(TelemetryContext));
            Assert.AreNotSame(context, clone);
            Assert.AreNotSame(context.Cloud, clone.Cloud);
            Assert.AreNotSame(context.Component, clone.Component);
            Assert.AreNotSame(context.Device, clone.Device);
            Assert.AreNotSame(context.Internal, clone.Internal);
            Assert.AreNotSame(context.Location, clone.Location);
            Assert.AreNotSame(context.Operation, clone.Operation);
            Assert.AreNotSame(context.Properties, clone.Properties);
            Assert.AreNotSame(context.Session, clone.Session);
            Assert.AreNotSame(context.User, clone.User);

            Assert.AreEqual(context.Cloud.RoleInstance, clone.Cloud.RoleInstance);
            Assert.AreEqual(context.Cloud.RoleName, clone.Cloud.RoleName);

            Assert.AreEqual(context.Component.Name, clone.Component.Name);
            Assert.AreEqual(context.Component.Version, clone.Component.Name);

            Assert.AreEqual(context.Device.Id, clone.Device.Id);
            Assert.AreEqual(context.Device.Model, clone.Device.Model);
            Assert.AreEqual(context.Device.OemName, clone.Device.OemName);
            Assert.AreEqual(context.Device.OperatingSystem, clone.Device.OperatingSystem);
            Assert.AreEqual(context.Device.Type, clone.Device.Type);

            Assert.AreEqual(context.InstrumentationKey, clone.InstrumentationKey);

            Assert.AreEqual(context.Internal.AgentVersion, clone.Internal.AgentVersion);
            Assert.AreEqual(context.Internal.NodeName, clone.Internal.NodeName);
            Assert.AreEqual(context.Internal.SdkVersion, clone.Internal.SdkVersion);

            Assert.AreEqual(context.Location.Ip, clone.Location.Ip);

            Assert.AreEqual(context.Operation.CorrelationVector, clone.Operation.CorrelationVector);
            Assert.AreEqual(context.Operation.Id, clone.Operation.Id);
            Assert.AreEqual(context.Operation.Name, clone.Operation.Name);
            Assert.AreEqual(context.Operation.ParentId, clone.Operation.ParentId);

            Assert.AreEqual(context.Properties.Count, clone.Properties.Count);

            Assert.AreEqual(context.Session.Id, clone.Session.Id);
            Assert.AreEqual(context.Session.IsFirst, clone.Session.IsFirst);

            Assert.AreEqual(context.User.AccountId, clone.User.AccountId);
            Assert.AreEqual(context.User.AuthenticatedUserId, clone.User.AuthenticatedUserId);
            Assert.AreEqual(context.User.Id, clone.User.Id);
            Assert.AreEqual(context.User.UserAgent, clone.User.UserAgent);
        }


        [TestMethod]
        public void CanCloneDataKeyTelemetryContext()
        {
            var context = generateInitiatedDataKeyTelemetryContext();

            var clone = context.DeepClone();

            Assert.IsInstanceOfType(clone, typeof(DataKeyTelemetryContext));
            Assert.AreNotSame(context, clone);
            Assert.AreNotSame(context.Cloud, clone.Cloud);
            Assert.AreNotSame(context.Component, clone.Component);
            Assert.AreNotSame(context.Data, (clone as ISupportDataKeyContext).Data);
            Assert.AreNotSame(context.Device, clone.Device);
            Assert.AreNotSame(context.Internal, clone.Internal);
            Assert.AreNotSame(context.Location, clone.Location);
            Assert.AreNotSame(context.Operation, clone.Operation);
            Assert.AreNotSame(context.Properties, clone.Properties);
            Assert.AreNotSame(context.Session, clone.Session);
            Assert.AreNotSame(context.User, clone.User);

            Assert.AreEqual(context.Cloud.RoleInstance, clone.Cloud.RoleInstance);
            Assert.AreEqual(context.Cloud.RoleName, clone.Cloud.RoleName);

            Assert.AreEqual(context.Component.Name, clone.Component.Name);
            Assert.AreEqual(context.Component.Version, clone.Component.Name);

            Assert.AreEqual(context.Data.RecordId, (clone as ISupportDataKeyContext).Data.RecordId);
            Assert.AreEqual(context.Data.RecordType, (clone as ISupportDataKeyContext).Data.RecordType);

            Assert.AreEqual(context.Device.Id, clone.Device.Id);
            Assert.AreEqual(context.Device.Model, clone.Device.Model);
            Assert.AreEqual(context.Device.OemName, clone.Device.OemName);
            Assert.AreEqual(context.Device.OperatingSystem, clone.Device.OperatingSystem);
            Assert.AreEqual(context.Device.Type, clone.Device.Type);

            Assert.AreEqual(context.InstrumentationKey, clone.InstrumentationKey);

            Assert.AreEqual(context.Internal.AgentVersion, clone.Internal.AgentVersion);
            Assert.AreEqual(context.Internal.NodeName, clone.Internal.NodeName);
            Assert.AreEqual(context.Internal.SdkVersion, clone.Internal.SdkVersion);

            Assert.AreEqual(context.Location.Ip, clone.Location.Ip);

            Assert.AreEqual(context.Operation.CorrelationVector, clone.Operation.CorrelationVector);
            Assert.AreEqual(context.Operation.Id, clone.Operation.Id);
            Assert.AreEqual(context.Operation.Name, clone.Operation.Name);
            Assert.AreEqual(context.Operation.ParentId, clone.Operation.ParentId);

            Assert.AreEqual(context.Properties.Count, clone.Properties.Count);

            Assert.AreEqual(context.Session.Id, clone.Session.Id);
            Assert.AreEqual(context.Session.IsFirst, clone.Session.IsFirst);

            Assert.AreEqual(context.User.AccountId, clone.User.AccountId);
            Assert.AreEqual(context.User.AuthenticatedUserId, clone.User.AuthenticatedUserId);
            Assert.AreEqual(context.User.Id, clone.User.Id);
            Assert.AreEqual(context.User.UserAgent, clone.User.UserAgent);
        }


        [TestMethod]
        public void CanGenerateAITelemetryContextTags()
        {
            var context = generateInitiatedTelemetryContext();
            var tags = context.ToContextTags(new AIContextTagKeys());
        }
        
    }
}
