const { Video, Tv, Gamepad2, Share2, Sun, VolumeX, Moon, Activity } = LucideIcons;
const { Toggle } = window.EyeYul_491522;

function AutomationsScreen({ theme, context, setContext, automations, setAutomations }) {
  const isLight = theme === 'light';
  const cardBg = isLight ? '#fff' : 'rgba(36,33,26,0.8)';
  const cardBorder = isLight ? '#e5e7eb' : '#373127';
  const label = isLight ? '#111827' : '#f7f2ea';
  const sub = isLight ? '#6b7280' : '#8c8273';

  const Row = ({ icon, title, desc, checked, onChange, last }) => (
    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', gap: 12, paddingBottom: last ? 0 : 14, marginBottom: last ? 0 : 14, borderBottom: last ? 'none' : `1px solid ${isLight ? '#f3f4f6' : '#312b22'}` }}>
      <div style={{ display: 'flex', gap: 10 }}>
        <span style={{ color: '#de7356', marginTop: 2 }}>{icon}</span>
        <div>
          <div style={{ fontSize: 12, fontWeight: 600, color: label }}>{title}</div>
          <div style={{ fontSize: 9, color: sub, marginTop: 2 }}>{desc}</div>
        </div>
      </div>
      <Toggle checked={checked} onChange={onChange} theme={theme} />
    </div>
  );

  return (
    <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 18 }}>
      <div style={{ border: `1px solid ${cardBorder}`, background: cardBg, borderRadius: 20, padding: 18 }}>
        <div style={{ fontFamily: 'var(--font-mono)', fontSize: 10, letterSpacing: '.1em', color: sub, textTransform: 'uppercase', marginBottom: 12 }}>Filtros de Contexto</div>
        <Row icon={<Video size={15} />} title="Reuniones / Videollamadas" desc="Pospone descansos durante Zoom, Meet, Teams." checked={context.videocalls} onChange={(v) => setContext({ ...context, videocalls: v })} />
        <Row icon={<Tv size={15} />} title="Reproducción de Video" desc="Detecta pantalla completa (Youtube, Netflix)." checked={context.videoPlayback} onChange={(v) => setContext({ ...context, videoPlayback: v })} />
        <Row icon={<Gamepad2 size={15} />} title="Juegos en Pantalla Completa" desc="No interrumpe partidas de gaming activas." checked={context.fullscreenGames} onChange={(v) => setContext({ ...context, fullscreenGames: v })} />
        <Row icon={<Share2 size={15} />} title="Grabación o Compartición" desc="Evita alertas si compartes pantalla." checked={context.screenShare} onChange={(v) => setContext({ ...context, screenShare: v })} last />
      </div>
      <div style={{ border: `1px solid ${cardBorder}`, background: cardBg, borderRadius: 20, padding: 18 }}>
        <div style={{ fontFamily: 'var(--font-mono)', fontSize: 10, letterSpacing: '.1em', color: sub, textTransform: 'uppercase', marginBottom: 12 }}>Acciones de Automatización</div>
        <Row icon={<Sun size={15} />} title="Atenuar la Pantalla" desc="Baja el brillo gradualmente durante la pausa." checked={automations.dimScreen} onChange={(v) => setAutomations({ ...automations, dimScreen: v })} />
        <Row icon={<VolumeX size={15} />} title="Pausar Reproducción de Música" desc="Pausa Spotify/Youtube al iniciar el descanso." checked={automations.pauseMusic} onChange={(v) => setAutomations({ ...automations, pauseMusic: v })} />
        <Row icon={<Moon size={15} />} title='Modo "No Molestar"' desc="Silencia notificaciones durante tus descansos." checked={automations.doNotDisturb} onChange={(v) => setAutomations({ ...automations, doNotDisturb: v })} />
        <Row icon={<Activity size={15} />} title="Pausar por Escritura Activa" desc="Pospone 2 min la pausa si estás escribiendo rápido." checked={automations.smartPauseWriting} onChange={(v) => setAutomations({ ...automations, smartPauseWriting: v })} last />
      </div>
    </div>
  );
}

window.AutomationsScreen = AutomationsScreen;
