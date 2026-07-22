import React from 'react';

export function Card({ children, theme = 'light', padding = 20, style }) {
  const dark = theme === 'dark';
  return (
    <div
      style={{
        borderRadius: 'var(--radius-xl)',
        border: `1px solid ${dark ? 'var(--color-dark-border-2)' : '#e5e7eb'}`,
        background: dark ? 'rgba(36,33,26,0.8)' : '#fff',
        padding,
        boxSizing: 'border-box',
        ...style,
      }}
    >
      {children}
    </div>
  );
}
