# Security & Governance

## Overview

**Security & Governance** ensures that the multi-agent system operates safely, transparently, and in compliance with organizational and regulatory requirements. It covers identity & access management (IAM), human-in-the-loop (HITL) oversight, content safety filters, and audit trails.

---

## 1. Identity & Access Management (IAM)

Every agent and tool interaction is governed by a least-privilege IAM policy.

### Agent Identity

Each agent is issued a **Managed Identity** (or service account) with a scoped permission set:

| Agent | Allowed Permissions |
|---|---|
| `CoordinatorAgent` | `delegate:tasks`, `read:session` |
| `MarketAnalysisAgent` | `read:web`, `read:market_data` |
| `InventoryAgent` | `read:inventory`, `write:reorder_request` |
| `CustomerProfileAgent` | `read:crm` |
| `ToolAccessLayer` | `invoke:all_tools` (acts as broker) |

### IAM Policy Example

```json
{
  "policy_id": "pol-inventory-agent",
  "principal": "InventoryAgent",
  "effect":    "Allow",
  "actions":   ["read:inventory", "write:reorder_request"],
  "resources": ["arn:wms:stock:*", "arn:erp:orders:reorder"],
  "conditions": {
    "time_of_day": "08:00-20:00 UTC",
    "max_requests_per_minute": 60
  }
}
```

---

## 2. Human-in-the-Loop (HITL) Oversight

Certain decisions are too high-stakes for autonomous agent action. The HITL mechanism pauses the workflow and routes the decision to a human reviewer.

### Trigger Conditions

| Condition | Example |
|---|---|
| Confidence below threshold | Agent confidence < 0.70 on a financial recommendation |
| High-impact action | Reorder quantity exceeds \$50,000 |
| Conflicting agent outputs | MarketAgent and InventoryAgent disagree on demand signal |
| Flagged content | Content safety filter raises a warning |

### HITL Flow

```
CoordinatorAgent
    │
    │  confidence=0.62 → below threshold
    ▼
HumanReviewQueue  ──→  Reviewer Dashboard
    │                        │
    │  ← Approved / Rejected ┘
    ▼
CoordinatorAgent resumes
```

```python
class HITLGate:
    def check(self, action: dict, confidence: float) -> str:
        if confidence < self.threshold or action.get("high_impact"):
            review_id = self.queue.submit(action)
            decision  = self.queue.wait_for_decision(review_id, timeout=3600)
            return decision   # "approved" | "rejected" | "modified"
        return "approved"
```

---

## 3. Content Safety Filters

All agent inputs and outputs pass through a content safety pipeline before being acted upon or returned to the user.

### Filter Pipeline

```
Raw Text
  │
  ├─ [1] PII Detector      → mask/redact personal data
  ├─ [2] Toxicity Classifier → block harmful content
  ├─ [3] Prompt Injection Guard → detect jailbreak attempts
  └─ [4] Data Leakage Check → ensure no internal data surfaces externally
  │
Sanitised Text
```

```python
class ContentSafetyFilter:
    def screen(self, text: str, direction: str = "output") -> dict:
        results = {
            "pii":       self.pii_detector.scan(text),
            "toxicity":  self.toxicity_model.score(text),
            "injection": self.injection_guard.detect(text),
        }
        if any(r["flagged"] for r in results.values()):
            return {"safe": False, "reasons": results}
        return {"safe": True, "text": text}
```

---

## 4. Audit Trail

Every agent action, tool call, HITL decision, and content safety event is written to an immutable audit log.

### Audit Log Entry

```json
{
  "log_id":      "log-ccc333",
  "timestamp":   "2025-04-01T10:05:32Z",
  "session_id":  "sess-aaa111",
  "agent_id":    "InventoryAgent",
  "event_type":  "tool_call",
  "tool":        "wms_api",
  "params_hash": "sha256:abc…",
  "outcome":     "success",
  "latency_ms":  142,
  "reviewer":    null
}
```

> **Note:** Raw parameters are hashed (not stored) in the log to prevent sensitive data from appearing in audit records.

---

## 5. Compliance Checklist

- [ ] All agents assigned a Managed Identity with least-privilege permissions.
- [ ] HITL gate configured with appropriate confidence thresholds.
- [ ] Content safety filter enabled on all agent inputs and outputs.
- [ ] Audit logs retained for minimum 90 days and stored in write-once storage.
- [ ] PII encrypted at rest and in transit (TLS 1.2+).
- [ ] Regular IAM policy reviews scheduled (quarterly minimum).

---

## Related Components

- [Tool Access Layer](./tool-access-layer.md)
- [Memory & Context](./memory-and-context.md)
- [Coordinator/Master Agent](./coordinator-agent.md)
