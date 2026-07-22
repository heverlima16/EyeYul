import React from 'react';

const TONES = {
  primary: { border: 'var(--color-primary-500)', color: 'var(--color-primary-500)' },
  amber:   { border: 'var(--color-amber-500)', color: 'var(--color-amber-600)' },
  neutral: { border: '#e5e7eb', color: '#374151' },
};

export function Toast({ icon, title, body, tone = 'primary', theme = 'light', onClose }) {
  const dark = theme === 'dark';
  const t = TONES[tone] || TONES.primary;
  return (
    <div
      style={{
        display: 'flex',
        alignItems: 'flex-start',
        gap: 10,
        padding: 14,
        borderRadius: 'var(--radius-xl)',
        border: `1px solid ${t.border}`,
        background: dark ? 'var(--color-dark-surface)' : '#fff',
        boxShadow: dark ? '0 10px 30px rgba(0,0,0,0.5)' : '0 10px 25px rgba(0,0,0,0.08)',
        maxWidth: 320,
        fontFamily: 'var(--font-sans)',
      }}
    >
      <div
        style={{
          width: 30, height: 30, borderRadius: 'var(--radius-md)',
          background: dark ? 'var(--color-dark-surface-3)' : '#f9fafb',
          display: 'flex', alignItems: 'center', justifyContent: 'center',
          color: t.color, flexShrink: 0,
        }}
      >
        {icon}
      </div>
      <div style={{ flex: 1, minWidth: 0 }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', gap: 8 }}>
          <span style={{ fontWeight: 'var(--font-weight-bold)', fontSize: 'var(--text-base)', color: dark ? 'var(--color-dark-text)' : '#111827' }}>{title}</span>
          {onClose && (
            <button onClick={onClose} style={{ border: 'none', background: 'none', cursor: 'pointer', color: dark ? 'var(--color-dark-text-3)' : '#9ca3af', fontSize: 12 }}>✕</button>
          )}
        </div>
        <p style={{ fontSize: 'var(--text-sm)', lineHeight: 'var(--leading-relaxed)', color: dark ? 'var(--color-dark-text-2)' : '#4b5563', margin: '4px 0 0' }}>{body}</p>
      </div>
    </div>
  );
}
