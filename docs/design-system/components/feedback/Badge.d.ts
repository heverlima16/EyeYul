import 'react';

/** Small uppercase mono status pill — "ACTIVO", "PROBAR", "AFK", license "OK". */
export interface BadgeProps {
  children: React.ReactNode;
  tone?: 'primary' | 'amber' | 'success' | 'neutral';
  mono?: boolean;
}

export function Badge(props: BadgeProps): JSX.Element;
