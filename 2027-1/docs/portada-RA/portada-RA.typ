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
  "Alatorre Fuentes Eduardo - 2023601658",
  "Cruz Cruz Guillermo - 2022601088",
  "Flores Roa Jorge Alejandro - 2022602815",
  "González Calzada Maximiliano - 2021601769",
  "Neri Mondragón Mónica - 2024600004",
  "Soto Soto Héctor - 2024601122",
)

#portada(
  "UNIDAD DE APRENDIZAJE", // titulo_carrera
  "Línea Curricular", // titulo_materia
  "EQUIPO", // titulo_practica
  "Periodo", // titulo_secuencia
  "Integrantes", // titulo_alumno
  "Secuencia", // titulo_profesorx
  "Profesora", // titulo_fecha
  "Ambientes Virtuales Inmersivos", // carrera
  "Simulación de Ambientes Virtuales", // materia
  "Equipo 7", // practica
  "2027 - 1", // secuencia
  integrantes, // alumnos
  "7NM78", // profesorx
  "Bustamante Tranquilino Rocío", // fecha
  encabezado: ("Plan 2021", "Ingeniería en Informática"),
  proyecto: "Reto II.3 La interfaz de la APK de mi RA",
)

#pagebreak()
#set par(justify: true, leading: 1.4em)
#set heading(numbering: "1.")
#set list(indent: 1.5em)
#show table: set par(justify: false)

#title("Link de la carpeta del proyecto en Google Drive")

#link(
  "https://drive.google.com/drive/folders/1gee-3BykC_bRlON9-YShZLZ8-FfFFswa?usp=share_link",
)[https://drive.google.com/drive/folders/1gee-3BykC_bRlON9-YShZLZ8-FfFFswa?usp=share_link]
