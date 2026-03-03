# Agent Communication Protocols

## Overview

For agents to collaborate reliably they must share a **common language**—agreed-upon message formats, transport mechanisms, and discovery rules. This project uses two complementary standards:

| Protocol | Layer | Purpose |
|---|---|---|
| **Agent2Agent (A2A)** | Agent ↔ Agent | Peer-to-peer task hand-off and status reporting between agents. |
| **Model Context Protocol (MCP)** | Agent ↔ Tool/Server | Standardised, secure access to external tools, databases, and models. |

---

## Agent2Agent (A2A)

A2A defines how one agent sends a task to another and receives a structured result.

### Message Schema

```json
{
  "a2a_version": "1.0",
  "message_id": "msg-abc123",
  "sender":     "CoordinatorAgent",
  "recipient":  "InventoryAgent",
  "type":       "task_request",
  "payload": {
    "task_id":  "task-xyz456",
    "goal":     "Report stock levels for SKUs [1001, 1002, 1003]",
    "deadline": "2025-04-01T12:00:00Z"
  }
}
```

### Response Schema

```json
{
  "a2a_version": "1.0",
  "message_id": "msg-def789",
  "in_reply_to": "msg-abc123",
  "sender":     "InventoryAgent",
  "recipient":  "CoordinatorAgent",
  "type":       "task_result",
  "status":     "success",
  "payload": {
    "task_id": "task-xyz456",
    "result":  { "1001": 42, "1002": 0, "1003": 120 }
  }
}
```

### Transport

- **In-process:** Direct Python/function call (development / single-process deployments).
- **HTTP REST:** Each agent exposes a `/tasks` endpoint; messages sent as JSON over HTTPS.
- **Message Queue:** Async delivery via a broker (e.g., Azure Service Bus, RabbitMQ) for resilience.

---

## Model Context Protocol (MCP)

MCP standardizes how agents attach *context* when calling external tools or LLM servers.

### MCP Context Object

```json
{
  "mcp_version": "1.0",
  "session_id":  "sess-aaa111",
  "agent_id":    "MarketAnalysisAgent",
  "permissions": ["read:market_data", "read:competitor_prices"],
  "history": [
    { "role": "user",  "content": "Analyse Q1 2025 retail trends." },
    { "role": "agent", "content": "Fetching data from market feed…" }
  ],
  "tool_call": {
    "tool":   "web_search",
    "params": { "query": "Q1 2025 retail market trends" }
  }
}
```

### How Agents Use MCP

```python
class MCPClient:
    def call_tool(self, tool_name: str, params: dict, context: MCPContext) -> dict:
        request = {
            "mcp_version": "1.0",
            "session_id":  context.session_id,
            "agent_id":    context.agent_id,
            "permissions": context.permissions,
            "tool_call":   {"tool": tool_name, "params": params},
        }
        response = self.http_client.post(f"/mcp/tools/{tool_name}", json=request)
        return response.json()
```

---

## Interoperability Rules

1. **Versioning:** Always include `a2a_version` / `mcp_version` fields to allow gradual upgrades.
2. **Idempotency:** Task requests carry a unique `message_id`; duplicate deliveries are safely ignored.
3. **Timeouts:** Every request includes a `deadline`; agents return a `timeout` status if exceeded.
4. **Validation:** Senders validate outbound messages against a JSON Schema before sending.

---

## Related Components

- [Coordinator/Master Agent](./coordinator-agent.md)
- [Tool Access Layer](./tool-access-layer.md)
- [Memory & Context](./memory-and-context.md)
