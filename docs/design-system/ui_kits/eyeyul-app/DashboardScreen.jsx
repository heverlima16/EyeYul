const { Play, Pause, RotateCcw, Zap, Video, Keyboard, Tv, UserCheck, Flame, Award } = LucideIcons;
const { ProgressRing, StatCard } = window.EyeYul_491522;

function formatTime(sec) {
  const m = Math.floor(sec / 60), s = sec % 60;
  return `${String(m).padStart(2, '0')}:${String(s).padStart(2, '0')}`;
}

function DashboardScreen({ theme, timer, totalDuration, paused, onTogglePause, onReset, onStartBreak, context, onToggleContext }) {
  const isLight = theme === 'light';
  const percent = (timer / totalDuration) * 100;
  const cardBg = isLight ? '#fff' : 'rgba(36,33,26,0.8)';
  const cardBorder = isLight ? '#e5e7eb' : '#373127';

  const simRow = (id, icon, label, sub, tone) => {
    const active = context === id;
    const toneColor = { amber: '#f59e0b', pink: '#de7356', blue: '#3b82f6' }[tone];
    return (
      <button key={id} onClick={() => onToggleContext(id)} style={{
        width: '100%', padding: 12, borderRadius: 14, textAlign: 'left', cursor: 'pointer',
        display: 'flex', justifyContent: 'space-between', alignItems: 'center',
        border: active ? `1px solid ${toneColor}` : `1px solid ${cardBorder}`,
        background: active ? toneColor : (isLight ? '#f9fafb' : '#1a1814'),
        boxShadow: active ? `0 6px 16px ${toneColor}40` : 'none',
        color: active ? '#fff' : (isLight ? '#374151' : '#8c8273'),
      }}>
        <div style={{ display: 'flex', gap: 10, alignItems: 'center' }}>
          {icon}
          <div>
            <div style={{ fontSize: 12, fontWeight: 600 }}>{label}</div>
            <div style={{ fontSize: 9, opacity: active ? 0.9 : 0.75, marginTop: 2 }}>{sub}</div>
          </div>
        </div>
        <span style={{ fontFamily: 'var(--font-mono)', fontSize: 9, fontWeight: 700, padding: '2px 8px', borderRadius: 999, background: active ? 'rgba(255,255,255,0.22)' : (isLight ? '#e5e7eb' : '#24211a'), color: active ? '#fff' : (isLight ? '#6b7280' : '#8c8273') }}>
          {active ? 'ACTIVO' : 'PROBAR'}
        </span>
      </button>
    );
  };

  return (
    <div style={{ display: 'grid', gridTemplateColumns: '7fr 5fr', gap: 20 }}>
      <div style={{ border: `1px solid ${cardBorder}`, background: cardBg, borderRadius: 24, padding: 22, display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'space-between', minHeight: 360 }}>
        <div style={{ width: '100%', display: 'flex', justifyContent: 'space-between', fontFamily: 'var(--font-mono)', fontSize: 10, color: isLight ? '#6b7280' : '#8c8273' }}>
          <span>ESTADO DEL SISTEMA</span>
          <span style={{ display: 'flex', alignItems: 'center', gap: 4, fontWeight: 700, textTransform: 'uppercase' }}>
            <span style={{ width: 6, height: 6, borderRadius: '50%', background: paused || context ? '#f59e0b' : '#de7356' }} />
            {context ? 'POSPUESTO' : paused ? 'PAUSADO' : 'TRABAJANDO'}
          </span>
        </div>
        <ProgressRing percent={percent} centerLabel={formatTime(timer)} subLabel="Próxima Pausa" theme={theme} color={paused ? 'var(--color-amber-500)' : 'var(--color-primary-500)'} />
        <div style={{ display: 'flex', gap: 10 }}>
          <button onClick={onTogglePause} style={{ padding: '10px 18px', borderRadius: 12, border: 'none', cursor: 'pointer', fontSize: 11, fontWeight: 700, display: 'flex', gap: 6, alignItems: 'center', background: paused ? '#de7356' : (isLight ? '#f3f4f6' : '#1d1a15'), color: paused ? '#fff' : (isLight ? '#111827' : '#ece5da'), border: paused ? 'none' : `1px solid ${cardBorder}` }}>
            {paused ? <Play size={14} /> : <Pause size={14} />}{paused ? 'Continuar' : 'Pausar'}
          </button>
          <button onClick={onReset} style={{ padding: '10px 16px', borderRadius: 12, border: `1px solid ${cardBorder}`, cursor: 'pointer', fontSize: 11, fontWeight: 700, display: 'flex', gap: 6, alignItems: 'center', background: isLight ? '#f3f4f6' : '#1d1a15', color: isLight ? '#374151' : '#a49987' }}>
            <RotateCcw size={14} />Reiniciar
          </button>
          <button onClick={onStartBreak} style={{ padding: '10px 18px', borderRadius: 12, border: 'none', cursor: 'pointer', fontSize: 11, fontWeight: 700, display: 'flex', gap: 6, alignItems: 'center', background: isLight ? '#de7356' : '#ece5da', color: isLight ? '#fff' : '#14120e' }}>
            <Zap size={14} />Descansar Ya
          </button>
        </div>
      </div>

      <div style={{ display: 'flex', flexDirection: 'column', gap: 18 }}>
        <div style={{ border: `1px solid ${cardBorder}`, background: cardBg, borderRadius: 24, padding: 18, display: 'flex', flexDirection: 'column', gap: 10 }}>
          <div style={{ fontSize: 13, fontWeight: 700, color: isLight ? '#111827' : '#f7f2ea' }}>Simulador de Actividad (S.O.)</div>
          <div style={{ fontSize: 11, color: isLight ? '#6b7280' : '#8c8273', marginBottom: 4 }}>Toca los botones para simular escenarios de uso en tu PC:</div>
          {simRow('videocall', <Video size={15} />, 'Llamada por Zoom o Teams', 'Detecta micrófono/cámara activos', 'amber')}
          {simRow('writing', <Keyboard size={15} />, 'Escritura Intensiva (Teclado)', 'Retrasa el descanso 2 minutos', 'pink')}
          {simRow('video', <Tv size={15} />, 'Video en Pantalla Completa', 'Pausa alertas durante Netflix/Youtube', 'amber')}
          {simRow('inactive', <UserCheck size={15} />, 'Inactividad del Usuario (AFK)', 'Resetea el contador si te alejas', 'blue')}
        </div>
        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 14 }}>
          <StatCard theme={theme} icon={<Flame size={17} />} label="Racha de Días" value="4 Días" tone="orange" />
          <StatCard theme={theme} icon={<Award size={17} />} label="Pausas Completas" value="15" tone="primary" />
        </div>
      </div>
    </div>
  );
}

window.DashboardScreen = DashboardScreen;
window.formatTime = formatTime;
