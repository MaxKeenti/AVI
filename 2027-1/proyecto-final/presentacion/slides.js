/* Orden y estructura de la presentación: única fuente de verdad.
   - El orden de este arreglo define el número de página de cada diapositiva.
   - `section` agrupa diapositivas bajo un tema del índice; las secciones se
     numeran solas en el orden en que aparecen aquí y alimentan el índice
     (slide1) y el número del kicker de cada diapositiva.
   - `kicker` es el texto del kicker sin su número; si se omite, se usa `section`.
   Para agregar, quitar o reordenar diapositivas basta con editar esta lista. */
(function () {
  var slides = [
    { file: 'slide0.html' },
    { file: 'slide1.html', kicker: 'Índice' },
    { file: 'slide2.html', section: 'Nombre del proyecto', kicker: 'Nombre' },
    { file: 'slide3.html', section: 'Objetivo general' },
    { file: 'slide4.html', section: 'Objetivos específicos' },
    { file: 'slide5.html', section: 'Justificación' },
    { file: 'slide6.html', section: 'Alcance y tecnología', kicker: 'Alcance' },
    { file: 'slide7.html', section: 'Alcance y tecnología', kicker: 'Alcance' },
    { file: 'slide8.html', section: 'Alcance y tecnología', kicker: 'Alcance de funciones' },
    { file: 'slide-acciones-1.html', section: 'Alcance y tecnología', kicker: 'Alcance · Acciones' },
    { file: 'slide-acciones-2.html', section: 'Alcance y tecnología', kicker: 'Alcance · Acciones' },
    { file: 'slide9.html', section: 'Alcance y tecnología', kicker: 'Alcance · Ambientación' },
    { file: 'slide10.html', section: 'Descripción del AVI' },
    { file: 'slide11.html', section: 'Descripción del AVI' },
    { file: 'slide12.html', section: 'Mapa del AVI' },
    { file: 'slide13.html', section: 'Experiencia de aprendizaje' },
    { file: 'slide14.html', section: 'Link del AVI (video)', kicker: 'Link del AVI' },
    { file: 'slide15.html', section: 'Link de la RA' },
    { file: 'slide16.html' },
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
