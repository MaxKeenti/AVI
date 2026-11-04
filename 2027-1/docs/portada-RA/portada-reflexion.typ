#import "portada-template.typ": portada

#show raw: set text(
  font: "Arial",
  weight: "medium",
  size: 1em,
)

#set text(
  font: "Arial",
  lang: "es",
  weight: "semibold",
)

#set page(
  paper: "us-letter",
  margin: (left: 3cm, top: 2.5cm, right: 2.5cm, bottom: 2.5cm),
  numbering: "1",
)

#let integrantes = (
  "González Calzada Maximiliano - 2021601769",
)

#portada(
  "UNIDAD DE APRENDIZAJE", // titulo_carrera
  "Línea Curricular", // titulo_materia
  "EQUIPO", // titulo_practica
  "Periodo", // titulo_secuencia
  "Integrantes", // titulo_alumno
  "Secuencia", // titulo_profesorx
  "Profesora", // titulo_fecha
  "Simuladores Virtuales", // carrera
  "Simulación de Ambientes Virtuales", // materia
  "Equipo 7", // practica
  "2027 - 1", // secuencia
  integrantes, // alumnos
  "7NM78", // profesorx
  "Bustamante Tranquilino Rocío", // fecha
  encabezado: ("Plan 2021", "Ingeniería en Informática"),
  proyecto: "Reto II.4 Avance del Proyecto",
)

#pagebreak()
#set par(justify: true, leading: 1.4em)
#set heading(numbering: "1.")
#set list(indent: 1.5em)
#show table: set par(justify: false)
