using System;
using System.Collections.Generic;

public class SqlCitationCluster : IVerdict
{
    public List<string> FetchVerdictsByAct(string actName)
    {
        return new List<string> { "State v. Makwanyane", "S v. Zuma" };
    }
}

public interface IVerdict
{
    List<string> FetchVerdictsByAct(string actName);
}

public class CitationVerificationEngine
{
    private readonly IVerdict _verdict;

    public CitationVerificationEngine(IVerdict verdict)
    {
        _verdict = verdict;
    }

    public void VerifyDocumentCitations(string legalBriefText, string relevantAct)
    {
        Console.WriteLine("Analyzing text payload for valid historical case citations...");
        List<string> officialVerdicts = _verdict.FetchVerdictsByAct(relevantAct);
        
        foreach (var verdict in officialVerdicts)
        {
            if (legalBriefText.Contains(verdict))
            {
                Console.WriteLine($"Match validated: Linked to official precedent -> {verdict}");
            }
        }
    }
}
