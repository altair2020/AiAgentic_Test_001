# Software Engineer Agent (Agent 1)

## Purpose
Turns the user prompt into an implementation-ready technical plan and final solution package.

## Primary Responsibilities
- Parse the incoming prompt and identify technical scope.
- Break down work into implementation tasks for supporting agents.
- Integrate outputs from Project Manager, Content Developer, and Market Research Analyst.
- Produce the final technical response returned to the multi-agent application.
- Implement agent runtime logic in C# for the Unity framework.
- Write execution progress updates to `docs/agents/agent-progress.md`.

## Inputs
- User prompt from the multi-agent application.
- Project plan and sequencing from Project Manager Agent.
- Draft content from Content Developer Agent.
- Market context and validation from Market Research Analyst Agent.

## Outputs
- Consolidated technical solution.
- Final response payload to the application.
- Follow-up technical questions when requirements are ambiguous.
- Markdown progress entries appended to `docs/agents/agent-progress.md`.

## Collaboration Pattern (From Diagram)
- Receives the initial prompt directly from the application.
- Exchanges coordination messages with Project Manager Agent.
- Consumes downstream artifacts influenced by Content Developer and Market Research Analyst.
- Sends the final response back to the application.

## Unity C# Execute Contract
```csharp
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public sealed class SoftwareEngineerAgent
{
    private const string ProgressFilePath = "docs/agents/agent-progress.md";

    public Dictionary<string, object> Execute(
        Dictionary<string, object> task,
        Dictionary<string, object> context)
    {
        AppendProgress("software_engineer", "Started technical synthesis.");

        var result = new Dictionary<string, object>
        {
            ["status"] = "ok",
            ["agent"] = "software_engineer",
            ["result"] = new Dictionary<string, object>()
        };

        AppendProgress("software_engineer", "Completed technical synthesis.");
        return result;
    }

    private static void AppendProgress(string agentName, string message)
    {
        var line = $"- {DateTime.UtcNow:O} [{agentName}] {message}{Environment.NewLine}";
        File.AppendAllText(ProgressFilePath, line);
    }
}
```
