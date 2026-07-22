const { Sparkles, Flame, Brain, Eye, Heart, Check, Shield, ShieldAlert, ShieldCheck, Calendar, Trash2, Plus, MessageSquare, Utensils, Coffee } = LucideIcons;
const { WeekdaySelector } = window.EyeYul_491522;

const MODES = [
  { id: 'equilibrado', name: 'Equilibrado', desc: 'La regla clásica: 20 mins de trabajo, 30 segs de descanso.', icon: Flame, time: 20, length: 30 },
  { id: 'enfoque', name: 'Enfoque Profundo', desc: 'Sesiones largas: 45 mins de trabajo, 1 min de pausa.', icon: Brain, time: 45, length: 60 },
  { id: 'cuidado', name: 'Cuidado Ocular', desc: 'Frecuente alivio: 15 mins de trabajo, 20 segs de pausa.', icon: Eye, time: 15, length: 20 },
  { id: 'bienestar', name: 'Bienestar Total', desc: 'Salud mental: 30 mins de trabajo, 45 segs de descanso.', icon: Heart, time: 30, length: 45 },
];

function RoutineScreen({ theme, activeSubTab, mode, setMode, plannedBreaks, strictness, setStrictness, messages, setMessages }) {
  const isLight = theme === 'light';
  const cardBg = isLight ? '#fff' : 'rgba(36,33,26,0.8)';
  const cardBorder = isLight ? '#e5e7eb' : '#373127';
  const [newMsg, setNewMsg] = React.useState('');
  const label = isLight ? '#111827' : '#f7f2ea';
  const sub = isLight ? '#6b7280' : '#a49987';

  if (activeSubTab === 'timer') {
    return (
      <div style={{ display: 'flex', flexDirection: 'column', gap: 18 }}>
        <div>
          <div style={{ fontSize: 13, fontWeight: 700, color: label, marginBottom: 4 }}>Modos de Trabajo Predefinidos</div>
          <div style={{ fontSize: 11, color: sub, marginBottom: 10 }}>Cambia rápidamente los intervalos con fórmulas optimizadas de salud:</div>
          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 12 }}>
            {MODES.map((m) => {
              const active = mode === m.id;
              const Icon = m.icon;
              return (
                <button key={m.id} onClick={() => setMode(m.id)} style={{
                  padding: 14, borderRadius: 16, textAlign: 'left', cursor: 'pointer', display: 'flex', gap: 12,
                  border: active ? '1px solid #de7356' : `1px solid ${cardBorder}`,
                  background: active ? '#de7356' : (isLight ? '#fff' : 'rgba(36,33,26,0.5)'),
                  boxShadow: active ? '0 6px 16px rgba(222,115,86,0.25)' : 'none',
                  transition: 'all 0.15s ease',
                }}>
                  <div style={{ width: 36, height: 36, borderRadius: 12, background: active ? 'rgba(255,255,255,0.18)' : 'rgba(222,115,86,0.1)', color: active ? '#fff' : '#de7356', display: 'flex', alignItems: 'center', justifyContent: 'center', flexShrink: 0 }}><Icon size={17} /></div>
                  <div>
                    <div style={{ fontSize: 12, fontWeight: 700, color: active ? '#fff' : label }}>{m.name}</div>
                    <div style={{ fontSize: 10, color: active ? 'rgba(255,255,255,0.85)' : sub, margin: '3px 0' }}>{m.desc}</div>
                    <div style={{ fontSize: 10, fontFamily: 'var(--font-mono)', color: active ? '#fff' : '#de7356' }}>{m.time}m trabajo / {m.length}s descanso</div>
                  </div>
                </button>
              );
            })}
          </div>
        </div>
      </div>
    );
  }

  if (activeSubTab === 'planned') {
    return (
      <div style={{ display: 'flex', flexDirection: 'column', gap: 14 }}>
        <div style={{ fontSize: 13, fontWeight: 700, color: label }}>Pausas Planificadas</div>
        <div style={{ fontSize: 11, color: sub, marginTop: -8 }}>Agenda descansos fijos: almuerzo, estiramientos, café.</div>
        {plannedBreaks.map((pb) => (
          <div key={pb.id} style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', padding: 14, borderRadius: 16, border: `1px solid ${cardBorder}`, background: cardBg }}>
            <div style={{ display: 'flex', gap: 10, alignItems: 'center' }}>
              <div style={{ width: 36, height: 36, borderRadius: 12, background: 'rgba(222,115,86,0.1)', color: '#de7356', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                {pb.icon === 'lunch' ? <Utensils size={16} /> : <Coffee size={16} />}
              </div>
              <div>
                <div style={{ fontSize: 12, fontWeight: 700, color: label, display: 'flex', gap: 8, alignItems: 'center' }}>{pb.name} <span style={{ fontFamily: 'var(--font-mono)', fontSize: 9, background: isLight ? '#f3f4f6' : '#24211a', color: '#de7356', padding: '1px 6px', borderRadius: 6 }}>{pb.startTime}</span></div>
                <div style={{ fontSize: 10, color: sub }}>Duración: {pb.duration / 60} mins · Días: {pb.days.map((d) => ['D', 'L', 'M', 'M', 'J', 'V', 'S'][d]).join(', ')}</div>
              </div>
            </div>
            <Trash2 size={14} color={sub} style={{ cursor: 'pointer' }} />
          </div>
        ))}
      </div>
    );
  }

  if (activeSubTab === 'limits') {
    const options = [
      { id: 'libre', label: 'Modo Libre', icon: ShieldCheck, tone: '#10b981', desc: 'Pospón o salta descansos sin límites. Ideal para gaming.' },
      { id: 'moderado', label: 'Modo Moderado', icon: Shield, tone: '#de7356', desc: 'Finaliza tras un mínimo de 10 segs, sin saltos directos.' },
      { id: 'estricto', label: 'Modo Estricto', icon: ShieldAlert, tone: '#f59e0b', desc: 'Bloquea la pantalla. Debes completar la pausa completa.' },
    ];
    return (
      <div style={{ display: 'flex', flexDirection: 'column', gap: 14 }}>
        <div style={{ fontSize: 13, fontWeight: 700, color: label }}>Grados de Restricción</div>
        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr 1fr', gap: 10 }}>
          {options.map((o) => {
            const active = strictness === o.id;
            const Icon = o.icon;
            return (
              <button key={o.id} onClick={() => setStrictness(o.id)} style={{
                padding: 14, borderRadius: 14, textAlign: 'left', cursor: 'pointer',
                border: active ? `1px solid ${o.tone}` : `1px solid ${cardBorder}`,
                background: active ? o.tone : cardBg,
                boxShadow: active ? `0 6px 16px ${o.tone}40` : 'none',
                color: active ? '#fff' : sub,
              }}>
                <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 8 }}>
                  <span style={{ fontSize: 11, fontWeight: 700 }}>{o.label}</span><Icon size={15} />
                </div>
                <p style={{ fontSize: 9, lineHeight: 1.5, margin: 0, color: active ? 'rgba(255,255,255,0.9)' : sub }}>{o.desc}</p>
              </button>
            );
          })}
        </div>
      </div>
    );
  }

  // messages
  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 14 }}>
      <div style={{ fontSize: 13, fontWeight: 700, color: label }}>Tus Frases de Bienestar Personalizadas</div>
      <div style={{ fontSize: 11, color: sub, marginTop: -8 }}>Escribe mensajes propios para mostrar durante los descansos:</div>
      <div style={{ display: 'flex', gap: 8 }}>
        <input value={newMsg} onChange={(e) => setNewMsg(e.target.value)} placeholder="Ej: Levántate a tomar un vaso de agua..." style={{ flex: 1, padding: '10px 14px', borderRadius: 12, border: `1px solid ${cardBorder}`, background: isLight ? '#fff' : '#1a1814', color: label, fontSize: 11, outline: 'none' }} />
        <button onClick={() => { if (newMsg.trim()) { setMessages([...messages, newMsg.trim()]); setNewMsg(''); } }} style={{ padding: '0 16px', borderRadius: 12, border: 'none', background: '#de7356', color: '#fff', fontWeight: 700, fontSize: 11, display: 'flex', alignItems: 'center', gap: 4, cursor: 'pointer' }}><Plus size={14} />Añadir</button>
      </div>
      {messages.length === 0 ? (
        <div style={{ padding: 16, borderRadius: 12, textAlign: 'center', fontSize: 11, fontStyle: 'italic', color: sub, background: isLight ? '#f9fafb' : 'rgba(26,24,20,0.4)', border: `1px solid ${cardBorder}` }}>No tienes mensajes personalizados. Se mostrarán frases por defecto.</div>
      ) : messages.map((m, i) => (
        <div key={i} style={{ display: 'flex', justifyContent: 'space-between', padding: '10px 14px', borderRadius: 12, border: `1px solid ${cardBorder}`, background: cardBg, fontSize: 11, color: label }}>
          <span>{m}</span><Trash2 size={13} color={sub} style={{ cursor: 'pointer' }} onClick={() => setMessages(messages.filter((_, idx) => idx !== i))} />
        </div>
      ))}
    </div>
  );
}

window.RoutineScreen = RoutineScreen;
window.MODES = MODES;
