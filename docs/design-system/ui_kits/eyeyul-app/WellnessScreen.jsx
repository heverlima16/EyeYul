const { ChevronsUp, Eye, BellRing } = LucideIcons;

function IntervalGrid({ options, value, onChange, cardBorder, isLight, sub }) {
  return (
    <div style={{ display: 'grid', gridTemplateColumns: `repeat(${options.length}, 1fr)`, gap: 6 }}>
      {options.map((m) => {
        const active = value === m;
        return (
          <button key={m} onClick={() => onChange(m)} style={{
            padding: '7px 4px', borderRadius: 10, fontFamily: 'var(--font-mono)', fontSize: 10, cursor: 'pointer',
            border: active ? '1px solid #de7356' : `1px solid ${cardBorder}`,
            background: active ? '#de7356' : (isLight ? '#f9fafb' : '#1a1814'),
            boxShadow: active ? '0 4px 12px rgba(222,115,86,0.25)' : 'none',
            color: active ? '#fff' : sub, fontWeight: active ? 700 : 400,
          }}>{m === 0 ? 'Off' : `${m}m`}</button>
        );
      })}
    </div>
  );
}

function WellnessScreen({ theme, postureInterval, setPostureInterval, blinkInterval, setBlinkInterval, onTest }) {
  const isLight = theme === 'light';
  const cardBg = isLight ? '#fff' : 'rgba(36,33,26,0.8)';
  const cardBorder = isLight ? '#e5e7eb' : '#373127';
  const label = isLight ? '#111827' : '#f7f2ea';
  const sub = isLight ? '#6b7280' : '#8c8273';

  const Box = ({ tone, icon, tag, title, desc, options, value, onChange, testType }) => (
    <div style={{ border: `1px solid ${cardBorder}`, background: cardBg, borderRadius: 20, padding: 18, display: 'flex', flexDirection: 'column', gap: 12, justifyContent: 'space-between' }}>
      <div style={{ display: 'flex', flexDirection: 'column', gap: 10 }}>
        <div style={{ display: 'flex', justifyContent: 'space-between' }}>
          <span style={{ fontFamily: 'var(--font-mono)', fontSize: 10, letterSpacing: '.1em', color: sub, textTransform: 'uppercase' }}>{tag}</span>
          <span style={{ color: '#de7356' }}>{icon}</span>
        </div>
        <div>
          <div style={{ fontSize: 13, fontWeight: 700, color: label }}>{title}</div>
          <p style={{ fontSize: 10, color: sub, lineHeight: 1.5, margin: '4px 0 0' }}>{desc}</p>
        </div>
        <IntervalGrid options={options} value={value} onChange={onChange} cardBorder={cardBorder} isLight={isLight} sub={sub} />
      </div>
      <button onClick={() => onTest(testType)} style={{ padding: '9px 0', borderRadius: 12, border: `1px solid ${cardBorder}`, background: isLight ? '#f9fafb' : '#1d1a15', color: label, fontSize: 11, fontWeight: 700, cursor: 'pointer', display: 'flex', alignItems: 'center', justifyContent: 'center', gap: 6 }}>
        <BellRing size={13} color="#de7356" />Probar Alerta
      </button>
    </div>
  );

  return (
    <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 18 }}>
      <Box tone="amber" icon={<ChevronsUp size={16} />} tag="Salud Corporal" title="Alerta de Postura"
        desc="Recordatorio flotante discreto para enderezar tu columna vertebral."
        options={[0, 10, 20, 30]} value={postureInterval} onChange={setPostureInterval} testType="posture" />
      <Box tone="pink" icon={<Eye size={16} />} tag="Humectación Ocular" title="Recordatorio de Parpadeo"
        desc="Señal visual diminuta para motivar parpadeos rítmicos contra el ojo seco."
        options={[0, 5, 10, 15]} value={blinkInterval} onChange={setBlinkInterval} testType="blink" />
    </div>
  );
}

window.WellnessScreen = WellnessScreen;
