Small uppercase status pill for state labels — "ACTIVO", "PROBAR", "AFK", "OK".

```jsx
<Badge tone="primary">ACTIVO</Badge>
<Badge tone="success">OK</Badge>
```

Always uppercase, mono by default (matches the app's system-status voice). Use `tone="amber"` for suspended/paused/warning states, `neutral` for idle "PROBAR" prompts.
