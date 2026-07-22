import React from 'react';

export function StatCard({ icon, label, value, tone = 'primary', theme = 'light' }) {
  const dark = theme === 'dark';
  const TONE_COLORS = {
    primary: 'var(--color-primary-500)',
    amber: 'var(--color-amber-500)',
    orange: '#f97316',
  };
  const c = TONE_COLORS[tone] || TONE_COLORS.primary;
  return (
    <div
      style={{
        display: 'flex', alignItems: 'center', gap: 12,
        padding: 16,
        borderRadius: 'var(--radius-xl)',
        border: `1px solid ${dark ? 'var(--color-dark-border-2)' : '#e5e7eb'}`,
        background: dark ? 'rgba(36,33,26,0.8)' : '#fff',
      }}
    >
      <div style={{
        width: 40, height: 40, borderRadius: 'var(--radius-lg)',
        background: `${c}1a`, border: `1px solid ${c}33`,
        display: 'flex', alignItems: 'center', justifyContent: 'center', color: c, flexShrink: 0,
      }}>
        {icon}
      </div>
      <div>
        <div style={{ fontFamily: 'var(--font-mono)', fontSize: 'var(--text-xs)', textTransform: 'uppercase', letterSpacing: 'var(--tracking-wide)', color: dark ? 'var(--color-dark-text-3)' : '#9ca3af' }}>{label}</div>
        <div style={{ fontFamily: 'var(--font-display)', fontWeight: 'var(--font-weight-bold)', fontSize: 'var(--text-2xl)', color: dark ? 'var(--color-dark-text)' : '#111827' }}>{value}</div>
      </div>
    </div>
  );
}
