// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;

// public class TelemetryMetricsService
// {
//     private readonly Dictionary<string, double> _cachedMetrics = new Dictionary<string, double>
//     {
//         { "CPU_Load", 42.5 },
//         { "Memory_Usage", 78.1 }
//     };

//     public async ValueTask<double> GetMetricsDataAsync(string metricKey)
//     {
//         if (_cachedMetrics.TryGetValue(metricKey, out double cachedValue))
//         {
//             return await ValueTask.FromResult(cachedValue);
//         }

//         return await FetchMetricsFromRemoteCloudAsync(metricKey);
//     }

//     private async ValueTask<double> FetchMetricsFromRemoteCloudAsync(string metricKey)
//     {
//         await Task.Delay(1000);
//         return await ValueTask.FromResult(55.4);
//     }
// }

// CORRECT WAY

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class TelemetryMetricsService
{
    private readonly Dictionary<string, double> _cachedMetrics = new Dictionary<string, double>
    {
        { "CPU_Load", 42.5 },
        { "Memory_Usage", 78.1 }
    };

    public async ValueTask<double> GetMetricsDataAsync(string metricKey)
    {
        if (_cachedMetrics.TryGetValue(metricKey, out double cachedValue))
        {
            return cachedValue;
        }

        return await FetchMetricsFromRemoteCloudAsync(metricKey);
    }

    private async ValueTask<double> FetchMetricsFromRemoteCloudAsync(string metricKey)
    {
        await Task.Delay(1000);
        return 55.4;
    }
}
