Pill on/off switch — every boolean setting in EyeYul (dim screen, pause music, office hours, context exclusions) uses this, never a checkbox.

```jsx
<Toggle checked={settings.automations.dimScreen} onChange={(v) => update({dimScreen: v})} />
```

`size="sm"` for dense settings rows; default `md` for standalone toggles. Always pair with a label + one-line description to its left (see the automations UI kit screen).
