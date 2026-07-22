import 'react';

/**
 * Pill switch for boolean settings — smart-pause exclusions, automations, office hours.
 */
export interface ToggleProps {
  checked?: boolean;
  onChange?: (next: boolean) => void;
  theme?: 'light' | 'dark';
  size?: 'sm' | 'md';
  disabled?: boolean;
}

export function Toggle(props: ToggleProps): JSX.Element;
