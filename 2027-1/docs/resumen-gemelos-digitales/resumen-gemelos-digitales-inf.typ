#import "portada-template.typ": portada

#set text(
  font: "ITC Avant Garde Gothic",
  lang: "es",
  weight: "semibold",
  size: 9pt,
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
  proyecto: "Gemelos digitales: resumen de lecturas",
)

#pagebreak()

#set par(justify: true, leading: 0.7em, spacing: 0.9em)
#set list(indent: 0.8em, spacing: 0.6em)
#show heading: set text(size: 10pt)
#show heading: set block(above: 1em, below: 0.6em)
#show table: set par(justify: false)

#align(center)[
  #text(15pt, weight: "bold")[Gemelos digitales: resumen de lecturas]
]

#columns(2, gutter: 0.8cm)[

  = ¿Qué es un gemelo digital?

  Un *gemelo digital* (`GD`) es una réplica virtual de un producto, proceso o sistema físico, alimentada con datos en tiempo real de sensores. Se apoya en `IoT`, `Big Data`, la nube, inteligencia artificial y _machine learning_ para monitorear, simular y predecir, con el fin de tomar mejores decisiones @varas2020gemelos. NVIDIA añade que los gemelos actuales son *físicamente precisos*, con física, materiales e iluminación reales, y sirven como campo de entrenamiento para robots e IA @nvidia2024exploring.

  = Origen y evolución

  - *Años 70 y 80:* la NASA replicó en tierra el módulo del Apolo 13 para resolver sus fallas y luego simuló naves para proteger a los astronautas @delbosque2019gemelos.

  - *2003:* Michael Grieves introduce el término en la Universidad de Michigan, ligado a la gestión del ciclo de vida del producto (`PLM`).

  - *2018 y 2019:* Gartner lo incluye entre las diez tendencias tecnológicas y prevé que la mitad de las industrias lo usará en 2021, con *10 %* más de eficiencia.

  - *Mercado:* de *3.8 a 35.8 mil millones de USD* entre 2019 y 2025 @varas2020gemelos.

  = Cómo funciona

  Se modela matemáticamente el objeto, se instalan sensores para recibir datos en tiempo real y se conecta todo a la nube para procesarlo @delbosque2019gemelos. Existen tres tipos: de *producto*, de *producción* y de *rendimiento*. Se visualizan en centros con pantallas, con `VR`, con `AR` o de forma mixta, lo que conecta el tema con este curso.

  = Ventajas

  - Probar diseños y cambios de producción sin prototipos físicos.

  - Mantenimiento predictivo y menos tiempo de inactividad.

  - Detección de cuellos de botella y ahorro de costos y energía.

  - Menor _time to market_ y productos personalizables.

  #colbreak()

  = Casos reales

  #table(
    columns: (auto, 1fr),
    inset: 3.5pt,
    table.header([*Empresa*], [*Uso y resultado*]),
    [Maserati], [Planta digitalizada con Siemens NX; menos errores y desarrollo más rápido.],
    [GE y Rolls Royce], [Mantenimiento predictivo de motores; GE tiene *800 mil* gemelos en una planta.],
    [Singapur Virtual], [Ciudad con tráfico, clima y demografía para planear obras.],
    [Defensa (España)], [Fragata F-110 y vehículo Dragón 8x8 simulados.],
    [Wistron], [Fábricas listas en *2.5 meses* en vez de 5; *40 %* menos defectos.],
    [Delta Electronics], [Datos sintéticos para entrenar visión artificial *100 veces* más rápido.],
    [Amazon Robotics], [Detección de marcadores del robot Proteus de *88.6 % a 98 %*.],
  )

  Los casos de NVIDIA, que también incluyen a Siemens y Pegatron, se construyen con *OpenUSD* y *Omniverse* para unir datos de distintas herramientas `CAD` y simular con física real @nvidia2024exploring.

  = Barreras

  - Dificultad para digitalizar plantas con equipos antiguos y protocolos propietarios.

  - Grandes volúmenes de datos que muchos sistemas no pueden almacenar ni procesar.

  - Alto costo, que limita su uso en las PyMEs; proyectos como SEDIT buscan abaratarlo @delbosque2019gemelos.

  = Conclusión

  Las tres fuentes coinciden en que el valor del gemelo no está en la réplica misma, sino en los *beneficios* que se obtienen de ella, y en que conviene construirlo por etapas: describir, predecir y simular @varas2020gemelos @delbosque2019gemelos. Su futuro depende de la evolución de la IA, la simulación física y los estándares abiertos. Para nuestro proyecto *Consultorio Seguro*, la idea es la misma: practicar en una réplica virtual evita riesgos y costos reales.
]

#line(length: 100%, stroke: 0.4pt)
#set text(size: 7pt)
#set par(leading: 0.5em, spacing: 0.5em)
#bibliography("media/referencias.bib", style: "apa", title: none)
