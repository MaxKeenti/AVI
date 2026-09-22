/* Thematic motion for the VR-Cirugía deck. Loaded by every slide after slide.js.
 *
 * Two layers:
 *   1. Ambient — a heartbeat blip that sweeps the EKG line (cloned at runtime so
 *      the slide markup stays untouched).
 *   2. Entrance — content blocks rise + sharpen into place, staggered, whenever
 *      the slide becomes the active one. The presenter shell signals activity via
 *      postMessage; a slide opened on its own just plays once on load.
 *
 * Honours prefers-reduced-motion: content is shown immediately with no motion. */
(function () {
  var slide = document.querySelector('.slide');
  if (!slide) return;

  var reduce = window.matchMedia &&
               window.matchMedia('(prefers-reduced-motion: reduce)').matches;

  /* ---- 1. EKG pulse: clone the static heartbeat path as a travelling blip ---- */
  (function ekgPulse() {
    if (reduce) return;
    var svg = document.querySelector('.ekg');
    var base = svg && svg.querySelector('path');
    if (!base) return;

    var pulse = base.cloneNode(true);
    pulse.classList.add('ekg-pulse');
    svg.appendChild(pulse);

    var len = 1600;
    try { len = pulse.getTotalLength() || len; } catch (e) {}
    // A long bright dash with a gap wider than the path → one glowing sweep on screen.
    var dash = 130;
    pulse.style.strokeDasharray = dash + ' ' + (len + 200);
    pulse.style.setProperty('--start', dash);          // just before the path start
    pulse.style.setProperty('--end', -(len + 80));     // just past the path end
  })();

  /* ---- 2. Build the ordered list of reveal targets ---------------------------
     Reveal the direct children of the main content area; for grid-like
     containers, reveal their items instead so cards/badges/legend rows cascade. */
  var GRID = '.cards, .badges, .meta, ol, ul.legend, ul.toc, .legend';

  function buildTargets() {
    var scope = document.querySelector('.content, .cover-pad, .scene, .closing');
    if (!scope) return [];
    var out = [];
    Array.prototype.forEach.call(scope.children, function (child) {
      if (child.matches && child.matches(GRID) && child.children.length) {
        Array.prototype.forEach.call(child.children, function (item) { out.push(item); });
      } else {
        out.push(child);
      }
    });
    return out;
  }

  var targets = buildTargets();

  if (reduce) {
    // No motion: make sure everything is plainly visible.
    targets.forEach(function (el) { el.classList.add('reveal-item', 'is-in'); });
    return;
  }

  targets.forEach(function (el, i) {
    el.classList.add('reveal-item');
    el.style.transitionDelay = (i * 75) + 'ms';
  });

  function reveal() {
    targets.forEach(function (el) { el.classList.add('is-in'); });
    slide.classList.remove('scan');
    void slide.offsetWidth;       // restart the scan animation on every entry
    slide.classList.add('scan');
  }
  function unreveal() {
    slide.classList.remove('scan');
    targets.forEach(function (el) { el.classList.remove('is-in'); });
  }

  /* ---- 3. Triggering --------------------------------------------------------- */
  var inFrame = window.parent && window.parent !== window;

  window.addEventListener('message', function (e) {
    var d = e.data;
    if (!d) return;
    if (d.type === 'vr-deck-active')        requestAnimationFrame(reveal);
    else if (d.type === 'vr-deck-inactive') unreveal();
  });

  if (inFrame) {
    // Tell the presenter we exist so it can (re)send "active" if we're current.
    try { window.parent.postMessage({ type: 'vr-deck-ready' }, '*'); } catch (e) {}
  } else {
    // Stand-alone slide: just play once after layout settles.
    requestAnimationFrame(function () { requestAnimationFrame(reveal); });
  }
})();
