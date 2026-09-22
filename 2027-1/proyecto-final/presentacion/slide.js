/* Scale the 1600×900 canvas to fit the viewport, preserving 16:9.
   Loaded by every slide after the .scaler element. */
(function () {
  var scaler = document.querySelector('.scaler');
  if (!scaler) return;
  function fit() {
    var s = Math.min(window.innerWidth / 1600, window.innerHeight / 900);
    scaler.style.transform = 'scale(' + s + ')';
    scaler.style.width = (1600 * s) + 'px';
    scaler.style.height = (900 * s) + 'px';
  }
  window.addEventListener('resize', fit);
  fit();
})();

/* Numeración dinámica: el número de página, el total y el número de sección del
   kicker se toman de slides.js, así agregar o reordenar diapositivas no obliga
   a editar cada archivo. */
(function () {
  var deck = window.DECK;
  if (!deck) return;

  var file = (location.pathname.split('/').pop() || 'slide0.html');
  var idx = deck.indexOfFile(file);
  if (idx === -1) return;
  var slide = deck.slides[idx];

  var tag = document.querySelector('.page-tag');
  if (tag) tag.innerHTML = '<b>' + deck.pad(idx + 1) + '</b> / ' + deck.total;

  var kicker = document.querySelector('.kicker');
  if (kicker) {
    var texto = slide.kicker || slide.section;
    if (texto) {
      var n = deck.sectionNumber(slide);
      kicker.textContent = n ? deck.pad(n) + ' · ' + texto : texto;
    }
  }
})();
