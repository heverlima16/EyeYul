import React from 'react';

const SIZE_STYLES = {
  sm: { padding: '6px 14px', fontSize: 'var(--text-xs)' },
  md: { padding: '10px 20px', fontSize: 'var(--text-base)' },
  lg: { padding: '14px 32px', fontSize: 'var(--text-lg)' },
};

function getVariantStyle(variant, theme) {
  const dark = theme === 'dark';
  switch (variant) {
    case 'primary':
      return {
        background: 'var(--color-primary-500)',
        color: '#fff',
        border: '1px solid transparent',
        boxShadow: '0 4px 15px rgba(222,115,86,0.2)',
      };
    case 'inverse':
      return dark
        ? { background: 'var(--color-dark-text)', color: 'var(--color-dark-bg)', border: '1px solid transparent' }
        : { background: '#111827', color: '#fff', border: '1px solid transparent' };
    case 'secondary':
      return dark
        ? { background: 'var(--color-dark-surface)', color: 'var(--color-dark-text)', border: '1px solid var(--color-dark-border-2)' }
        : { background: '#f3f4f6', color: '#1f2937', border: '1px solid #d1d5db' };
    case 'ghost':
      return dark
        ? { background: 'transparent', color: 'var(--color-dark-text-3)', border: '1px solid transparent' }
        : { background: 'transparent', color: '#6b7280', border: '1px solid transparent' };
    default:
      return {};
  }
}

export function Button({
  children,
  variant = 'primary',
  size = 'md',
  pill = true,
  icon = null,
  disabled = false,
  theme = 'light',
  onClick,
  style,
}) {
  const variantStyle = getVariantStyle(variant, theme);
  return (
    <button
      onClick={onClick}
      disabled={disabled}
      style={{
        display: 'inline-flex',
        alignItems: 'center',
        justifyContent: 'center',
        gap: '6px',
        fontFamily: 'var(--font-sans)',
        fontWeight: 'var(--font-weight-semibold)',
        letterSpacing: 'var(--tracking-tight)',
        borderRadius: pill ? 'var(--radius-full)' : 'var(--radius-lg)',
        cursor: disabled ? 'not-allowed' : 'pointer',
        opacity: disabled ? 0.4 : 1,
        transition: 'all 0.2s ease',
        ...SIZE_STYLES[size],
        ...variantStyle,
        ...style,
      }}
    >
      {icon}
      <span>{children}</span>
    </button>
  );
}
