import React from 'react';

export function ProgressRing({ percent, size = 180, strokeWidth = 5, centerLabel, subLabel, theme = 'light', color = 'var(--color-primary-500)' }) {
  const dark = theme === 'dark';
  const r = (size - strokeWidth) / 2;
  const circumference = 2 * Math.PI * r;
  const offset = circumference * (1 - percent / 100);
  return (
    <div style={{ position: 'relative', width: size, height: size, display: 'inline-flex', alignItems: 'center', justifyContent: 'center' }}>
      <svg width={size} height={size} style={{ transform: 'rotate(-90deg)' }}>
        <circle cx={size / 2} cy={size / 2} r={r} fill="none" stroke={dark ? 'var(--color-dark-surface-3)' : '#f3f4f6'} strokeWidth={strokeWidth} />
        <circle
          cx={size / 2} cy={size / 2} r={r} fill="none" stroke={color} strokeWidth={strokeWidth}
          strokeDasharray={circumference} strokeDashoffset={offset} strokeLinecap="round"
          style={{ transition: 'stroke-dashoffset 0.5s ease' }}
        />
      </svg>
      <div style={{ position: 'absolute', display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
        <span style={{ fontFamily: 'var(--font-mono)', fontWeight: 'var(--font-weight-bold)', fontSize: 'var(--text-4xl)', color: dark ? 'var(--color-dark-text)' : '#111827' }}>
          {centerLabel}
        </span>
        {subLabel && (
          <span style={{ fontFamily: 'var(--font-mono)', fontSize: 'var(--text-xs)', textTransform: 'uppercase', letterSpacing: 'var(--tracking-widest)', color: dark ? 'var(--color-dark-text-3)' : '#9ca3af', marginTop: 4 }}>
            {subLabel}
          </span>
        )}
      </div>
    </div>
  );
}
