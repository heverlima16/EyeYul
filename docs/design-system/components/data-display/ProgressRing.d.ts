import 'react';

/** Circular countdown ring with mono center label — the main break timer and break-overlay countdown. */
export interface ProgressRingProps {
  /** 0-100 */
  percent: number;
  size?: number;
  strokeWidth?: number;
  centerLabel: string;
  subLabel?: string;
  theme?: 'light' | 'dark';
  color?: string;
}

export function ProgressRing(props: ProgressRingProps): JSX.Element;
