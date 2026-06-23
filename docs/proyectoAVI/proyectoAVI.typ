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
  "Ambientes Virtuales Inmersivos",
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
Consultorio Seguro: Simulador de Manejo de RPBI en el Consultorio Dental.

El proyecto consiste en un entorno en 3D, desarrollado en Unity, que recrea un consultorio dental operativo y muestra, mediante señalización dentro de la escena, la correcta disposición de los Residuos Peligrosos Biológico-Infecciosos (RPBI) y los desechos comunes. La experiencia funciona como apoyo visual y educativo para reforzar prácticas de bioseguridad en el manejo de residuos dentro del consultorio.

= Objetivo general
Desarrollar un entorno de simulación interactiva en 3D utilizando el motor gráfico Unity, diseñado para capacitar y evaluar a estudiantes y profesionales de la odontología en la correcta clasificación, manejo y disposición de los Residuos Peligrosos Biológico-Infecciosos (RPBI) y los desechos comunes dentro de un consultorio dental operativo.

Con ello se busca promover el cumplimiento de las normativas de bioseguridad vigentes, en particular la NOM-087-ECOL-SSA1-2002, y reducir los riesgos de contaminación cruzada mediante un espacio de práctica seguro donde el usuario aprende sin exposición a residuos reales.

= Objetivos específicos
- *Modelado fiel del consultorio:* Diseñar y texturizar en 3D un consultorio dental operativo, con la unidad dental, el instrumental y las estaciones de residuos representativas de un entorno real.

- *Señalización de la disposición:* Colocar letreros en cada contenedor que indiquen, conforme a la NOM-087, qué tipo de residuo corresponde: RPBI rojo, RPBI amarillo, punzocortantes o desechos comunes.

- *Recorrido educativo:* Permitir recorrer y observar el consultorio y sus puntos de disposición correcta como referencia visual de bioseguridad.

- *Optimización y fidelidad visual:* Asegurar un rendimiento estable (FPS) y una representación realista que mantenga la escena fiel a un consultorio dental real.

= Justificación
El manejo incorrecto de los RPBI en la práctica odontológica representa un riesgo real de contaminación cruzada e infecciones para el personal, los pacientes y el medio ambiente. La NOM-087-ECOL-SSA1-2002 establece cómo clasificar, envasar y disponer estos residuos, pero su cumplimiento depende de una capacitación constante.

Un entorno 3D permite recorrer un consultorio realista y consultar, mediante su señalización, cómo se clasifica y dónde se dispone cada residuo sin exposición a material biológico-infeccioso real ni el costo de los insumos. Además, sirve como referencia visual previa a la práctica clínica y facilita que el usuario relacione cada tipo de residuo con el contenedor correspondiente.

Frente a la capacitación teórica tradicional, "Consultorio Seguro" sitúa al estudiante o profesional dentro del consultorio y le muestra, en su contexto real, dónde corresponde cada residuo. Así refuerza la cultura de bioseguridad y prepara para la operación real del consultorio.

= Alcance
El presente proyecto tiene como alcance el desarrollo de un entorno en 3D que represente fielmente un consultorio dental operativo, con sus objetos estáticos y la señalización que indica dónde debe disponerse cada tipo de residuo entre los contenedores de RPBI y de desechos comunes.

La primera versión se centra en la representación realista del consultorio y en la señalización de la disposición correcta conforme a la NOM-087. Las interacciones dinámicas, la evaluación automática y el reporte de desempeño se contemplan como funciones de ampliación del simulador. El proyecto no abarca procedimientos clínicos sobre pacientes ni el tratamiento final de los residuos fuera del consultorio.

Se plantea como una herramienta de apoyo visual para la capacitación, dirigida a escuelas de odontología, clínicas y consultorios dentales que deseen reforzar el cumplimiento de la bioseguridad de forma accesible y segura.

== Público objetivo
Estudiantes y profesionales de la odontología, así como escuelas, clínicas y consultorios dentales que busquen reforzar la capacitación de su personal en el manejo de RPBI y el cumplimiento de la bioseguridad mediante un entorno 3D de referencia.

== Tecnologías a utilizar
- Unity

== Alcance de funciones
Las funciones principales contempladas para el entorno de simulación son:

- *Navegación:* Desplazarse libremente por el consultorio dental, observar las estaciones de trabajo y acercarse a cada contenedor de residuos.

- *Retroalimentación normativa:* Mostrar retroalimentación inmediata que indique la clasificación correcta y su fundamento en la NOM-087.

== Funciones de simulación y ambientación
Para que el entorno se perciba como un consultorio dental realista, se consideran los siguientes elementos de ambientación:

- *Estaciones de residuos:* Contenedores y bolsas con el código de colores oficial, incluyendo rojo, amarillo, recipiente rígido para punzocortantes y bote de desechos comunes, etiquetados conforme a la norma.

- *Señalización y EPP:* Carteles de bioseguridad, símbolo universal de riesgo biológico y equipo de protección personal, como guantes y cubrebocas, para contextualizar la práctica.

- *Audio y avisos:* Sonidos de confirmación y alerta, junto con avisos en pantalla que refuercen cada clasificación correcta o incorrecta.

= Descripción del AVI

== Escenario: consultorio dental virtual
El escenario principal es un consultorio dental virtual capturado desde Unity. La escena integra el sillón operativo, las lámparas, monitores, superficies de trabajo, contenedores de residuos y mobiliario de apoyo distribuidos dentro del espacio.

#figure(
  image("media/consultorio-unity.jpg", width: 100%),
  caption: [Captura real del consultorio dental virtual usado como base del entorno de simulación.],
)

== Elementos del entorno
Los elementos principales del consultorio se organizan de la siguiente manera:

- *Sillón operativo:* Zona principal de trabajo dentro de la escena.

- *Módulo de instrumentos:* Controles, charola y brazo articulado junto al usuario.

- *Objetos operativos:* Lámparas, monitores y soportes de atención.

- *Botes de residuos:* Contenedores ubicados en el consultorio para la disposición de desechos.

- *Mobiliario operativo:* Gabinetes y superficies de apoyo del lado derecho.

- *Carro de insumos:* Área de material y apoyo complementario.

== Mapa del consultorio
El mapa del AVI traduce la vista del piso a zonas funcionales para que el usuario identifique qué puede operar, dónde debe depositar residuos y qué mobiliario funciona como soporte. La presentación usa tres categorías visuales:

- *Objetos operativos:* Sillón, lámparas, monitores, brazos articulados y equipo que participa directamente en la atención.

- *Botes de residuos:* Contenedores distribuidos en el consultorio para la disposición de desechos.

- *Mobiliario operativo:* Gabinetes, mesas, carros y superficies de soporte para insumos.

= Experiencia de aprendizaje
Consultorio Seguro propone un aprendizaje situado e inmersivo: el usuario construye conocimiento clasificando y disponiendo los residuos de la práctica dental en primera persona, en lugar de memorizar listas de forma pasiva.

La experiencia se apoya en cuatro principios:

- *Inmersión y memoria espacial:* Estar dentro del consultorio asocia cada tipo de residuo con el contenedor y el lugar donde se desecha, afianzando la retención.

- *Aprendizaje por descubrimiento:* Al clasificar cada residuo, el usuario identifica su categoría, color de contenedor y fundamento normativo a su propio ritmo.

- *Evaluación formativa:* El conteo de aciertos, el puntaje y la retroalimentación inmediata convierten cada error en una oportunidad de aprendizaje sin riesgo real.

- *Entorno seguro y accesible:* La práctica puede repetirse cuantas veces sea necesario sin exposición a material biológico-infeccioso ni costos de insumos.

= Link del AVI (video)
La presentación contempla una demostración en video del recorrido por el consultorio virtual, donde se mostrarán la navegación, la clasificación de residuos y la evaluación del usuario.

La diapositiva 14 incluye el enlace y el código QR para abrir la demostración en YouTube: #link("https://youtu.be/_EkJtI_x0YQ")[https://youtu.be/_EkJtI_x0YQ].

Como respaldo, el archivo del video también está disponible en Google Drive: #link("https://drive.google.com/file/d/1LpUhhuKyaJJVXR6mcvx2zW7g96GyZExz/view?usp=share_link")[Abrir video en Google Drive].

= Recursos de Realidad Aumentada
Como complemento del AVI, el proyecto incluye recursos de Realidad Aumentada para apoyar la práctica con marcadores, una aplicación Android y materiales visuales:

- *Marcadores:* Imágenes objetivo, fichas de residuos y contenedores para imprimir o mostrar en pantalla y disparar la RA. #link("https://drive.google.com/drive/folders/1nqiziG4qK4j3Tog_Zz_Rl81YPB6Bvx4U?usp=share_link")[Abrir marcadores].

- *APK de Android:* Aplicación instalable para ejecutar la experiencia de realidad aumentada. #link("https://drive.google.com/drive/folders/1Caovl9w3DpYd4OJhrxzAMj7uMnNNbfKc?usp=share_link")[Abrir APK].

- *Imágenes y recursos:* Capturas, texturas y materiales de apoyo de la RA. #link("https://drive.google.com/drive/folders/1AuYtH9qQYxTZmUDnP651eMoApvZJiv8D?usp=share_link")[Abrir recursos].

- *Carpeta completa:* #link("https://drive.google.com/drive/folders/1ryy2PMGYoQVi6LqJKbDiL4df30f2zy4U?usp=share_link")[Abrir carpeta completa de Realidad Aumentada].

#align(center)[
  #image("media/ra-folder-qr.svg", width: 4cm)
]

= Cierre
Consultorio Seguro demuestra cómo un entorno de simulación 3D puede apoyar la capacitación en el manejo de RPBI en el consultorio dental de manera segura, accesible y didáctica. El proyecto refuerza la bioseguridad, reduce riesgos de contaminación cruzada y ofrece una base para ampliar la experiencia con interacciones, evaluación y recursos de realidad aumentada.
