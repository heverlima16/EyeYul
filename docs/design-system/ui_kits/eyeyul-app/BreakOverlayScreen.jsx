const { Clock, Music, ChevronUp, Eye, Wind, ChevronRight } = LucideIcons;
const { Button } = window.EyeYul_491522;

function BreakOverlayScreen({ mode = 'break', duration = 30, onFinished, onSkipped }) {
  const [timeLeft, setTimeLeft] = React.useState(duration);
  const [visual, setVisual] = React.useState(mode === 'posture' ? 'posture' : mode === 'blink' ? 'blink' : 'breath');

  React.useEffect(() => {
    if (timeLeft <= 0) return;
    const t = setInterval(() => setTimeLeft((p) => Math.max(0, p - 1)), 1000);
    return () => clearInterval(t);
  }, [timeLeft]);

  const bg = visual === 'posture'
    ? 'linear-gradient(135deg, #1c0e05, #2c1303 60%, #120500)'
    : visual === 'blink'
    ? 'linear-gradient(135deg, #0c051a, #1c0836 60%, #04010a)'
    : 'linear-gradient(135deg, #121b2a, #1b2a41 60%, #0d131a)';

  const fmt = (s) => `${String(Math.floor(s / 60)).padStart(2, '0')}:${String(s % 60).padStart(2, '0')}`;

  return (
    <div style={{
      position: 'absolute', inset: 0, background: bg, display: 'flex', flexDirection: 'column',
      justifyContent: 'space-between', alignItems: 'center', padding: '32px 40px', color: '#ece5da',
      fontFamily: 'var(--font-sans)', overflow: 'hidden',
    }}>
      <div style={{ position: 'absolute', inset: 0, background: 'rgba(0,0,0,0.3)', backdropFilter: 'blur(30px)' }} />
      <div style={{ width: '100%', display: 'flex', justifyContent: 'space-between', zIndex: 1 }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: 8, fontSize: 10, fontFamily: 'var(--font-mono)', letterSpacing: '.1em', color: '#a49987', opacity: 0.8 }}>
          <span style={{ width: 6, height: 6, borderRadius: '50%', background: '#de7356' }} />EYEYUL SECURE EYE LOCK
        </div>
        <div style={{ display: 'flex', alignItems: 'center', gap: 6, background: 'rgba(255,255,255,0.05)', padding: '6px 14px', borderRadius: 999, fontSize: 12, fontFamily: 'var(--font-mono)', color: '#f0a08c' }}>
          <Clock size={13} />12:58
        </div>
        <div style={{ display: 'flex', alignItems: 'center', gap: 6, background: 'rgba(255,255,255,0.05)', padding: '6px 12px', borderRadius: 999, fontSize: 10 }}>
          <Music size={13} color="#de7356" /><span style={{ fontFamily: 'var(--font-mono)', fontWeight: 700 }}>MINDFUL ZEN ON</span>
        </div>
      </div>

      <div style={{ zIndex: 1, textAlign: 'center', display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 24, maxWidth: 620 }}>
        {visual === 'posture' && (
          <>
            <div style={{ width: 96, height: 96, borderRadius: '50%', background: 'rgba(28,22,18,0.6)', border: '1px solid rgba(245,158,11,0.2)', display: 'flex', alignItems: 'center', justifyContent: 'center' }}><ChevronUp size={40} color="#de7356" /></div>
            <div>
              <div style={{ display: 'inline-block', fontSize: 9, fontFamily: 'var(--font-mono)', letterSpacing: '.1em', background: 'rgba(245,158,11,0.1)', color: '#fcd34d', border: '1px solid rgba(245,158,11,0.2)', padding: '3px 12px', borderRadius: 999, marginBottom: 10 }}>SISTEMA DE POSTURA ACTIVO</div>
              <h1 style={{ fontFamily: 'var(--font-display)', fontSize: 34, fontWeight: 700, margin: '0 0 10px' }}>¡Ponte de Pie!</h1>
              <p style={{ fontSize: 13, color: '#a49987', lineHeight: 1.6 }}>Despégate de la silla, estira tus piernas y alinea tu columna.</p>
            </div>
          </>
        )}
        {visual === 'blink' && (
          <>
            <div style={{ width: 96, height: 96, borderRadius: '50%', background: 'linear-gradient(135deg,#6b21a8,#de7356,#f97316)', display: 'flex', alignItems: 'center', justifyContent: 'center' }}><Eye size={38} color="#14120e" /></div>
            <div>
              <div style={{ display: 'inline-block', fontSize: 9, fontFamily: 'var(--font-mono)', letterSpacing: '.1em', background: 'rgba(222,115,86,0.1)', color: '#f0a08c', border: '1px solid rgba(222,115,86,0.2)', padding: '3px 12px', borderRadius: 999, marginBottom: 10 }}>HUMECTACIÓN ACTIVA</div>
              <h1 style={{ fontFamily: 'var(--font-display)', fontSize: 34, fontWeight: 700, margin: '0 0 10px' }}>Cierra los Ojos</h1>
              <p style={{ fontSize: 13, color: '#a49987', lineHeight: 1.6 }}>Mantén los ojos cerrados un momento y deja que se lubriquen naturalmente.</p>
            </div>
          </>
        )}
        {visual === 'breath' && (
          <>
            <div style={{ width: 96, height: 96, borderRadius: '50%', border: '1px solid rgba(255,255,255,0.1)', display: 'flex', alignItems: 'center', justifyContent: 'center', background: 'rgba(222,115,86,0.08)' }}><Wind size={30} color="#f0a08c" /></div>
            <div>
              <h1 style={{ fontFamily: 'var(--font-display)', fontSize: 40, fontWeight: 700, margin: '0 0 10px' }}>Mirada al Horizonte</h1>
              <p style={{ fontSize: 14, color: '#a49987', lineHeight: 1.6, maxWidth: 480, margin: '0 auto' }}>Enfoca tus ojos en un punto lejano para relajar el enfoque ocular de cerca.</p>
            </div>
          </>
        )}

        <div style={{ display: 'flex', gap: 6, background: 'rgba(18,17,14,0.6)', border: '1px solid rgba(255,255,255,0.1)', padding: 6, borderRadius: 16 }}>
          {[['blink', 'Cerrar Ojos'], ['posture', 'Ponte de Pie'], ['breath', 'Mirar Horizonte']].map(([v, l]) => (
            <button key={v} onClick={() => setVisual(v)} style={{ padding: '8px 14px', borderRadius: 12, border: 'none', fontSize: 11, fontWeight: 700, cursor: 'pointer', background: visual === v ? '#de7356' : 'transparent', color: visual === v ? '#fff' : '#a49987' }}>{l}</button>
          ))}
        </div>

        <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 8, marginTop: 6 }}>
          <div style={{ width: 60, height: 1, background: 'rgba(255,255,255,0.15)' }} />
          <span style={{ fontSize: 44, fontFamily: 'var(--font-mono)', fontWeight: 700 }}>{fmt(timeLeft)}</span>
          <span style={{ fontSize: 9, fontFamily: 'var(--font-mono)', letterSpacing: '.1em', color: '#8c8273', textTransform: 'uppercase' }}>Tiempo restante de descanso</span>
        </div>
      </div>

      <div style={{ zIndex: 1, display: 'flex', gap: 16 }}>
        <button onClick={onSkipped} style={{ padding: '12px 26px', borderRadius: 999, border: '1px solid rgba(255,255,255,0.1)', background: 'rgba(255,255,255,0.05)', color: '#ece5da', fontSize: 11, fontWeight: 600, cursor: 'pointer' }}>Saltar Descanso</button>
        <button onClick={onFinished} style={{ padding: '12px 30px', borderRadius: 999, border: 'none', background: '#de7356', color: '#fff', fontSize: 11, fontWeight: 700, cursor: 'pointer', display: 'flex', alignItems: 'center', gap: 6 }}>Continuar<ChevronRight size={15} /></button>
      </div>
    </div>
  );
}

window.BreakOverlayScreen = BreakOverlayScreen;
