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
#title("Talismán virtual: Simulación de una estación del Metro CDMX")

= Objetivo general
Creación de una experiencia interactiva de realidad virtual que replique la infraestructura del METRO de la CDMX, en especifico de la estacion Talismán, para sensibilizar a los usuarios sobre la eficiencia del transporte público, la reducción de la huella de carbono y los protocolos de seguridad, transformando la percepción del entorno urbano a través de una navegación inmersiva y educativa.

= Objetivos específicos
- Modelado Técnico y Fiel: Diseñar y texturizar un modelo 3D  de una estación representativa del Metro de la CDMX para reflejar el entorno de la estacion dentro del ambiente virtual inmersivo.

- Interactividad Didáctica: Implementar puntos de información interactivos dentro del recorrido virtual que expliquen el funcionamiento técnico del metro, su historia, las normas de seguridad para el usuario y la presencia del mamut.

- Conciencia Ambiental y Biodiversidad: Integrar un módulo de Sustentabilidad Urbana que visualice el impacto positivo del uso del metro en la reducción de emisiones de CO2, incluyendo la importancia del cuidado de la fauna que a veces llega a las vías como perros o especies rescatadas.

- Optimización del Rendimiento: Asegurar que el ambiente virtual mantenga una tasa de cuadros por segundo (FPS) estable para evitar el mareo por movimiento, permitiendo una navegación fluida tanto para investigadores como para estudiantes.

= Justificación
El proyecto "Talismán virtual: Simulación de una estación del Metro CDMX" surge como una iniciativa para explorar el potencial de los ambientes virtuales inmersivos en la educación y la investigación. La elección de una estación del metro como escenario se debe a su relevancia en la vida cotidiana de millones de personas, así como a su complejidad técnica y social que ofrece un amplio campo de estudio. Al crear una simulación detallada y realista de una estación del metro, se busca proporcionar a los usuarios una experiencia educativa que les permita comprender mejor el funcionamiento de este sistema de transporte, su impacto ambiental y su importancia en la movilidad urbana. Además, el proyecto pretende fomentar la conciencia sobre la sustentabilidad y la biodiversidad, destacando cómo el uso del transporte público puede contribuir a la reducción de emisiones de CO2 y al cuidado de la fauna urbana. En un contexto donde la tecnología y la educación están cada vez más interconectadas, este proyecto se posiciona como una herramienta innovadora para la enseñanza y la investigación en el ámbito de los ambientes virtuales, ofreciendo una experiencia inmersiva que combina el aprendizaje con la exploración interactiva.

= Alcance
El presente proyecto tiene como alcance el desarrollo de un ambiente virtual inmersivo que represente de manera interactiva una estación del Metro de la Ciudad de México. Este entorno permitirá a los usuarios recorrer digitalmente distintos espacios de la estación, observar elementos estructurales y conocer información relevante sobre su funcionamiento.
El proyecto se enfocará principalmente en la simulación de espacios clave como andenes, accesos, señalización y áreas de circulación de usuarios. Asimismo, se incorporarán elementos interactivos que proporcionen información educativa relacionada con el sistema de transporte, normas de seguridad y aspectos históricos del metro.
Cabe mencionar que este proyecto se plantea como una simulación con fines educativos y de demostración tecnológica, por lo que no busca replicar todo el sistema del metro de manera completa, sino mostrar un modelo representativo que permita comprender su funcionamiento y utilidad dentro de un entorno virtual.

== Público objetivo
Personas que usan el metro como los empleados, investigadores, estudiantes o trabajadores; como apoyo general y contenido didactico.

== Tecnologías a utilizar
- Unity
- Blender
- Meta Quest Pro

== Alcance de funciones
Las funciones con las que el usario podra tener interaccion en este ambiente virtual se incluye:

- Navegación: El usuario se podra desplazar por la estación como en la vida real, podra cambiar de direccion y simular viaje en metro.

- Interacción con Objetos: Posibilidad de  activar torniquetes, salir de la estacion, simular una recarga de saldo, observar el mamut o hablar con el personal, beber de un bebedero de agua.

- Sistema de Información: Visualización de mapa interactivos para el transbordo desde Talismán, mapa interactivo con la red el metro, paneles que muestran la hora actual y la simbologia de la estación junto con anuncios publicitarios.

Las funciones de simulación y comportamiento que se implementaran para dar realismo al ambiente virtual son:

- Ciclo de Arribo y Salida: Simular del movimiento de los trenes, saber donde es la apertura y cierre de puertas y tiempo estimado de espera al abordar.

- Simulación de   Autoridad: Generación de agentes que cumplen la funcion de ser trabajadores del metro que proporcionen datos interesantes acerca del metro.

- Sistema de Audio Ambiental: Activación de sonidos espaciales (el sonido del metro acercándose, ruido del metro cuando esta a punto de cerrar las puertas,ruido de fondo de la calle, ruido de pago con tarjeta de movilidad integrada, entre otros).

#pagebreak()

#set page(
  width: auto,
  height: auto,
  margin: 1cm,
  fill: white,
)

= Mapa del escenario y distribución

#let mapa = image("media/logos/Metro Talismán.pdf", height: 3.5cm)
#align(center)[#mapa]
