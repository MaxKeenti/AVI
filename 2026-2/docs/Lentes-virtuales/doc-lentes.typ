#import "@preview/cetz:0.4.2"
#import "portada-template.typ": portada

#show raw: set text(
  font: "JetBrainsMono NFM",
  weight: "medium",
  size: 1em,
)

// Importamos la librería codly para numeración automática de líneas
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

#align(center)[
  #v(2cm)
  #text(22pt, weight: "bold")[EVOLUCIÓN DE LOS LENTES VIRTUALES]
]
= = Antecedentes Ópticos: La Base de la RV
La tecnología actual de realidad virtual se sustenta en principios físicos descubiertos hace casi dos siglos.

- *Estereoscopio (1838):* Inventado por *Sir Charles Wheatstone*, quien descubrió que presentar dos imágenes ligeramente diferentes a cada ojo permitía al cerebro crear una ilusión de profundidad y tridimensionalidad.
- *Visor de Brewster (1849):* Considerado el primer antepasado directo de los visores actuales, consistía en una caja de madera equipada con lentes diseñadas específicamente para visualizar fotografías en 3D de manera portátil.
- *View-Master (1939):* Este juguete icónico, basado en discos de cartón con transparencias, fue fundamental porque sentó las bases conceptuales para soluciones futuras de bajo costo como Google Cardboard.

= Google Cardboard: La Democratización (2014)
Este proyecto cambió la industria al demostrar que la inmersión no requería de hardware extremadamente costoso.

- *Origen:* Surgió como un proyecto de "tiempo libre" de los ingenieros David Coz y Damien Henry en las oficinas de Google París.
- *Filosofía "Low-Fi":* Su objetivo principal era democratizar la RV utilizando materiales simples como cartón corrugado y lentes biconvexas de plástico, convirtiendo el smartphone del usuario en el "cerebro" del sistema.
- *Legado:* Impulsó proyectos educativos como *Google Expeditions* y estableció el estándar de consumo de contenido inmersivo en plataformas como YouTube VR.
- *Costo:* Altamente accesible, situándose en un rango de entre *5 y 20 USD*.

#pagebreak()

= Meta Quest (Oculus): Autonomía y Consumo
Meta ha liderado la transición de la realidad virtual conectada a PC hacia sistemas totalmente independientes.

- *Historia:* La marca se origina con Oculus VR, fundada por Palmer Luckey en 2012. Tras el éxito del prototipo Oculus Rift, la empresa fue adquirida por Facebook (ahora Meta) en 2014.
- *Versiones Clave:*
  - *Oculus Quest (2019):* Marcó un hito como el primer visor totalmente independiente que no requería cables ni PC.
  - *Meta Quest 2 (2020):* Se consolidó como uno de los visores más populares del mundo por ser más ligero, potente y económico.
  - *Meta Quest 3 (2023):* La evolución actual, enfocada en Realidad Mixta (MR) avanzada y un rendimiento gráfico superior.
- *Costos:* Varían desde los *MXN 5,899* (Quest 3S) hasta los *MXN 15,999* (Quest Pro).

= HTC Vive: Precisión Industrial y "Room-Scale"
Resultado de una alianza estratégica entre HTC y Valve Corporation para competir en el mercado de alta gama.

- *Innovaciones:*
  - *Room-Scale VR:* Permitió por primera vez caminar físicamente en un espacio de hasta 4.5 $m^2$ con una traducción exacta al entorno virtual.
  - *Precisión Láser:* Utiliza estaciones base (*Lighthouses*) para un rastreo milimétrico que elimina casi por completo el mareo por movimiento.
  - *Sistema Chaperone:* Una cuadrícula de seguridad que alerta al usuario para evitar colisiones con el mundo real.
- *Enfoque:* Actualmente es la opción predilecta en el sector empresarial (medicina, diseño industrial y militar) debido a su alta fidelidad.

#pagebreak()

= Apple Vision Pro: Computación Espacial (2024)
Apple redefine el visor no como un accesorio, sino como una plataforma de cómputo completa.

- *Concepto:* Se define como *Computación Espacial*, permitiendo al usuario interactuar con contenido digital como si estuviera físicamente integrado en su espacio real.
- *Características Únicas:*
  - *Control Natural:* Eliminación de mandos físicos, utilizando únicamente el seguimiento de los ojos, gestos de las manos y comandos de voz.
  - *EyeSight:* Una innovación que proyecta los ojos del usuario en la parte externa del visor para mantener la conexión social con las personas alrededor.
  - *Propósito:* Convertir el entorno entero en una pantalla infinita, eliminando las limitaciones de los monitores tradicionales.

= Microsoft HoloLens: Realidad Mixta Profesional
Microsoft se enfoca en el sector industrial con un dispositivo que no bloquea la visión del usuario.

- *Definición:* Gafas de Realidad Mixta (MR) que superponen hologramas transparentes sobre el mundo real.
- *Hardware:* Es un sistema autónomo que incluye CPU, GPU y una *HPU (Holographic Processing Unit)* para el mapeo espacial.
- *Aplicaciones:* Fundamental en el mantenimiento remoto industrial, planificación de cirugías complejas y visualización de modelos arquitectónicos (BIM).
- *Costo:* Orientado al mercado corporativo, con un precio cercano a los *3,500 USD*.

#pagebreak()

= Referencias y Consultas
- Microsoft Learn: Documentación HoloLens 2.
- Apple Newsroom: Computación Espacial Vision Pro.
- Xataka: Análisis de hardware VR/MR.
- Bravent: Implementaciones de Realidad Mixta.
