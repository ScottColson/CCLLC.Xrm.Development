# CCLCC.Xrm.Development
Provides a set of classes to simplify Xrm Plugin and WorkflowActivity development.


# Assembly: CCLCCIoCContainer
Provides a simple IoC container to simplify dependency injection within other assemblies.

# Assembly: CCLCCTelemetry
Provides a telemetry system that is compatible with Application Insights but can also be used with a custom telemetry endpoint. This work is an alternate implementation of the Microsoft Application Insights core SDK that is compatible with operating inside a Dynamics CRM sandboxed process.

# Assembly: CCLCCXrmSdk
Provides a base plugin implementation with telemetry support and extensions for caching and configuration management. Depends on CCLCCIoCContainer and CCLCCTelemetry.

# Assembly: CCLCCXrmWorkflow
Provides a base workflow activity implementation with telemetry support and extensions for caching and configuration management. Depends on Depends on CCLCCIoCContainer, CCLCCTelemetry, and CCLCCXrmSdk.

# Assembly: CCLCCXrmUtilities
Provides a set of commonly used functions used in Xrm Plugin and WorkflowActivity development.