/* Entrada de cada diapositiva. Cargado por todas después de chrome.js.
 *
 * Los bloques de contenido aparecen en cascada cada vez que la diapositiva se
 * activa. El presentador avisa la activación por postMessage; una diapositiva
 * abierta sola se anima una vez al cargar.
 *
 * Respeta prefers-reduced-motion: el contenido se muestra sin movimiento. */
(function () {
  var slide = document.querySelector('.slide');
  if (!slide) return;

  var reduce = window.matchMedia &&
               window.matchMedia('(prefers-reduced-motion: reduce)').matches;

  /* Se revelan los hijos directos de la tarjeta; en los contenedores marcados
     con data-cascada se revelan sus elementos para que caigan uno tras otro. */
  function buildTargets() {
    var scope = document.querySelector('.tarjeta');
    if (!scope) return [];
    var out = [];
    Array.prototype.forEach.call(scope.children, function (child) {
      if (child.classList.contains('marco')) return;
      if (child.hasAttribute('data-cascada') && child.children.length) {
        Array.prototype.forEach.call(child.children, function (item) { out.push(item); });
      } else {
        out.push(child);
      }
    });
    return out;
  }

  var targets = buildTargets();

  if (reduce) {
    targets.forEach(function (el) { el.classList.add('reveal-item', 'is-in'); });
    return;
  }

  targets.forEach(function (el, i) {
    el.classList.add('reveal-item');
    el.style.transitionDelay = (i * 70) + 'ms';
  });

  function reveal() { targets.forEach(function (el) { el.classList.add('is-in'); }); }
  function unreveal() { targets.forEach(function (el) { el.classList.remove('is-in'); }); }

  var inFrame = window.parent && window.parent !== window;

  window.addEventListener('message', function (e) {
    var d = e.data;
    if (!d) return;
    if (d.type === 'vr-deck-active')        requestAnimationFrame(reveal);
    else if (d.type === 'vr-deck-inactive') unreveal();
  });

  if (inFrame) {
    // Avisa al presentador para que reenvíe «active» si esta es la actual.
    try { window.parent.postMessage({ type: 'vr-deck-ready' }, '*'); } catch (e) {}
  } else {
    requestAnimationFrame(function () { requestAnimationFrame(reveal); });
  }
})();
