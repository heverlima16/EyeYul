import 'react';

/** Row of 7 single-letter day toggles (D L M M J V S) for scheduling office hours / planned breaks. */
export interface WeekdaySelectorProps {
  /** Indices 0(Sun)-6(Sat) currently active. */
  selected?: number[];
  onToggle?: (dayIndex: number) => void;
  theme?: 'light' | 'dark';
  size?: number;
}

export function WeekdaySelector(props: WeekdaySelectorProps): JSX.Element;
