import 'react';

/**
 * Floating micro-notification card — posture/blink/system alerts stack top-right.
 */
export interface ToastProps {
  icon: React.ReactNode;
  title: string;
  body: string;
  tone?: 'primary' | 'amber' | 'neutral';
  theme?: 'light' | 'dark';
  onClose?: () => void;
}

export function Toast(props: ToastProps): JSX.Element;
