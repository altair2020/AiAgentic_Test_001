# Tool Access Layer

## Overview

The **Tool Access Layer** is the secure gateway through which agents interact with external systems—databases, REST APIs, LLM model endpoints, file stores, and more. Rather than each agent holding raw credentials and calling external services directly, all access is brokered through this layer, which enforces authentication, authorization, rate-limiting, and audit logging.

---

## Architecture

```
Agent
  │
  │  MCP-formatted request  (see agent-communication-protocols.md)
  ▼
┌─────────────────────────────────┐
│        Tool Access Layer        │
│  ┌──────────┐  ┌─────────────┐  │
│  │  Auth /  │  │  Rate       │  │
│  │  IAM     │  │  Limiter    │  │
│  └──────────┘  └─────────────┘  │
│  ┌──────────────────────────┐   │
│  │    Tool Registry         │   │
│  │  web_search │ wms_api    │   │
│  │  crm_api    │ llm_model  │   │
│  └──────────────────────────┘   │
└─────────────────────────────────┘
  │
  ▼
External Systems (APIs, Databases, Models)
```

---

## Tool Registry

Each tool is registered with its endpoint, required permission scope, and schema:

```python
TOOL_REGISTRY = {
    "web_search": Tool(
        endpoint   = "https://api.search.example.com/v1/search",
        auth_type  = "api_key",
        permission = "read:web",
        input_schema  = {"query": "string"},
        output_schema = {"results": "list[string]"},
    ),
    "wms_api": Tool(
        endpoint   = "https://wms.internal/api/v2/stock",
        auth_type  = "oauth2",
        permission = "read:inventory",
        input_schema  = {"sku_list": "list[string]"},
        output_schema = {"levels": "dict[string, int]"},
    ),
    "crm_api": Tool(
        endpoint   = "https://crm.internal/api/customers",
        auth_type  = "oauth2",
        permission = "read:crm",
        input_schema  = {"filter": "dict"},
        output_schema = {"customers": "list[dict]"},
    ),
    "llm_model": Tool(
        endpoint   = "https://models.internal/v1/chat",
        auth_type  = "managed_identity",
        permission = "invoke:llm",
        input_schema  = {"messages": "list[dict]"},
        output_schema = {"content": "string"},
    ),
}
```

---

## Tool Execution Example

```python
class ToolAccessLayer:
    def __init__(self, tool_registry, iam_client, rate_limiter, audit_logger):
        self.registry     = tool_registry
        self.iam          = iam_client
        self.rate_limiter = rate_limiter
        self.audit        = audit_logger

    def call(self, tool_name: str, params: dict, agent_id: str, permissions: list[str]) -> dict:
        tool = self.registry[tool_name]

        # 1. Authorisation check
        if tool.permission not in permissions:
            raise PermissionError(f"Agent '{agent_id}' lacks '{tool.permission}'")

        # 2. Rate limiting
        self.rate_limiter.check(agent_id, tool_name)

        # 3. Audit log (before call)
        self.audit.log(agent_id=agent_id, tool=tool_name, params=params, event="call_start")

        # 4. Execute
        token    = self.iam.get_token(tool.auth_type)
        response = http_post(tool.endpoint, json=params, token=token)

        # 5. Audit log (after call)
        self.audit.log(agent_id=agent_id, tool=tool_name, event="call_end", status=response.status)

        return response.json()
```

---

## MCP Server Integration

When using the **Model Context Protocol**, the Tool Access Layer acts as an MCP Server:

```
Agent  →  MCP request  →  Tool Access Layer (MCP Server)  →  External System
```

The MCP server validates the `permissions` array in the incoming context object before routing the `tool_call` to the correct backend.

---

## Security Considerations

- Credentials are **never** passed to agents; agents only hold a short-lived token scoped to their permitted tools.
- All parameters are **validated against the registered schema** before the external call is made.
- Responses are **sanitized** to strip any data the requesting agent is not authorized to see.
- See [Security & Governance](./security-and-governance.md) for full IAM policies.

---

## Related Components

- [Agent Communication Protocols](./agent-communication-protocols.md)
- [Specialized Agents](./specialized-agents.md)
- [Security & Governance](./security-and-governance.md)
