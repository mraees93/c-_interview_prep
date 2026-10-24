// using System;
// using System.Collections.Generic;

// public class UserProfile { public string UserId { get; set; } = string.Empty; public string PreferencesJson { get; set; } = string.Empty; }

// public interface IUserPreferencePipeline
// {
//     void ExtractThemeSettings(UserProfile user);
//     void ModifySecurityTokens(UserProfile user);
//     void BroadcastUpdateToClients(UserProfile user);
// }

// public interface ILogger1
// {
//     abstract static void LogPipelineMetric(string action, string uid, string SessionId);
// }

// public abstract class User
// {
//     protected string SessionId { get; set; }
//     public List<string> TrackedPreferences { get; set; } = new List<string>();
// }

// public class StandardSubscriberPipeline : User, ILogger1
// {
//     protected string SessionId { get; set; } = "Sub_Session_99";
//     public List<string> TrackedPreferences { get; set; } = new List<string>();

//     public static void LogPipelineMetric(string action, string uid, string SessionId) => Console.WriteLine($"[METRIC] Session {SessionId} ran {action} for User {uid}");

//     public void ExtractThemeSettings(UserProfile user)
//     {
//         LogPipelineMetric("Theme_Extraction", user.UserId);
//         TrackedPreferences.Add("DarkMode=True");
//     }

//     public void ModifySecurityTokens(UserProfile user)
//     {
//         LogPipelineMetric("Token_Modification", user.UserId);
//         user.PreferencesJson += ";TokenExpiry=3600";
//     }

//     public void BroadcastUpdateToClients(UserProfile user) => Console.WriteLine("Broadcasting real-time SignalR packet to client browsers.");
// }

// public class GuestReadOnlyPipeline : IUserPreferencePipeline
// {
//     public string SessionId { get; set; } = "Guest_Session_01";
//     public List<string> TrackedPreferences { get; set; } = new List<string>();

//     public void LogPipelineMetric(string action, string uid) => Console.WriteLine($"[METRIC] Session {SessionId} ran {action} for User {uid}");

//     public void ExtractThemeSettings(UserProfile user)
//     {
//         LogPipelineMetric("Theme_Extraction", user.UserId);
//         TrackedPreferences.Add("AnonymousDefaultTheme");
//     }

//     public void ModifySecurityTokens(UserProfile user) => throw new InvalidOperationException("Security Breach: Guest sessions are strictly prohibited from generating or modifying security tokens!");

//     public void BroadcastUpdateToClients(UserProfile user) => Console.WriteLine("Broadcasting real-time SignalR packet to client browsers.");
// }
