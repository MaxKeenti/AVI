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
  "Ambientes Virtuales Inmersivos", // carrera
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

#show link: set text(fill: rgb("#336B75"))
#show link: it => underline(text(fill: rgb("#336B75"), it))
Carpeta del equipo: #link("https://drive.google.com/drive/folders/1Xc2jnpCpinsPkV-dof3KsvBKfH1W1DBe")[*drive.google.com/drive/folders...*]

#block[
  #set text(size: 9pt)
  #set par(leading: .65em)
  #outline(title: "Índice", depth: 2)
]

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

La construcción se realiza con `Unity`, con recursos de modelado y texturizado 3D para representar el consultorio y sus interacciones. Esta entrega tiene como objetivo el escritorio con teclado y ratón. La realidad aumentada para dispositivos `Android` se plantea como una ampliación; no forma parte de la implementación comprobada aquí.

=== Funcionalidad

Las funciones principales del prototipo serán:

- Caminar libremente por el consultorio.

- Mirar los muebles y los objetos que hay alrededor.

- Identificar los diferentes tipos de residuos.

- Identificar los recipientes que corresponden a cada tipo de residuo.

- Buscar información usando señalizaciones.

- Recibir mensajes cuando la clasificación sea correcta o incorrecta.

- Obtener una puntuación según las decisiones que se tomen.

- Usar avisos en español para que la interacción sea clara.

- Considerar la Realidad Aumentada como ampliación futura del simulador de escritorio.

=== Acciones de simulación

El prototipo se organiza alrededor de cinco acciones, cada una recrea una tarea que se hace en el consultorio y termina con residuos que el usuario debe clasificar:

- *Clasificación por el tipo de desecho:* es la acción central del simulador. El usuario toma un residuo, lo identifica y lo deposita en el recipiente que le corresponde según la *NOM-087-ECOL-SSA1-2002*, el sistema le responde si acertó y por qué.

- *Esterilización de instrumentos de trabajo:* el usuario sigue la ruta del instrumental sucio hasta el área de esterilización, aquí distingue lo que se reutiliza después del proceso de lo que se desecha directamente, como los objetos punzocortantes de un solo uso.

- *Muestra de materiales:* el usuario revisa los materiales y consumibles del consultorio, con esto aprende a reconocer cuáles generan residuos peligrosos y cuáles terminan en la basura común antes de usarlos.

- *Exploración, curación o retiro de piezas dentales:* se representa un procedimiento breve para generar los residuos que le corresponden, por ejemplo, gasas con sangre, piezas dentales extraídas y material de curación. El procedimiento no se simula a detalle clínico, sirve para dar contexto al residuo.

- *Manejo de equipo radiográfico:* la práctica representa una secuencia de radiografía digital: preparar sensor y barrera, representar la toma y retirar barreras. El usuario activa estaciones con mensajes y un tiempo abreviado; el brazo del equipo permanece estático y no se genera radiación. Los residuos del caso son barrera y guantes sin sangre visible, sin químicos de revelado.

Cada acción funciona como un escenario independiente, así, el usuario puede repetir solo la que le interese y el sistema registra sus aciertos y errores por separado.

El proyecto se va a enfocar en clasificar y organizar los residuos dentro del consultorio, los procedimientos odontológicos se representan solo en la medida necesaria para generar los residuos de cada acción, no se busca reproducirlos con detalle clínico ni trabajar con pacientes reales, tampoco se incluye el transporte, tratamiento o disposición final de los residuos después de que salen del consultorio.

Así, la propuesta quiere servir como una herramienta que ayuda al usuario a aprender y practicar antes de enfrentar una situación real, lo hace en un entorno virtual donde cometer errores no implica ningún riesgo para la salud.

= Fundamento del contenido educativo

El ejercicio aplica casos concretos y no equipara todo material usado con un RPBI. La referencia es la NOM-087-SEMARNAT-SSA1-2002, publicada con la denominación ECOL; el catálogo oficial consultado la identifica como vigente @economia2003rpbi.

- Gasas y algodón saturados de sangre: residuos no anatómicos, bolsa roja.

- Agujas y hojas de bisturí usadas: recipiente rígido rojo para punzocortantes.

- Pieza extraída sin conservador ni amalgama en el caso simulado: patológico sólido, bolsa amarilla. Esta aplicación del apartado 4.3.1 requiere revisión del especialista del proyecto.

- Envolturas limpias y barreras con saliva sin sangre: destino común solamente bajo las condiciones descritas por cada caso; se excluyen los supuestos infecciosos especiales y los riesgos químicos.

La separación y el envasado se basan en los apartados 4 y 6.2 y la tabla 2 de la norma @semarnat2003clasificacion.

El instrumental reutilizable sigue una ruta diferente: limpieza, secado, inspección, empaque y esterilización validada de acuerdo con el fabricante. La NOM-013-SSA2-2015 sustenta esta secuencia; llevar un objeto a una estación virtual no representa un ciclo real ni certifica que esté estéril @salud2016bucales.

= Desarrollo de Consultorio Seguro

== Diseño y alcance

Consultorio Seguro se desarrolla en Unity como una clínica odontológica interactiva con cinco salas de práctica. La arquitectura, el equipamiento y las actividades forman una escena editable; los recursos externos cuentan con documentación de procedencia, licencia y adaptaciones.

El diseño visual plantea un interior contemporáneo con proporciones reales, mobiliario utilizable, materiales físicamente basados y luz natural combinada con iluminación clínica. La recepción, la sala de procedimientos y las demás áreas comparten una paleta y criterios de acabado. El estado comprobado de la implementación se registra en la sección de verificación.

La recepción cuenta con un mostrador de planta redondeada, carpintería de archivo, listones de roble a escala y un fondo claro con emblema dental propio. La espera utiliza tapicería cálida, directorio compacto y motivos botánicos originales; el pasillo dispone de protección mural continua. La distribución mantiene libres las rutas de pacientes y personal.

En procedimientos y radiografía, los sillones dentales cuentan con cojines contorneados, costuras, grano fino de vinilo, fuelle y pedal. Las salas disponen de taburetes regulables y, en procedimientos, de una unidad de agua e instrumental con mangueras y escupidera. Estos complementos son geometría propia; las adaptaciones de cada modelo externo se describen en el catálogo y los créditos.

== Programa arquitectónico

La clínica se sitúa en la planta baja de un edificio alto, con acceso identificable desde la banqueta. El entorno incluye edificios vecinos, calle y mobiliario urbano. Los pisos superiores son contexto exterior; la actividad educativa se desarrolla en la clínica.

La distribución consta de una recepción con mostrador reconocible desde la entrada, espacio de atención al paciente y espacio de trabajo del personal. Una sala de espera con sillones, mesas auxiliares, plantas e información para pacientes deja libre la ruta hacia el pasillo central. Las cinco salas de práctica se organizan a ambos lados del pasillo, junto con una sala de personal y almacenamiento.

#block(breakable: false)[
#table(
  columns: (auto, 1fr, 1.6fr),
  inset: 7pt,
  align: left,
  table.header([*Sala*], [*Actividad*], [*Organización funcional*]),
  [01], [Clasificación], [Charola de residuos y destinos de separación accesibles.],
  [02], [Esterilización], [Recepción de instrumental usado, limpieza y preparación para reprocesamiento.],
  [03], [Materiales], [Almacenamiento ordenado y reconocimiento de consumibles.],
  [04], [Procedimientos], [Unidad dental, iluminación operatoria y charola al alcance del usuario.],
  [05], [Radiografía], [Equipo y barreras de protección; clasificación de desechables del caso simulado.],
)
]

Los nombres y números se colocan a altura de lectura y se orientan para guiar en ambos sentidos. Las instrucciones y el manual de cada práctica deben permanecer visibles desde el espacio de uso, sin atravesar muebles para consultarlos.

== Materiales, iluminación y modelos

La paleta emplea blancos cálidos, verde salvia, madera clara y tapicería neutra. Los acabados distinguen porcelanato satinado, yeso pintado, madera, acero, cerámica, vidrio y plásticos médicos. Las texturas externas de madera y muro incluyen color, normal y rugosidad; su fuente está en el catálogo. La iluminación y las reflexiones deben evaluarse desde la altura de los ojos, especialmente al cruzar del exterior a la recepción y de esta a las salas.

Los modelos de espera y vegetación proceden de Poly Haven: Modern Arm Chair 01 de Vibrant Nordic y Potted Plant 02 de Rico Cilliers. Las texturas Oak Veneer 01 y Beige Wall 001 proporcionan variación superficial. El equipamiento odontológico y los objetos de la biblioteca se documentan con sus fuentes y adaptaciones individuales.

Una licencia válida no garantiza fidelidad clínica. Parte del equipo de 3D Assets tiene geometría simplificada y el proveedor declara generación mediante inteligencia artificial. El modelo de jeringa es genérico. La biblioteca incluye un maniquí de exhibición para evaluación que no forma parte de la escena. Estas limitaciones se mantienen explícitas; el objetivo de representación arquitectónica no constituye una afirmación de fotorrealismo ya verificado.

== Interacción y accesibilidad

El recorrido es en primera persona. Los controles usan teclado y ratón, con avisos de interacción en español. El manual está disponible desde el inicio, la pausa y cada sala, con regreso claro al contexto de origen. La puntuación ofrece retroalimentación sobre la decisión, y cada práctica puede repetirse.

La asistencia visual de instrumentos utiliza un contorno blanco discreto y un contorno dorado al apuntar. Se limita a objetos cercanos de la sala actual, desaparece durante la pausa y al sostener o completar el objeto y no codifica el recipiente correcto. Su comportamiento y legibilidad se comprueban en Unity, no por la mera presencia del código.

Los controles definidos son WASD o flechas para caminar, ratón para mirar, Mayús izquierda para aumentar la velocidad y E para interactuar a menos de 2,6 m. R devuelve el objeto sostenido. M abre el manual y Esc gestiona pausa y regreso. Cada acierto suma 10 puntos y cada error resta 5, con un mínimo de cero; el reinicio de la práctica está en el menú de pausa.

La distribución definida en el constructor contiene 13 objetos y 10 pasos previos:

#table(
  columns: (2fr, 1fr, 1fr),
  inset: 6pt,
  table.header([*Práctica*], [*Pasos*], [*Objetos*]),
  [Clasificación], [0], [3],
  [Esterilización], [3], [2],
  [Materiales], [1], [3],
  [Procedimientos], [3], [3],
  [Radiografía digital], [3], [2],
)

Estos recuentos describen la escena; la aprobación de sus recorridos e interacciones se registra por separado.

== Escena editable y vistas

La escena principal se guarda en `Assets/ConsultorioSeguro/Escenas/ConsultorioSeguro.unity`. La jerarquía organiza arquitectura, techos y torre, equipamiento, entorno urbano, iluminación, interacción y vistas. Los objetos se inspeccionan y editan antes de iniciar Play.

El menú *Consultorio Seguro → Vistas* ofrece exterior, recepción, espera, pasillo, las cinco salas, personal y corte superior. El corte oculta techos y torre únicamente para la inspección del editor. La herramienta de captura utiliza cámaras de Unity a 1,65 m para los interiores; la cámara exterior también se sitúa a 1,65 m y dirige la mirada hacia la torre; únicamente el corte utiliza una posición elevada. Los menús y el comportamiento durante el recorrido requieren una comprobación separada en ejecución.


== Registro exhaustivo de recursos externos

El siguiente catálogo registra 51 recursos originales en 18 carpetas, de `Assets/Terceros`, incluidos archivos de evaluación que pueden no aparecer en la escena. Las dependencias de software y los recursos esenciales de TextMesh Pro se describen a continuación y no forman parte de ese recuento. Cada entrada incluye fuente, autor, licencia, archivos y modificaciones. Se conservan los nombres originales para identificar las páginas de descarga. La fecha de consulta corresponde al registro de procedencia de cada recurso.

La biblioteca registra originales y derivados, mientras que los créditos de la aplicación y `CREDITOS.md` reproducen estas atribuciones. Los recursos CC0 permiten adaptación; los tres recursos CC BY 3.0 mantienen autor, fuente, licencia y declaración de cambios. Noto Sans Regular se conserva bajo SIL Open Font License 1.1 con su aviso de derechos y licencia íntegra. Los metadatos de 3D Assets especifican diferentes modelos de IA: no se atribuye un único modelo generador a toda la biblioteca.

#block[
  #include "media/catalogo-recursos.typ"
]

== Dependencias y recursos esenciales de Unity

El proyecto declara Unity 6000.4.6f1, Universal Render Pipeline 17.4.0, Input System 1.19.0, uGUI 2.0.0, Unity glTFast 6.20.0 y Unity Test Framework 1.6.0. Los paquetes se conservan sin modificaciones; sus versiones están en `Packages/manifest.json` y sus dependencias transitivas en `Packages/packages-lock.json`.

*TextMesh Pro / uGUI 2.0.0.* Autor y distribuidor: Unity Technologies ApS. Fuente: #link("https://docs.unity3d.com/Packages/com.unity.ugui@2.0/manual/index.html")[documentación oficial de uGUI]. Licencia: #link("https://unity.com/legal/licenses/unity-companion-license")[Unity Companion License]. Se importaron 33 recursos esenciales de shaders, ajustes, estilos y fuentes de respaldo del archivo `TMP Essential Resources.unitypackage` del paquete instalado. Los 33 coinciden con el original mediante SHA-256; no se modificaron. El aviso de licencia se conserva en `Assets/TextMesh Pro/LICENSE-Unity.md`.

*Liberation Sans.* Recurso de respaldo incluido en el paquete de Unity. Avisos de autoría: datos digitalizados © 2010 Google Corporation; © 2012 Red Hat, Inc. Fuente: #link("https://github.com/liberationfonts/liberation-fonts")[Liberation Fonts]. Licencia: #link("https://github.com/liberationfonts/liberation-fonts/blob/main/LICENSE")[SIL Open Font License 1.1]. La fuente TTF, sus atlas SDF y materiales se importaron sin cambios; la licencia íntegra acompaña al archivo. Los rótulos de la clínica usan Noto Sans y los menús usan la fuente integrada `LegacyRuntime.ttf` de Unity.

*Unity glTFast 6.20.0.* Autor: Unity Technologies y autores de Unity glTFast. Fuente: #link("https://docs.unity3d.com/Packages/com.unity.cloud.gltfast@6.20/manual/index.html")[documentación oficial]. Licencia: #link("https://www.apache.org/licenses/LICENSE-2.0")[Apache License 2.0]. Paquete de importación conservado sin cambios.

`Documentacion/DEPENDENCIAS.md` y `procedencia-dependencias.json` del proyecto enumeran la procedencia de los recursos distribuidos con Unity, sus archivos y comprobaciones. El catálogo de recursos externos contiene la atribución individual de Noto Sans y de los modelos y texturas.

#pagebreak()

= Verificación y evidencias

*Verificación al 5 de octubre de 2026:* Unity 6000.4.6f1 abre y compila la escena; las siete pruebas de PlayMode terminan aprobadas, sin fallos. La escena contiene 10 pasos y 13 objetos entre las cinco prácticas. El informe XML acompaña al proyecto; la ejecución documentada termina a las 22:08:56 de Ciudad de México (6 de octubre, 04:08:56 UTC).

La prueba de integración desplaza el CharacterController real desde la banqueta hasta recepción, recorre el frente y la parte posterior del registro, la espera, el pasillo, las cinco salas y el área de personal, y vuelve a salir. La interacción se comprueba mediante rayos desde la cámara a altura de ojos y los colliders de la escena. Se verifican manuales, pasos, recogida, consulta de recipientes, depósito, puntuación y regreso a los menús. Pruebas adicionales cubren errores, restitución de objetos, orden, reinicio, atajo M y restricciones del contorno.

Se inspeccionaron 18 imágenes capturadas en Unity: 12 vistas arquitectónicas y seis de ejecución. Las capturas de ejecución utilizan temporalmente el Canvas en modo ScreenSpaceCamera para que Camera.Render incluya la interfaz real. Los archivos de origen y sus sumas SHA-256 permiten identificar cada imagen.

El proyecto se abrió desde Unity Hub y se inspeccionaron recepción, procedimientos y radiografía mediante las vistas del editor, con la escena guardada y visible sin entrar en Play. Mediante clics y teclado en la interfaz del editor se verificaron inicio, manual, desplazamiento de lectura, créditos, Volver, pausa, Reanudar y atajo M. No se completó una caminata manual con teclado y ratón por todas las salas; el recorrido íntegro documentado es automático. Tampoco se evaluaron rendimiento en otros equipos, accesibilidad con dispositivos alternativos ni todos los ángulos de oclusión del contorno.

La iluminación combina luz directa, rellenos y ocho sondas de reflexión guardadas; no se horneó iluminación global. El renderizado Forward+ usa sombras suaves y mezcla de sondas con proyección de caja. Los vidrios transparentes no proyectan sombras opacas. Parte del equipo dental presenta geometría simplificada. Las imágenes muestran el nivel visual obtenido, sin afirmar que se haya alcanzado fotorrealismo.

Los registros `Capturas/REVISION.md`, `ENTREGA.md` y `Verificacion/pruebas-playmode.xml` detallan la evidencia. Las cuatro figuras siguientes son copias íntegras de las capturas; `media/origen-capturas-unity.json` conserva sus sumas.

#include "media/evidencias-unity.typ"

= Límites del simulador

La aplicación representa decisiones de separación y manejo inicial; no reproduce una extracción, una esterilización validada o un estudio radiográfico real. Quedan fuera de alcance la certificación de instalaciones, dosimetría, transporte y disposición final. El contenido debe ser revisado por el profesional de referencia antes de utilizarse como capacitación clínica formal.

El simulador está orientado al escritorio. La exportación a Android y la realidad aumentada requieren su propia implementación y verificación; no se declaran completas.

#pagebreak()

#bibliography("media/referencias.bib", style: "apa")
