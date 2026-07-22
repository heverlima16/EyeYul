Circular icon-only button — use for close (X), dismiss, and toolbar glyph actions where a label would be redundant.

```jsx
<IconButton icon={<X size={16} />} title="Cerrar" onClick={close} />
```

`active` highlights it with the terracotta accent (e.g. a selected wallpaper swatch). Always pass `title` for accessibility since there's no visible label.
