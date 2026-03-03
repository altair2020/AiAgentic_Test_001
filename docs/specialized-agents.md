# Specialized Agents

## Overview

**Specialized Agents** are focused workers that each own a single domain of expertise. Unlike a general-purpose assistant, a Specialized Agent is scoped, fine-tuned (or system-prompted), and granted only the tools it needs to perform its job—following the principle of least privilege.

---

## Example Agents

### 1. Market Analysis Agent

| Attribute | Value |
|---|---|
| **Purpose** | Identify market trends, competitor pricing, and demand signals. |
| **Tools** | Web search API, financial data feed, internal sales database. |
| **Output** | Structured JSON report: `{ trend, confidence, source[] }` |

```python
class MarketAnalysisAgent:
    def execute(self, task: Task, context: dict) -> dict:
        raw_data = self.tools.web_search(task.query)
        return self._summarise(raw_data, context)
```

---

### 2. Inventory Tracking Agent

| Attribute | Value |
|---|---|
| **Purpose** | Monitor stock levels, flag shortages, and suggest reorder quantities. |
| **Tools** | Warehouse Management System (WMS) API, ERP connector. |
| **Output** | `{ sku, current_stock, reorder_point, suggested_order_qty }` |

```python
class InventoryAgent:
    def execute(self, task: Task, context: dict) -> dict:
        levels = self.tools.wms_api.get_stock(task.sku_list)
        return self._flag_alerts(levels)
```

---

### 3. Customer Profiling Agent

| Attribute | Value |
|---|---|
| **Purpose** | Segment customers, identify high-value accounts, and surface churn risk. |
| **Tools** | CRM API, analytics platform, embeddings model. |
| **Output** | `{ segment, lifetime_value_estimate, churn_risk_score }` |

```python
class CustomerProfileAgent:
    def execute(self, task: Task, context: dict) -> dict:
        crm_data = self.tools.crm_api.get_customers(task.filter)
        return self._profile(crm_data, context)
```

---

## Agent Registration

Every agent advertises its capabilities so the Coordinator can route tasks correctly:

```python
AGENT_REGISTRY = {
    "market_analysis":   MarketAnalysisAgent(tools=market_tools),
    "inventory":         InventoryAgent(tools=inventory_tools),
    "customer_profile":  CustomerProfileAgent(tools=crm_tools),
}
```

---

## Adding a New Specialized Agent

1. Create a class that implements the `execute(task, context) -> dict` interface.
2. Register it in `AGENT_REGISTRY` with a unique capability key.
3. Grant it only the tool permissions it requires (see [Tool Access Layer](./tool-access-layer.md)).
4. Document its input/output schema so the Coordinator can validate data at boundaries.

---

## Related Components

- [Coordinator/Master Agent](./coordinator-agent.md)
- [Tool Access Layer](./tool-access-layer.md)
- [Agent Communication Protocols](./agent-communication-protocols.md)
