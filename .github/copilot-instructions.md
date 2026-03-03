# GitHub Copilot Instructions – Multi-Agent AI System

This file configures GitHub Copilot's behaviour for this repository. Copilot should follow these guidelines when generating code, documentation, and suggestions.

---

## Project Context

This repository is a **placeholder / reference implementation** of a **multi-agent AI system** built around six core components:

1. **Coordinator/Master Agent** – Orchestrates task delegation and result aggregation.
2. **Specialized Agents** – Domain-specific workers (market analysis, inventory, customer profiling).
3. **Agent Communication Protocols** – A2A and MCP message standards for inter-agent and agent-tool communication.
4. **Tool Access Layer** – Secure, audited gateway for agent access to external APIs and databases.
5. **Memory & Context** – Persistent session, working, and long-term memory stores.
6. **Security & Governance** – IAM policies, HITL oversight, content safety filters, and audit trails.

---

## Coding Style

- Language: **Python 3.11+** for agent logic; JSON for message schemas.
- Type hints are **required** on all function signatures.
- Use **dataclasses** or **Pydantic models** for structured data (tasks, messages, context objects).
- Follow **PEP 8** formatting; max line length 100 characters.
- Prefer **dependency injection** over global state—pass `memory_store`, `tool_registry`, etc. as constructor arguments.
- Every public method should have a **docstring** describing its purpose, parameters, and return value.

---

## Naming Conventions

| Concept | Convention | Example |
|---|---|---|
| Agent class | `PascalCase` + `Agent` suffix | `MarketAnalysisAgent` |
| Tool name | `snake_case` | `web_search`, `wms_api` |
| MCP/A2A message type | `snake_case` string | `"task_request"`, `"task_result"` |
| Permission scope | `action:resource` | `"read:inventory"`, `"invoke:llm"` |
| Session / memory key | `snake_case` prefixed by type | `sess_`, `mem_`, `task_` |

---

## Architecture Rules

- **Agents must not call external services directly.** All external calls go through the `ToolAccessLayer`.
- **Agents must not share credentials.** Authentication is handled by the `ToolAccessLayer` using managed identities or short-lived tokens.
- **All agent inputs and outputs must pass through `ContentSafetyFilter`** before being forwarded or returned.
- **HITL gate must be checked** before any action flagged as `high_impact=True`.
- **Session state lives in `MemoryStore`**, not in agent instance variables.

---

## Suggested File Layout

```
docs/
  coordinator-agent.md          # Core component examples
  specialized-agents.md
  agent-communication-protocols.md
  tool-access-layer.md
  memory-and-context.md
  security-and-governance.md
src/
  agents/
    coordinator.py
    market_analysis.py
    inventory.py
    customer_profile.py
  protocols/
    a2a.py                      # A2A message schemas
    mcp.py                      # MCP context & client
  tools/
    tool_access_layer.py
    tool_registry.py
  memory/
    memory_store.py
  security/
    iam.py
    hitl_gate.py
    content_safety.py
    audit_logger.py
tests/
  test_coordinator.py
  test_specialized_agents.py
  test_protocols.py
  test_tool_access_layer.py
  test_memory_store.py
  test_security.py
```

---

## When Adding a New Agent

1. Create a class in `src/agents/` implementing the `execute(task: Task, context: dict) -> dict` interface.
2. Register it in `AGENT_REGISTRY` with a unique capability key.
3. Define its IAM policy (permitted `actions` and `resources`) in `src/security/iam.py`.
4. Grant only the tool permissions it needs in `TOOL_REGISTRY`.
5. Add integration tests in `tests/test_<agent_name>.py`.
6. Document it in `docs/specialized-agents.md`.

---

## When Adding a New Tool

1. Register the tool in `TOOL_REGISTRY` with its endpoint, auth type, permission scope, and I/O schema.
2. Ensure the corresponding IAM policy grants at least one agent `read:` or `invoke:` access to the new permission.
3. Add a schema validation test to `tests/test_tool_access_layer.py`.

---

## Documentation Standards

- Every new component should have a corresponding `.md` file in `docs/` following the style of existing files.
- Code examples in docs must be syntactically correct and consistent with `src/` implementations.
- Use **Markdown tables** for structured comparisons; use **fenced code blocks** with a language tag for all code samples.

---

## Security Reminders (Copilot-specific)

- **Do not** generate code that embeds API keys, passwords, or secrets as string literals.
- **Do not** generate code that bypasses the `ToolAccessLayer` or `ContentSafetyFilter`.
- **Always** include an IAM permission check when generating new tool-call code.
- Flag any suggested pattern that grants an agent `invoke:all_tools` unless it is explicitly the `ToolAccessLayer`.

---

## References

- [Coordinator/Master Agent](./docs/coordinator-agent.md)
- [Specialized Agents](./docs/specialized-agents.md)
- [Agent Communication Protocols](./docs/agent-communication-protocols.md)
- [Tool Access Layer](./docs/tool-access-layer.md)
- [Memory & Context](./docs/memory-and-context.md)
- [Security & Governance](./docs/security-and-governance.md)
