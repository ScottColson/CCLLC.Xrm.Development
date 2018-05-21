# CCLLC.Xrm.Development
Provides a set of classes to simplify Xrm Plugin and WorkflowActivity development.


# Assembly: CCLLCIoCContainer
Provides a simple IoC container to simplify dependency injection within other assemblies.

# Assembly: CCLLCTelemetry
Provides a telemetry system that is compatible with Application Insights but can also be used with a custom telemetry endpoint. This work is an alternate implementation of the Microsoft Application Insights core SDK that is compatible with operating inside a Dynamics CRM sandboxed process.

# Assembly: CCLLCXrmSdk
Provides a base plugin implementation with telemetry support and extensions for caching and configuration management. Depends on CCLLCIoCContainer and CCLLCTelemetry.

# Assembly: CCLLCXrmWorkflow
Provides a base workflow activity implementation with telemetry support and extensions for caching and configuration management. Depends on Depends on CCLLCIoCContainer, CCLLCTelemetry, and CCLLCXrmSdk.

# Assembly: CCLLCXrmUtilities
Provides a set of commonly used functions used in Xrm Plugin and WorkflowActivity development.