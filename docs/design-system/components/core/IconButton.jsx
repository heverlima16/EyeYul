import React from 'react';

export function IconButton({ icon, active = false, theme = 'light', size = 32, title, onClick, style }) {
  const dark = theme === 'dark';
  return (
    <button
      title={title}
      onClick={onClick}
      style={{
        width: size,
        height: size,
        display: 'inline-flex',
        alignItems: 'center',
        justifyContent: 'center',
        borderRadius: 'var(--radius-full)',
        border: active ? '1px solid var(--color-primary-500)' : '1px solid transparent',
        background: active
          ? 'var(--accent-soft)'
          : (dark ? 'transparent' : 'transparent'),
        color: active ? 'var(--color-primary-500)' : (dark ? 'var(--color-dark-text-3)' : '#6b7280'),
        cursor: 'pointer',
        transition: 'all 0.2s ease',
        ...style,
      }}
    >
      {icon}
    </button>
  );
}
