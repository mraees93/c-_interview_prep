/*
using System;

public class TrialPayload
{
    public string CaseNumber { get; set; } = string.Empty;
    public string RequiredJurisdiction { get; set; } = string.Empty;
}

public class LocalSqlServerConnection
{
    public void ExecuteScheduleCommand(string caseNum, string judgeName)
    {
        Console.WriteLine($"[SQL SERVER] Scheduled Case {caseNum} under Judge {judgeName}.");
    }
}

public class SchedulingEngine
{
    private readonly LocalSqlServerConnection _dbConnection;

    public SchedulingEngine()
    {
        _dbConnection = new LocalSqlServerConnection();
    }

    public void AllocateJudgeToTrial(TrialPayload payload, string allocationModel)
    {
        Console.WriteLine($"Evaluating allocation queue for Case: {payload.CaseNumber}...");

        string assignedJudge = "Default Magistrate";

        if (allocationModel == "StandardCivil")
        {
            assignedJudge = "Judge Pillay (High Court)";
        }
        else if (allocationModel == "CommercialArbitration")
        {
            assignedJudge = "Advocate Ndlovu (Senior Counsel)";
        }
        else if (allocationModel == "SupremeCourtAppeal")
        {
            throw new NotSupportedException("Supreme Court appeals require a multi-judge panel; single-judge allocation is blocked!");
        }

        _dbConnection.ExecuteScheduleCommand(payload.CaseNumber, assignedJudge);
    }
}
*/

public class TrialPayload
{
    public string CaseNumber { get; set; } = string.Empty;
    public string RequiredJurisdiction { get; set; } = string.Empty;
}

public interface IDatabaseConnection
{
    void ExecuteScheduleCommand(string caseNum, string judgeName);
}

public class LocalSqlServerConnection : IDatabaseConnection
{
    public void ExecuteScheduleCommand(string caseNum, string judgeName)
    {
        Console.WriteLine($"[SQL SERVER] Scheduled Case {caseNum} under Judge {judgeName}.");
    }
}

public interface ITrialAllocationHandler 
{
    public string GetAssignedJudge(TrialPayload payload);
}

class StandardCivil : ITrialAllocationHandler 
{
    public string GetAssignedJudge(TrialPayload payload)
    {
        return "Judge Pillay (High Court)";
    }
}

class CommercialArbitration : ITrialAllocationHandler 
{
    public string GetAssignedJudge(TrialPayload payload)
    {
        return "Advocate Ndlovu (Senior Counsel)";
    }
}

public class SchedulingEngine
{
    private readonly IDatabaseConnection _dbConnection;

    public SchedulingEngine(IDatabaseConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public void ScheduleJudgeToTrial(TrialPayload payload, ITrialAllocationHandler allocationHandler)
    {
        Console.WriteLine($"Evaluating allocation queue for Case: {payload.CaseNumber}...");

        string judge = allocationHandler.GetAssignedJudge(payload);
        _dbConnection.ExecuteScheduleCommand(payload.CaseNumber, judge);
    }
}
