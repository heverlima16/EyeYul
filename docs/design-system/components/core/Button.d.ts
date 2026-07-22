import 'react';

/**
 * Pill-shaped action button — the app's primary interactive control (terracotta
 * primary, inverse dark/light, soft secondary, and quiet ghost variants).
 */
export interface ButtonProps {
  children: React.ReactNode;
  /** Visual style. */
  variant?: 'primary' | 'inverse' | 'secondary' | 'ghost';
  size?: 'sm' | 'md' | 'lg';
  /** Fully rounded (default) vs rounded-rect. */
  pill?: boolean;
  icon?: React.ReactNode;
  disabled?: boolean;
  theme?: 'light' | 'dark';
  onClick?: () => void;
  style?: React.CSSProperties;
}

export function Button(props: ButtonProps): JSX.Element;
