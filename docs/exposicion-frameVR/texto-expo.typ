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

#title[
  Dispositivos que se manejan en ambientes virtuales Inmersivos
]

= Dispositivos para Ambientes Virtuales Inmersivos

La creación de experiencias inmersivas en mundos virtuales es posible gracias a un ecosistema de hardware especializado. Estos dispositivos trabajan en conjunto para estimular los sentidos, rastrear los movimientos del usuario y permitir la interacción con el entorno digital, generando una convincente sensación de presencia @bhowmik2024virtual.

A continuación, se clasifican los principales tipos de dispositivos utilizados en la actualidad.

== 1. Dispositivos de Visualización (La Puerta de Entrada)

Son el elemento central de cualquier experiencia de realidad virtual. Su función es sumergir al usuario en el mundo digital, aislando o complementando el entorno físico.

- *Pantallas montadas en la cabeza (HMD - Head-Mounted Displays)* : Son los cascos o gafas que contienen las pantallas y ópticas necesarias para proyectar imágenes tridimensionales. Se dividen en dos grandes categorías según su método de conexión y procesamiento @acevedo2024systematic:
  - *Tethered (Con Cable)*: Se conectan físicamente a un ordenador o consola de videojuegos de alta potencia, que se encarga de todo el procesamiento gráfico. Ofrecen la máxima fidelidad visual y experiencias más complejas. Ejemplos: Valve Index, HTC Vive Pro 2, PlayStation VR2 @alkhatib2024technological.
  - *Standalone (Autónomos)*: Tienen todo el sistema computacional, las pantallas y la batería integrados en el propio casco, lo que los hace inalámbricos y extremadamente portátiles, aunque con una potencia gráfica inferior a los tethered. Ejemplos: Meta Quest 3, Pico 4 @alkhatib2024technological.

- *Gafas de Realidad Aumentada (AR)* : A diferencia de los HMD inmersivos, estos dispositivos tienen pantallas transparentes que superponen información y objetos digitales sobre el mundo real. La sensación de inmersión es menor, pero permiten la interacción con el entorno físico @elgazi2025immersive. Ejemplo: Microsoft HoloLens.

== 2. Dispositivos de Entrada e Interacción (Manos Digitales)

Permiten al usuario tocar, coger y manipular objetos, así como desplazarse e interactuar con el entorno virtual.

- *Controladores de movimiento* : Son los dispositivos manuales más comunes. Incorporan sensores (IMUs), botones, joysticks y gatillos analógicos. Son rastreados en el espacio 3D para que sus movimientos se traduzcan con precisión en el mundo virtual @tukur2025panoramic. Ejemplos: PlayStation VR2 Sense Controllers, Meta Touch Plus Controllers.
- *Guantes hápticos* : Representan un salto en la interacción natural. Equipados con sensores de flexión, capturan el movimiento individual de cada dedo. Además, algunos modelos incorporan sistemas de retroalimentación háptica para simular la sensación de tocar y sujetar objetos virtuales @bhowmik2024virtual.
- *Sistemas de seguimiento ocular (Eye Tracking)* : Integrados en algunos HMDs avanzados, permiten saber hacia dónde mira el usuario. Esto sirve para interacciones sociales más realistas y para optimizar el rendimiento gráfico mediante renderizado foveado (solo se renderiza en alta calidad la zona a la que se mira) @tukur2025panoramic.
- *Seguimiento corporal completo (Full-Body Tracking)* : Utilizan sensores adicionales (generalmente colocados en cintura y tobillos) para capturar los movimientos de todo el cuerpo, permitiendo que el avatar digital del usuario refleje con precisión sus acciones @acevedo2024systematic.

== 3. Dispositivos de Retroalimentación Sensorial (Sintiendo lo Virtual)

Estos dispositivos buscan involucrar otros sentidos más allá de la vista y el oído para profundizar en la inmersión.

- *Dispositivos hápticos* : Van más allá de la simple vibración de los mandos. Los chalecos y trajes hápticos utilizan una serie de actuadores para simular impactos, rozaduras o la sensación de la lluvia sobre el cuerpo @bhowmik2024virtual. Su objetivo es añadir una capa de fisicalidad a la experiencia virtual.
- *Audio espacial 3D* : El sonido es crucial para la inmersión. Los sistemas de audio espacial, ya sean integrados en el HMD o mediante auriculares especializados, utilizan algoritmos (HRTF) para simular que los sonidos provienen de direcciones y distancias específicas dentro del espacio 3D, engañando al cerebro para que perciba un entorno acústico realista @bhowmik2024virtual.

== 4. Dispositivos de Locomoción y Periféricos Especializados (Movimiento y Realismo)

Solucionan la limitación del espacio físico y aumentan el realismo en aplicaciones específicas.

- *Cintas de marcha omnidireccionales* : Permiten al usuario caminar, correr o gatear en cualquier dirección mientras se mantiene físicamente en el mismo lugar. El dispositivo detecta el movimiento y lo traduce en desplazamiento dentro del mundo virtual, ofreciendo una locomoción natural sin necesidad de espacio físico real @tukur2025panoramic.
- *Periféricos de simulación* : Son dispositivos diseñados para aumentar el realismo en géneros o aplicaciones específicas. Incluyen volantes con pedales para simuladores de conducción, palancas de vuelo (flight sticks) para aviación o réplicas de armas @acevedo2024systematic.

== 5. Tecnologías de Rastreo (El "Cómo" nos ven)

Aunque no son dispositivos que el usuario sostenga, son sistemas fundamentales integrados en el equipo o el entorno para que el sistema de realidad virtual pueda seguir los movimientos.

- *Sistemas de seguimiento "Outside-In"* : Utilizan estaciones base o sensores externos fijos (colocados en las esquinas de la habitación) que emiten láser o luz infrarroja. Estos sensores detectan los marcadores en el HMD y los controladores para triangular su posición con precisión milimétrica y baja latencia @tukur2025panoramic.
- *Sistemas de seguimiento "Inside-Out"* : Este método más moderno integra cámaras y sensores directamente en el HMD. Las cámaras observan el entorno y, mediante algoritmos de visión por ordenador, calculan la posición del casco en relación a puntos fijos de la habitación, eliminando la necesidad de sensores externos y facilitando la configuración @alkhatib2024technological.

#bibliography("media/referencias.bib", style: "apa")
