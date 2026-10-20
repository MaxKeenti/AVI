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
  proyecto: "Expectativas de la Realidad Extendida como tecnología core en la industria",
)

#pagebreak()
#set par(justify: true, leading: 1.4em)
#set heading(numbering: "1.")
#set list(indent: 1.5em)
#show table: set par(justify: false)

#title("Expectativas de la Realidad Extendida")

#block[
  #set text(size: 9pt)
  #set par(leading: .65em)
  #outline(title: "Índice", depth: 2)
]

#pagebreak()

= Introducción

La *Realidad Extendida* (`XR`) dejó de ser solo una promesa del entretenimiento, hoy aparece en fábricas, hospitales, universidades, tiendas y ejércitos, sin embargo, no todos los sectores esperan lo mismo de ella ni la adoptan al mismo ritmo.

Esta indagación revisa qué se espera de la `XR` como tecnología *core*, es decir, como una parte central de la operación y no como un complemento ocasional, la revisión se organiza por enfoques de la industria: académico, comercial, industrial, salud, gobierno y defensa, y capacitación corporativa. Al final se contrastan las expectativas con los retos que todavía frenan su adopción.

= ¿Qué es la Realidad Extendida?

El término `XR` agrupa a las tecnologías que mezclan lo real con lo virtual, su base teórica más citada es el *continuo realidad-virtualidad* de Milgram y Kishino, que va de un entorno completamente real a uno completamente virtual; entre ambos extremos está la *realidad mixta*, que combina elementos de los dos @milgram1994taxonomy.

- *Realidad Aumentada (`AR`):* agrega objetos virtuales sobre el entorno real, por ejemplo, instrucciones sobre una máquina vistas desde unos lentes o un celular.

- *Realidad Virtual (`VR`):* sustituye por completo el entorno real con uno generado por computadora, normalmente mediante un visor.

- *Realidad Mixta (`MR`):* los objetos virtuales se anclan al espacio físico y el usuario interactúa con ambos al mismo tiempo.

#figure(
  image("media/img/XR.png", width: 70%),
  caption: [Imagen denotando las diferencias entre AR, VR y MR.],
)

= Panorama general del mercado

Las cifras del mercado varían mucho porque cada consultora mide cosas diferentes, aun así, todas coinciden en un crecimiento fuerte hacia 2030.

#block(breakable: false)[
  #table(
    columns: (1.2fr, 2fr, 1fr),
    inset: 7pt,
    align: left,
    table.header([*Fuente*], [*Expectativa*], [*Horizonte*]),
    [ABI Research], [Software y servicios `XR` pasan de *44.7 a 299.3 mil millones de USD* @abi2024xr.], [2024 - 2030],
    [PwC], [La `VR` y la `AR` aportarían hasta *1.5 billones de USD* al PIB mundial, la mayor parte por `AR` @pwc2019seeing.], [2019 - 2030],
    [IDC], [Los lentes inteligentes pasan de *13.6 a 27.3 millones* de unidades al año @ubrani2026smartglasses.], [2026 - 2030],
  )
]

El cambio más reciente es de forma: los visores de `VR` pierden terreno, mientras que los lentes inteligentes ligeros, apoyados en inteligencia artificial, crecieron *167 %* en el primer trimestre de 2026 @ubrani2026smartglasses. Al mismo tiempo, la división Reality Labs de Meta perdió cerca de *19 mil millones de USD* en 2025 @auganix2026reality, lo que muestra que la inversión todavía va muy por delante de las ganancias.

= Expectativas por enfoque de la industria

== Enfoque académico

En las universidades se espera que la `XR` permita practicar lo que en un aula solo se explica, por ejemplo, recorrer un órgano en anatomía, operar una máquina peligrosa o visitar un sitio histórico. La revisión de Radianti y colaboradores identificó *18 áreas de aplicación* de la `VR` inmersiva en educación superior, aunque señala que la mayoría de los estudios evalúa la usabilidad y no el aprendizaje, y que la tecnología sigue más en proyectos experimentales que en clases regulares @radianti2020systematic.

Para explicar cuándo la inmersión sí mejora el aprendizaje, el modelo `CAMIL` propone que sus ventajas vienen de la *presencia* y la *agencia*, siempre que el diseño controle la carga cognitiva @makransky2021camil. El Horizon Report de EDUCAUSE mantiene la `XR` como una tecnología de alto potencial, pero también la ubica como una de las más costosas por el equipo y por el esfuerzo de crear contenido @brown2020horizon.

La expectativa académica, entonces, es que la `XR` sea core en el aprendizaje basado en competencias y en la investigación, pero con evidencia más rigurosa sobre resultados.

#figure(
  image("media/img/EscuelaXR.jpg", width: 50%),
  caption: [Realidad extendida siendo usada en una escuela.],
)

== Enfoque comercial

En el comercio y el consumo se espera que la `XR` cambie la forma de comprar y de entretenerse. En un estudio de Publicis y Snap, cerca de *tres de cada cuatro* encuestados dijeron que la `AR` tendrá un papel mayor en sus compras durante los siguientes cinco años, sobre todo para probar productos de forma virtual o ver muebles en casa antes de comprarlos @marketingdive2021shopping.

La apuesta de las grandes empresas se mueve hacia los lentes inteligentes: EssilorLuxottica vendió *7 millones* de lentes Ray-Ban y Oakley con Meta en 2025, el triple que el año anterior @roadtovr2026glasses. ABI Research estima que el software y los servicios `XR` de consumo llegarán a *169.4 mil millones de USD* en 2030 @abi2024xr.

#figure(
  image("media/img/ProbarXR.jpg", width: 50%),
  caption: [Realidad extendida siendo usada para probar como se verian los productos.],
)

== Enfoque industrial y de manufactura

En la industria la expectativa es más concreta: usar `XR` junto con gemelos digitales para diseñar, simular procesos, capacitar operadores y dar soporte remoto. En el estudio de Deloitte y el Manufacturing Leadership Council, el *92 %* de más de 350 ejecutivos de manufactura dijo que su empresa ya experimenta o implementa al menos un caso de uso del metaverso industrial, y esperan mejoras de *12 % o más* en calidad, rendimiento y productividad en uno a tres años @deloitte2023industrial.

Esto coincide con PwC, que atribuye la mayor parte del impacto económico esperado a la `AR`, por su uso en desarrollo de productos, mantenimiento y procesos @pwc2019seeing.

#figure(
  image("media/img/EmpresasXR.jpg", width: 50%),
  caption: [Realidad extendida siendo usada para dar mantenimiento a maquinaria.],
)

== Enfoque de salud

En salud se espera que la `XR` reduzca riesgos al permitir que el personal practique sin pacientes reales. Un metaanálisis de *31 ensayos* en cirugía laparoscópica encontró que el entrenamiento con `VR` es más efectivo que el entrenamiento con video y al menos tan bueno como el entrenamiento con cajas de práctica @alaker2016laparoscopic. Este enfoque es el mismo que sigue nuestro proyecto *Consultorio Seguro*, donde el usuario practica la clasificación de residuos sin tocar materiales peligrosos.

#figure(
  image("media/img/SaludXR.jpg", width: 50%),
  caption: [Realidad extendida siendo usada para simular un quirofano.],
)

== Enfoque de gobierno y defensa

En defensa se espera que la `MR` dé al soldado información táctica en tiempo real. El programa `IVAS` del Ejército de Estados Unidos, que usaba visores basados en HoloLens, pasó a Anduril y se reorganizó como *Soldier Borne Mission Command*; según el Congressional Research Service, ninguna de las unidades desarrolladas originalmente se va a desplegar porque eran demasiado caras para producir a escala @crs2026ivas. El caso muestra que la expectativa sigue en pie, pero que el costo del equipo puede detener un programa completo.

#figure(
  image("media/img/MiliciaXR.jpg", width: 60%),
  caption: [Realidad extendida siendo usada para estrategizar en el campo de batalla.],
)

== Enfoque de capacitación corporativa

Las empresas esperan que la `XR` haga la capacitación más rápida y medible. En un estudio de PwC con nuevos gerentes, quienes se capacitaron con `VR` terminaron hasta *4 veces más rápido* que en el aula y se sintieron hasta *275 % más seguros* de aplicar lo aprendido; el costo de crear el contenido es mayor al principio, pero se iguala con el del aula a partir de unos *375 participantes* @pwc2020vlearning.

#figure(
  image("media/img/CapacitacionXR.jpg", width: 60%),
  caption: [Realidad extendida siendo usada para capacitación corporativa.],
)

#pagebreak()

= Comparativa de expectativas

#block(breakable: false)[
  #table(
    columns: (1fr, 1.6fr, 1fr),
    inset: 7pt,
    align: left,
    table.header([*Enfoque*], [*Expectativa principal*], [*Madurez*]),
    [Académico], [Aprendizaje práctico y seguro, basado en competencias.], [Experimental],
    [Comercial], [Prueba virtual de productos y lentes inteligentes de uso diario.], [En crecimiento],
    [Industrial], [Gemelos digitales, simulación, mantenimiento y soporte remoto.], [Adopción temprana],
    [Salud], [Entrenamiento quirúrgico y clínico sin riesgo para pacientes.], [Con evidencia],
    [Defensa], [Información táctica en tiempo real para el soldado.], [Reorganizándose],
    [Corporativo], [Capacitación más rápida y medible.], [Con evidencia],
  )
]

= Retos que frenan la adopción

- *Costo:* el *51 %* de los ejecutivos de manufactura lo señala como la principal barrera @deloitte2023industrial, y fue la causa de que no se desplegaran las unidades originales de `IVAS` @crs2026ivas.

- *Talento:* faltan personas con las habilidades para crear y mantener soluciones `XR` @deloitte2023industrial.

- *Integración:* conectar la `XR` con los sistemas que ya existen en la empresa es la tercera barrera más citada @deloitte2023industrial.

- *Ciberseguridad y privacidad:* más del *70 %* de los ejecutivos la considera uno de los mayores riesgos @deloitte2023industrial, porque los dispositivos capturan video, voz y datos del entorno.

- *Evidencia de resultados:* en educación todavía se mide más la usabilidad que el aprendizaje @radianti2020systematic.

- *Rentabilidad:* las pérdidas de Reality Labs muestran que el mercado de consumo aún no paga la inversión @auganix2026reality.

#pagebreak()

= ¿Es la Realidad Extendida una tecnología core?

Con base en lo revisado, la `XR` ya es core en algunas tareas concretas: capacitación y simulación, diseño de productos y soporte en campo, en estas tareas la evidencia muestra beneficios medibles y el costo se justifica cuando se capacita a muchas personas o cuando equivocarse en la vida real es caro o peligroso.

En cambio, todavía no es core para el consumidor general ni para la mayoría de las empresas, el visor de `VR` como dispositivo de uso diario no despegó como se esperaba y la industria está cambiando su apuesta hacia lentes ligeros con inteligencia artificial @ubrani2026smartglasses.

#figure(
  image("media/img/FuturoXR.jpg", width: 50%),
  caption: [La realidad aumentada se puede usar para la separación de basura en base a su tipo.],
)

== Conclusión:

La expectativa general es que la `XR` se vuelva una capa más de la operación, igual que en su momento lo fueron el celular o la nube, pero cada enfoque la adopta a un ritmo diferente. La academia busca evidencia de aprendizaje, el comercio busca dispositivos cómodos y baratos, la industria busca productividad y la salud y la defensa buscan reducir riesgos. Las expectativas más realistas son las que unen a la `XR` con un problema concreto, como lo hace nuestro simulador de manejo de residuos, donde practicar en un entorno virtual evita riesgos reales.

#figure(
  image("media/img/BasuraXR.jpg", width: 50%),
  caption: [En el futuro, la realidad extendida se integrará en la operación cotidiana.],
)

#pagebreak()

#bibliography("media/referencias.bib", style: "apa")
