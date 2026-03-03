# Coordinator / Master Agent

## Overview

The **Coordinator Agent** (also called the Master Agent or Orchestrator) is the central hub of the multi-agent system. It receives high-level goals, decomposes them into sub-tasks, delegates those sub-tasks to the appropriate Specialized Agents, and assembles the results into a coherent final output.

---

## Responsibilities

| Responsibility | Description |
|---|---|
| **Task Decomposition** | Breaks a complex user request into discrete, actionable sub-tasks. |
| **Agent Routing** | Selects the correct Specialized Agent(s) for each sub-task based on capability metadata. |
| **Result Aggregation** | Collects partial results from agents and synthesizes a unified response. |
| **Workflow Coherence** | Detects conflicts or gaps between agent outputs and triggers re-planning when needed. |
| **Human Escalation** | Pauses the workflow and requests human review when confidence is below a threshold. |

---

## Example Scenario – Retail Operations

**User Goal:** *"Give me a weekly operational report for our flagship store."*

```
CoordinatorAgent
├── → MarketAnalysisAgent   (retrieve market trends for the week)
├── → InventoryAgent        (fetch low-stock and overstock alerts)
├── → CustomerProfileAgent  (summarise top customer segments)
└── ← Aggregates outputs → generates "Weekly Operational Report"
```

---

## Pseudocode Skeleton

```python
class CoordinatorAgent:
    def __init__(self, agent_registry, memory_store):
        self.registry = agent_registry   # map of capability → agent
        self.memory  = memory_store      # persistent context (see memory-and-context.md)

    def run(self, goal: str) -> str:
        sub_tasks = self._decompose(goal)
        results   = {}

        for task in sub_tasks:
            agent   = self.registry.get_agent_for(task)
            results[task.id] = agent.execute(task, context=self.memory.get_context())

        return self._aggregate(results)

    def _decompose(self, goal: str) -> list[Task]:
        # Use an LLM planner or rule-based system to break goal into tasks
        ...

    def _aggregate(self, results: dict) -> str:
        # Merge partial outputs into a final coherent response
        ...
```

---

## Key Design Decisions

- **Stateless vs. Stateful:** The coordinator itself is stateless; all session data lives in the Memory & Context layer.
- **Synchronous vs. Asynchronous:** Sub-tasks that are independent run in parallel; dependent tasks run sequentially.
- **Retry & Fallback:** Failed agent calls are retried up to *N* times before the coordinator falls back to a default response or escalates to a human.

---

## Related Components

- [Specialized Agents](./specialized-agents.md)
- [Agent Communication Protocols](./agent-communication-protocols.md)
- [Memory & Context](./memory-and-context.md)
- [Security & Governance](./security-and-governance.md)
