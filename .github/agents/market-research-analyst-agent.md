# Market Research Analyst Agent (Agent 4)

## Purpose
Provides market intelligence, audience insights, and competitive context to improve decision quality.

## Primary Responsibilities
- Analyze market signals and user needs relevant to the task.
- Validate assumptions and identify opportunity or risk factors.
- Supply evidence that informs planning, implementation, and messaging.
- Maintain traceable insight notes for downstream agents.
- Implement market-analysis workflow logic in C# for the Unity framework.
- Write execution progress updates to `docs/agents/agent-progress.md`.

## Inputs
- Research goals from Project Manager Agent.
- Product or feature context from Software Engineer Agent.
- Messaging targets from Content Developer Agent.

## Outputs
- Insight reports and recommendation summaries.
- Priority signals for planning adjustments.
- Audience and competitive notes for content and implementation.
- Markdown progress entries appended to `docs/agents/agent-progress.md`.

## Collaboration Pattern (From Diagram)
- Participates in a two-way collaboration loop with Project Manager Agent.
- Feeds market insights into Software Engineer Agent decision-making.
- Supports Content Developer Agent with audience and positioning context.

## Unity C# Execute Contract
```csharp
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public sealed class MarketResearchAnalystAgent
{
    private const string ProgressFilePath = "docs/agents/agent-progress.md";

    public Dictionary<string, object> Execute(
        Dictionary<string, object> task,
        Dictionary<string, object> context)
    {
        AppendProgress("market_research_analyst", "Market analysis started.");

        var result = new Dictionary<string, object>
        {
            ["status"] = "ok",
            ["agent"] = "market_research_analyst",
            ["insights"] = new List<object>()
        };

        AppendProgress("market_research_analyst", "Market analysis completed.");
        return result;
    }

    private static void AppendProgress(string agentName, string message)
    {
        var line = $"- {DateTime.UtcNow:O} [{agentName}] {message}{Environment.NewLine}";
        File.AppendAllText(ProgressFilePath, line);
    }
}
```
