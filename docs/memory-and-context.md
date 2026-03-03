# Memory & Context

## Overview

**Memory & Context** gives the multi-agent system the ability to remember information across turns, tasks, and sessions. Without persistent memory, every agent invocation starts from scratch; with it, agents can reason over history, personalize responses, and avoid repeating work.

---

## Memory Types

| Type | Scope | Storage | Example Use |
|---|---|---|---|
| **Working Memory** | Single task / turn | In-process dict | Intermediate sub-task results during one coordinator run. |
| **Session Memory** | One user session | Redis / in-memory cache | Conversation history for multi-turn dialogue. |
| **Long-term Memory** | Cross-session | Vector DB + relational DB | Customer preferences, past purchase history, learned facts. |
| **Shared Memory** | Cross-agent | Distributed cache | Inventory snapshot shared between InventoryAgent and CoordinatorAgent. |

---

## Data Model

### Session Record

```json
{
  "session_id":    "sess-aaa111",
  "user_id":       "user-xyz",
  "created_at":    "2025-04-01T09:00:00Z",
  "last_active":   "2025-04-01T09:42:00Z",
  "turn_count":    5,
  "conversation": [
    { "role": "user",  "content": "Show me this week's stock alerts." },
    { "role": "agent", "content": "Here are 3 low-stock items: …" }
  ],
  "agent_scratchpad": {
    "InventoryAgent": { "last_checked_skus": ["1001","1002"] }
  }
}
```

### Long-term Memory Entry

```json
{
  "memory_id":   "mem-bbb222",
  "user_id":     "user-xyz",
  "type":        "preference",
  "content":     "User prefers summaries in bullet-point format.",
  "embedding":   [0.12, -0.34, 0.56, "…"],
  "created_at":  "2025-03-15T14:00:00Z",
  "last_used":   "2025-04-01T09:42:00Z",
  "source":      "CustomerProfileAgent"
}
```

---

## Memory Store Interface

```python
class MemoryStore:
    # ── Session Memory ──────────────────────────────────────────────
    def get_session(self, session_id: str) -> dict: ...
    def save_session(self, session_id: str, data: dict) -> None: ...
    def append_turn(self, session_id: str, role: str, content: str) -> None: ...

    # ── Long-term Memory ────────────────────────────────────────────
    def store_fact(self, user_id: str, fact: str, source_agent: str) -> str: ...
    def recall(self, user_id: str, query: str, top_k: int = 5) -> list[dict]:
        """Semantic search over stored facts using vector embeddings."""
        ...

    # ── Shared / Working Memory ─────────────────────────────────────
    def set(self, key: str, value, ttl_seconds: int = 300) -> None: ...
    def get(self, key: str): ...
```

---

## How Agents Use Memory

```python
class CoordinatorAgent:
    def run(self, goal: str, session_id: str) -> str:
        # Load conversation history for context
        context = self.memory.get_session(session_id)

        # Recall relevant long-term facts
        facts = self.memory.recall(context["user_id"], query=goal)

        # … run sub-tasks, passing context and facts …

        # Persist updated session
        self.memory.append_turn(session_id, role="agent", content=final_response)
        return final_response
```

---

## Retention & Privacy

| Policy | Setting |
|---|---|
| Session TTL | 24 hours (configurable) |
| Long-term fact retention | 90 days (configurable per tenant) |
| PII handling | PII fields encrypted at rest; access restricted to owning agent |
| User data deletion | `memory_store.delete_user(user_id)` removes all records |

---

## Related Components

- [Coordinator/Master Agent](./coordinator-agent.md)
- [Agent Communication Protocols](./agent-communication-protocols.md)
- [Security & Governance](./security-and-governance.md)
