/* Marco común de las diapositivas: esquinas, línea EKG, marca, kicker y número
   de página. Antes estaba copiado en cada archivo; ahora se inyecta aquí a
   partir de slides.js, así el marco se edita en un solo lugar.
   Cargar después de slides.js y antes de anim.js. */
(function () {
  var slide = document.querySelector('.scaler .slide');
  var deck = window.DECK;
  if (!slide || !deck) return;

  var file = (location.pathname.split('/').pop() || 'slide0.html');
  var idx = deck.indexOfFile(file);
  var datos = idx === -1 ? {} : deck.slides[idx];

  var EKG_PATH =
    'M0 18 H300 l10 -13 7 26 9 -20 10 16 H760 l10 -13 7 26 9 -20 10 16 ' +
    'H1180 l10 -13 7 26 9 -20 10 16 H1460';

  var partes = [
    '<span class="corner tl"></span><span class="corner tr"></span>',
    '<span class="corner bl"></span><span class="corner br"></span>',
    '<svg class="ekg" viewBox="0 0 1460 36" preserveAspectRatio="none">' +
      '<path d="' + EKG_PATH + '"/></svg>',
  ];

  // El kicker y el número de página solo aparecen en las diapositivas de
  // contenido; la portada y el cierre llevan la marca vacía.
  var texto = datos.kicker || datos.section;
  if (texto) {
    var n = deck.sectionNumber(datos);
    partes.push('<span class="kicker">' + (n ? deck.pad(n) + ' · ' : '') + texto + '</span>');
    partes.push('<span class="brand-tag">Consultorio Seguro</span>');
    if (idx !== -1) {
      partes.push(
        '<span class="page-tag"><b>' + deck.pad(idx + 1) + '</b> / ' + deck.total + '</span>'
      );
    }
  } else {
    partes.push('<span class="brand-tag"></span>');
  }

  slide.insertAdjacentHTML('afterbegin', partes.join('\n'));
})();
