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
#title("VR-Cirugía: Quirófano Educativo en Realidad Virtual")

= Objetivo general
Desarrollo de una experiencia educativa de realidad virtual que sitúe al usuario dentro de un quirófano, permitiéndole conocer de forma interactiva los procedimientos preoperatorios, el instrumental quirúrgico y el equipamiento médico presente en una sala de operaciones. A través de esta inmersión, se busca acercar a estudiantes de medicina a estos entornos críticos de manera accesible, mostrando además cómo las tecnologías de realidad virtual, aumentada y mixta están transformando la práctica quirúrgica moderna.

= Objetivos específicos
- Modelado del Quirófano: Diseñar y texturizar un modelo 3D de un quirófano equipado con instrumental y maquinaria médica representativa, reflejando fielmente el entorno de una sala de operaciones real dentro del ambiente virtual inmersivo.

- Interactividad Educativa: Implementar puntos de información interactivos sobre el instrumental y equipamiento del quirófano que permitan al usuario identificar cada elemento, conocer su función y comprender su papel dentro de los protocolos preoperatorios.

- Protocolos Preoperatorios: Recrear de forma guiada procedimientos que no requieren modelos humanos, tales como el conteo de instrumental quirúrgico, la verificación del equipo de anestesia, la preparación del campo estéril y el checklist de seguridad quirúrgica de la OMS.

- Optimización del Rendimiento: Asegurar que el ambiente virtual mantenga una tasa de cuadros por segundo (FPS) estable para evitar el mareo por movimiento, permitiendo una navegación fluida para los estudiantes.

= Justificación
El proyecto "VR-Cirugía" surge como una iniciativa para explorar el potencial de los ambientes virtuales inmersivos en la educación médica. El quirófano es un entorno de alta complejidad técnica al que los estudiantes de medicina tienen acceso limitado durante su formación; la realidad virtual ofrece la posibilidad de familiarizarse con este espacio sin las restricciones logísticas, de seguridad y de disponibilidad que implica un quirófano real. Al crear una simulación interactiva de una sala de operaciones, se busca proporcionar a los estudiantes una experiencia educativa que les permita comprender los protocolos preoperatorios, identificar el instrumental y el equipamiento médico, y entender cómo la tecnología inmersiva está siendo adoptada en la práctica quirúrgica contemporánea. En un contexto donde la realidad virtual, aumentada y mixta están siendo incorporadas en hospitales y centros de formación médica para mejorar procedimientos quirúrgicos, este proyecto demuestra de primera mano ese potencial al colocar al usuario dentro del mismo entorno donde estas tecnologías se aplican. Además, contribuye a reducir la aprensión que genera el quirófano tanto en estudiantes como en pacientes, haciendo accesible el conocimiento sobre lo que ocurre en este espacio y cómo cada elemento presente puede salvar vidas.

= Alcance
El presente proyecto tiene como alcance el desarrollo de un ambiente virtual inmersivo que represente de manera interactiva un quirófano equipado con instrumental y maquinaria médica. Este entorno permitirá a los usuarios explorar la sala de operaciones, interactuar con los elementos presentes y recibir información educativa sobre su uso y los protocolos asociados.
El proyecto se enfocará en la recreación de procedimientos preoperatorios que no involucran modelos humanos, tales como el conteo y la identificación de instrumental quirúrgico, la verificación del equipo de anestesia, la preparación del campo estéril y la aplicación del checklist de seguridad quirúrgica de la OMS. No se pretende simular procedimientos quirúrgicos en sí mismos ni interacciones con pacientes.
Este proyecto se plantea como una simulación con fines educativos y de demostración tecnológica, dirigida a instituciones y escuelas de medicina que deseen apoyar el aprendizaje de sus alumnos mediante ambientes virtuales, permitiéndoles adentrarse de forma accesible y segura en un entorno de suma importancia clínica.

== Público objetivo
Estudiantes de medicina y ciencias de la salud, así como escuelas y facultades de medicina que busquen complementar la formación de sus alumnos mediante herramientas de realidad virtual como apoyo didáctico en el área quirúrgica.

== Tecnologías a utilizar
- Unity

== Alcance de funciones
Las funciones con las que el usuario podrá tener interacción en este ambiente virtual incluyen:

- Navegación: El usuario podrá desplazarse libremente por el quirófano, observar el espacio desde distintos ángulos y acercarse a cada pieza de equipamiento.

- Interacción con Instrumental: Posibilidad de seleccionar instrumentos quirúrgicos sobre la mesa Mayo para visualizar su nombre, clasificación y uso dentro del procedimiento operatorio.

- Interacción con Equipamiento: Explorar el funcionamiento de la máquina de anestesia, el monitor de signos vitales, las lámparas cialíticas y el contenedor RPBI, accediendo a información educativa sobre cada uno.

- Protocolo de Conteo de Instrumental: Simulación guiada del conteo de instrumental quirúrgico como protocolo de seguridad, verificando que cada pieza se encuentre presente antes y después de un procedimiento.

- Checklist de Seguridad Quirúrgica: Recreación interactiva del checklist de seguridad quirúrgica de la OMS (sign-in, time-out, sign-out), guiando al usuario a través de cada fase de verificación.

Las funciones de simulación y ambientación que se implementarán para dar realismo al ambiente virtual son:

- Iluminación Quirúrgica: Simulación del comportamiento de las lámparas cialíticas, incluyendo la posibilidad de ajustar su posición y dirección.

- Monitoreo de Signos Vitales: Pantalla del monitor mostrando lecturas simuladas de ECG, SpO2 y presión arterial para ambientar el entorno clínico.

- Sistema de Audio Ambiental: Activación de sonidos espaciales propios del quirófano (pitido del monitor de signos vitales, ventilación de la máquina de anestesia, sonido ambiental de la sala).

#pagebreak()


= Escenario: Quirófano virtual

#let quirofano = image("media/OR.png", height: 8cm)

#align(center)[#quirofano]

