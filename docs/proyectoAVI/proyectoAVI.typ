#import "@preview/cetz:0.4.2"
#import "portada-template.typ": portada

#show raw: set text(
  font: "JetBrainsMono NFM",
  weight: "medium",
  size: 1em,
)

#import "@preview/codly:1.3.0": *
#show: codly-init.with()

#codly(
  languages: (
    ts: (name: "TypeScript", icon: "", color: rgb("#3178C6")),
    typ: (name: "Typst", icon: "", color: rgb("#239DAD")),
  ),
  number-format: n => str(n),
)

#set text(
  font: "ITC Avant Garde Gothic",
  lang: "es",
  weight: "semibold",
)

#set page(
  paper: "us-letter",
  margin: (left: 3cm, top: 2.5cm, right: 2.5cm, bottom: 2.5cm),
  numbering: "1",
)

#let integrantes = (
  "Alatorre Fuentes Eduardo - 2023601658",
  "Cruz Cruz Guillermo - 2022601088",
  "Flores Roa Jorge Alejandro - 20222602815",
  "Gonzalez Calzada Maximiliano - 2021601769",
  "Neri Mondragón Mónica - 2024600004",
  "Soto Soto Héctor - 2024601122",
)

#portada(
  "UNIDAD DE APRENDIZAJE",
  "LÍNEA CURRICULAR",
  "EQUIPO",
  "PERIODO",
  "INTEGRANTES",
  "SECUENCIA",
  "PROFESORA",
  "Ambientes Virtuales",
  "Simulación de Ambientes Virtuales",
  "Equipo 5",
  "2026 - 2",
  integrantes,
  "6NM69",
  "Bustamante Tranquilino Rocío",
)

#pagebreak()
#set par(justify: true, leading: 1.4em)
#set heading(numbering: "1.")
#set list(indent: 1.5em)

= Nombre del proyecto
#title("Atlalilco virtual: Simulación de una estación del Metro CDMX")

= Objetivo general
Creación de una experiencia interactiva de realidad virtual que replique la inflaestructura del METRO de la CDMX para sensibilizar a los usuarios sobre la eficiencia del transporte público, la reducción de la huella de carbono y los protocolos de seguridad, transformando la percepción del entorno urbano a través de una navegación inmersiva y educativa. 

= Objetivos específicos
- Modelado Técnico y Fiel: Diseñar y texturizar un modelo 3D detallado de una estación representativa del Metro de la CDMX para garantizar el realismo dentro del ambiente virtual inmersivo.

- Interactividad Didáctica: Implementar puntos de información interactivos dentro del recorrido virtual que expliquen el funcionamiento técnico del metro, su historia y las normas de seguridad para el usuario.

- Conciencia Ambiental y Biodiversidad: Integrar un módulo de Sustentabilidad Urbana que visualice el impacto positivo del uso del metro en la reducción de emisiones de CO2, incluyendo la importancia del cuidado de la fauna que a veces llega a las vías como perros o especies rescatadas.

- Optimización del Rendimiento: Asegurar que el ambiente virtual mantenga una tasa de cuadros por segundo (FPS) estable para evitar el mareo por movimiento, permitiendo una navegación fluida tanto para investigadores como para estudiantes.

= Justificación
El proyecto "Atlalilco virtual: Simulación de una estación del Metro CDMX" surge como una iniciativa para explorar el potencial de los ambientes virtuales inmersivos en la educación y la investigación. La elección de una estación del metro como escenario se debe a su relevancia en la vida cotidiana de millones de personas, así como a su complejidad técnica y social que ofrece un amplio campo de estudio. Al crear una simulación detallada y realista de una estación del metro, se busca proporcionar a los usuarios una experiencia educativa que les permita comprender mejor el funcionamiento de este sistema de transporte, su impacto ambiental y su importancia en la movilidad urbana. Además, el proyecto pretende fomentar la conciencia sobre la sustentabilidad y la biodiversidad, destacando cómo el uso del transporte público puede contribuir a la reducción de emisiones de CO2 y al cuidado de la fauna urbana. En un contexto donde la tecnología y la educación están cada vez más interconectadas, este proyecto se posiciona como una herramienta innovadora para la enseñanza y la investigación en el ámbito de los ambientes virtuales, ofreciendo una experiencia inmersiva que combina el aprendizaje con la exploración interactiva.

= Alcance
El presente proyecto tiene como alcance el desarrollo de un ambiente virtual inmersivo que represente de manera interactiva una estación del Metro de la Ciudad de México. Este entorno permitirá a los usuarios recorrer digitalmente distintos espacios de la estación, observar elementos estructurales y conocer información relevante sobre su funcionamiento.
El proyecto se enfocará principalmente en la simulación de espacios clave como andenes, accesos, señalización y áreas de circulación de usuarios. Asimismo, se incorporarán elementos interactivos que proporcionen información educativa relacionada con el sistema de transporte, normas de seguridad y aspectos históricos del metro.
Cabe mencionar que este proyecto se plantea como una simulación con fines educativos y de demostración tecnológica, por lo que no busca replicar todo el sistema del metro de manera completa, sino mostrar un modelo representativo que permita comprender su funcionamiento y utilidad dentro de un entorno virtual.
== Público objetivo
Personas que usan el metro como los empleados, investigadores, estudiantes o trabajadores; como apoyo general y contenido didactico.

== Tecnologías a utilizar
- Unreal Engine
- Blender
- Meta Quest Pro

== Alcance de funciones
Las funciones con las que el usario podra tener interaccion en este ambiente virtual se incluye:

- Navegación: El usuario se podra desplazar por la estación como en la vida real, podra cambiar de direccion y simular el transbordo a otra estacion.

- Interacción con Objetos: Posibilidad de abrir puertas del ascensor, activar torniquetes, simular una recarga de saldo o presionar botones de emrgencia.

- Sistema de Información: Visualización de mapas interactivos para el transbordo segun la estacion o paneles que muestran el tiempo de llegada de los trenes al interactuar con ellos.

Las funciones de simulación y comportamiento que se implementaran para dar realismo al ambiente virtual son:

- Ciclo de Arribo y Salida: Simular del movimiento de los trenes, saber donde es la apertura y cierre de puertas y tiempo estimado de espera al abordar.

- Simulación de Multitudes: Generación de agentes que caminan por los pasillos, suben y bajan del tren para dar realismo al ambiente.

- Sistema de Audio Ambiental: Activación de sonidos espaciales (el sonido del tren acercándose, anuncios por altavoz, ruido de fondo de la gente).

#pagebreak()

#set page(
  width: auto,
  height: auto,
  margin: 1cm,
  fill: white,
)

= Mapa del escenario y distribución

#set text(fallback: true)

// 1. DEFINE LABELS EXTERNALLY
// Defining these here protects them from Cetz's internal `grid` and `stack` functions.
#let entrance-arrow = box(
  fill: rgb("#da7347"),
  radius: 2pt,
  inset: (x: 4pt, y: 2pt),
  align(center + horizon)[#text(fill: black, weight: "bold", size: 8pt)[→]],
)

#let atlalilco-label = box[
  #grid(
    columns: 2,
    gutter: 6pt,
    box(fill: rgb("#ea6c38"), radius: 2pt, width: 18pt, height: 18pt)[
      #align(center + horizon)[#text(fill: white, size: 12pt)[m]]
    ],
    stack(dir: ttb, spacing: 3pt, text(fill: white, size: 14pt)[Atlalilco], box(
      fill: rgb("#e8c050"),
      radius: 1pt,
      inset: (x: 3pt, y: 1pt),
    )[
      #text(fill: black, size: 7pt)[12]
    ]),
  )
]

// 2. DRAW THE MAP
#cetz.canvas({
  import cetz.draw: *

  // BASE CANVAS (Roads Background)
  rect((0, 0), (15, 16), fill: rgb("#3a4b66"), stroke: none)

  // MAP GEOMETRY (Rotated Group)
  group(name: "map-geometry", {
    // In 0.4.2, transformations go at the top of the group
    set-origin((7.5, 8))
    rotate(-8.5deg)

    let block-color = rgb("#233144")
    let park-color = rgb("#0c3a38")

    // --- BLOCKS (Left Side) ---
    rect((-10, 3.5), (-2.5, 10), radius: 0.2, fill: park-color, stroke: none)
    rect((-10, -0.5), (-1.5, 3.0), radius: 0.2, fill: block-color, stroke: none)
    rect((-10, -10), (-2.0, -1.0), radius: 0.2, fill: block-color, stroke: none)

    // --- BLOCKS (Right Side) ---
    rect((1.5, 5.5), (10, 10), radius: 0.2, fill: block-color, stroke: none)
    rect((1.5, 0), (8, 4.5), radius: 0.2, fill: block-color, stroke: none)
    rect((1.5, -10), (10, -0.5), radius: 0.2, fill: block-color, stroke: none)

    // --- TRANSLUCENT SUBWAY OVERLAY ---
    line(
      (-1.5, -10),
      (1.5, -10), // Bottom segment
      (1.5, -2.0),
      (4.5, -2.0), // Right entrance extrusion bottom
      (4.5, 1.0),
      (1.5, 1.0), // Right entrance extrusion top
      (1.5, 10),
      (-1.5, 10), // Top segment
      (-1.5, 2.0),
      (-4.0, 2.0), // Left entrance extrusion top
      (-4.0, -0.5),
      (-1.5, -0.5), // Left entrance extrusion bottom
      fill: rgb(74, 104, 152, 130),
      stroke: none,
      close: true,
    )

    // --- MAIN ROAD MEDIAN ---
    line((0, -10), (0, 10), stroke: 2.5pt + rgb("#eab94a"))

    // --- STREET NAME ---
    content(
      (-0.8, -3),
      angle: 90deg,
      [#text(fill: rgb(255, 255, 255, 50), size: 14pt, tracking: 2pt)[AVENIDA TLÁHUAC]],
    )
  })

  // 3. INSERT LABELS (Global Unrotated Coordinates)
  // By referencing the variables, we safely bypass the namespace issues
  content((4.8, 7.8), entrance-arrow)
  content((9.2, 5.8), entrance-arrow)
  content((7.5, 8.0), atlalilco-label)
})
