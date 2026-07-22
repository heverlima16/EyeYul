# EyeYul Design System

EyeYul es una app de escritorio (Windows) de bienestar digital para personas que trabajan largas horas frente a la pantalla — programadores, oficinistas, gamers. Vigila el uso de pantalla en segundo plano y ofrece pausas visuales inteligentes (regla 20-20-20), recordatorios de postura y parpadeo, pausas planificadas (almuerzo, estiramientos) y automatizaciones (atenuar pantalla, pausar música, no molestar) — todo en español, con un tono cálido y de cuidado personal, nunca corporativo.

**Fuente:** carpeta de código adjunta `eyeyul/` (React + TypeScript + Tailwind v4 + Framer Motion "motion/react" + lucide-react), generada originalmente vía Google AI Studio (`eyeyul/README.md` referencia `https://ai.studio/apps/323a9a2a-1195-40a1-8deb-7618eb70d0dd`). No se adjuntó Figma ni otro repositorio. No existía una librería de componentes separada en el código — es una sola app monolítica (`App.tsx` + 6 componentes de pantalla) construida a puro Tailwind inline; los componentes de este sistema son una extracción/generalización de los patrones repetidos que sí aparecen en ese código (botones, toggles, badges, tarjetas, etc.), no un catálogo que ya existiera.

## Componentes (`components/`)
- **core/** — `Button`, `IconButton`
- **forms/** — `Toggle`, `Slider`, `WeekdaySelector`
- **navigation/** — `SidebarNavItem`, `SegmentedTabs`
- **feedback/** — `Badge`, `Toast`
- **data-display/** — `Card`, `ProgressRing`, `StatCard`

### Intentional additions
None of the above were invented beyond what the source code visibly reuses — every component maps to a repeated pattern seen at least 3+ times across `App.tsx` and the 6 screen components (e.g. the pill toggle appears ~15 times, the terracotta button ~10 times, the bordered rounded card ~20 times). No extra "usual suspect" primitives (Tabs-as-a-library, Dialog, Tooltip, Avatar, etc.) were added since the source has no modal/tooltip/avatar pattern.

## UI Kit (`ui_kits/eyeyul-app/`)
Interactive click-through recreation of the desktop app: Onboarding wizard, Dashboard (main cockpit), Settings (routine/limits/messages tabs), Wellness reminders, Automations, Stats, and the immersive full-screen Break Overlay.

## Tokens (`tokens/`)
`colors.css`, `typography.css`, `spacing.css`, `fonts.css` — imported by root `styles.css`.

## Assets (`assets/`)
`eyeyul-mark.svg` — the app's in-product mark (a closed, smiling-eye blob), copied verbatim from the SVG paths hardcoded in `App.tsx`/`Onboarding.tsx`. **This is not a separately designed logo file** — EyeYul has no standalone brand-mark asset in the source; this inline SVG is the only visual identity mark that exists. No other images, icons, or illustrations were found in the codebase (`eyeyul/assets/` was empty).

---

## CONTENT FUNDAMENTALS

- **Language:** 100% Spanish (es), with an `en` locale flag defined in settings but no English strings actually written — Spanish is the ground truth.
- **Voice:** informal **tú**, never usted. Warm, encouraging, a little playful — like a caring friend, not a corporate wellness vendor. "¡Espalda Erguida!", "¡Parpadea!", "¡Ponte de Pie!", "¡Todo Listo para Cuidar tus Ojos!".
- **Sentence style:** short, imperative or exclamatory for prompts ("Comenzar", "Descansar Ya", "Cierra los Ojos"); slightly longer, explanatory sentences for descriptions, often citing a mechanism ("La regla 20-20-20 reduce drásticamente la fatiga ocular digital", "Frente a las pantallas parpadeamos un 60% menos").
- **Casing:** Sentence case for headlines and body copy. ALL CAPS is reserved for tiny mono-font system/status labels ("ESTADO DEL SISTEMA", "SALUD VISUAL DIGITAL", "SISTEMA DE SEGURIDAD VISUAL ACTIVO", "PROBAR", "ACTIVO", "AFK") — never for real sentences.
- **Emoji:** essentially none in UI copy — the one exception is a single 👍 used once in an onboarding success icon. Iconography (lucide-react glyphs) does all the expressive work instead of emoji.
- **Numbers & science-lite framing:** copy leans on light physiological facts to justify behavior ("blinking 60% less at screens", "20-20-20 rule") — informative but not clinical/cold.
- **Vibe:** protective, gentle-nudge, never guilt-tripping. Even "strict mode" copy is matter-of-fact ("Modo Estricto Activo: Tu pantalla se encuentra bloqueada por completo") rather than scolding. Footer copyright line: "EYEYUL OCULAR BIOMONITORING FOR WINDOWS & MAC" / "© 2026 EYEYUL WELLNESS INC. TODO REGISTRADO." — mixes an English all-caps tagline with Spanish legal line, reflecting its dev-tool origin.

## VISUAL FOUNDATIONS

- **Color:** primary accent is a warm **terracotta/clay** (`#de7356`, with a full 50–950 ramp) — explicitly swapped in over Tailwind's default pink scale in the source (`index.css` comment: *"Overriding standard pink color scale with Claude terracotta / bronze / clay sienna palette"*). Amber (`#f59e0b`) is the secondary accent for posture/warning states. Supporting hues: emerald (success/"libre"), blue (AFK/inactivity), purple (focus mode) — each used sparingly and only for that one semantic meaning.
- **Neutrals are warm, not cool grey.** Light theme sits on cream/off-white (`#fbf9f6`, `#f5f1ea`) rather than pure white/gray-50. Dark theme is a warm near-black bronze (`#14120e`, `#1d1a15`, `#24211a`) — never blue-black or true `#000`.
- **Type:** `Inter` (UI/body sans), `Space Grotesk` (display — headlines, big numbers, brand wordmark), `JetBrains Mono` (system/status labels, timers, all-caps tags). The mono face carries a lot of the app's "technical/OS" personality — used constantly for tiny uppercase labels and the countdown clock.
- **Spacing & density:** the app is dense and small by desktop-utility standards — body text runs 10–13px, not the 16px web default. Generous internal padding inside cards (16–24px) compensates for the small type so it doesn't feel cramped.
- **Corner radii:** very round throughout — 12–16px on buttons/inputs, 16–24px on cards, 24–32px on major panels (the whole simulated app window), and full pill/capsule shapes for every toggle, tab switcher, and badge. No sharp corners anywhere.
- **Cards:** thin 1px border (no shadow-only cards), soft warm-tinted shadows only on floating/elevated surfaces (the whole app window, toasts, the onboarding modal) — never on inline content cards, which rely on the border alone.
- **Backgrounds:** soft multi-stop gradients used only for full-bleed page/wallpaper backgrounds (3 selectable "Fondo" wallpapers: Amber Peak warm gradient, Nebula cool-blue gradient, Slate Glass neutral gradient) and for the immersive break overlay backdrop. Never gradients on buttons or cards — those stay flat/solid.
- **Blur & transparency:** heavy use of `backdrop-blur` on the break overlay (`blur-[35px]` over a black/30 scrim) to simulate a real OS lock/dim effect, and light `backdrop-blur-xl` on floating chips/toasts and the onboarding window. Used specifically for "this is covering your screen" moments — not decorative on ordinary UI.
- **Animation:** powered by Framer Motion (`motion/react`). Entrances are fade+slight-slide (`opacity 0→1`, `y 10-30px→0`), never bouncy pop-ins except the deliberate spring easing on the break-overlay exercise illustrations (`type: spring, stiffness 100, damping 15`). Ambient/looping animation is slow and breathing (`pulseGlow`, `float`, 4-10s durations, `easeInOut`) — reinforces the calm/wellness tone. No aggressive infinite spins or bounces.
- **Hover/press states:** hover = slightly lighter/darker fill shift + border color deepens (e.g. gray-50→white, or border-gray-200→300); active/press states mostly rely on the same darker fill (`active:bg-pink-600`) rather than a scale/shrink effect, though the primary onboarding CTA does get a `hover:scale-[1.03]` lift.
- **Borders:** always 1px, low-contrast (gray-200 in light, `#2d2820`/`#373127` in dark) — borders define structure, not decoration.
- **Imagery:** no photography anywhere in the app — 100% vector/iconographic (lucide-react icons + a handful of custom inline SVG illustrations for the breathing/posture/blink exercises). Color vibe of what illustration exists is warm-toned in light mode, jewel-toned (amber/purple/blue) glowing orbs in the dark break-overlay.
- **Layout rules:** the entire app is one simulated desktop window (rounded panel with macOS-style traffic-light dots) sitting on a full-bleed wallpaper — sidebar + content pane layout, optional simulated second monitor panel that slides in from the right.

## ICONOGRAPHY

- **Icon system:** exclusively **lucide-react** (stroke-based, ~2px stroke weight, 14–20px typical size, `currentColor`). No icon font, no PNG icon sets, no emoji-as-icon usage. This is a CDN/npm-available set, so the design system links it from CDN (unpkg) rather than fabricating substitutes.
- **Custom exceptions:** a handful of bespoke inline SVGs exist only for the 3 break-exercise illustrations (breathing horizon/sun, standing-up figure, blinking eye) and the one brand mark (closed-eye blob) — these are hand-authored in the source, not part of a reusable icon set, and are treated as illustration, not iconography.
- **Emoji:** essentially unused (one 👍 in onboarding step 4).
- **Color:** icons are almost always `currentColor`, tinted via their parent's text color (terracotta for active/primary states, warm gray for inactive).

---

## Index

```
styles.css              → root stylesheet, @imports every token file below
tokens/
  fonts.css              Google Fonts CDN import (Inter, Space Grotesk, JetBrains Mono)
  colors.css              primary/amber/semantic + warm neutral light & dark scales
  typography.css          font families, sizes, weights, tracking, line-height
  spacing.css             spacing scale, radii, shadows
assets/
  eyeyul-mark.svg          in-product closed-eye brand mark (see note above)
components/
  core/Button, IconButton
  forms/Toggle, Slider, WeekdaySelector
  navigation/SidebarNavItem, SegmentedTabs
  feedback/Badge, Toast
  data-display/Card, ProgressRing, StatCard
guidelines/               foundation specimen cards (Colors, Type, Spacing, Brand)
ui_kits/eyeyul-app/        interactive recreation of the desktop app
SKILL.md                   Claude-Code-compatible skill wrapper for this system
```

## Caveats / open questions for the user
- No Figma file or additional codebase pages (marketing site, docs, etc.) were provided — this system reflects the **one** app screenshot-equivalent source (`eyeyul/`) in full.
- Fonts are loaded from Google Fonts CDN exactly as the source app did — no local `.ttf`/`.woff2` files existed to copy in. Let us know if you have real font license files to embed instead.
- No real logo file exists anywhere in the source; the inline eye-blob SVG is the closest thing to a mark. If EyeYul has an actual logo (Illustrator/Figma/PNG), please attach it and we'll swap it in.
- Component set was generalized from repeated inline patterns (Tailwind utility clusters), not from a pre-existing component library — variant names/props are our best-fit abstraction, not literal source constants.
