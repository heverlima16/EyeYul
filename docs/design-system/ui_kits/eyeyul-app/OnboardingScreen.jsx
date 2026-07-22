const { useState } = React;
const { Sparkles, ArrowRight, ChevronLeft, ChevronRight, Check, Heart, Activity } = LucideIcons;
const { Button, SegmentedTabs } = window.EyeYul_491522;

const TIME_OPTIONS = [10, 20, 30, 45];
const LEN_OPTIONS = [15, 30, 45, 60];

function OnboardingScreen({ onComplete }) {
  const [step, setStep] = useState(1);
  const [timeBetween, setTimeBetween] = useState(20);
  const [breakLen, setBreakLen] = useState(30);
  const [wellnessTab, setWellnessTab] = useState('posture');
  const [postureInt, setPostureInt] = useState(10);
  const [blinkInt, setBlinkInt] = useState(5);

  const next = () => (step < 4 ? setStep(step + 1) : onComplete({ timeBetween, breakLen }));
  const back = () => step > 1 && setStep(step - 1);

  return (
    <div style={{
      minHeight: '100%', display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center',
      padding: 24, background: 'linear-gradient(135deg, #fbf9f6, #f5f1ea 60%, #ebdcc2)',
      fontFamily: 'var(--font-sans)', position: 'relative', overflow: 'hidden',
    }}>
      <div style={{
        width: '100%', maxWidth: 780, minHeight: 540, background: 'rgba(255,255,255,0.97)', borderRadius: 32,
        boxShadow: '0 24px 80px rgba(100,80,60,0.12)', border: '1px solid #e5e0d5', display: 'flex', flexDirection: 'column',
        overflow: 'hidden', position: 'relative', zIndex: 1,
      }}>
        <div style={{ padding: '18px 24px 6px', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <div style={{ display: 'flex', gap: 6 }}>
            <span style={{ width: 11, height: 11, borderRadius: '50%', background: '#ff5f56cc' }} />
            <span style={{ width: 11, height: 11, borderRadius: '50%', background: '#ffbd2ecc' }} />
            <span style={{ width: 11, height: 11, borderRadius: '50%', background: '#27c93fcc' }} />
          </div>
          <span style={{ fontFamily: 'var(--font-mono)', fontSize: 10, letterSpacing: '.15em', color: '#9ca3af' }}>EYEYUL // SET UP WIZARD</span>
          <div style={{ width: 44 }} />
        </div>

        <div style={{ flex: 1, display: 'flex', flexDirection: 'column', justifyContent: 'center', padding: '16px 40px' }}>
          {step === 1 && (
            <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', textAlign: 'center', gap: 22 }}>
              <div style={{ width: 100, height: 100, borderRadius: 28, background: '#fff', border: '1px solid #e5e0d5', display: 'flex', alignItems: 'center', justifyContent: 'center', boxShadow: '0 10px 30px rgba(0,0,0,0.05)' }}>
                <img src={window.__resources ? window.__resources.eyeyulMark : "../../assets/eyeyul-mark.svg"} style={{ width: 56, height: 56 }} />
              </div>
              <div>
                <h1 style={{ fontFamily: 'var(--font-display)', fontSize: 32, fontWeight: 500, margin: 0, color: '#111827' }}>Construye hábitos de pantalla saludables.</h1>
                <p style={{ fontSize: 15, color: '#6b7280', maxWidth: 460, margin: '10px auto 0' }}>Evita la fatiga visual digital de forma gentil, adaptando el ritmo del software a tu vida humana.</p>
              </div>
              <Button variant="inverse" size="lg" icon={<ArrowRight size={18} />} onClick={next}>Comenzar</Button>
            </div>
          )}

          {step === 2 && (
            <div style={{ display: 'flex', flexDirection: 'column', gap: 18 }}>
              <div style={{ textAlign: 'center' }}>
                <div style={{ width: 44, height: 44, borderRadius: 16, background: 'rgba(222,115,86,0.1)', border: '1px solid rgba(222,115,86,0.2)', display: 'flex', alignItems: 'center', justifyContent: 'center', margin: '0 auto 8px', color: '#de7356' }}><Heart size={20} /></div>
                <h2 style={{ fontFamily: 'var(--font-display)', fontSize: 24, margin: 0 }}>Configura tu Rutina de Descanso</h2>
                <p style={{ fontSize: 13, color: '#6b7280', maxWidth: 480, margin: '6px auto 0' }}>La regla 20-20-20 sugiere descansar la vista cada 20 minutos. Personalízalo según tu necesidad.</p>
              </div>
              <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 16 }}>
                <div style={{ border: '1px solid #e5e7eb', borderRadius: 16, padding: 18 }}>
                  <div style={{ fontWeight: 600, fontSize: 14, marginBottom: 4 }}>Tiempo entre descansos</div>
                  <div style={{ fontSize: 11, color: '#6b7280', marginBottom: 12 }}>¿Cada cuánto quieres que YulEye te recuerde hacer una pausa?</div>
                  <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 8 }}>
                    {TIME_OPTIONS.map((m) => (
                      <button key={m} onClick={() => setTimeBetween(m)} style={{
                        padding: '10px 12px', borderRadius: 10, fontFamily: 'var(--font-mono)', fontSize: 12, cursor: 'pointer',
                        border: timeBetween === m ? '1px solid #de7356' : '1px solid #e5e7eb',
                        background: timeBetween === m ? '#de7356' : '#fafafa',
                        boxShadow: timeBetween === m ? '0 4px 12px rgba(222,115,86,0.25)' : 'none',
                        color: timeBetween === m ? '#fff' : '#6b7280',
                        display: 'flex', justifyContent: 'space-between', fontWeight: timeBetween === m ? 700 : 400,
                      }}>{m} mins {timeBetween === m && <Check size={14} />}</button>
                    ))}
                  </div>
                </div>
                <div style={{ border: '1px solid #e5e7eb', borderRadius: 16, padding: 18 }}>
                  <div style={{ fontWeight: 600, fontSize: 14, marginBottom: 4 }}>Duración de la pausa</div>
                  <div style={{ fontSize: 11, color: '#6b7280', marginBottom: 12 }}>Cuánto tiempo se atenuará la pantalla para descansar los ojos.</div>
                  <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 8 }}>
                    {LEN_OPTIONS.map((s) => (
                      <button key={s} onClick={() => setBreakLen(s)} style={{
                        padding: '10px 12px', borderRadius: 10, fontFamily: 'var(--font-mono)', fontSize: 12, cursor: 'pointer',
                        border: breakLen === s ? '1px solid #de7356' : '1px solid #e5e7eb',
                        background: breakLen === s ? '#de7356' : '#fafafa',
                        boxShadow: breakLen === s ? '0 4px 12px rgba(222,115,86,0.25)' : 'none',
                        color: breakLen === s ? '#fff' : '#6b7280',
                        display: 'flex', justifyContent: 'space-between', fontWeight: breakLen === s ? 700 : 400,
                      }}>{s >= 60 ? '1 min' : `${s} segs`} {breakLen === s && <Check size={14} />}</button>
                    ))}
                  </div>
                </div>
              </div>
            </div>
          )}

          {step === 3 && (
            <div style={{ display: 'flex', flexDirection: 'column', gap: 16 }}>
              <div style={{ textAlign: 'center' }}>
                <div style={{ width: 44, height: 44, borderRadius: 16, background: 'rgba(245,158,11,0.1)', border: '1px solid rgba(245,158,11,0.2)', display: 'flex', alignItems: 'center', justifyContent: 'center', margin: '0 auto 8px', color: '#f59e0b' }}><Activity size={20} /></div>
                <h2 style={{ fontFamily: 'var(--font-display)', fontSize: 24, margin: 0 }}>Recordatorios de Bienestar</h2>
                <p style={{ fontSize: 13, color: '#6b7280', maxWidth: 480, margin: '6px auto 0' }}>Pequeños micro-impulsos para cuidar tu postura y mantener tus ojos húmedos.</p>
              </div>
              <div style={{ display: 'flex', justifyContent: 'center' }}>
                <SegmentedTabs value={wellnessTab} onChange={setWellnessTab} options={[{ label: 'Postura Corporal', value: 'posture' }, { label: 'Parpadeo Frecuente', value: 'blink' }]} />
              </div>
              <div style={{ display: 'grid', gridTemplateColumns: '1fr', gap: 8, maxWidth: 320, margin: '0 auto', width: '100%' }}>
                {(wellnessTab === 'posture' ? [0, 10, 20, 30] : [0, 5, 10, 15]).map((m) => {
                  const val = wellnessTab === 'posture' ? postureInt : blinkInt;
                  const set = wellnessTab === 'posture' ? setPostureInt : setBlinkInt;
                  return (
                    <button key={m} onClick={() => set(m)} style={{
                      padding: '10px 14px', borderRadius: 12, fontFamily: 'var(--font-mono)', fontSize: 12, cursor: 'pointer', textAlign: 'left',
                      border: val === m ? '1px solid #de7356' : '1px solid #e5e7eb',
                      background: val === m ? '#de7356' : '#fff',
                      boxShadow: val === m ? '0 4px 12px rgba(222,115,86,0.25)' : 'none',
                      color: val === m ? '#fff' : '#6b7280',
                      display: 'flex', justifyContent: 'space-between',
                    }}>{m === 0 ? 'Desactivado' : `Cada ${m} minutos`} {val === m && <Check size={14} />}</button>
                  );
                })}
              </div>
            </div>
          )}

          {step === 4 && (
            <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', textAlign: 'center', gap: 14 }}>
              <div style={{ width: 44, height: 44, borderRadius: 16, background: '#ecc341', color: '#14120e', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: 20, fontWeight: 700 }}>👍</div>
              <h2 style={{ fontFamily: 'var(--font-display)', fontSize: 24, margin: 0 }}>¡Todo Listo para Cuidar tus Ojos!</h2>
              <p style={{ fontSize: 13, color: '#6b7280', maxWidth: 460 }}>La configuración inicial ha concluido. YulEye se integrará en tu segundo plano con total discreción.</p>
            </div>
          )}
        </div>

        <div style={{ padding: '18px 32px', borderTop: '1px solid #f0ece3', display: 'flex', justifyContent: 'space-between', alignItems: 'center', background: '#fbfaf8' }}>
          <button onClick={back} style={{ visibility: step > 1 ? 'visible' : 'hidden', border: 'none', background: '#f3f4f6', borderRadius: 12, padding: '8px 16px', fontSize: 11, fontWeight: 600, display: 'flex', alignItems: 'center', gap: 4, cursor: 'pointer', color: '#6b7280' }}><ChevronLeft size={14} />Atrás</button>
          <div style={{ display: 'flex', gap: 6 }}>
            {[1, 2, 3, 4].map((i) => <span key={i} style={{ height: 6, width: i === step ? 22 : 6, borderRadius: 999, background: i === step ? '#de7356' : '#e5e0d5', transition: 'all .2s' }} />)}
          </div>
          <button onClick={next} style={{ border: 'none', background: '#111827', color: '#fff', borderRadius: 12, padding: '8px 20px', fontSize: 11, fontWeight: 700, display: 'flex', alignItems: 'center', gap: 4, cursor: 'pointer' }}>{step === 4 ? 'Iniciar App' : 'Siguiente'}<ChevronRight size={14} /></button>
        </div>
      </div>
    </div>
  );
}

window.OnboardingScreen = OnboardingScreen;
