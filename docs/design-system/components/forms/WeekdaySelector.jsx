import React from 'react';

const DAYS = ['D', 'L', 'M', 'M', 'J', 'V', 'S'];

export function WeekdaySelector({ selected = [], onToggle, theme = 'light', size = 28 }) {
  const dark = theme === 'dark';
  return (
    <div style={{ display: 'flex', gap: 4 }}>
      {DAYS.map((label, idx) => {
        const isSel = selected.includes(idx);
        return (
          <button
            key={idx}
            onClick={() => onToggle && onToggle(idx)}
            style={{
              width: size,
              height: size,
              borderRadius: 'var(--radius-md)',
              fontSize: 'var(--text-sm)',
              fontWeight: 'var(--font-weight-bold)',
              fontFamily: 'var(--font-sans)',
              border: isSel ? 'none' : `1px solid ${dark ? 'var(--color-dark-border-2)' : '#d1d5db'}`,
              background: isSel ? 'var(--color-primary-500)' : (dark ? 'var(--color-dark-surface-3)' : '#fff'),
              color: isSel ? '#fff' : (dark ? 'var(--color-dark-text-3)' : '#6b7280'),
              cursor: 'pointer',
              transition: 'all 0.15s ease',
            }}
          >
            {label}
          </button>
        );
      })}
    </div>
  );
}
