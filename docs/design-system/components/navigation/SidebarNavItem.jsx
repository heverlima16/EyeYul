import React from 'react';

export function SidebarNavItem({ icon, label, active = false, theme = 'light', onClick }) {
  const dark = theme === 'dark';
  return (
    <button
      onClick={onClick}
      style={{
        width: '100%',
        display: 'flex',
        alignItems: 'center',
        gap: 10,
        padding: '8px 12px',
        borderRadius: 'var(--radius-lg)',
        border: 'none',
        textAlign: 'left',
        fontFamily: 'var(--font-sans)',
        fontSize: 'var(--text-base)',
        fontWeight: active ? 'var(--font-weight-semibold)' : 'var(--font-weight-medium)',
        cursor: 'pointer',
        background: active ? 'var(--color-primary-500)' : 'transparent',
        color: active ? '#fff' : (dark ? 'rgba(236,229,218,0.7)' : '#374151'),
        boxShadow: active ? '0 4px 12px rgba(222,115,86,0.15)' : 'none',
        transition: 'all 0.15s ease',
      }}
    >
      {icon}
      <span>{label}</span>
    </button>
  );
}
