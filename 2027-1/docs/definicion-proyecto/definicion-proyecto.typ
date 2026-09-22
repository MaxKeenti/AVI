#import "portada-template.typ": portada

#show raw: set text(
  font: "JetBrainsMono NFM",
  weight: "medium",
  size: 1em,
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
  "Flores Roa Jorge Alejandro - 2022602815",
  "González Calzada Maximiliano - 2021601769",
  "Neri Mondragón Mónica - 2024600004",
  "Soto Soto Héctor - 2024601122",
)

#portada(
  "UNIDAD DE APRENDIZAJE", // titulo_carrera
  "LÍNEA CURRICULAR", // titulo_materia
  "EQUIPO", // titulo_practica
  "PERIODO", // titulo_secuencia
  "INTEGRANTES", // titulo_alumno
  "SECUENCIA", // titulo_profesorx
  "PROFESORA", // titulo_fecha
  "Ambientes Virtuales Inmersivos", // carrera
  "Simulación de Ambientes Virtuales", // materia
  "Equipo 7", // practica
  "2027 - 1", // secuencia
  integrantes, // alumnos
  "7NM78", // profesorx
  "Bustamante Tranquilino Rocío", // fecha
)

#pagebreak()
#set par(justify: true, leading: 1.4em)
#set heading(numbering: "1.")
#set list(indent: 1.5em)

#title("Definición del Proyecto - Equipo 7")

Carpeta del equipo: #link("https://drive.google.com/drive/folders/1Xc2jnpCpinsPkV-dof3KsvBKfH1W1DBe")[*Drive*]

#outline(title: "Índice")

#pagebreak()

= Nombre del prototipo simulador

Prototipo de simulador para el manejo de *Residuos Peligrosos Biológico-Infecciosos* en un consultorio, enfocado a la capacitación en bioseguridad mediante un entorno 3D interactivo.

= Planteamiento del problema

En cualquier consultorio se producen residuos de diferentes tipos mientras se atiende a los pacientes, algunos de estos residuos pueden ser peligrosos si no se separan y se tiran en el lugar correcto; por eso, las personas que estudian o trabajan en un consultorio necesitan saber cómo reconocerlos y en qué recipiente debe ir cada uno.

En México existe la *NOM-087-ECOL-SSA1-2002*, esta norma trata sobre el manejo de los Residuos Peligrosos Biológico-Infecciosos, el problema es que saber la norma en teoría no asegura que alguien pueda aplicarla cuando está en el consultorio. Muchas veces el aprendizaje se queda en lo teórico, se leen textos, se ven imágenes o se escuchan explicaciones, pero no hay oportunidad de practicar en un lugar parecido al real.

Tampoco sería buena idea hacer prácticas con residuos reales, esto implica riesgos para los estudiantes y el personal, se necesitarían materiales que no siempre están disponibles.

De ahí surge la idea de crear un simulador 3D de un consultorio con `Unity`. La idea es que el usuario pueda moverse por ese espacio virtual, así podrá encontrar distintos residuos y saber en qué contenedor debe tirarlos, de esta manera, practica de forma segura y sin tocar materiales reales.

= Marco referencial

== Manejo de Residuos Peligrosos Biológico-Infecciosos en un consultorio

Un consultorio genera diferentes tipos de residuos, algunos se pueden tirar con la basura común u otros necesitan un manejo especial por el riesgo que representan, los Residuos Peligrosos Biológico-Infecciosos deben separarse desde el momento en que se producen, en él se debe poner especial cuidado en todo lo que ha estado en contacto con sangre o fluidos, y también en los objetos punzocortantes que se usan durante la atención.

Si la separación se hace mal, residuos que necesitan un manejo especial terminan mezclados con la basura común, por eso, una parte clave de la bioseguridad es saber cuáles son los tipos de residuos y los recipientes que les corresponden.

== NOM-087-ECOL-SSA1-2002

Para este proyecto, la *NOM-087-ECOL-SSA1-2002* es una de las referencias principales, esta norma da las reglas para clasificar y manejar los Residuos Peligrosos Biológico-Infecciosos, en el simulador se van a mostrar los recipientes y colores que se usan para separar estos residuos, incluyendo los de punzocortantes y los de residuos comunes, así, el usuario podrá relacionar de forma más directa cada residuo con el lugar donde debe depositarlo.

== Uso de la simulación 3D en la educación

La simulación 3D permite crear espacios virtuales que copian situaciones o lugares reales, una de sus ventajas es que la persona puede interactuar con esos espacios sin correr los riesgos de una situación real.

En este caso, se va a usar la simulación para mostrar un consultorio, el usuario podrá caminar por el consultorio y ver elementos como mesa de operación, el instrumental, el mobiliario y las diferentes estaciones para dejar los residuos, la intención no es solo que memorice qué residuo va en cada recipiente, también se busca que lo vea dentro del espacio donde normalmente trabajaría.

== Especialista de referencia

Para hacer bien el proyecto, se va a tomar en cuenta el conocimiento de un profesional con experiencia en bioseguridad y manejo de Residuos Peligrosos Biológico-Infecciosos, su participación va a servir para comprobar que la distribución del consultorio y las situaciones que se muestran en el simulador se parezcan a un entorno odontológico real.

También se usará la *NOM-087-ECOL-SSA1-2002* como base para todo lo que tenga que ver con la clasificación y manejo de los residuos.

= Marco metodológico

== Objetivo general

Crear un simulador interactivo en 3D con `Unity`, este simulador permite a estudiantes y profesionales practicar cómo identificar, clasificar y manejar Residuos Peligrosos Biológico-Infecciosos y residuos comunes dentro de un consultorio virtual.

== Objetivos específicos

- Crear un consultorio virtual en 3D con los elementos más comunes de un consultorio real.

- Incluir distintos recipientes para mostrar la separación de Residuos Peligrosos Biológico-Infecciosos, punzocortantes y residuos comunes.

- Poner letreros que indiquen al usuario qué tipo de residuo va en cada contenedor.

- Permitir que el usuario recorra el consultorio y se acerque a los puntos donde se generan o depositan residuos.

- Agregar mensajes que digan si la clasificación fue correcta o incorrecta y expliquen brevemente la razón.

- Incluir un sistema simple de evaluación con aciertos, errores y puntuación, para que el usuario pueda medir su desempeño.

== Justificación

El manejo adecuado de los residuos es clave para la seguridad en un consultorio, si no se separan bien, aumentan los riesgos para el personal, los pacientes y también para el medio ambiente, por eso nos pareció importante buscar nuevas formas de apoyar la capacitación en este tema. Una de ellas es usar la tecnología para crear un espacio donde se pueda practicar sin usar residuos reales.

El simulador va a mostrar un consultorio muy parecido a uno real, el usuario podrá moverse por el lugar, ver los recipientes y practicar la clasificación, si se equivoca, recibirá una explicación y podrá intentarlo otra vez.

Otra ventaja es que la práctica se puede repetir tantas veces como sea necesario, sin gastar materiales ni generar basura, esto hace que el simulador sirva como complemento de las clases teóricas y como preparación antes de entrar a un consultorio real.

Además, el proyecto quiere mostrar que herramientas como los entornos 3D y la *Realidad Aumentada* pueden usarse en áreas diferentes al entretenimiento, en este caso, sirven como apoyo para enseñar bioseguridad en consultorios.

== Hipótesis

Si se crea un simulador 3D de un consultorio que ayude a identificar y clasificar distintos tipos de residuos según la *NOM-087-ECOL-SSA1-2002*, los usuarios podrán entender y acostumbrarse mejor a la forma correcta de manejar los Residuos Peligrosos Biológico-Infecciosos y los residuos comunes, esto se logra al practicar en un entorno seguro y sin riesgos.

== Alcance

=== Público

El proyecto está pensado sobre todo para estudiantes y personas que trabajan en consultorios o clínicas, también puede usarse como material de apoyo en escuelas del área de la salud para complementar las clases de bioseguridad y manejo de residuos.

=== Tecnología

El simulador se va a hacer sobre todo con `Unity`, esta herramienta permite crear el consultorio y programar las interacciones, también se van a usar recursos de modelado y texturizado 3D para que se parezca más a un consultorio real. Además, se piensa usar Realidad Aumentada por medio de una aplicación para dispositivos `Android`.

=== Funcionalidad

Las funciones principales del prototipo serán:

- Caminar libremente por el consultorio.

- Mirar los muebles y los objetos que hay alrededor.

- Identificar los diferentes tipos de residuos.

- Identificar los recipientes que corresponden a cada tipo de residuo.

- Buscar información usando señalizaciones.

- Recibir mensajes cuando la clasificación sea correcta o incorrecta.

- Obtener una puntuación según las decisiones que se tomen.

- Usar sonidos y avisos para que la interacción sea más clara.

- Usar recursos de Realidad Aumentada como complemento del simulador.

El proyecto se va a enfocar en clasificar y organizar los residuos dentro del consultorio, no busca simular procedimientos odontológicos completos ni trabajar con pacientes reales, tampoco incluye el transporte, tratamiento o disposición final de los residuos después de que salen del consultorio.

Así, la propuesta quiere servir como una herramienta que ayuda al usuario a aprender y practicar antes de enfrentar una situación real, lo hace en un entorno virtual donde cometer errores no implica ningún riesgo para la salud.
