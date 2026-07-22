const { useState, useEffect } = React;
const { Toast } = window.EyeYul_491522;
const { ChevronsUp, Eye: EyeIcon, RotateCcw, Monitor } = LucideIcons;

const DEFAULT_PLANNED = [
  { id: 'b1', name: 'Almuerzo Energético', startTime: '13:00', duration: 900, days: [1, 2, 3, 4, 5], icon: 'lunch' },
  { id: 'b2', name: 'Café de la Tarde', startTime: '16:30', duration: 300, days: [1, 2, 3, 4, 5], icon: 'coffee' },
];

function App() {
  const [onboarded, setOnboarded] = useState(true);
  const [themeMode, setThemeMode] = useState('light'); // 'light' | 'dark' | 'system'
  const [systemPrefersDark, setSystemPrefersDark] = useState(
    typeof window !== 'undefined' && window.matchMedia ? window.matchMedia('(prefers-color-scheme: dark)').matches : false
  );
  const [activeTab, setActiveTab] = useState('dashboard');
  const [activeSubTab, setActiveSubTab] = useState('timer');
  const [search, setSearch] = useState('');

  const [totalDuration, setTotalDuration] = useState(20 * 60);
  const [timer, setTimer] = useState(20 * 60);
  const [paused, setPaused] = useState(false);
  const [context, setContext] = useState(null);
  const [overlayMode, setOverlayMode] = useState(null); // 'break' | 'posture' | 'blink'

  const [mode, setMode] = useState('equilibrado');
  const [strictness, setStrictness] = useState('libre');
  const [messages, setMessages] = useState([]);
  const [postureInterval, setPostureInterval] = useState(10);
  const [blinkInterval, setBlinkInterval] = useState(5);
  const [automations, setAutomations] = useState({ dimScreen: true, pauseMusic: true, doNotDisturb: false, smartPauseWriting: true });
  const [contextFlags, setContextFlags] = useState({ videocalls: true, videoPlayback: true, fullscreenGames: false, screenShare: false });
  const [toasts, setToasts] = useState([]);

  useEffect(() => {
    if (paused || context || overlayMode) return;
    const t = setInterval(() => setTimer((p) => (p <= 1 ? 0 : p - 1)), 1000);
    return () => clearInterval(t);
  }, [paused, context, overlayMode]);

  useEffect(() => { if (timer === 0 && !overlayMode) setOverlayMode('break'); }, [timer]);

  useEffect(() => {
    if (!window.matchMedia) return;
    const mq = window.matchMedia('(prefers-color-scheme: dark)');
    const handler = (e) => setSystemPrefersDark(e.matches);
    mq.addEventListener ? mq.addEventListener('change', handler) : mq.addListener(handler);
    return () => (mq.removeEventListener ? mq.removeEventListener('change', handler) : mq.removeListener(handler));
  }, []);

  const theme = themeMode === 'system' ? (systemPrefersDark ? 'dark' : 'light') : themeMode;

  const pushToast = (icon, title, body) => {
    const id = Math.random().toString(36).slice(2);
    setToasts((t) => [{ id, icon, title, body }, ...t].slice(0, 3));
    setTimeout(() => setToasts((t) => t.filter((x) => x.id !== id)), 4000);
  };

  const isLight = theme === 'light';
  const wallpaper = isLight
    ? 'linear-gradient(135deg,#fbf9f6,#f5f1ea 60%,#ebdcc2)'
    : 'linear-gradient(135deg,#120f0c,#1a140f 60%,#14100b)';

  return (
    <div style={{ minHeight: '100vh', background: wallpaper, padding: 20, fontFamily: 'var(--font-sans)', position: 'relative', boxSizing: 'border-box' }}>
      <div style={{
        maxWidth: 1180, margin: '0 auto', borderRadius: 28, overflow: 'hidden', display: 'flex', minHeight: 700,
        border: `1px solid ${isLight ? 'rgba(0,0,0,0.08)' : '#2d2820'}`,
        boxShadow: isLight ? '0 24px 60px rgba(0,0,0,0.08)' : '0 24px 80px rgba(0,0,0,0.7)',
        background: isLight ? 'rgba(250,249,246,0.97)' : 'rgba(29,26,21,0.97)',
      }}>
        <window.Sidebar theme={theme} themeMode={themeMode} setThemeMode={setThemeMode} activeTab={activeTab} setActiveTab={setActiveTab}
          activeSubTab={activeSubTab} setActiveSubTab={setActiveSubTab} search={search} setSearch={setSearch} />

        <div style={{ flex: 1, display: 'flex', flexDirection: 'column' }}>
          <div style={{ padding: '18px 24px', borderBottom: `1px solid ${isLight ? '#e5e7eb' : 'rgba(45,40,32,0.6)'}`, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
            <div>
              <h2 style={{ margin: 0, fontSize: 15, fontWeight: 700, color: isLight ? '#111827' : '#f7f2ea' }}>
                {{ dashboard: 'Vista General (S.O. Cockpit)', routine: 'Rutina de Descansos Programados', wellness: 'Postura Erguida y Salud Ocular', automations: 'Smart Pause (Reglas Automatizadas)', stats: 'Historial de Productividad y Logros' }[activeTab]}
              </h2>
              <p style={{ margin: '2px 0 0', fontSize: 10, fontFamily: 'var(--font-mono)', textTransform: 'uppercase', letterSpacing: '.08em', color: isLight ? '#6b7280' : '#8c8273' }}>
                {{ dashboard: 'Control en vivo del temporizador ocular', routine: 'Frecuencia de descansos y horas de oficina', wellness: 'Configuración de biomonitoreo de webcam', automations: 'Reglas inteligentes y automatización', stats: 'Screen score y rachas diarias' }[activeTab]}
              </p>
            </div>
          </div>

          <div style={{ flex: 1, padding: 24, overflowY: 'auto' }}>
            {activeTab === 'dashboard' && (
              <window.DashboardScreen theme={theme} timer={timer} totalDuration={totalDuration} paused={paused}
                onTogglePause={() => setPaused((p) => !p)} onReset={() => setTimer(totalDuration)}
                onStartBreak={() => setOverlayMode('break')} context={context}
                onToggleContext={(id) => { setContext((c) => (c === id ? null : id)); pushToast(<Monitor size={14} />, 'Simulación de Contexto', `Estado cambiado a: ${id}`); }} />
            )}
            {activeTab === 'routine' && (
              <window.RoutineScreen theme={theme} activeSubTab={activeSubTab} mode={mode} setMode={setMode}
                plannedBreaks={DEFAULT_PLANNED} strictness={strictness} setStrictness={setStrictness}
                messages={messages} setMessages={setMessages} />
            )}
            {activeTab === 'wellness' && (
              <window.WellnessScreen theme={theme} postureInterval={postureInterval} setPostureInterval={setPostureInterval}
                blinkInterval={blinkInterval} setBlinkInterval={setBlinkInterval}
                onTest={(type) => { setOverlayMode(type); pushToast(type === 'posture' ? <ChevronsUp size={14} /> : <EyeIcon size={14} />, type === 'posture' ? '¡Espalda Erguida!' : '¡Parpadea!', type === 'posture' ? 'Siéntate con la espalda recta.' : 'Parpadea 5 veces seguidas.'); }} />
            )}
            {activeTab === 'automations' && (
              <window.AutomationsScreen theme={theme} context={contextFlags} setContext={setContextFlags} automations={automations} setAutomations={setAutomations} />
            )}
            {activeTab === 'stats' && <window.StatsScreen theme={theme} />}
          </div>
        </div>
      </div>

      <div style={{ position: 'fixed', top: 24, right: 24, display: 'flex', flexDirection: 'column', gap: 10, zIndex: 40, width: 300 }}>
        {toasts.map((t) => <Toast key={t.id} icon={t.icon} title={t.title} body={t.body} theme={theme} onClose={() => setToasts((ts) => ts.filter((x) => x.id !== t.id))} />)}
      </div>

      {overlayMode && (
        <window.BreakOverlayScreen mode={overlayMode} duration={overlayMode === 'break' ? 30 : 8}
          onFinished={() => { setOverlayMode(null); if (overlayMode === 'break') setTimer(totalDuration); }}
          onSkipped={() => { setOverlayMode(null); if (overlayMode === 'break') setTimer(totalDuration); }} />
      )}
    </div>
  );
}

window.App = App;
