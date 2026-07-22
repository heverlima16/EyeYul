import 'react';

/**
 * Sidebar navigation row (icon + label), filled terracotta pill when active.
 */
export interface SidebarNavItemProps {
  icon: React.ReactNode;
  label: string;
  active?: boolean;
  theme?: 'light' | 'dark';
  onClick?: () => void;
}

export function SidebarNavItem(props: SidebarNavItemProps): JSX.Element;
