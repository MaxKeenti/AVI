#import "portada-template.typ": portada

#set text(
  font: "ITC Avant Garde Gothic",
  lang: "es",
  weight: "semibold",
  size: 10pt,
)

#set page(
  paper: "us-letter",
  margin: (left: 2cm, top: 1.6cm, right: 2cm, bottom: 1.6cm),
)

#portada(
  "UNIDAD DE APRENDIZAJE", // titulo_carrera
  "Línea Curricular", // titulo_materia
  "EQUIPO", // titulo_practica
  "Periodo", // titulo_secuencia
  "Alumno", // titulo_alumno
  "Secuencia", // titulo_profesorx
  "Profesora", // titulo_fecha
  "Ambientes Virtuales Inmersivos", // carrera
  "Simulación de Ambientes Virtuales", // materia
  "Equipo 7", // practica
  "2027 - 1", // secuencia
  ("González Calzada Maximiliano - 2021601769",), // alumnos
  "7NM78", // profesorx
  "Bustamante Tranquilino Rocío", // fecha
  encabezado: ("Plan 2021", "Ingeniería en Informática"),
  proyecto: "Gemelos digitales: reflexión",
)

#pagebreak()

#set par(justify: true, leading: 0.7em, spacing: 1em)
#show heading: set text(size: 11pt)
#show heading: set block(above: 1.1em, below: 0.6em)

#align(center)[
  #text(15pt, weight: "bold")[Gemelos digitales: una reflexión]
]

= Lo que esperaba y lo que encontré

Antes de estas lecturas pensaba que un gemelo digital era un modelo 3D muy detallado de una máquina o una fábrica. Las tres fuentes me hicieron ver que el modelo es la parte menos importante: lo que convierte una réplica en gemelo es su conexión constante con el objeto real por medio de sensores, `IoT` y la nube, y la capacidad de usar esos datos para monitorear, predecir y simular @varas2020gemelos. Sin datos en tiempo real, un gemelo es sólo una maqueta.

= Una idea más vieja de lo que parece

Me llamó la atención que el concepto existiera mucho antes que el nombre. La NASA ya replicaba en tierra el módulo del Apolo 13 para resolver sus fallas a distancia, y Michael Grieves acuñó el término hasta 2003 @delbosque2019gemelos. Esto me hace pensar que muchas "tendencias" tecnológicas son ideas antiguas que por fin se vuelven prácticas y baratas gracias al cómputo actual.

= Entre la promesa y la realidad

Las fuentes no hablan con el mismo tono, y eso me parece importante. El artículo de Varas y la tesis de del Bosque son entusiastas, pero reconocen barreras: plantas con equipos antiguos y protocolos propietarios, volúmenes de datos difíciles de procesar y costos que dejan fuera a muchas PyMEs @delbosque2019gemelos. El ebook de NVIDIA, en cambio, sólo presenta casos de éxito: Wistron pone en marcha fábricas en *2.5 meses* en lugar de 5 y reduce sus defectos *40 %*, y Amazon Robotics sube la detección de marcadores de su robot Proteus de *88.6 % a 98 %* @nvidia2024exploring. Los resultados impresionan, pero es un documento comercial que también vende su plataforma, así que lo leo con cautela. Algo parecido ocurre con el pronóstico de Gartner de que la mitad de las industrias usaría gemelos en 2021: sirve para medir el entusiasmo, no necesariamente la adopción real.

= Lo que más me hizo pensar

La idea más novedosa para mí fue que el gemelo ya no sólo refleja el mundo, sino que sirve como campo de entrenamiento para robots e inteligencia artificial. Delta Electronics entrena sus modelos de visión *100 veces* más rápido con datos sintéticos generados en su gemelo @nvidia2024exploring. Eso cambia su propósito: deja de ser una herramienta de observación y se vuelve un lugar donde se aprende sin riesgo. También me pareció valiosa la advertencia de que el valor no está en la réplica, sino en los beneficios que produce, y de que conviene construirla por etapas @varas2020gemelos.

= Relación con el curso y con nuestro proyecto

El tema conecta directamente con la materia, porque los gemelos se visualizan con `VR`, `AR` o realidad mixta @delbosque2019gemelos, justo lo que estamos aprendiendo a construir. *Consultorio Seguro* no es un gemelo en sentido estricto, porque no recibe datos de un objeto real, pero comparte la idea central: practicar en una réplica virtual para equivocarse sin consecuencias reales. Las lecturas me dejan una pregunta para el proyecto: ¿qué tan fiel debe ser la simulación para que lo aprendido dentro sirva afuera?

= Conclusión

Me quedo con una visión más completa y más crítica. Los gemelos digitales ofrecen beneficios medibles, pero dependen de buenos datos, infraestructura y dinero, y no todas las empresas están listas para ellos. Como futuro ingeniero en informática, me interesa menos la imagen espectacular del gemelo y más el trabajo invisible que lo sostiene: integrar datos de fuentes distintas, procesarlos y convertirlos en decisiones.

#line(length: 100%, stroke: 0.4pt)
#set text(size: 8pt)
#set par(leading: 0.5em, spacing: 0.5em)
#bibliography("media/referencias.bib", style: "apa", title: none)
