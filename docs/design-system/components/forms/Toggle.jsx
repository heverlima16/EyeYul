import React from 'react';

export function Toggle({ checked = false, onChange, theme = 'light', size = 'md', disabled = false }) {
  const dark = theme === 'dark';
  const dims = size === 'sm' ? { w: 32, h: 18, knob: 14 } : { w: 40, h: 24, knob: 16 };
  return (
    <button
      role="switch"
      aria-checked={checked}
      disabled={disabled}
      onClick={() => onChange && onChange(!checked)}
      style={{
        width: dims.w,
        height: dims.h,
        borderRadius: 'var(--radius-full)',
        padding: 4,
        border: checked ? 'none' : `1px solid ${dark ? 'var(--color-dark-border-2)' : '#d1d5db'}`,
        background: checked ? 'var(--color-primary-500)' : (dark ? 'var(--color-dark-surface-3)' : '#e5e7eb'),
        cursor: disabled ? 'not-allowed' : 'pointer',
        opacity: disabled ? 0.5 : 1,
        transition: 'background 0.2s ease',
        display: 'inline-flex',
        alignItems: 'center',
        boxSizing: 'border-box',
      }}
    >
      <div
        style={{
          width: dims.knob,
          height: dims.knob,
          borderRadius: 'var(--radius-full)',
          background: dark ? 'var(--color-dark-text)' : '#fff',
          boxShadow: '0 1px 3px rgba(0,0,0,0.3)',
          transform: checked ? `translateX(${dims.w - dims.knob - 8}px)` : 'translateX(0)',
          transition: 'transform 0.2s ease',
        }}
      />
    </button>
  );
}
