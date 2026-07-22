const { Hourglass, ShieldCheck, Flame, Info } = LucideIcons;
const { StatCard } = window.EyeYul_491522;

const WEEKLY = [
  { day: 'Lunes', breaks: 14, posture: 88, blink: 95, hours: 8.2 },
  { day: 'Martes', breaks: 16, posture: 92, blink: 90, hours: 8.5 },
  { day: 'Miércoles', breaks: 12, posture: 80, blink: 85, hours: 7.8 },
  { day: 'Jueves', breaks: 18, posture: 95, blink: 98, hours: 9.0 },
  { day: 'Viernes', breaks: 15, posture: 94, blink: 92, hours: 8.1 },
  { day: 'Sábado', breaks: 4, posture: 98, blink: 100, hours: 2.5 },
  { day: 'Domingo', breaks: 2, posture: 100, blink: 100, hours: 1.2 },
];

function StatsScreen({ theme }) {
  const [selected, setSelected] = React.useState(4);
  const isLight = theme === 'light';
  const cardBg = isLight ? '#fff' : 'rgba(36,33,26,0.8)';
  const cardBorder = isLight ? '#e5e7eb' : '#373127';
  const label = isLight ? '#111827' : '#f7f2ea';
  const sub = isLight ? '#6b7280' : '#8c8273';
  const day = WEEKLY[selected];

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 18 }}>
      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr 1fr', gap: 14 }}>
        <StatCard theme={theme} icon={<Hourglass size={17} />} label="Tiempo Ocular Protegido" value="105 minutos" tone="primary" />
        <StatCard theme={theme} icon={<ShieldCheck size={17} />} label="Eficiencia de Postura" value="92% promedio" tone="amber" />
        <StatCard theme={theme} icon={<Flame size={17} />} label="Racha de Consistencia" value="4 días seguidos" tone="orange" />
      </div>
      <div style={{ display: 'grid', gridTemplateColumns: '8fr 4fr', gap: 18 }}>
        <div style={{ border: `1px solid ${cardBorder}`, background: cardBg, borderRadius: 20, padding: 18 }}>
          <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 14 }}>
            <div>
              <div style={{ fontSize: 12, fontWeight: 700, color: label }}>Descansos Completados por Día</div>
              <div style={{ fontSize: 10, color: sub }}>Toca cualquier barra para inspeccionar esa métrica.</div>
            </div>
            <span style={{ fontFamily: 'var(--font-mono)', fontSize: 9, fontWeight: 700, color: '#de7356', background: 'rgba(222,115,86,0.1)', padding: '2px 8px', borderRadius: 6, height: 'fit-content' }}>Esta Semana</span>
          </div>
          <div style={{ display: 'flex', alignItems: 'flex-end', gap: 8, height: 140 }}>
            {WEEKLY.map((d, i) => {
              const active = selected === i;
              return (
                <button key={i} onClick={() => setSelected(i)} style={{ flex: 1, display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 6, background: 'none', border: 'none', cursor: 'pointer' }}>
                  <span style={{ fontSize: 9, fontFamily: 'var(--font-mono)', fontWeight: 700, color: active ? '#de7356' : sub }}>{d.breaks}</span>
                  <div style={{ width: '100%', height: `${Math.max(15, (d.breaks / 20) * 100)}%`, borderRadius: '6px 6px 0 0', background: active ? 'linear-gradient(to top, #de7356, #f97316)' : (isLight ? '#e5e7eb' : '#3e372b') }} />
                  <span style={{ fontSize: 9, fontFamily: 'var(--font-mono)', color: active ? '#de7356' : sub, fontWeight: active ? 700 : 400 }}>{d.day.slice(0, 3)}</span>
                </button>
              );
            })}
          </div>
        </div>
        <div style={{ border: `1px solid ${cardBorder}`, background: cardBg, borderRadius: 20, padding: 18, display: 'flex', flexDirection: 'column', gap: 12 }}>
          <span style={{ fontFamily: 'var(--font-mono)', fontSize: 10, letterSpacing: '.1em', color: sub, textTransform: 'uppercase' }}>Detalles: {day.day}</span>
          {[['Pausas 20-20-20', `${day.breaks} de 18`], ['Postura Erguida', `${day.posture}%`], ['Frecuencia Parpadeo', `${day.blink}%`], ['Horas de Pantalla', `${day.hours} hrs`]].map(([k, v], i) => (
            <div key={i} style={{ display: 'flex', justifyContent: 'space-between', fontSize: 11, borderBottom: i < 3 ? `1px solid ${isLight ? '#f3f4f6' : '#312b22'}` : 'none', paddingBottom: i < 3 ? 8 : 0 }}>
              <span style={{ color: sub }}>{k}</span><span style={{ fontWeight: 700, fontFamily: 'var(--font-mono)', color: label }}>{v}</span>
            </div>
          ))}
          <div style={{ display: 'flex', gap: 8, padding: 10, borderRadius: 12, background: isLight ? '#f9fafb' : '#1a1814', fontSize: 10, color: sub, lineHeight: 1.5 }}>
            <Info size={13} color="#de7356" style={{ flexShrink: 0, marginTop: 1 }} />
            <span>{day.breaks >= 15 ? '¡Excelente constancia! Tus ojos recibieron humectación periódica.' : 'Menor cumplimiento detectado por videollamadas prolongadas.'}</span>
          </div>
        </div>
      </div>
    </div>
  );
}

window.StatsScreen = StatsScreen;
