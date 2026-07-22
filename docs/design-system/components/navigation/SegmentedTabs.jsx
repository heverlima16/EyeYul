import React from 'react';

export function SegmentedTabs({ options, value, onChange, theme = 'light', fullWidth = false }) {
  const dark = theme === 'dark';
  return (
    <div
      style={{
        display: fullWidth ? 'flex' : 'inline-flex',
        width: fullWidth ? '100%' : 'auto',
        gap: 4,
        padding: 4,
        boxSizing: 'border-box',
        borderRadius: 'var(--radius-full)',
        background: dark ? 'var(--color-dark-surface-3)' : '#f3f4f6',
        border: `1px solid ${dark ? 'var(--color-dark-border-2)' : '#e5e7eb'}`,
      }}
    >
      {options.map((opt) => {
        const active = opt.value === value;
        return (
          <button
            key={opt.value}
            onClick={() => onChange && onChange(opt.value)}
            style={{
              flex: fullWidth ? 1 : 'none',
              padding: fullWidth ? '6px 4px' : '6px 20px',
              borderRadius: 'var(--radius-full)',
              border: 'none',
              fontFamily: 'var(--font-sans)',
              fontSize: fullWidth && options.length > 2 ? 'var(--text-sm)' : 'var(--text-base)',
              fontWeight: 'var(--font-weight-medium)',
              cursor: 'pointer',
              whiteSpace: 'nowrap',
              background: active ? 'var(--color-primary-500)' : 'transparent',
              color: active ? '#fff' : (dark ? 'var(--color-dark-text-3)' : '#6b7280'),
              boxShadow: active ? '0 6px 16px rgba(222,115,86,0.2)' : 'none',
              transition: 'all 0.15s ease',
            }}
          >
            {opt.label}
          </button>
        );
      })}
    </div>
  );
}
