#import "@preview/cetz:0.4.2"
#import "portada-template.typ": portada

#show raw: set text(
  font: "JetBrainsMono NFM",
  weight: "medium",
  size: 1em,
)

// Importamos la librería codly para numeración automática de líneas
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

#show heading: set text(font: "Fraunces 72pt", weight: "black")

#set page(
  paper: "us-letter",
  margin: (left: 3cm, top: 2.5cm, right: 2.5cm, bottom: 2.5cm),
  numbering: "1",
)

#let integrantes = (
  "Gonzalez Calzada Maximiliano - 2021601769",
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

#title("Reto II.1 Diferencias en la RX")

= Tabla comparativa de lentes virtuales

#table(
  columns: 8,
  align: center,
  fill: (x, y) => if y == 0 { rgb("#5fa949") } else { rgb("#d4edda") },
  text(weight: "bold", fill: white)[Característica],
  text(weight: "bold", fill: white)[RV],
  text(weight: "bold", fill: white)[RA],
  text(weight: "bold", fill: white)[RM],
  text(weight: "bold", fill: white)[Set Virtual],
  text(weight: "bold", fill: white)[Actor Virtual],
  text(weight: "bold", fill: white)[Escenario Virtual],
  text(weight: "bold", fill: white)[Personaje Virtual],

  [Tiempo Real], [✓], [✓], [✓], [✓], [✓], [X], [X],
  [3D], [✓], [✓], [✓], [✓], [✓], [✓], [✓],
  [Interactividad], [✓], [✓], [✓], [✓], [✓], [X], [X],
  [Inmersión], [✓], [X], [✓], [✓], [✓], [X], [X],
  [Cámara], [X], [✓], [✓], [✓], [✓], [X], [✓],
  [Holografía], [X], [X], [✓], [X], [X], [X], [✓],
  [Dispositivos RV], [✓], [✓], [✓], [✓], [✓], [—], [—],
  [Engine], [✓], [✓], [✓], [✓], [✓], [X], [X],
  [Sensores], [X], [X], [X], [✓], [—], [—], [—],
)
