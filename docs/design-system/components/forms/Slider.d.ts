import 'react';

/** Labeled range input for numeric settings (work interval, break length, snooze limits). */
export interface SliderProps {
  value: number;
  min?: number;
  max?: number;
  step?: number;
  onChange?: (next: number) => void;
  label?: string;
  unit?: string;
  theme?: 'light' | 'dark';
}

export function Slider(props: SliderProps): JSX.Element;
