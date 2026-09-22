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
    ts: (name: "TypeScript", icon: "", color: rgb("#FFB81C")),
    typ: (name: "Typst", icon: "", color: rgb("#FFB81C")),
  ),
  number-format: n => str(n),
)

#set text(
  font: "ITC Avant Garde Gothic",
  lang: "es",
  weight: "semibold",
  fill: rgb("#2A2A2A"),
)

#show heading: set text(font: "Fraunces 72pt", weight: "black", fill: rgb("#E67D22"))

#set page(
  paper: "us-letter",
  margin: (left: 3cm, top: 2.5cm, right: 2.5cm, bottom: 2.5cm),
  numbering: "1",
  fill: rgb("#F7F6F1"),
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
  fill: (x, y) => if y == 0 { rgb("#E67D22") } else { rgb("#F7F6F1") },
  text(weight: "bold", fill: rgb("#2A2A2A"))[Característica],
  text(weight: "bold", fill: rgb("#2A2A2A"))[RV],
  text(weight: "bold", fill: rgb("#2A2A2A"))[RA],
  text(weight: "bold", fill: rgb("#2A2A2A"))[RM],
  text(weight: "bold", fill: rgb("#2A2A2A"))[Set Virtual],
  text(weight: "bold", fill: rgb("#2A2A2A"))[Actor Virtual],
  text(weight: "bold", fill: rgb("#2A2A2A"))[Escenario Virtual],
  text(weight: "bold", fill: rgb("#2A2A2A"))[Personaje Virtual],

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

== Explicaciones de la tabla comparativa

=== Tiempo Real

La capacidad de procesamiento en *tiempo real* es fundamental en los ambientes virtuales e inmersivos. RV, RA, RM y Set Virtual requieren actualizaciones instantáneas para mantener la coherencia visual y la respuesta de usuario. Sin embargo, los "Actor Virtual" y "Personaje Virtual" pueden estar precargados o no necesitar actualización en tiempo real si funcionan como elementos estáticos o con animaciones predefinidas. En el contexto de sistemas de captura de movimiento o renderizado en vivo, todos los componentes deben sincronizarse en tiempo real, pero el personaje virtual frecuentemente puede reproducir secuencias grabadas, justificando la "X" en las columnas Escenario Virtual y Personaje Virtual.

=== 3D

La representación tridimensional es esencial en prácticamente todas las plataformas excepto en soluciones muy básicas. RV, RA, RM y Set Virtual requieren coordenadas 3D para la composición espacial. Actor Virtual y Personaje Virtual también utilizan modelos 3D para representación realista. La "✓" generalizada en esta fila refleja que el 3D es el estándar de la industria moderna. Técnicamente, sin 3D no habría inmersión ni interacción espacial significativa.

=== Interactividad

La interacción del usuario es crítica en RV, RA, RM y Set Virtual, donde los usuarios pueden manipular objetos, navegar espacios y afectar el entorno. Sin embargo, "Actor Virtual" y "Personaje Virtual" típicamente ejecutan comportamientos predeterminados o animaciones sin responder a inputs del usuario en tiempo real. En contextos de captura de movimiento, el personaje sigue al actor, pero no hay interacción bidireccional. La presencia de "X" en Escenario Virtual refleja que algunos escenarios pueden ser no-interactivos (visualizaciones pasivas).

=== Inmersión

La inmersión es el corazón de la experiencia VR, RA y RM cuando se implementan correctamente. RV proporciona inmersión total; RA e inmersión parcial (mantienes conexión con el mundo real); RM combina ambas. Set Virtual ofrece inmersión mediante la composición de elementos virtuales. Sin embargo, "Actor Virtual" y "Personaje Virtual" son componentes de la escena, no contenedores de la experiencia, por lo que no generan inmersión por sí solos—son herramientas dentro de un sistema más grande. La "X" en Escenario Virtual indica que un escenario sin interfaz de usuario adecuada puede ser poco inmersivo.

=== Cámara

La gestión de cámara es crítica en RA, RM y Set Virtual para sincronizar la perspectiva virtual con la cámara física o digital. RV no tiene cámara tradicional sino perspectiva first-person. "Actor Virtual" requiere cámara para capturar su movimiento en motion capture. "Personaje Virtual" en Set Virtual puede estar frente a cámara (justifica la "✓"), pero en entornos de realidad aumentada pura, el personaje no está necesariamente capturado por cámara ("X" en Escenario Virtual).

=== Holografía

La holografía es una tecnología especializada de visualización tridimensional sin gafas. Solo ciertos sistemas avanzados de RM implementan verdadera holografía. RV y RA convencionales no usan holografía. "Personaje Virtual" podría representarse holográficamente en futuras aplicaciones (de ahí la "✓"), pero es más conceptual que práctico actualmente. La mayoría de soluciones en educación y entretenimiento no implementan holografía por costo y complejidad.

=== Dispositivos RV

Los dispositivos RV (headsets como Meta Quest, HTC Vive) son requisitos explícitos para RV pura. RA también requiere dispositivos específicos (como HoloLens o smartphones). RM necesita hardware especializado. Set Virtual depende del hardware de renderizado disponible en estudios. Sin embargo, "Escenario Virtual" y "Personaje Virtual" son conceptos independientes del hardware—pueden implementarse en cualquier plataforma. La "—" indica no aplicabilidad: un escenario o personaje virtual existe conceptualmente sin requerir dispositivos RV específicos.

=== Engine

Un motor gráfico (como Unity o Unreal) es esencial para desarrollar y renderizar RV, RA, RM y Set Virtual. "Actor Virtual" requiere un engine para la captura de movimiento y retargeteo a modelos 3D. Sin embargo, "Escenario Virtual" y "Personaje Virtual" pueden conceptualizarse sin engine (como descripciones o arte conceptual), de ahí las "X". En la práctica educativa, si se implementan, necesitarían engine, pero como conceptos abstractos no lo requieren.

=== Sensores

Los sensores de movimiento y posición son fundamentales en RV (controllers, trackers) y RM (rastreo ambiental y de mano). RA también usa sensores pero de forma más limitada. Set Virtual requiere sensores para captura de movimiento. "Actor Virtual", "Escenario Virtual" y "Personaje Virtual" como conceptos no requieren sensores directamente (la "—" indica no aplicabilidad conceptual), aunque la implementación práctica de captura de movimiento sí los necesitaría. Las "—" reflejan que estos elementos son de representación, no de entrada de usuario.
