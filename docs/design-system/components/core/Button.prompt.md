Pill-shaped button used for all primary/secondary/ghost actions across EyeYul — use whenever the UI needs a tappable action (start break, save setting, confirm).

```jsx
<Button variant="primary" icon={<Zap size={14} />}>Descansar Ya</Button>
<Button variant="secondary">Reiniciar</Button>
<Button variant="ghost">Saltar Descanso</Button>
```

Variants: `primary` (terracotta, filled), `inverse` (near-black/near-white, used for hero CTAs like "Comenzar"), `secondary` (subtle bordered), `ghost` (text-only, used inside dark overlays). Sizes: `sm | md | lg`. Set `pill={false}` for the rarer rounded-rect treatment. Pass `theme="dark"` when placing on a dark surface so secondary/ghost contrast correctly.
