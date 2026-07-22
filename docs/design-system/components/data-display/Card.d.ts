import 'react';

/**
 * Bordered rounded panel — the base container for every grouped section in the app.
 */
export interface CardProps {
  children: React.ReactNode;
  theme?: 'light' | 'dark';
  padding?: number;
  style?: React.CSSProperties;
}

export function Card(props: CardProps): JSX.Element;
