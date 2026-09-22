#import "portada-template.typ": portada

#show raw: set text(
  font: "JetBrainsMono NFM",
  weight: "medium",
  size: 1em,
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

// TODO: Actualizar integrantes del Equipo 7
#let integrantes = (
  "Gonzalez Calzada Maximiliano - 2021601769",
)

// TODO: Confirmar secuencia y profesora del periodo 2027 - 1
#portada(
  "UNIDAD DE APRENDIZAJE", // titulo_carrera
  "LÍNEA CURRICULAR", // titulo_materia
  "EQUIPO", // titulo_practica
  "PERIODO", // titulo_secuencia
  "INTEGRANTES", // titulo_alumno
  "SECUENCIA", // titulo_profesorx
  "PROFESORA", // titulo_fecha
  "Ambientes Virtuales Inmersivos", // carrera
  "Simulación de Ambientes Virtuales", // materia
  "Equipo 7", // practica
  "2027 - 1", // secuencia
  integrantes, // alumnos
  "6NM69", // profesorx
  "Bustamante Tranquilino Rocío", // fecha
)

#pagebreak()
#set par(justify: true, leading: 1.4em)
#set heading(numbering: "1.")
#set list(indent: 1.5em)

#title("Definición del Proyecto - Equipo 7")

= Nombre del proyecto

= Objetivo general

= Objetivos específicos

= Justificación

= Alcance
