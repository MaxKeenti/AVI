/* Shared theme bootstrap for the VR-Cirugía deck.
 *
 * Reads the preferred theme synchronously (so there is no flash before paint),
 * toggles the .light class on <html>, and stays in sync with the presenter
 * shell via postMessage and with other tabs via the storage event.
 * Loaded by every slide and by presenter.html. */
(function () {
  var KEY = 'vrDeckTheme';
  var root = document.documentElement;

  function apply(theme) {
    if (theme === 'light') root.classList.add('light');
    else root.classList.remove('light');
  }
  function read() { try { return localStorage.getItem(KEY); } catch (e) { return null; } }
  function write(v) { try { localStorage.setItem(KEY, v); } catch (e) {} }

  // Initial paint: localStorage first, then ?theme= query, default dark.
  var saved = read();
  if (!saved) {
    var m = (location.search || '').match(/[?&]theme=(light|dark)/);
    if (m) saved = m[1];
  }
  apply(saved === 'light' ? 'light' : 'dark');

  // Presenter -> slide messages
  window.addEventListener('message', function (e) {
    var data = e.data;
    if (!data || data.type !== 'vr-deck-theme') return;
    var value = data.value === 'light' ? 'light' : 'dark';
    apply(value); write(value);
  });

  // Cross-tab sync
  window.addEventListener('storage', function (e) {
    if (e.key !== KEY) return;
    apply(e.newValue === 'light' ? 'light' : 'dark');
  });

  // Tiny API for the presenter to drive its own toggle.
  window.VRDeckTheme = {
    get: function () { return root.classList.contains('light') ? 'light' : 'dark'; },
    set: function (value) {
      value = value === 'light' ? 'light' : 'dark';
      apply(value); write(value); return value;
    },
    toggle: function () { return this.set(this.get() === 'light' ? 'dark' : 'light'); }
  };
})();
