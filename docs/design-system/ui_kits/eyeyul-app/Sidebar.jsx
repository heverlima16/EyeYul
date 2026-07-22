const { Search, X, Activity, Award, Heart, Sparkles, Clock, Calendar, Shield, MessageSquare, Sun, Moon } = LucideIcons;
const { SidebarNavItem, SegmentedTabs } = window.EyeYul_491522;

function Sidebar({ theme, themeMode, setThemeMode, activeTab, setActiveTab, activeSubTab, setActiveSubTab, search, setSearch }) {
  const isLight = theme === 'light';
  return (
    <div style={{
      width: 240, flexShrink: 0, display: 'flex', flexDirection: 'column', justifyContent: 'space-between',
      padding: 18, borderRight: `1px solid ${isLight ? '#e5e7eb' : '#2d2820'}`,
      background: isLight ? 'var(--color-light-surface-2)' : '#13110e',
    }}>
      <div style={{ display: 'flex', flexDirection: 'column', gap: 16 }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: 8, paddingBottom: 10, borderBottom: `1px dashed ${isLight ? 'rgba(0,0,0,0.08)' : 'rgba(255,255,255,0.08)'}` }}>
          <div style={{ display: 'flex', gap: 5 }}>
            <span style={{ width: 9, height: 9, borderRadius: '50%', background: '#ff5f56cc' }} />
            <span style={{ width: 9, height: 9, borderRadius: '50%', background: '#ffbd2ecc' }} />
            <span style={{ width: 9, height: 9, borderRadius: '50%', background: '#27c93fcc' }} />
          </div>
          <span style={{ fontWeight: 700, fontSize: 12, color: isLight ? '#111827' : '#f7f2ea' }}>EyeYul</span>
          <span style={{ fontFamily: 'var(--font-mono)', fontSize: 9, fontWeight: 700, color: '#de7356', background: 'rgba(222,115,86,0.1)', padding: '1px 6px', borderRadius: 6 }}>v1.2.0</span>
        </div>

        <div style={{ position: 'relative' }}>
          <Search size={13} style={{ position: 'absolute', left: 10, top: 9, opacity: 0.4 }} />
          <input value={search} onChange={(e) => setSearch(e.target.value)} placeholder="Buscar..." style={{
            width: '100%', boxSizing: 'border-box', padding: '7px 10px 7px 28px', borderRadius: 12, fontSize: 11,
            border: `1px solid ${isLight ? '#d1d5db' : '#383227'}`, background: isLight ? '#fff' : '#1c1a16',
            color: isLight ? '#111827' : '#ece5da', outline: 'none',
          }} />
        </div>

        <div style={{ display: 'flex', flexDirection: 'column', gap: 14 }}>
          <div>
            <div style={{ fontFamily: 'var(--font-mono)', fontSize: 9, fontWeight: 700, textTransform: 'uppercase', letterSpacing: '.12em', color: isLight ? '#9ca3af' : '#8c8273', padding: '0 8px', marginBottom: 6 }}>Monitor e Historial</div>
            <div style={{ display: 'flex', flexDirection: 'column', gap: 4 }}>
              <SidebarNavItem theme={theme} icon={<Activity size={15} />} label="Vista General" active={activeTab === 'dashboard'} onClick={() => setActiveTab('dashboard')} />
              <SidebarNavItem theme={theme} icon={<Award size={15} />} label="Historial de Uso" active={activeTab === 'stats'} onClick={() => setActiveTab('stats')} />
            </div>
          </div>
          <div>
            <div style={{ fontFamily: 'var(--font-mono)', fontSize: 9, fontWeight: 700, textTransform: 'uppercase', letterSpacing: '.12em', color: isLight ? '#9ca3af' : '#8c8273', padding: '0 8px', marginBottom: 6 }}>Ajustes de Alerta</div>
            <div style={{ display: 'flex', flexDirection: 'column', gap: 4 }}>
              <SidebarNavItem theme={theme} icon={<Heart size={15} />} label="Postura y Parpadeo" active={activeTab === 'wellness'} onClick={() => setActiveTab('wellness')} />
              <SidebarNavItem theme={theme} icon={<Sparkles size={15} />} label="Smart Pause" active={activeTab === 'automations'} onClick={() => setActiveTab('automations')} />
            </div>
          </div>
          <div>
            <div style={{ fontFamily: 'var(--font-mono)', fontSize: 9, fontWeight: 700, textTransform: 'uppercase', letterSpacing: '.12em', color: isLight ? '#9ca3af' : '#8c8273', padding: '0 8px', marginBottom: 6 }}>Rutina de Descansos</div>
            <div style={{ display: 'flex', flexDirection: 'column', gap: 4 }}>
              {[
                { id: 'timer', label: 'Frecuencia', icon: Clock },
                { id: 'planned', label: 'Agenda', icon: Calendar },
                { id: 'limits', label: 'Límites', icon: Shield },
                { id: 'messages', label: 'Frases', icon: MessageSquare },
              ].map((s) => (
                <SidebarNavItem key={s.id} theme={theme} icon={<s.icon size={15} />} label={s.label}
                  active={activeTab === 'routine' && activeSubTab === s.id}
                  onClick={() => { setActiveTab('routine'); setActiveSubTab(s.id); }} />
              ))}
            </div>
          </div>
        </div>
      </div>

      <div style={{ display: 'flex', flexDirection: 'column', gap: 10, paddingTop: 12, borderTop: `1px solid ${isLight ? 'rgba(0,0,0,0.06)' : 'rgba(255,255,255,0.06)'}` }}>
        <div style={{ fontFamily: 'var(--font-mono)', fontSize: 9, fontWeight: 700, textTransform: 'uppercase', letterSpacing: '.12em', color: isLight ? '#9ca3af' : '#8c8273', padding: '0 8px' }}>Apariencia</div>
        <SegmentedTabs theme={theme} value={themeMode} onChange={setThemeMode} fullWidth options={[{ label: 'Claro', value: 'light' }, { label: 'Oscuro', value: 'dark' }, { label: 'Sistema', value: 'system' }]} />
        <div style={{ display: 'flex', justifyContent: 'space-between', fontFamily: 'var(--font-mono)', fontSize: 9, color: isLight ? '#9ca3af' : '#5e564a', padding: '0 8px' }}>
          <span>Licencia Activa</span><span style={{ color: '#10b981', fontWeight: 700 }}>OK</span>
        </div>
      </div>
    </div>
  );
}

window.Sidebar = Sidebar;
