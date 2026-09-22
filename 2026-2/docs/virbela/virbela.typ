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
  "Alatorre Fuentes Eduardo - 2023601658",
  "Cruz Cruz Guillermo - 2022601088",
  "Flores Roa Jorge Alejandro - 20222602815",
  "Gonzalez Calzada Maximiliano - 2021601769",
  "Neri Mondragón Mónica - 2024600004",
  "Soto Soto Héctor - 2024601122",
)

// LLAMADA RESPETANDO EL ORDEN DEL TEMPLATE
#portada(
  "UNIDAD DE APRENDIZAJE", // titulo_carrera
  "LÍNEA CURRICULAR", // titulo_materia
  "EQUIPO", // titulo_practica
  "SECUENCIA", // titulo_secuencia
  "INTEGRANTES", // titulo_alumno
  "PROFESORA", // titulo_profesorx
  "PERIODO", // titulo_fecha
  "Ambientes Virtuales", // carrera
  "Simulación de Ambientes Virtuales", // materia
  "Equipo 5", // practica
  "6NM69", // secuencia
  integrantes, // alumnos (el array)
  "Bustamante Tranquilino Rocío", // profesorx
  "2026 - 2", // fecha
)

#pagebreak()

#set heading(numbering: "1.")

#title("Reto II.2 El Metaverso en Virbela")

= Link del video
#link("https://drive.google.com/file/d/1yLkIHbtvlA4Aewo-FbV4GQuZzMeB8E-J/view?usp=share_link")[
  https://drive.google.com/file/d/1yLkIHbtvlA4Aewo-FbV4GQuZzMeB8E-J/view?usp=share_link
]
