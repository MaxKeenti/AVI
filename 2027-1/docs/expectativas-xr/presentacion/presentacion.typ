#show raw: set text(
  font: "JetBrainsMono NFM",
  weight: "medium",
  size: 1em,
)

#set text(
  font: "ITC Avant Garde Gothic",
  lang: "es",
  weight: "semibold",
  size: 18pt,
)

#set page(
  paper: "presentation-16-9",
  margin: (x: 1.6cm, top: 1.3cm, bottom: 1.6cm),
)

#let integrantes = (
  "Alatorre Fuentes Eduardo - 2023601658",
  "Cruz Cruz Guillermo - 2022601088",
  "Flores Roa Jorge Alejandro - 2022602815",
  "González Calzada Maximiliano - 2021601769",
  "Neri Mondragón Mónica - 2024600004",
  "Soto Soto Héctor - 2024601122",
)

// === Paleta ===
#let tinta = rgb("#1A1433")
#let violeta = rgb("#7C4DFF")
#let turquesa = rgb("#00A39A")
#let coral = rgb("#E5484D")
#let ambar = rgb("#E8930C")
#let lavanda = rgb("#EEEAFB")
#let gris = rgb("#5F5A73")
#let suave = rgb("#B9B3D9")

#set par(leading: 0.75em, spacing: 0.9em)
#set list(indent: 0.4em, body-indent: 0.5em, spacing: 0.7em, marker: text(fill: violeta)[●])
#show strong: set text(weight: "bold")

// === Componentes ===
#let pie(oscuro) = context {
  set text(10pt, fill: if oscuro { suave } else { gris })
  grid(
    columns: (1fr, auto),
    [Equipo 7 · Ambientes Virtuales Inmersivos · 7NM78], counter(page).display(),
  )
}

#let diapositiva(titulo, cuerpo, oscuro: false) = page(
  fill: if oscuro { tinta } else { white },
  footer: pie(oscuro),
)[
  #set text(fill: if oscuro { white } else { tinta })
  #if titulo != none {
    text(28pt, weight: "bold", titulo)
    v(0.5cm)
  }
  #cuerpo
]

#let fuente(contenido) = place(bottom + left, dy: 0.2cm, text(11pt, fill: gris, contenido))

#let circulo(contenido, color, tam: 1.3cm) = box(
  circle(radius: tam / 2, fill: color, inset: 0pt)[
    #set align(center + horizon)
    #text(fill: white, weight: "bold", size: tam * 0.42, contenido)
  ],
)

#let tarjeta(cuerpo, fondo: lavanda, alto: auto) = block(
  width: 100%,
  height: alto,
  fill: fondo,
  radius: 10pt,
  inset: 0.55cm,
  cuerpo,
)

#let cifra(color, numero, etiqueta) = tarjeta(alto: 100%)[
  #block(below: 0.45cm, text(44pt, weight: "bold", fill: color, top-edge: "cap-height", bottom-edge: "baseline", numero))
  #text(15pt, etiqueta)
]

#let chip(color, contenido) = box(
  fill: color.lighten(85%),
  stroke: 1pt + color,
  radius: 20pt,
  inset: (x: 0.35cm, y: 0.2cm),
  text(14pt, fill: color.darken(30%), contenido),
)

#let enfoque(numero, nombre, color) = grid(
  columns: (auto, 1fr),
  column-gutter: 0.4cm,
  align: horizon,
  circulo(str(numero), color, tam: 1.1cm), text(28pt, weight: "bold", nombre),
)

// === 1. Portada ===
#let blanco(ruta, mapa) = {
  let svg = read(ruta)
  for (de, a) in mapa { svg = svg.replace(de, a) }
  bytes(svg)
}

#page(fill: tinta, footer: none)[
  #set text(fill: white)
  #grid(
    columns: (auto, 1fr, auto),
    align: (left + horizon, center + horizon, right + horizon),
    image(blanco("media/logos/IPN_Logo.svg", (("#5e5e5e", "#FFFFFF"),)), format: "svg", height: 2cm),
    text(12pt, fill: suave)[Instituto Politécnico Nacional · UPIICSA],
    image(
      blanco("media/logos/UPIICSA_Logo.svg", (("#5e5e5e", "#FFFFFF"), ("#adadad", "#D9D4F0"))),
      format: "svg",
      height: 2cm,
    ),
  )
  #v(1fr)
  #text(16pt, fill: suave)[Indagación · Ambientes Virtuales Inmersivos]
  #v(0.2cm)
  #block(width: 85%, text(36pt, weight: "bold")[
    Expectativas de la Realidad Extendida como tecnología #text(fill: violeta.lighten(30%))[core] en la industria
  ])
  #v(1fr)
  #grid(
    columns: (1.4fr, 1fr),
    column-gutter: 1cm,
    text(12pt)[
      #text(fill: suave)[Equipo 7] \
      #for alumno in integrantes.sorted() [#alumno.split(" - ").at(0) \ ]
    ],
    align(bottom, text(12pt)[
      #text(fill: suave)[Profesora] \
      Bustamante Tranquilino Rocío \
      #v(0.2cm)
      #text(fill: suave)[Secuencia y periodo] \
      7NM78 · 2027 - 1
    ]),
  )
]

// === 2. Pregunta guía ===
#diapositiva[¿Complemento o tecnología core?][
  #set list(spacing: 0.55em)
  #grid(
    columns: (1fr, 1fr),
    column-gutter: 0.8cm,
    rows: 7.6cm,
    tarjeta(alto: 100%)[
      #circulo("C", gris)
      #v(0.2cm)
      #text(20pt, weight: "bold")[Complemento]
      #set text(16pt)
      - Pilotos y demostraciones aisladas.
      - Se usa de vez en cuando.
      - Si se apaga, la operación sigue igual.
    ],
    tarjeta(alto: 100%, fondo: violeta.lighten(85%))[
      #circulo("★", violeta)
      #v(0.2cm)
      #text(20pt, weight: "bold")[Tecnología core]
      #set text(16pt)
      - Forma parte central de la operación.
      - Se usa a escala y con resultados medibles.
      - Sin ella, el proceso cambia o se detiene.
    ],
  )
  #v(0.4cm)
  #text(16pt, fill: gris)[Pregunta guía: ¿qué espera cada enfoque de la industria para que la XR pase de la columna izquierda a la derecha?]
]

// === 3. ¿Qué es la XR? ===
#diapositiva[¿Qué es la Realidad Extendida?][
  El término XR agrupa las tecnologías del *continuo realidad-virtualidad*.
  #v(0.6cm)
  #block(width: 100%, height: 1.1cm, radius: 30pt, fill: gradient.linear(turquesa, violeta, tinta.lighten(20%)))
  #v(-0.1cm)
  #grid(
    columns: (1fr, 1fr, 1fr, 1fr),
    align: (left, center, center, right),
    text(14pt, fill: gris)[Entorno real], [], [], text(14pt, fill: gris)[Entorno virtual],
  )
  #v(0.4cm)
  #grid(
    columns: (1fr, 1fr, 1fr),
    column-gutter: 0.6cm,
    tarjeta(alto: 4.2cm)[
      #text(22pt, weight: "bold", fill: turquesa)[AR] \
      #text(15pt)[*Realidad Aumentada:* agrega objetos virtuales sobre el entorno real.]
    ],
    tarjeta(alto: 4.2cm)[
      #text(22pt, weight: "bold", fill: violeta)[MR] \
      #text(15pt)[*Realidad Mixta:* lo virtual se ancla al espacio físico y responde a él.]
    ],
    tarjeta(alto: 4.2cm)[
      #text(22pt, weight: "bold", fill: tinta)[VR] \
      #text(15pt)[*Realidad Virtual:* sustituye por completo el entorno real.]
    ],
  )
  #fuente[Fuente: @milgram1994taxonomy.]
]

// === 4. Mercado en cifras ===
#diapositiva[Todas las proyecciones crecen hacia 2030][
  #grid(
    columns: (1fr, 1fr, 1fr),
    column-gutter: 0.6cm,
    rows: 6.4cm,
    cifra(violeta)[299.3][*mil millones de USD* en software y servicios XR para 2030, desde 44.7 en 2024.],
    cifra(turquesa)[1.5][*billones de USD* que la VR y la AR podrían sumar al PIB mundial; la mayor parte por AR.],
    cifra(coral)[27.3][*millones de lentes inteligentes* al año en 2030, desde 13.6 millones en 2026.],
  )
  #v(0.4cm)
  #text(15pt, fill: gris)[Las cifras no se comparan entre sí: cada consultora mide un alcance distinto.]
  #fuente[Fuentes: @abi2024xr; @pwc2019seeing; @ubrani2026smartglasses.]
]

// === 5. Inversión vs ganancias ===
#let perdidas = (("2020", 6.62), ("2021", 10.19), ("2022", 13.71), ("2023", 16.12), ("2024", 17.72), ("2025", 19.19))

#diapositiva[La inversión va por delante de las ganancias][
  #grid(
    columns: (1.5fr, 1fr),
    column-gutter: 1cm,
    [
      #text(15pt, fill: gris)[Pérdida operativa anual de Meta Reality Labs (miles de millones de USD)]
      #v(0.3cm)
      #grid(
        columns: (1fr,) * perdidas.len(),
        column-gutter: 0.35cm,
        align: center + bottom,
        ..perdidas.map(((anio, valor)) => [
          #text(14pt, weight: "bold")[#str(valor)]
          #v(0.1cm, weak: true)
          #rect(
            width: 100%,
            height: valor / 19.19 * 5.6cm,
            fill: if anio == "2025" { coral } else { suave },
            radius: (top: 4pt),
          )
        ]),
        ..perdidas.map(((anio, valor)) => text(13pt, fill: gris, anio)),
      )
    ],
    [
      #tarjeta[
        #text(36pt, weight: "bold", fill: violeta)[7 M]
        #v(0.1cm)
        #text(14pt)[lentes Ray-Ban y Oakley con Meta vendidos en 2025, el triple que en 2024.]
      ]
      #tarjeta[
        #text(36pt, weight: "bold", fill: turquesa)[+167 %]
        #v(0.1cm)
        #text(14pt)[lentes inteligentes en el primer trimestre de 2026; los visores VR pierden terreno.]
      ]
    ],
  )
  #fuente[Fuentes: @auganix2026reality; @roadtovr2026glasses; @ubrani2026smartglasses.]
]

// === 6. Enfoques ===
#let enfoques = (
  ("Académico", violeta),
  ("Comercial", coral),
  ("Industrial", ambar),
  ("Salud", turquesa),
  ("Gobierno y defensa", gris),
  ("Capacitación corporativa", tinta),
)

#diapositiva[Seis enfoques, seis expectativas][
  #v(0.3cm)
  #grid(
    columns: (1fr, 1fr, 1fr),
    column-gutter: 0.6cm,
    row-gutter: 0.6cm,
    ..enfoques
      .enumerate(start: 1)
      .map(((i, (nombre, color))) => tarjeta(alto: 3.4cm)[
        #circulo(str(i), color)
        #v(0.25cm)
        #text(19pt, weight: "bold", nombre)
      ]),
  )
]

// === 7. Académico ===
#diapositiva(none)[
  #enfoque(1, "Enfoque académico", violeta)
  #v(0.5cm)
  #grid(
    columns: (1.5fr, 1fr),
    column-gutter: 0.8cm,
    rows: 6.6cm,
    [
      *Expectativa:* practicar lo que en el aula solo se explica: anatomía, máquinas peligrosas, sitios históricos.
      #v(0.2cm)
      - El modelo CAMIL explica cuándo la inmersión ayuda a aprender:
      #v(0.1cm)
      #h(0.9cm) #chip(violeta)[Presencia] #h(0.2cm) #chip(violeta)[Agencia] #h(0.2cm) #chip(gris)[Carga cognitiva controlada]
      - EDUCAUSE la ve con alto potencial, pero entre las más costosas por equipo y contenido.
    ],
    cifra(violeta)[18][*áreas de aplicación* de la VR en educación superior. Aun así, la mayoría de estudios mide usabilidad, no aprendizaje.],
  )
  #fuente[Fuentes: @radianti2020systematic; @makransky2021camil; @brown2020horizon.]
]

// === 8. Comercial ===
#diapositiva(none)[
  #enfoque(2, "Enfoque comercial", coral)
  #v(0.5cm)
  *Expectativa:* cambiar la forma de comprar y de entretenerse, con dispositivos de uso diario.
  #v(0.4cm)
  #grid(
    columns: (1fr, 1fr, 1fr),
    column-gutter: 0.6cm,
    rows: 6.4cm,
    cifra(coral)[3 de 4][encuestados creen que la AR pesará más en sus compras en cinco años: probar productos y ver muebles en casa.],
    cifra(coral)[7 M][lentes inteligentes vendidos en 2025: la apuesta se mueve del visor a los lentes.],
    cifra(coral)[169.4][*mil millones de USD* esperados en software y servicios XR de consumo para 2030.],
  )
  #fuente[Fuentes: @marketingdive2021shopping; @roadtovr2026glasses; @abi2024xr.]
]

// === 9. Industrial ===
#diapositiva(none)[
  #enfoque(3, "Enfoque industrial y de manufactura", ambar)
  #v(0.5cm)
  #grid(
    columns: (1fr, 1.4fr),
    column-gutter: 0.8cm,
    block(height: 7.4cm, cifra(ambar)[92 %][de más de 350 ejecutivos de manufactura ya experimenta o implementa casos del *metaverso industrial*.]),
    [
      *Expectativa:* unir XR y gemelos digitales para operar mejor.
      #v(0.3cm)
      #chip(ambar)[Gemelos digitales] #chip(ambar)[Simulación de procesos] \
      #v(0.15cm)
      #chip(ambar)[Capacitación de operadores] #chip(ambar)[Soporte remoto]
      #v(0.5cm)
      #tarjeta[
        #block(below: 0.35cm, text(32pt, weight: "bold", fill: ambar, top-edge: "cap-height", bottom-edge: "baseline")[+12 %]) #text(15pt)[esperado en calidad, rendimiento y productividad en 1 a 3 años.]
      ]
    ],
  )
  #fuente[Fuentes: @deloitte2023industrial; @pwc2019seeing.]
]

// === 10. Salud ===
#diapositiva(none)[
  #enfoque(4, "Enfoque de salud", turquesa)
  #v(0.5cm)
  *Expectativa:* que el personal practique sin poner en riesgo a pacientes reales.
  #v(0.4cm)
  #grid(
    columns: (1fr, 1.6fr),
    column-gutter: 0.8cm,
    rows: 6.8cm,
    cifra(turquesa)[31][*ensayos* en cirugía laparoscópica analizados en un metaanálisis.],
    tarjeta(alto: 100%)[
      #text(15pt, fill: gris)[Resultado del entrenamiento con VR]
      #v(0.2cm)
      #grid(
        columns: (auto, 1fr),
        row-gutter: 0.35cm,
        column-gutter: 0.3cm,
        align: horizon,
        circulo(">", turquesa, tam: 0.9cm), text(16pt)[más efectivo que el entrenamiento con video],
        circulo("≥", turquesa, tam: 0.9cm), text(16pt)[al menos igual que las cajas de práctica],
      )
      #v(0.3cm)
      #text(15pt)[Es el mismo principio de nuestro *Consultorio Seguro*: practicar la clasificación de residuos sin tocar materiales peligrosos.]
    ],
  )
  #fuente[Fuente: @alaker2016laparoscopic.]
]

// === 11. Defensa ===
#let hitos = (
  ([Origen], [IVAS: visores basados en HoloLens para el Ejército de EE. UU.]),
  ([Abril 2025], [El contrato pasa de Microsoft a Anduril.]),
  ([Sept. 2025], [Se reorganiza como *Soldier Borne Mission Command*.]),
  ([Julio 2026], [Las unidades originales no se despliegan: muy caras a escala.]),
)

#diapositiva(none)[
  #enfoque(5, "Enfoque de gobierno y defensa", gris)
  #v(0.5cm)
  *Expectativa:* dar al soldado información táctica en tiempo real con MR.
  #v(0.6cm)
  #grid(
    columns: (1fr,) * hitos.len(),
    column-gutter: 0.5cm,
    ..hitos
      .enumerate(start: 1)
      .map(((i, (fecha, texto))) => [
        #circulo(str(i), if i == hitos.len() { coral } else { gris }, tam: 1cm)
        #v(0.2cm)
        #text(16pt, weight: "bold", fecha) \
        #text(14pt, texto)
      ]),
  )
  #v(0.5cm)
  #text(15pt, fill: gris)[La expectativa sigue en pie, pero el costo del equipo puede detener un programa completo.]
  #fuente[Fuente: @crs2026ivas.]
]

// === 12. Corporativo ===
#diapositiva(none)[
  #enfoque(6, "Enfoque de capacitación corporativa", tinta)
  #v(0.5cm)
  *Expectativa:* capacitación más rápida, medible y repetible. Estudio de PwC con nuevos gerentes:
  #v(0.4cm)
  #grid(
    columns: (1fr, 1fr, 1fr),
    column-gutter: 0.6cm,
    rows: 6cm,
    cifra(violeta)[4×][hasta cuatro veces *más rápido* que en el aula.],
    cifra(turquesa)[275 %][hasta 275 % *más seguros* de aplicar lo aprendido.],
    cifra(ambar)[375][participantes para que el costo de la VR *se iguale* al del aula.],
  )
  #fuente[Fuente: @pwc2020vlearning.]
]

// === 13. Comparativa ===
#let madurez(color, etiqueta) = box(
  fill: color,
  radius: 20pt,
  inset: (x: 0.3cm, y: 0.15cm),
  text(13pt, fill: white, weight: "bold", etiqueta),
)

#diapositiva[Comparativa de expectativas][
  #set text(16pt)
  #table(
    columns: (1fr, 2.4fr, 1.1fr),
    inset: (x: 0.3cm, y: 0.32cm),
    align: (left + horizon, left + horizon, left + horizon),
    stroke: (x, y) => if y > 0 { (top: 0.5pt + suave) },
    fill: (x, y) => if y == 0 { tinta },
    table.header(..([Enfoque], [Expectativa principal], [Madurez]).map(t => text(fill: white, weight: "bold", t))),
    [Académico], [Aprendizaje práctico y seguro, basado en competencias.], madurez(gris)[Experimental],
    [Comercial], [Prueba virtual de productos y lentes de uso diario.], madurez(ambar)[En crecimiento],
    [Industrial], [Gemelos digitales, simulación y soporte remoto.], madurez(ambar)[Adopción temprana],
    [Salud], [Entrenamiento clínico sin riesgo para pacientes.], madurez(turquesa)[Con evidencia],
    [Defensa], [Información táctica en tiempo real.], madurez(coral)[Reorganizándose],
    [Corporativo], [Capacitación más rápida y medible.], madurez(turquesa)[Con evidencia],
  )
]

// === 14. Retos ===
#let retos = (
  ("$", "Costo", [Principal barrera para el *51 %* de los ejecutivos.], coral),
  ("T", "Talento", [Faltan perfiles para crear y mantener soluciones.], ambar),
  ("↔", "Integración", [Conectar la XR con los sistemas existentes.], violeta),
  ("!", "Ciberseguridad", [Riesgo mayor para *más del 70 %*: capturan video, voz y entorno.], coral),
  ("?", "Evidencia", [En educación se mide más la usabilidad que el aprendizaje.], turquesa),
  ("↓", "Rentabilidad", [El consumo aún no paga la inversión.], gris),
)

#diapositiva[Retos que frenan la adopción][
  #grid(
    columns: (1fr, 1fr, 1fr),
    column-gutter: 0.6cm,
    row-gutter: 0.5cm,
    ..retos.map(((icono, nombre, texto, color)) => tarjeta(alto: 4.6cm)[
      #grid(
        columns: (auto, 1fr),
        column-gutter: 0.3cm,
        align: horizon,
        circulo(icono, color, tam: 1cm), text(18pt, weight: "bold", nombre),
      )
      #v(0.2cm)
      #text(14pt, texto)
    ]),
  )
  #fuente[Fuentes: @deloitte2023industrial; @crs2026ivas; @radianti2020systematic; @auganix2026reality.]
]

// === 15. ¿Es core? ===
#diapositiva[¿Es la XR una tecnología core?][
  #set list(spacing: 0.55em)
  #grid(
    columns: (1fr, 1fr),
    column-gutter: 0.8cm,
    rows: 8.6cm,
    tarjeta(alto: 100%, fondo: turquesa.lighten(85%))[
      #circulo("✓", turquesa)
      #v(0.2cm)
      #text(20pt, weight: "bold")[Ya es core en…]
      #set text(16pt)
      - Capacitación y simulación.
      - Diseño de productos y gemelos digitales.
      - Soporte en campo y mantenimiento.
      #v(0.2cm)
      #text(14pt, fill: gris)[Cuando se capacita a muchas personas o equivocarse en la realidad es caro o peligroso.]
    ],
    tarjeta(alto: 100%, fondo: coral.lighten(88%))[
      #circulo("×", coral)
      #v(0.2cm)
      #text(20pt, weight: "bold")[Todavía no en…]
      #set text(16pt)
      - El consumidor general.
      - El visor VR como dispositivo diario.
      - La mayoría de las empresas.
      #v(0.2cm)
      #text(14pt, fill: gris)[La apuesta cambia hacia lentes ligeros con inteligencia artificial.]
    ],
  )
  #fuente[Fuente: @ubrani2026smartglasses.]
]

// === 16. Conclusión ===
#diapositiva(oscuro: true)[Conclusión][
  #v(0.3cm)
  #text(24pt)[
    La XR apunta a ser una capa más de la operación, pero cada enfoque la adopta a su ritmo.
  ]
  #v(0.6cm)
  #grid(
    columns: (1fr, 1fr, 1fr, 1fr),
    column-gutter: 0.5cm,
    ..(
      ("Academia", "busca evidencia de aprendizaje", violeta),
      ("Comercio", "busca dispositivos cómodos y baratos", coral),
      ("Industria", "busca productividad", ambar),
      ("Salud y defensa", "buscan reducir riesgos", turquesa),
    ).map(((quien, que, color)) => block(
      width: 100%,
      inset: 0.4cm,
      radius: 10pt,
      stroke: 1.5pt + color,
    )[
      #text(17pt, weight: "bold", fill: color.lighten(25%), quien) \
      #text(14pt, que)
    ]),
  )
  #v(0.6cm)
  #text(16pt, fill: suave)[Las expectativas más realistas unen la XR a un problema concreto, como nuestro simulador de manejo de residuos.]
]

// === 17. Referencias ===
#page(footer: pie(false))[
  #text(28pt, weight: "bold")[Referencias]
  #v(0.3cm)
  #set text(10pt)
  #set par(leading: 0.42em, spacing: 0.5em, hanging-indent: 0.8cm)
  #columns(2, gutter: 0.8cm)[
    #bibliography("media/referencias.bib", style: "apa", title: none)
  ]
]
