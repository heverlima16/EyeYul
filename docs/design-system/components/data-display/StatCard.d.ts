import 'react';

/** Icon + label + big value metric tile — streaks, breaks completed, protected minutes. */
export interface StatCardProps {
  icon: React.ReactNode;
  label: string;
  value: string;
  tone?: 'primary' | 'amber' | 'orange';
  theme?: 'light' | 'dark';
}

export function StatCard(props: StatCardProps): JSX.Element;
