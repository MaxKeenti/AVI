# Consultorio Seguro

Simulador educativo en español para practicar la separación de residuos y el manejo inicial del instrumental en una clínica dental. Esta versión se construye desde una escena e implementación nuevas; conserva únicamente recursos externos documentados de la biblioteca anterior.

**Estado de la entrega:** escena nueva guardada, 18 capturas reales de Unity y 7 pruebas PlayMode aprobadas. El recorrido completo y las cinco prácticas se comprobaron automáticamente. [ENTREGA.md](ENTREGA.md) distingue la inspección visual, la prueba en el editor y lo que sigue sin ensayarse.

## Abrir y explorar

Abre esta carpeta desde Unity Hub con **Unity 6000.4.6f1**. El proyecto utiliza **Universal Render Pipeline 17.4.0** e **Input System 1.19.0**. Abre `Assets/ConsultorioSeguro/Escenas/ConsultorioSeguro.unity`. La escena y los materiales se guardan como recursos editables; no es necesario ejecutar el juego para inspeccionar su geometría. Pulsa Play y selecciona **Entrar al consultorio** para comenzar el recorrido.

| Control | Acción |
| --- | --- |
| WASD o flechas | Caminar |
| Ratón | Mirar |
| Mayús izquierda | Caminar más rápido |
| E | Interactuar con el objeto apuntado, a menos de 2,6 m |
| R | Devolver el objeto sostenido a su lugar |
| M | Abrir el manual / volver |
| Esc | Abrir pausa, reanudar o volver desde la ayuda |

El manual explica los controles desde el menú inicial, la pausa y las salas. Los contornos blancos ayudan a localizar instrumentos cercanos de la sala; el objetivo apuntado cambia a dorado. Su color no indica la clasificación correcta.

## Inspeccionar y editar la escena

El menú **Consultorio Seguro → Vistas** ofrece exterior, recepción, espera, pasillo, cada sala, personal y corte superior. El corte oculta techos y torre solo para la inspección del editor; cualquier otra vista los vuelve a mostrar. También hay cámaras guardadas, desactivadas, en **07 · Vistas de inspección**.

La jerarquía separa arquitectura, techos y torre, mobiliario y equipamiento, entorno urbano, iluminación, interacción y vistas. Se pueden seleccionar y mover objetos, cambiar materiales o ajustar luces directamente, sin ejecutar Play.

**Consultorio Seguro → Guardar capturas de Unity** produce imágenes de 1800 × 1125 y guarda las sondas de reflexión de las habitaciones. **Crear escena nueva** reconstruye y guarda la escena desde el constructor; sustituye los ajustes manuales de la escena actual. Para conservar una variante editada, guárdala primero con otro nombre.

## Cinco prácticas

| Sala | Propósito |
| --- | --- |
| 01 · Clasificación | Reconocer residuos y elegir un destino de separación |
| 02 · Esterilización | Distinguir instrumental reutilizable y desechables; reconocer el reprocesamiento |
| 03 · Materiales | Identificar consumibles y su destino según el estado descrito |
| 04 · Procedimientos | Relacionar una actividad dental simulada con los residuos generados |
| 05 · Radiografía | Identificar barreras y residuos de la actividad radiográfica simulada |

Se pueden visitar las prácticas en cualquier orden. Cada sala presenta primero sus pasos y después los objetos por clasificar. Cada acierto suma 10 puntos y cada error resta 5, con mínimo de cero. **Esc → Reiniciar esta práctica** restablece sus materiales y resultados.

La clínica incluye recepción, espera, pasillo central, sala de personal y almacenamiento. Las actividades apoyan el aprendizaje de clasificación; no constituyen entrenamiento práctico para efectuar procedimientos en pacientes ni validan un proceso de esterilización.

La escena define **13 objetos por clasificar y 10 pasos previos**, distribuidos entre las cinco prácticas. Radiografía representa preparación, toma digital y retiro de barreras mediante estaciones y mensajes; el brazo del equipo permanece estático.

## Recursos y edición

- `Assets/ConsultorioSeguro/`: implementación nueva y recursos propios de la clínica.
- `Assets/Terceros/`: modelos, texturas y sus registros `FUENTE.md` / `procedencia.json`.
- `Assets/Resources/Creditos.txt`: atribuciones visibles en la aplicación.
- [CREDITOS.md](CREDITOS.md): catálogo completo con fuente, autor, licencia y modificaciones de cada recurso original.
- [Documentacion/catalogo-recursos.json](Documentacion/catalogo-recursos.json): catálogo estructurado de 51 recursos en 18 carpetas; incluye recursos conservados para evaluación.
- [Documentacion/DEPENDENCIAS.md](Documentacion/DEPENDENCIAS.md): paquetes Unity, recursos esenciales de TextMesh Pro y fuente de respaldo Liberation Sans, fuera del recuento anterior.
- `../docs/definicion-proyecto/definicion-proyecto.typ`: definición académica y catálogo autónomo de fuentes.

Los nuevos sillones y plantas de Poly Haven conservan sus materiales PBR y mapas de 2K. Las texturas de madera y muro también proceden de Poly Haven. Los modelos de 3D Assets declaran generación mediante inteligencia artificial; algunos equipos y consumibles tienen geometría simplificada. Los créditos describen las adaptaciones para evitar atribuir exactitud clínica a modelos genéricos.

## Base educativa

La clasificación utiliza casos concretos sustentados en la [NOM-087-SEMARNAT-SSA1-2002](https://platiica.economia.gob.mx/normalizacion/nom-087-semarnat-ssa1-2002/). El manejo de instrumental reutilizable se apoya en la [NOM-013-SSA2-2015](https://sidof.segob.gob.mx/notas/docFuente/5462039). La definición Typst explica los supuestos y limita el alcance. Antes del uso docente clínico, corresponde al especialista del proyecto revisar los casos y el protocolo institucional.

## Comprobar la entrega

Consulta la matriz de [ENTREGA.md](ENTREGA.md). Una captura visual y una prueba automatizada prueban aspectos diferentes: se registran por separado los recorridos e interacciones ejecutados, la inspección de imágenes y los puntos todavía no ensayados. No se afirma compatibilidad verificada con Android, realidad aumentada, realidad virtual ni otros equipos.
