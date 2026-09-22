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

#title("Reto I.2 El cuerpo como Interfaz y los Avatares con IA")

= ¿Qué papel juega la corporalidad  como interfáz en un ambiente virtual inmersivo?
En un ambiente virtual inmersivo, el cuerpo no desaparece ni es sustituido, sino que asume el rol de una *interfaz expandida a través de su representación digital, comúnmente conocida como avatar*. El cuerpo se convierte en una multiplicidad y en una "superficie blanda" o de contacto que permite habitar el ciberespacio y facilitar interacciones sociales, comunicativas y afectivas con otros.

Basado en las fuentes, la corporalidad actúa como interfaz principalmente en tres dimensiones:

== Como interfaz de la percepción exteriorizada:
El cuerpo virtualizado permite materializar y exteriorizar los cambios de perspectiva y puntos de vista que antes solo ocurrían en la imaginación. A través del software, el usuario puede alternar instantáneamente su visión espacial (pasar de primera a tercera persona, de vista frontal a trasera o aérea), utilizando su cuerpo digital como el ancla desde la cual percibe e interactúa con el entorno.

== Como laboratorio de experimentación de identidades (cuerpo cultural):
En los mundos virtuales, la imagen del cuerpo es completamente maleable. Los avatares actúan como una extensión del usuario que refleja su autoimagen, íconos, aspiraciones o nostalgia. La corporalidad se vuelve un lienzo para realizar operaciones visuales como la proyección, inversión, camuflaje o hibridación, permitiendo al usuario configurar nuevas formas de subjetividad e identidad basadas en sus deseos y elecciones.

== Como proveedor de potenciales kinestésicos y gestuales (cuerpo tecnológico):
El cuerpo físico provee esquemas de movimiento que pueden ser capturados (mediante periféricos como el Kinect o trajes de captura) y transcodificados para desplazar y animar al avatar en el metaverso. Incluso cuando no se captura el movimiento en tiempo real, se utilizan archivos de animación (como los formatos BVH) que dotan al avatar de una "sensación de vida", lo que incrementa profundamente la interactividad y el nivel de inmersión del usuario.

=== La dialéctica de encarnación y desencarnación:
Esta interfaz corporal genera un juego constante entre encarnación y desencarnación. Por un lado, el sujeto "insufla" vida al avatar y se proyecta en él (encarnación) para actuar en el mundo virtual. Por otro lado, puede tomar distancia de esa misma representación (desencarnación), alternando dinámicamente entre el mundo actual en el que reside físicamente y el mundo virtual inmersivo.

== Conclusión:
En conclusión, considero que el papel del cuerpo como interfaz es ser el *vehículo performativo que conecta al usuario con la realidad mixta*. Lejos de aislar a la persona de su realidad biológica, estas tecnologías convierten al cuerpo en un "hipercuerpo" interconectado que estimula su poder para explorar, jugar, aprender y abrir nuevos mundos posibles.

= ¿Considera que los Avatares con IA aportan un nivel de interacción  en los ambientes virtuales inmersivos?
Sí, por lo que leí, los avatares con Inteligencia Artificial (frecuentemente denominados `bots` o `NPCs` por sus siglas en inglés para `Non-Player Characters`) aportan un nivel de interacción fundamental y enriquecedor en los ambientes virtuales inmersivos.

De acuerdo con las lecturas, el aporte de la IA en estos avatares se manifiesta en las siguientes dimensiones:

- *Mayor dinamismo y credibilidad:* La introducción de *bots* permite que el software interactúe con los operadores humanos de manera semejante a como lo haría una persona real. Esto desarrolla una mayor interacción con los usuarios y proporciona grandes ventajas sobre aquellos mundos virtuales que solo poseen objetos inanimados, otorgando a los espacios virtuales *mayor dinamismo y credibilidad*.
- *Roles interactivos específicos:* Los avatares controlados por IA pueden asumir distintos grados de complejidad para interactuar socialmente en el entorno. Las fuentes destacan categorías como los NPCs con *IA parcial* (quienes toman el rol de recepcionistas, guías de turismo o camareros) y los NPCs con *IA programada* o `chatbots` (quienes pueden fungir como profesores de lenguaje, maestros, presentadores o actores).
- *Comportamiento cognitivo y toma de decisiones:* En la evolución de la animación gráfica, el nivel más avanzado es el "aspecto cognitivo", el cual es resultado directo de la aplicación de técnicas de Inteligencia Artificial como el razonamiento, la representación de conocimiento y la planificación. Al emplear este aspecto, se logra simular *"Vida Artificial"*, obteniendo avatares que poseen la capacidad de tomar decisiones y reaccionar de forma autónoma a los estímulos del ambiente.
- *Diseño de experiencias multimodales:* La presencia de distintas formas de inteligencia artificial interactuando simultáneamente con los avatares de los usuarios humanos se considera un elemento central del diseño multimodal. Esto enriquece la arquitectura de la experiencia, haciendo que el usuario se sienta inmerso en un mundo habitado y reactivo.

== Conclusión:
En conclusión, pienso que la integración de IA en los avatares transforma profundamente la interacción, permitiendo que la máquina no solo sea un escenario pasivo, sino un *agente activo que enriquece la simulación, la comunicación y el aprendizaje* dentro del metaverso.
