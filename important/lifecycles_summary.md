DI Container - main kitchen db board

object lifecycle management (lifescycle modifiers): T, S, S

Transient: disposable cup - a short-lived object that is created every single time it is requested by a classes constructor and disposed when that task completes - Mapping utilities - automapper

Scoped: jug of water - a short lived object/instance that is created on every single browser HTTP web request and disposed when that web request ends. - Over the network DBContext

Singleton: configurable passage geyser heater - long-lived object/instance that is created on program.cs startup boot and shared globally by all taps - BackgroundService, Serilog logging, redis caching

Traps:

You can add 3 instances of same singleton - its an anti-pattern adding unecessary objects on the heap taking up memory space, last one gets used.

Adding a short-lived transient/scoped service/instance inside a long-lived singleton instance creates a captive dependency, the jug of water poisons the configurable passage geyser heater - trapping/holding hostage the short-lived dependency inside the long-lived sigleton forever. 
Leaking db sockets and triggering an eventually fatal connection pool startvation crash.

If you have to add short-lived transient or scoped inside a long-lived singleton (if a singleton must query the db) yoou have to inject an IServiceScopeFactory inside the singletons constructor. This micro-scoped boundary that disposes of the connection immediately upon method completion.

The singleton has to be thread-safe but it can either be stateful(mutable) or stateless(immutable - readonly)

TODO