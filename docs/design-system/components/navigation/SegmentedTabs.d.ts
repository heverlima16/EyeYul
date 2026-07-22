import 'react';

/** Pill-shaped segmented control (2-4 options) for mode switches — e.g. Postura/Parpadeo, Claro/Oscuro. */
export interface SegmentedTabsOption {
  label: string;
  value: string;
}
export interface SegmentedTabsProps {
  options: SegmentedTabsOption[];
  value: string;
  onChange?: (value: string) => void;
  theme?: 'light' | 'dark';
  /** Stretch to fill container width, splitting space evenly — use for 3-option controls in narrow containers. */
  fullWidth?: boolean;
}

export function SegmentedTabs(props: SegmentedTabsProps): JSX.Element;
