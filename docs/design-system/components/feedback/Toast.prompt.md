Floating notification card, stacked top-right of the app — posture alerts, blink reminders, system messages (break started, snooze used, etc).

```jsx
<Toast icon={<ChevronsUp size={16}/>} title="¡Espalda Erguida!" body="Hemos detectado inclinación en tu cuello." tone="amber" onClose={dismiss} />
```

Tone maps to alert type: `primary` (blink/pink), `amber` (posture), `neutral` (system). Auto-dismiss after a few seconds in production; keep max ~4 stacked.
