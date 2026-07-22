import React from 'react';

const TONES = {
  primary: { bg: 'rgba(222,115,86,0.1)', color: 'var(--color-primary-500)', border: 'rgba(222,115,86,0.3)' },
  amber:   { bg: 'rgba(245,158,11,0.1)', color: 'var(--color-amber-500)', border: 'rgba(245,158,11,0.3)' },
  success: { bg: 'rgba(16,185,129,0.1)', color: 'var(--color-emerald-500)', border: 'rgba(16,185,129,0.3)' },
  neutral: { bg: 'rgba(107,114,128,0.1)', color: '#6b7280', border: 'rgba(107,114,128,0.25)' },
};

export function Badge({ children, tone = 'primary', mono = true }) {
  const t = TONES[tone] || TONES.primary;
  return (
    <span
      style={{
        display: 'inline-block',
        padding: '3px 10px',
        borderRadius: 'var(--radius-full)',
        fontFamily: mono ? 'var(--font-mono)' : 'var(--font-sans)',
        fontSize: 'var(--text-xs)',
        fontWeight: 'var(--font-weight-bold)',
        textTransform: 'uppercase',
        letterSpacing: 'var(--tracking-wide)',
        background: t.bg,
        color: t.color,
      }}
    >
      {children}
    </span>
  );
}
