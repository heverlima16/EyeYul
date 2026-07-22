import 'react';

/**
 * Circular icon-only button for compact chrome — window controls, close buttons,
 * notification dismiss.
 */
export interface IconButtonProps {
  icon: React.ReactNode;
  active?: boolean;
  theme?: 'light' | 'dark';
  size?: number;
  title?: string;
  onClick?: () => void;
  style?: React.CSSProperties;
}

export function IconButton(props: IconButtonProps): JSX.Element;
