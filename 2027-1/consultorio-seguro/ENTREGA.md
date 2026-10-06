# Entrega y validación de Consultorio Seguro

Nueva construcción, verificación actualizada el 4 de octubre de 2026. Este documento diferencia comprobaciones documentales, pruebas de ejecución e inspección visual. No hereda resultados de la versión anterior.

## Comprobado documentalmente

- Procedencia de **51 recursos originales** en **18 carpetas**; los modelos físicos OBJ/GLB/glTF presentes tienen una entrada en el catálogo.
- El catálogo cubre los 74 modelos, 38 texturas y la fuente Noto de `Assets/Terceros`, sin archivos ausentes ni recursos visuales sin entrada.
- Las 42 sumas SHA-256 registradas de Poly Haven y Noto coinciden con los archivos locales. Véase `Documentacion/verificacion-procedencia.json`.
- Los cuatro paquetes nuevos de Poly Haven registran autor, URL, licencia CC0, modificaciones, descargas y SHA-256.
- Las dependencias distribuidas de Unity se registran aparte: 33 recursos esenciales de TextMesh Pro coinciden con el paquete original; licencias Unity Companion y OFL de Liberation Sans preservadas. Véase `Documentacion/DEPENDENCIAS.md`.
- Los modelos con atribución requerida conservan autor, fuente, enlace a CC BY 3.0 y modificaciones.
- Fuentes oficiales de las NOM y supuestos educativos revisados para la nueva implementación. La revisión documental no es aprobación de un especialista clínico.

## Resultado de Unity

**7 pruebas PlayMode aprobadas; 0 fallos.** Registro reproducible: [pruebas-playmode.xml](Verificacion/pruebas-playmode.xml). La prueba de integración abre la escena entregable y recorre la geometría con el CharacterController real; apunta desde altura de ojos y utiliza los colliders reales. Los botones se activan por su evento de interfaz. Las seis pruebas adicionales comprueban estados, decisiones erróneas, repetición y entrada de teclado simulada.

| Requisito | Resultado y evidencia |
| --- | --- |
| Apertura y compilación | Unity 6000.4.6f1 abre y compila la escena. También se abrió desde Unity Hub y se inspeccionó recepción en Scene View sin Play. |
| Circulación | Recorrido automático completo: banqueta, acceso, registro frontal y posterior, espera, pasillo, cinco salas, personal, regreso y salida. |
| Cinco prácticas | 10 pasos y 13 objetos completados mediante interacción real de la escena; puntuaciones finales comprobadas. |
| Menús y manual | Inicio, pausa, volver, reanudar y seis paneles de manual comprobados; desplazamiento del manual y conservación íntegra de créditos largos. Atajo M comprobado con eventos del sistema de entrada. |
| Decisiones y repetición | Acierto, error, recuperación de pose, ausencia de puntuación duplicada, orden de pasos y reinicio de proceso comprobados en pruebas específicas. |
| Contorno | Límite de distancia y sala; ocultamiento al pausar, sostener y completar. Captura 16 muestra blanco y dorado, 17 el objeto sostenido sin ayuda. El shader respeta profundidad; no se hizo una prueba visual exhaustiva de oclusión desde cada ángulo. |
| Materiales, orientación y señales | Inspección de las 12 vistas de arquitectura. Corregidos equipo radiográfico sobredimensionado, orientaciones, muebles que cerraban el paso, rótulos superpuestos y contactos al ras del muro. |
| Edición y vistas | Escena serializada, siete grupos de jerarquía y menú de vistas. Recepción visible antes de Play confirmada en el editor. |
| Capturas | 18 imágenes reales de Unity, con método y origen registrados en `Capturas/ORIGEN.txt` y `ORIGEN-RUNTIME.txt`. |
| Typst | Fuente autónoma con catálogo, dependencias y cuatro imágenes originales de Unity; PDF generado. |

## Alcance de la revisión visual

Se inspeccionaron las capturas a altura normal de ojos, el corte superior y la recepción dentro del editor. Mediante clics, rueda y teclado en la interfaz del editor se verificaron inicio, manual desde inicio y pausa, desplazamiento del manual, créditos, Volver, Escape, Reanudar y atajo M. La interfaz se comprobó en Full HD (1920 × 1080); Unity quedó abierto en recepción fuera de Play. **No se completó una caminata manual con teclado y ratón por todas las salas**: el recorrido íntegro y las interacciones fueron automatizados. No se equipara una imagen de cámara con una prueba de entrada física.

La escena usa iluminación directa, rellenos y ocho sondas de reflexión guardadas. No contiene una iluminación global horneada. Unity ajusta automáticamente la resolución de sombras adicionales para caber en su atlas; las imágenes entregadas incluyen ese ajuste. Parte del instrumental y del equipo dental conserva geometría simplificada; esta entrega no debe describirse como una visualización fotorrealista terminada.

## Procedimiento de aceptación

1. Abrir la escena nueva, comprobar jerarquía, materiales y vistas del editor antes de Play.
2. Iniciar el juego y consultar manual y créditos; regresar sin perder el contexto.
3. Caminar desde la calle hasta recepción a altura de ojos. Recorrer la zona de pacientes y el acceso del personal al mostrador.
4. Recorrer el pasillo en ambos sentidos y cada sala. Comprobar que no es necesario atravesar mobiliario ni acercarse excesivamente a un muro para leer.
5. En cada práctica, probar al menos una decisión errónea, completar la secuencia correcta, reiniciar y repetir. Comprobar objetos sostenidos y su restablecimiento.
6. Comprobar el contorno en instrumentos pequeños: distancia, sala actual, objetivo dorado y desaparición al pausar, sostener o completar. Comprobar que no atraviese superficies opacas ni indique el destino correcto.
7. Capturar exterior, recepción, pasillo, cada sala, sala de personal y planta. Anotar defectos pendientes y conservar el registro de Unity.

## Fuera de la comprobación actual

No se ha evaluado todavía rendimiento en equipos de destino, accesibilidad con dispositivos alternativos, Android, realidad aumentada, realidad virtual ni formación clínica con usuarios. La geometría y el material de los objetos educativos no son especificaciones para construir una clínica real.
