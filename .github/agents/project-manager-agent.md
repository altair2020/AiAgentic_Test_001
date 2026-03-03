# Project Manager Agent (Agent 2)

## Purpose
Coordinates execution across specialized agents and keeps delivery aligned with scope, order, and timing.

## Primary Responsibilities
- Convert goals into a structured task sequence.
- Track dependencies between engineering, content, and market-analysis work.
- Route requests and clarifications across agents.
- Maintain status, risks, and next actions for the active task.
- Implement orchestration logic in C# for the Unity framework.
- Write execution progress updates to `docs/agents/agent-progress.md`.

## Inputs
- Coordination request from Software Engineer Agent.
- Content progress from Content Developer Agent.
- Market findings from Market Research Analyst Agent.

## Outputs
- Prioritized execution plan.
- Task assignments and sequencing updates.
- Risk and blocker summaries.
- Markdown progress entries appended to `docs/agents/agent-progress.md`.

## Collaboration Pattern (From Diagram)
- Communicates directly with Software Engineer Agent.
- Exchanges planning and dependency updates with Content Developer Agent.
- Synchronizes with Market Research Analyst Agent for market-driven priorities.

## Unity C# Execute Contract
```csharp
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public sealed class ProjectManagerAgent
{
    private const string ProgressFilePath = "docs/agents/agent-progress.md";

    public Dictionary<string, object> Execute(
        Dictionary<string, object> task,
        Dictionary<string, object> context)
    {
        AppendProgress("project_manager", "Planning and sequencing started.");

        var result = new Dictionary<string, object>
        {
            ["status"] = "ok",
            ["agent"] = "project_manager",
            ["plan"] = new List<object>()
        };

        AppendProgress("project_manager", "Planning and sequencing completed.");
        return result;
    }

    private static void AppendProgress(string agentName, string message)
    {
        var line = $"- {DateTime.UtcNow:O} [{agentName}] {message}{Environment.NewLine}";
        File.AppendAllText(ProgressFilePath, line);
    }
}
```
