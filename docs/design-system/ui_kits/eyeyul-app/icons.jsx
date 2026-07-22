/* Minimal stroke-icon shim in lucide's visual style (rounded caps, 2px stroke,
   24x24 viewBox, currentColor) — used because this design system project can't
   reliably load an external icon package inside a sandboxed iframe. The real app
   uses lucide-react directly; if wiring this UI kit into a live build, swap this
   file for `import { Search, X, ... } from 'lucide-react'` and delete it. */
(function () {
  const { createElement: h } = React;
  function icon(name, paths) {
    return function Icon({ size = 16, color = 'currentColor', style }) {
      return h('svg', {
        width: size, height: size, viewBox: '0 0 24 24', fill: 'none',
        stroke: color, strokeWidth: 2, strokeLinecap: 'round', strokeLinejoin: 'round', style,
      }, paths.map((p, i) => h(p.tag || 'path', { key: i, ...p.attrs })));
    };
  }
  window.LucideIcons = {
    Search: icon('Search', [{ tag: 'circle', attrs: { cx: 11, cy: 11, r: 7 } }, { attrs: { d: 'M21 21l-4.3-4.3' } }]),
    X: icon('X', [{ attrs: { d: 'M18 6L6 18' } }, { attrs: { d: 'M6 6l12 12' } }]),
    Activity: icon('Activity', [{ attrs: { d: 'M22 12h-4l-3 9L9 3l-3 9H2' } }]),
    Award: icon('Award', [{ tag: 'circle', attrs: { cx: 12, cy: 8, r: 6 } }, { attrs: { d: 'M8.2 13.5L7 22l5-3 5 3-1.2-8.5' } }]),
    Heart: icon('Heart', [{ attrs: { d: 'M20.8 4.6a5.5 5.5 0 00-7.8 0L12 5.6l-1-1a5.5 5.5 0 10-7.8 7.8l1 1L12 21l7.8-7.6 1-1a5.5 5.5 0 000-7.8z' } }]),
    Sparkles: icon('Sparkles', [{ attrs: { d: 'M12 3l1.8 4.9L18 9.5l-4.2 1.6L12 16l-1.8-4.9L6 9.5l4.2-1.6z' } }, { attrs: { d: 'M19 15l.7 2 2 .7-2 .7-.7 2-.7-2-2-.7 2-.7z' } }]),
    Clock: icon('Clock', [{ tag: 'circle', attrs: { cx: 12, cy: 12, r: 9 } }, { attrs: { d: 'M12 7v5l3 2' } }]),
    Calendar: icon('Calendar', [{ tag: 'rect', attrs: { x: 3, y: 5, width: 18, height: 16, rx: 2 } }, { attrs: { d: 'M16 3v4M8 3v4M3 10h18' } }]),
    Shield: icon('Shield', [{ attrs: { d: 'M12 3l7 3v6c0 4.5-3 7.5-7 9-4-1.5-7-4.5-7-9V6z' } }]),
    ShieldAlert: icon('ShieldAlert', [{ attrs: { d: 'M12 3l7 3v6c0 4.5-3 7.5-7 9-4-1.5-7-4.5-7-9V6z' } }, { attrs: { d: 'M12 8v4' } }, { attrs: { d: 'M12 16h.01' } }]),
    ShieldCheck: icon('ShieldCheck', [{ attrs: { d: 'M12 3l7 3v6c0 4.5-3 7.5-7 9-4-1.5-7-4.5-7-9V6z' } }, { attrs: { d: 'M9 12l2 2 4-4' } }]),
    MessageSquare: icon('MessageSquare', [{ attrs: { d: 'M4 4h16v12H8l-4 4z' } }]),
    Sun: icon('Sun', [{ tag: 'circle', attrs: { cx: 12, cy: 12, r: 4 } }, { attrs: { d: 'M12 2v2M12 20v2M4.9 4.9l1.4 1.4M17.7 17.7l1.4 1.4M2 12h2M20 12h2M4.9 19.1l1.4-1.4M17.7 6.3l1.4-1.4' } }]),
    Moon: icon('Moon', [{ attrs: { d: 'M20 14.5A8 8 0 019.5 4 8 8 0 1020 14.5z' } }]),
    Play: icon('Play', [{ tag: 'polygon', attrs: { points: '6,3 20,12 6,21' } }]),
    Pause: icon('Pause', [{ tag: 'rect', attrs: { x: 6, y: 4, width: 4, height: 16 } }, { tag: 'rect', attrs: { x: 14, y: 4, width: 4, height: 16 } }]),
    RotateCcw: icon('RotateCcw', [{ attrs: { d: 'M3 12a9 9 0 109-9 9 9 0 00-6.7 3M3 4v5h5' } }]),
    Zap: icon('Zap', [{ tag: 'polygon', attrs: { points: '13,2 3,14 11,14 11,22 21,10 13,10' } }]),
    Video: icon('Video', [{ tag: 'rect', attrs: { x: 2, y: 6, width: 14, height: 12, rx: 2 } }, { tag: 'polygon', attrs: { points: '22,8 16,12 22,16' } }]),
    Keyboard: icon('Keyboard', [{ tag: 'rect', attrs: { x: 2, y: 6, width: 20, height: 12, rx: 2 } }, { attrs: { d: 'M6 10h.01M10 10h.01M14 10h.01M18 10h.01M6 14h12' } }]),
    Tv: icon('Tv', [{ tag: 'rect', attrs: { x: 3, y: 6, width: 18, height: 13, rx: 2 } }, { attrs: { d: 'M8 21h8M12 19v2' } }]),
    UserCheck: icon('UserCheck', [{ tag: 'circle', attrs: { cx: 9, cy: 8, r: 4 } }, { attrs: { d: 'M2 21c0-4 3-6 7-6M16 11l2 2 4-4' } }]),
    Flame: icon('Flame', [{ attrs: { d: 'M12 2c1 4-3 5-3 9a5 5 0 0010 0c0-2-1-3-2-4 0 2-1 3-2 2 1-3-1-5-3-7z' } }]),
    ChevronLeft: icon('ChevronLeft', [{ attrs: { d: 'M15 18l-6-6 6-6' } }]),
    ChevronRight: icon('ChevronRight', [{ attrs: { d: 'M9 18l6-6-6-6' } }]),
    ChevronUp: icon('ChevronUp', [{ attrs: { d: 'M18 15l-6-6-6 6' } }]),
    ChevronsUp: icon('ChevronsUp', [{ attrs: { d: 'M7 11l5-5 5 5' } }, { attrs: { d: 'M7 18l5-5 5 5' } }]),
    Check: icon('Check', [{ attrs: { d: 'M20 6L9 17l-5-5' } }]),
    Brain: icon('Brain', [{ attrs: { d: 'M9 3a3 3 0 00-3 3v1a3 3 0 00-2 5 3 3 0 002 5v1a3 3 0 006 0V6a3 3 0 00-3-3z' } }, { attrs: { d: 'M15 3a3 3 0 013 3v1a3 3 0 012 5 3 3 0 01-2 5v1a3 3 0 01-6 0V6a3 3 0 013-3z' } }]),
    Eye: icon('Eye', [{ attrs: { d: 'M2 12s3.5-7 10-7 10 7 10 7-3.5 7-10 7-10-7-10-7z' } }, { tag: 'circle', attrs: { cx: 12, cy: 12, r: 3 } }]),
    Trash2: icon('Trash2', [{ attrs: { d: 'M3 6h18M8 6V4h8v2M6 6l1 14h10l1-14' } }]),
    Plus: icon('Plus', [{ attrs: { d: 'M12 5v14M5 12h14' } }]),
    Utensils: icon('Utensils', [{ attrs: { d: 'M6 3v7a2 2 0 002 2 2 2 0 002-2V3M8 12v9M18 3c-2 0-3 2-3 5v3h3M18 11v10' } }]),
    Coffee: icon('Coffee', [{ attrs: { d: 'M3 8h14v6a4 4 0 01-4 4H7a4 4 0 01-4-4z' } }, { attrs: { d: 'M17 9h2a2 2 0 010 4h-2' } }]),
    BellRing: icon('BellRing', [{ attrs: { d: 'M6 8a6 6 0 1112 0c0 5 2 6 2 6H4s2-1 2-6' } }, { attrs: { d: 'M10.3 21a2 2 0 003.4 0' } }]),
    Gamepad2: icon('Gamepad2', [{ tag: 'rect', attrs: { x: 2, y: 7, width: 20, height: 10, rx: 4 } }, { attrs: { d: 'M8 10v4M6 12h4M15 11h.01M18 13h.01' } }]),
    Share2: icon('Share2', [{ tag: 'circle', attrs: { cx: 18, cy: 5, r: 3 } }, { tag: 'circle', attrs: { cx: 6, cy: 12, r: 3 } }, { tag: 'circle', attrs: { cx: 18, cy: 19, r: 3 } }, { attrs: { d: 'M8.6 10.6l6.8-3.8M8.6 13.4l6.8 3.8' } }]),
    VolumeX: icon('VolumeX', [{ attrs: { d: 'M11 5L6 9H2v6h4l5 4z' } }, { attrs: { d: 'M23 9l-6 6M17 9l6 6' } }]),
    Hourglass: icon('Hourglass', [{ attrs: { d: 'M5 3h14M5 21h14M6 3c0 6 5 7 6 9-1 2-6 3-6 9M18 3c0 6-5 7-6 9 1 2 6 3 6 9' } }]),
    Info: icon('Info', [{ tag: 'circle', attrs: { cx: 12, cy: 12, r: 9 } }, { attrs: { d: 'M12 16v-4M12 8h.01' } }]),
    ArrowRight: icon('ArrowRight', [{ attrs: { d: 'M5 12h14M13 6l6 6-6 6' } }]),
    Wind: icon('Wind', [{ attrs: { d: 'M3 8h9a3 3 0 10-3-3M3 16h12a3 3 0 11-3 3M3 12h15a3 3 0 10-3-3' } }]),
    Music: icon('Music', [{ attrs: { d: 'M9 18V5l12-2v13' } }, { tag: 'circle', attrs: { cx: 6, cy: 18, r: 3 } }, { tag: 'circle', attrs: { cx: 18, cy: 16, r: 3 } }]),
  };
})();
