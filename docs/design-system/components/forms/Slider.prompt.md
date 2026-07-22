Labeled range slider — used for "Tiempo de Trabajo", "Duración del Descanso", and snooze limit adjustments.

```jsx
<Slider label="Tiempo de Trabajo" value={20} min={5} max={90} step={5} unit=" minutos" onChange={setValue} />
```

Value + unit render mono/bold in the accent color above the track, matching the app's live numeric readouts.
