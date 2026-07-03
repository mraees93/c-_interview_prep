using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SOLID_Principles.practice_snippets.practice_2;

namespace SOLID_Principles.practice_snippets.practice_2


{
public interface INotificationStrategy
{
    Task<Task> SendNotificationAsync(SupportTicket ticket);
}

public class UrgentNotificationStrategy : INotificationStrategy
    {
       public async Task<Task> SendNotificationAsync(SupportTicket ticket) {
            Console.WriteLine($"[SMS NOTIFICATION] Sent emergency alert for ticket {ticket.Id}");
            return Task.CompletedTask;
       }
    }
}

public class FixedTicketProcessor
    {
        private readonly FileLogger _logger;

        public FixedTicketProcessor(FileLogger logger)
    {
        _logger = logger;
    }

    public async Task ProcessTicketAsync(SupportTicket ticket, INotificationStrategy notificationStrategy)
    {
        _logger.LogInfo($"Starting processing for ticket {ticket.Id}");
        await notificationStrategy.SendNotificationAsync(ticket);

        ticket.Status = "Processing";
        _logger.LogInfo($"Ticket {ticket.Id} status updated to Processing");
    }
        
    }