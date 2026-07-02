// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;

// // Assume these models are defined elsewhere in your system
// public class SupportTicket 
// { 
//     public int Id { get; set; } 
//     public string Title { get; set; } = string.Empty;
//     public string Priority { get; set; } = "Low"; // "Low", "High", "Urgent"
//     public string Status { get; set; } = "Open";
// }

// public class FileLogger
// {
//     public void LogInfo(string message) => Console.WriteLine($"[FILE LOG]: {message}");
//     public void LogError(string message) => Console.Error.WriteLine($"[FILE ERROR]: {message}");
// }

// interface ITicket
// {
//     Task ProcessTicketAsync();
// }

// public class TicketProcessor
// {
//     private readonly FileLogger _logger;

//     public TicketProcessor()
//     {
//         _logger = new FileLogger();
//     }

//     // --------------------------- OCP VIOLATION -- create interface for different priorities then implement those different interfaces on 4 priority classes------------------------------------------------
//     // public virtual async Task ProcessTicketAsync(SupportTicket ticket)
//     // {
//     //     _logger.LogInfo($"Starting processing for ticket {ticket.Id}");

//     //     if (ticket.Priority == "Urgent")
//     //     {
//     //         // Urgent tickets require instant SMS routing
//     //         Console.WriteLine($"[SMS NOTIFICATION] Sent emergency alert for ticket {ticket.Id}");
//     //     }
//     //     else if (ticket.Priority == "High")
//     //     {
//     //         // High priority goes to email queue
//     //         Console.WriteLine($"[EMAIL NOTIFICATION] Sent queue update for ticket {ticket.Id}");
//     //     }
//     //     else
//     //     {
//     //         // Low priority gets basic dashboard logging
//     //         Console.WriteLine($"[DASHBOARD NOTIFICATION] Ticket {ticket.Id} queued.");
//     //     }

//     //     ticket.Status = "Processing";
//     //     _logger.LogInfo($"Ticket {ticket.Id} status updated to Processing");
//     //     await Task.CompletedTask;
//     // }

//     public class UrgentTicket : ITicket
//     {
//         private readonly FileLogger _logger;

//         public UrgentTicket()
//         {
//             _logger = new FileLogger();
//         }
//         public async Task ProcessTicketAsync(SupportTicket ticket)
//         {
//             Console.WriteLine($"[SMS NOTIFICATION] Sent emergency alert for ticket {ticket.Id}");
//             ticket.Status = "Processing";
//             _logger.LogInfo($"Ticket {ticket.Id} status updated to Processing");
//             await Task.CompletedTask;
//         }
//     }
// }

// public class ExecutiveTicketProcessor : TicketProcessor
// {
//     //------------------------- 
//     public override async Task ProcessTicketAsync(SupportTicket ticket)
//     {
//         if (ticket.Priority != "Urgent")
//         {
//             throw new ArgumentException("Executive processor can only handle Urgent tickets!");
//         }

//         await base.ProcessTicketAsync(ticket);
//     }
// }
