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
  "Línea Curricular", // titulo_materia
  "EQUIPO", // titulo_practica
  "Periodo", // titulo_secuencia
  "Integrantes", // titulo_alumno
  "Secuencia", // titulo_profesorx
  "Profesora", // titulo_fecha
  "OPTATIVA III: Simuladores Virtuales", // carrera
  "Simulación de Ambientes Virtuales", // materia
  "Equipo 7", // practica
  "2027 - 1", // secuencia
  integrantes, // alumnos
  "7NM78", // profesorx
  "Bustamante Tranquilino Rocío", // fecha
  encabezado: ("Plan 2021", "Ingeniería en Informática"),
  proyecto: "Prototipo de simulador para el manejo de Residuos Peligrosos Biológico-Infecciosos en un consultorio, enfocado a la capacitación en bioseguridad mediante un entorno 3D interactivo",
)

#pagebreak()
#set par(justify: true, leading: 1.4em)
#set heading(numbering: "1.")
#set list(indent: 1.5em)

#title("Definición del Proyecto - Equipo 7")

#show link: set text(fill: blue)
#show link: it => underline(text(fill: blue, it))
Carpeta del equipo: #link("https://drive.google.com/drive/folders/1Xc2jnpCpinsPkV-dof3KsvBKfH1W1DBe")[*drive.google.com/drive/folders...*]

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

=== Acciones de simulación

El prototipo se organiza alrededor de cinco acciones, cada una recrea una tarea que se hace en el consultorio y termina con residuos que el usuario debe clasificar:

- *Clasificación por el tipo de desecho:* es la acción central del simulador. El usuario toma un residuo, lo identifica y lo deposita en el recipiente que le corresponde según la *NOM-087-ECOL-SSA1-2002*, el sistema le responde si acertó y por qué.

- *Esterilización de instrumentos de trabajo:* el usuario sigue la ruta del instrumental sucio hasta el área de esterilización, aquí distingue lo que se reutiliza después del proceso de lo que se desecha directamente, como los objetos punzocortantes de un solo uso.

- *Muestra de materiales:* el usuario revisa los materiales y consumibles del consultorio, con esto aprende a reconocer cuáles generan residuos peligrosos y cuáles terminan en la basura común antes de usarlos.

- *Exploración, curación o retiro de piezas dentales:* se representa un procedimiento breve para generar los residuos que le corresponden, por ejemplo, gasas con sangre, piezas dentales extraídas y material de curación. El procedimiento no se simula a detalle clínico, sirve para dar contexto al residuo.

- *Manejo de equipo radiográfico:* el usuario enciende el equipo de rayos X y lo ve prepararse hasta quedar listo para usarse en un paciente, la acción muestra la secuencia de encendido, la colocación del brazo y las barreras de protección que se ponen antes de la toma, estas barreras son las que después se convierten en residuo.

Cada acción funciona como un escenario independiente, así, el usuario puede repetir solo la que le interese y el sistema registra sus aciertos y errores por separado.

El proyecto se va a enfocar en clasificar y organizar los residuos dentro del consultorio, los procedimientos odontológicos se representan solo en la medida necesaria para generar los residuos de cada acción, no se busca reproducirlos con detalle clínico ni trabajar con pacientes reales, tampoco se incluye el transporte, tratamiento o disposición final de los residuos después de que salen del consultorio.

Así, la propuesta quiere servir como una herramienta que ayuda al usuario a aprender y practicar antes de enfrentar una situación real, lo hace en un entorno virtual donde cometer errores no implica ningún riesgo para la salud.

= Marco Teórico

= Desarrollo Prototipo Simulador

== Entorno urbano del consultorio

El consultorio se ubica en la planta baja de una torre de aproximadamente 21 metros, dentro de una avenida de 89 metros de longitud. Las dos aceras conectan el acceso con edificios vecinos de entre 16 y 32 metros, arbolado, luminarias y automóviles. Se conserva el paso peatonal frente a la clínica para orientar el recorrido desde el punto de inicio.

La ampliación permite recorrer una calle más extensa sin cambiar las cinco actividades del consultorio. Los pisos superiores y los edificios vecinos forman parte del escenario exterior y no incluyen interiores visitables.

=== Distribución interior por salas

La clínica se amplía hacia la parte posterior mediante un ala de 15 por 19.5 metros, conectada a la recepción y al vestíbulo original. Un pasillo central de 2.5 metros comunica cinco salas de práctica y una sala de descanso. Las puertas numeradas y el directorio del vestíbulo permiten reconocer el recorrido.

#table(
  columns: (auto, 1fr, 1.6fr),
  inset: 6pt,
  align: left,
  table.header([*Sala*], [*Actividad*], [*Equipamiento principal*]),
  [01], [Clasificación], [Carrito de curación y recipientes para clasificar los residuos.],
  [02], [Esterilización], [Mostrador, tarja, charola de instrumental sucio y autoclave.],
  [03], [Materiales], [Estante y consumibles para identificar su destino después del uso.],
  [04], [Procedimientos], [Unidad dental, maniquí, lámpara operatoria y mesa de instrumental.],
  [05], [Radiografía], [Sillón independiente, maniquí, equipo de rayos X y mesa para barreras.],
)

Cada actividad se activa al entrar en su sala. Los residuos desechables se clasifican en los recipientes locales; el instrumental reutilizable se lleva al área de esterilización. Se conservan las secuencias, la puntuación y el reinicio de las cinco prácticas. Los muebles y equipos reutilizan los modelos externos ya registrados; la distribución arquitectónica y los letreros son elaboración del equipo. Los árboles, vehículos y demás modelos exteriores se mantienen.

== Modelos 3D de terceros y sus fuentes

El prototipo incorpora modelos existentes para sustituir el mobiliario y gran parte de los objetos de práctica. Se conservan los componentes de interacción, los destinos de clasificación y la lógica de las actividades. Fecha de consulta y descarga de los recursos: *28 de septiembre de 2026*.

#table(
  columns: (1.2fr, 1.8fr),
  inset: 7pt,
  align: left,
  table.header([*Fuente y autor*], [*Modelos utilizados y adaptación*]),
  [#link("https://kenney.nl/assets/furniture-kit")[Furniture Kit] — Kenney],
  [Sillas, planta, monitor, lavabo, banca y bote municipal. Escala, orientación y materiales.],
  [#link("https://kenney.nl/assets/car-kit")[Car Kit] — Kenney],
  [Sedán para los automóviles de la calle. Ajuste de dimensiones y orientación.],
  [#link("https://kenney.nl/assets/nature-kit")[Nature Kit] — Kenney],
  [Roble para el arbolado urbano. Ajuste de altura.],
  [#link("https://kenney.nl/assets/city-kit-commercial")[City Kit Commercial] — Kenney],
  [Edificios comerciales building-a, building-b y building-c para los vecinos y la torre sobre la clínica. Adaptación de dimensiones y orientación; repetición de plantas intermedias en building-b y building-c para conservar las proporciones de ventanas y cornisas.],
  [#link("https://3dassets.dev/packs/dental-practice-and-surgery")[Dental Practice and Surgery] — 3D Assets],
  [Sillón reclinado, lámpara operatoria, brazo de rayos X, gabinetes, tarja, autoclave, mostrador, puerta, recipientes, charola, espejo dental, explorador y algodón. Separación de piezas, escala y adaptación de colores.],
  [#link("https://3dassets.dev/packs/field-medicine-and-recovery")[Field Medicine and Recovery] — 3D Assets],
  [Pila de gasas. Escala y color para representar su estado en cada práctica.],
  [#link("https://3dassets.dev/packs/tattoo-and-piercing-studio")[Tattoo and Piercing Studio] — 3D Assets],
  [Mesa rodante, sin los objetos decorativos originales; hoja extraída del rollo de barrera para barreras, fundas y láminas desechables.],
  [#link("https://3dassets.dev/assets/retail-store-fixtures-and-mall-mannequin-seated-05ab1b9a")[Seated mannequin] — 3D Assets],
  [Maniquí del paciente. Se retira la base de exhibición, se adapta la postura al sillón y se asignan materiales de piel y ropa.],
  [#link("https://3dassets.dev/assets/art-gallery-and-exhibition-rooms-gloves-and-tools-tray-029ffe6f")[Gloves and tools tray] — 3D Assets],
  [Guantes extraídos de la bandeja y recoloreados. El modelo original representa guantes de manipulación de arte; su adaptación clínica es esquemática.],
  [#link("https://3dassets.dev/assets/fast-food-and-drive-thru-drinks-cup-small-185a05b6")[Drinks cup, small] — 3D Assets],
  [Vaso desechable del paciente. Adaptación de escala.],
  [#link("https://poly.pizza/m/MURJ8NK4N9")[Syringe] — J-Toastie],
  [Jeringa y agujas separadas de su malla. Escala y orientación; la jeringa genérica es una aproximación visual, no un modelo exacto de carpule.],
  [#link("https://poly.pizza/m/66NBoNdhb03")[Tooth] — sugamo],
  [Pieza dental extraída. Escala y orientación.],
  [#link("https://poly.pizza/m/9yKgpOpblnf")[Scalpel] — Poly by Google],
  [Hoja de bisturí separada de la malla original y normalizada a la escala de la práctica.],
)

*Licencias.* Los paquetes de Kenney y los modelos de 3D Assets están publicados bajo #link("https://creativecommons.org/publicdomain/zero/1.0/")[CC0 1.0 Universal]. Syringe, Tooth y Scalpel se distribuyen bajo #link("https://creativecommons.org/licenses/by/3.0/")[Creative Commons Atribución 3.0]. Se conservan el nombre del autor, el enlace y las modificaciones en los créditos y en los archivos de procedencia del proyecto.

*Procedencia y elaboración.* El proveedor 3D Assets identifica sus recursos como generados mediante inteligencia artificial; los metadatos consultados indican Claude Opus 5. Son modelos publicados por terceros, no modelados originalmente por el equipo. Las adaptaciones OBJ se obtienen con `Tools/preparar_modelos.py`; los originales GLB y sus enlaces individuales se conservan en `Assets/Terceros`, junto con los archivos `FUENTE.md` y `procedencia.json`.

*Alcance de la sustitución.* El brazo importado acompaña la animación del equipo de rayos X. Los residuos importados permanecen dentro de sus objetos interactivos para tomarlos, clasificarlos y reiniciarlos. Se mantienen los colores de los recipientes del simulador. El cubrebocas, los fórceps de extracción, la lima endodóntica y la aguja de sutura aún utilizan representaciones provisionales; no se atribuyen a los paquetes anteriores. También se conservan la arquitectura, señalización didáctica, luces y controles propios. La biblioteca contiene otros modelos descargados para evaluación que todavía no se muestran en la escena.

= Conclusiones

= Glosario

#pagebreak()

= Anexos: avances del prototipo

Las siguientes capturas muestran el estado actual de la escena en `Unity`.

#figure(
  image("media/PHOTO-2026-09-22-13-26-53.jpg", width: 100%),
  caption: [Menú principal del simulador, con las opciones de inicio, ubicación, créditos y salida.],
)

#figure(
  image("media/PHOTO-2026-09-22-13-26-52 6.jpg", width: 100%),
  caption: [El mismo menú colocado dentro de la escena, sobre la imagen del consultorio.],
)

#figure(
  image("media/PHOTO-2026-09-22-13-26-51.jpg", width: 100%),
  caption: [Exterior del entorno urbano, con la lona institucional que ubica el acceso al consultorio.],
)

#figure(
  image("media/PHOTO-2026-09-22-13-26-52.jpg", width: 100%),
  caption: [Personaje del usuario frente al aviso de audio que acompaña el recorrido exterior.],
)

#pagebreak()

#figure(
  image("media/PHOTO-2026-09-22-13-26-52 2.jpg", width: 100%),
  caption: [Fachada del consultorio dental con la señalización informativa del área de tratamiento.],
)

#figure(
  image("media/PHOTO-2026-09-22-13-26-52 3.jpg", width: 100%),
  caption: [Acceso lateral del consultorio, con el lavabo y los carteles de bioseguridad.],
)

#figure(
  image("media/PHOTO-2026-09-22-13-26-52 5.jpg", width: 100%),
  caption: [Vista aérea de la escena: el consultorio y su ubicación dentro del entorno urbano.],
)

#pagebreak()

#figure(
  image("media/PHOTO-2026-09-22-13-26-52 4.jpg", width: 100%),
  caption: [Interior del consultorio, con el equipo suspendido, el mobiliario y las estaciones de trabajo.],
)

#figure(
  image("media/PHOTO-2026-09-22-13-26-53 2.jpg", width: 100%),
  caption: [Otra vista del interior, con las lámparas, los monitores y los contenedores de residuos.],
)

#figure(
  image("media/PHOTO-2026-09-22-13-26-53 3.jpg", width: 100%),
  caption: [Detalle del equipo y el instrumental colocados junto a la unidad dental.],
)

#figure(
  image("media/PHOTO-2026-09-22-13-26-53 4.jpg", width: 100%),
  caption: [Charola con instrumental, piezas dentales y material de curación, residuos que el usuario deberá clasificar.],
)

#pagebreak()

#figure(
  image("media/PHOTO-2026-09-22-13-29-32 3.jpg", width: 100%),
  caption: [
    Mensaje de instrucciones dentro del entorno: explica al usuario que debe
    clasificar los desechos entre la bolsa roja, el bote de punzocortantes y la
    basura común.
  ],
)

#figure(
  image("media/PHOTO-2026-09-22-13-29-32 2.jpg", width: 100%),
  caption: [
    Área de insumos y superficies de apoyo, con los frascos, el mobiliario y el
    bote donde se depositan los residuos.
  ],
)

#figure(
  image("media/PHOTO-2026-09-22-13-29-32.jpg", width: 100%),
  caption: [Mobiliario auxiliar de la sala, junto a los controles del equipo en la pared.],
)

= Apéndices
