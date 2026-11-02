/* Orden y estructura de la presentación: única fuente de verdad.
   - El orden de este arreglo define el número de página de cada diapositiva.
   - `section` agrupa diapositivas bajo un tema; las secciones se numeran solas
     en el orden en que aparecen aquí y alimentan el número de la etiqueta.
   - `kicker` es el texto de la etiqueta sin su número; si se omite, se usa `section`.
   - `fondo` es la captura de Unity (media/fondos/) que se ve detrás de la tarjeta.
   Para agregar, quitar o reordenar diapositivas basta con editar esta lista. */
(function () {
  var slides = [
    { file: 'portada.html', fondo: 'acceso' },
    { file: 'publico-objetivo.html', section: 'Público objetivo', fondo: 'espera' },
    { file: 'alcance-publico.html', section: 'Alcance', kicker: 'Alcance · 1. Público', fondo: 'recepcion' },
    { file: 'alcance-tecnologico.html', section: 'Alcance', kicker: 'Alcance · 2. Tecnológico', fondo: 'corte-superior' },
    { file: 'alcance-funcional.html', section: 'Alcance', kicker: 'Alcance · 3. Funcional', fondo: 'clasificacion' },
    { file: 'alcance-funcional-practicas.html', section: 'Alcance', kicker: 'Alcance · 3. Funcional', fondo: 'procedimientos' },
    { file: 'arquitectura.html', section: 'Arquitectura', fondo: 'pasillo' },
    { file: 'arquitectura-flujo.html', section: 'Arquitectura', fondo: 'personal' },
    { file: 'arquitectura-ra.html', section: 'Arquitectura', kicker: 'Arquitectura · Realidad aumentada', fondo: 'corte-superior' },
    { file: 'ods.html', section: 'Objetivos de Desarrollo Sostenible', kicker: 'ODS', fondo: 'exterior' },
    { file: 'ods-03.html', section: 'Objetivos de Desarrollo Sostenible', kicker: 'ODS 3', fondo: 'esterilizacion' },
    { file: 'ods-04.html', section: 'Objetivos de Desarrollo Sostenible', kicker: 'ODS 4', fondo: 'espera' },
    { file: 'ods-08.html', section: 'Objetivos de Desarrollo Sostenible', kicker: 'ODS 8', fondo: 'radiografia' },
    { file: 'ods-12.html', section: 'Objetivos de Desarrollo Sostenible', kicker: 'ODS 12', fondo: 'materiales' },
    { file: 'cierre.html', fondo: 'acceso' },
  ];

  // --- Secciones únicas, en orden de aparición ---
  var sections = [];
  slides.forEach(function (s) {
    if (s.section && sections.indexOf(s.section) === -1) sections.push(s.section);
  });

  function pad(n) { return (n < 10 ? '0' : '') + n; }

  window.DECK = {
    slides: slides,
    sections: sections,
    total: slides.length,
    pad: pad,
    // Índice (1-based) de la sección a la que pertenece una diapositiva.
    sectionNumber: function (slide) {
      return slide.section ? sections.indexOf(slide.section) + 1 : null;
    },
    // Índice (0-based) de una diapositiva a partir de su nombre de archivo.
    indexOfFile: function (file) {
      for (var i = 0; i < slides.length; i++) if (slides[i].file === file) return i;
      return -1;
    },
  };
})();
