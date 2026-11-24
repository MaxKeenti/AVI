# Verificación de Unity

El informe **pruebas-playmode.xml** conserva la verificación del 5 de octubre: 7 pruebas aprobadas, 0 fallos, ejecución terminada el 5 de octubre de 2026 a las 22:08:56, hora de Ciudad de México (6 de octubre a las 04:08:56 UTC).

La carpeta Diagnosticos conserva pruebas fallidas anteriores como registro del proceso de corrección. No representa el estado final. Tras corregir la circulación detrás de recepción y la posición de los taburetes, la prueba completa posterior comprueba la escena guardada y las cinco prácticas.

La inspección mediante la interfaz de Unity confirmó recepción en Scene View sin Play, inicio, manual desde inicio y pausa, desplazamiento con rueda, créditos, Volver, Escape, Reanudar y atajo M. La interfaz se observó a resolución Full HD. El recorrido íntegro por todas las salas se verificó automáticamente; no se realizó una caminata manual exhaustiva.

La matriz completa de evidencia y límites está en ../ENTREGA.md.

El 5 de octubre se volvió a abrir la escena guardada desde Unity Hub y se inspeccionaron recepción, procedimientos y radiografía con las vistas del editor, sin entrar en Play. Los nuevos muebles, cojines y materiales son visibles en la escena editable. Unity quedó en la vista de recepción.

## Mira de exploración · 6 de octubre de 2026

El informe **pruebas-mira-2026-10-06.xml** registra 7 pruebas de PlayMode aprobadas y 0 fallos; la ejecución termina el 6 de octubre de 2026 a las 10:19:04, hora de Ciudad de México (16:19:04 UTC), en Unity 6000.4.6f1. El informe fechado `pruebas-playmode.xml` permanece sin cambios.

Las capturas de ejecución 13–18 corresponden a esta prueba automatizada. En `16-silueta-dorada.png` y `17-material-en-mano.png` se observa la mira como una cruz blanca con borde oscuro, visible tanto al apuntar a un objeto como al sostenerlo. `Capturas/ORIGEN-RUNTIME.txt` y `Capturas/manifiesto-capturas.json` registran el origen, las fechas UTC y las sumas SHA-256; las capturas de arquitectura mantienen sus fechas originales.

## Observación del aviso de audio

El 6 de octubre de 2026 se investigó el aviso nativo `FMOD failed to switch back to normal output` (código 32). El registro contenía una aparición; no había llamadas de audio personalizadas en los scripts de la clínica. Se reinició Unity con el audio habilitado y sin cambiar la suspensión de salida. El registro de la sesión iniciada tras el reinicio no mostró otra aparición durante la comprobación. Esto no confirma una corrección permanente: queda pendiente reproducir un período prolongado de inactividad y reanudación.

## Acciones de simulación · 23 de noviembre de 2026

El informe **pruebas-simulacion-2026-11-23.xml** registra 21 pruebas de PlayMode aprobadas y 0 fallos; la ejecución termina el 23 de noviembre de 2026 a las 18:28:39, hora de Ciudad de México (24 de noviembre a las 00:28:39 UTC), en Unity 6000.4.6f1. Incluye las 7 pruebas anteriores, con el recorrido completo de la escena entregable, y 14 pruebas nuevas en `SimulationActionsTests`: física (caída, lanzamiento fuera de la sala, reinicio), movimiento (agacharse, sillón, brazo de rayos X), partículas (grifo, ráfagas de clasificación y confeti), manipulación (tapa, viaje e inspección, trayectoria al recipiente) y animaciones (controladores generados, panel de paso y lámpara).

La prueba de integración vuelve a escribir las capturas 13–18. Esas imágenes se restauraron a su versión registrada para conservar las sumas SHA-256 de `Capturas/manifiesto-capturas.json`; no muestran las acciones de simulación. No se realizó una prueba manual con teclado y ratón de la inspección con clic derecho ni del lanzamiento con G.
