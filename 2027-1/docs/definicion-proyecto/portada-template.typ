// === Template function definition ===
#let portada(
  titulo_carrera,
  titulo_materia,
  titulo_practica,
  titulo_secuencia,
  titulo_alumno,
  titulo_profesorx,
  titulo_fecha,
  carrera,
  materia,
  practica,
  secuencia,
  alumnos,
  profesorx,
  fecha,
  encabezado: (),
  proyecto: none,
) = [
  #set text(font: "ITC Avant Garde Gothic", size: 12pt)

  #set page(
    paper: "us-letter",
    margin: (left: 3cm, top: 2.5cm, right: 2.5cm, bottom: 2.5cm),
  )
  #set align(center)
  #set par(leading: 0.6em, spacing: 0.9em)

  // === Logos (recoloreados con los colores institucionales) ===
  #let recolor(path, mapa) = {
    let svg = read(path)
    for (de, a) in mapa { svg = svg.replace(de, a) }
    bytes(svg)
  }
  #let logo-ipn = image(
    recolor("media/logos/IPN_Logo.svg", (("#5e5e5e", "#6C1D45"),)),
    format: "svg",
    height: 3.2cm,
  )
  #let logo-upiicsa = image(
    recolor(
      "media/logos/UPIICSA_Logo.svg",
      (("#5e5e5e", "#0f9647"), ("#adadad", "#fff200")),
    ),
    format: "svg",
    height: 3.2cm,
  )

  // === Header with logos ===
  #grid(
    columns: (auto, 1fr, auto),
    align: (left + top, center + bottom, right + top),
    logo-ipn, text(14pt)[Instituto Politécnico Nacional], logo-upiicsa,
  )
  #v(0.2cm)
  #text(13pt)[
    Unidad Profesional Interdisciplinaria de Ingeniería y Ciencias Sociales y Administrativas
  ]

  // === Single column (los 1fr reparten el espacio sobrante) ===
  #v(1fr)
  #for linea in encabezado [#linea \ ]

  #v(1fr)
  #carrera

  #titulo_materia: #materia

  #if proyecto != none {
    block(width: 88%, inset: (y: 0.3cm), text(15pt, weight: "bold", proyecto))
  }

  #practica

  #v(1fr)
  #titulo_alumno

  #if type(alumnos) == array {
    for alumno in alumnos.sorted() [#alumno \ ]
  } else {
    alumnos
  }

  #v(1fr)
  #titulo_fecha: #fecha

  // --- Footer ---
  #v(1fr)
  #grid(
    columns: (1fr, 1fr),
    align(left)[#titulo_profesorx: #profesorx],
    align(right)[#titulo_secuencia #secuencia],
  )
]
