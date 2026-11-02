/* Marco común de las diapositivas: captura de fondo, etiqueta de sección y pie
   con el número de página. Se inyecta aquí a partir de slides.js, así el marco
   se edita en un solo lugar.
   Cargar después de slides.js y antes de anim.js. */
(function () {
  var slide = document.querySelector('.scaler .slide');
  var deck = window.DECK;
  if (!slide || !deck) return;

  var file = (location.pathname.split('/').pop() || 'portada.html');
  var idx = deck.indexOfFile(file);
  var datos = idx === -1 ? {} : deck.slides[idx];

  if (datos.fondo) {
    slide.insertAdjacentHTML('afterbegin',
      '<div class="fondo" style="background-image:url(media/fondos/' + datos.fondo + '.jpg)"></div>');
  }

  // La etiqueta y el pie solo aparecen en las diapositivas de contenido; la
  // portada y el cierre llevan su propia composición.
  var tarjeta = slide.querySelector('.tarjeta');
  var texto = datos.kicker || datos.section;
  if (!tarjeta || !texto) return;

  var n = deck.sectionNumber(datos);
  tarjeta.insertAdjacentHTML('afterbegin',
    '<span class="etiqueta marco">' + (n ? deck.pad(n) + ' · ' : '') + texto + '</span>');
  tarjeta.insertAdjacentHTML('beforeend',
    '<p class="pie marco">Consultorio Seguro · ' + deck.pad(idx + 1) + ' de ' + deck.total + '</p>');
})();
