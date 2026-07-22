import React from 'react';

export function Slider({ value, min = 0, max = 100, step = 1, onChange, label, unit = '', theme = 'light' }) {
  const dark = theme === 'dark';
  return (
    <div style={{ fontFamily: 'var(--font-sans)' }}>
      {label && (
        <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 6, fontSize: 'var(--text-base)' }}>
          <span style={{ fontWeight: 'var(--font-weight-semibold)', color: dark ? 'var(--color-dark-text)' : '#374151' }}>{label}</span>
          <span style={{ fontFamily: 'var(--font-mono)', fontWeight: 'var(--font-weight-bold)', color: 'var(--color-primary-500)' }}>
            {value}{unit}
          </span>
        </div>
      )}
      <input
        type="range"
        min={min}
        max={max}
        step={step}
        value={value}
        onChange={(e) => onChange && onChange(Number(e.target.value))}
        style={{
          width: '100%',
          height: 6,
          borderRadius: 'var(--radius-full)',
          appearance: 'none',
          background: dark ? 'rgba(236,229,218,0.1)' : '#e5e7eb',
          accentColor: 'var(--color-primary-500)',
          cursor: 'pointer',
        }}
      />
    </div>
  );
}
