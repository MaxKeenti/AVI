# Refinamiento visual de Consultorio Seguro

Registro del 5 de octubre de 2026. Esta ficha documenta la construcción y las adaptaciones; la evidencia de la escena guardada, las capturas y las pruebas vigentes se registra en [ENTREGA.md](../ENTREGA.md) y [Capturas/REVISION.md](../Capturas/REVISION.md). Una modificación del constructor por sí sola no acredita una revisión visual en Unity.

## Materiales de los recursos externos

| Recurso | Adaptación en la escena | Archivos fuente |
| --- | --- | --- |
| Modern Arm Chair 01, de Vibrant Nordic | La tapicería utiliza un albedo uniforme cálido y neutro. El material no asigna el mapa de color oscuro del cojín; mantiene los mapas de normales, rugosidad y oclusión del recurso. | Geometría y texturas originales conservadas. |
| Oak Veneer 01, de Jenelle van Heerden | Variantes de material con repetición UV independiente por superficie, más ajustes de relieve, suavidad y oclusión para revestimientos y detalles. | Mapas originales y derivados existentes conservados. |
| Beige Wall 001, de Dimitrios Savva y Rico Cilliers | El yeso interior utiliza color uniforme y un relieve normal reducido. No asigna la fotografía de color, su oclusión ni el mapa empaquetado de metalicidad y suavidad al material de pared. | Mapas originales conservados; el cambio es la asignación del material en Unity. |

Las fuentes, licencias y fechas de consulta originales figuran en los `FUENTE.md` y `procedencia.json` de cada carpeta de `Assets/Terceros`. Las fechas de consulta no se sustituyen por la fecha de este refinamiento. Los cambios también se reflejan en el catálogo estructurado, los créditos visibles de la aplicación y el catálogo de la definición Typst.

## Construcción propia de recepción y pasillo

[ClinicBuilder.InteriorDetail.cs](../Assets/ConsultorioSeguro/Scripts/Editor/ClinicBuilder.InteriorDetail.cs) genera el mostrador de recepción de planta redondeada y sus cantos, el fondo y la carpintería de tono piedra claro, los acentos estrechos de roble y el emblema dental de líneas. También genera el directorio en español, los motivos botánicos en relieve dentro de marcos, los paneles de protección de pared del pasillo y los pequeños documentos, charola y bolígrafo del registro.

Estas formas son geometría propia del proyecto: no proceden de imágenes descargadas, fotografías ni modelos adicionales de terceros. El roble asignado a algunas superficies conserva la atribución de Oak Veneer 01. Las sillas y plantas conservan sus correspondientes atribuciones externas.

## Equipamiento dental

[ClinicBuilder.DentalDetail.cs](../Assets/ConsultorioSeguro/Scripts/Editor/ClinicBuilder.DentalDetail.cs) sustituye visualmente los cojines planos del sillón importado por tapicería contorneada de elaboración propia, con costuras y grano de vinilo procedural. Añade detalles hidráulicos, pedal y cable. El sillón conserva el bastidor CC0; las atribuciones describen la adaptación.

La unidad dental de agua e instrumental incorpora consola, piezas de mano, mangueras, escupidera y vaso. Los taburetes tienen asiento, ajuste de altura y base de cinco ruedas. Ambas piezas son geometría propia y disponen de colisiones en sus volúmenes principales. La lámpara y el brazo radiográfico importados reciben lentes y detalles de colimador originales del proyecto, sin modificar los archivos GLB de origen.

## Iluminación y reflejos

La configuración de la escena combina iluminación de techo, luz exterior y luces de relleno interiores. Se ajustan la intensidad y el alcance de la oclusión ambiental para reducir halos oscuros; la emisión de los difusores y el resplandor se reducen. Se eliminan los puntos de relleno centrales situados a la altura del sillón porque causaban brillos sin una luminaria física cercana; se conservan el ambiente y los rellenos junto a las ventanas. Se revisan exposición, contraste y saturación dentro de la misma paleta de blancos cálidos y salvia.

Las sondas de reflexión usan proyección de caja y mezcla entre sondas. Los cristales transparentes dejan de proyectar sombras opacas sobre el interior, manteniendo su geometría y sus colisiones. Estos ajustes pertenecen al constructor de la escena; no son modificaciones de una imagen externa ni un nuevo recurso descargado.

Los valores implementados se conservan en [ClinicBuilder.Architecture.cs](../Assets/ConsultorioSeguro/Scripts/Editor/ClinicBuilder.Architecture.cs). Esta ficha no presenta la iluminación como una solución de iluminación global precalculada ni sustituye la comparación de capturas.

## Integridad y alcance de la procedencia

La biblioteca mantiene **51 recursos externos originales**. Cambiar materiales o añadir geometría propia en la escena no crea nuevas fuentes externas. Los archivos de origen importados se conservan; las sumas SHA-256 registradas en los metadatos de Poly Haven corresponden a esos archivos y a sus derivados ya existentes, no a una captura ni al archivo de escena.

La autoría de la arquitectura, señalización, disposición del mobiliario y detalles procedurales de la escena corresponde a la elaboración de Consultorio Seguro. Los modelos externos siguen atribuidos a sus creadores. Los cambios visuales no certifican fidelidad anatómica, funcionamiento clínico de equipos ni una equivalencia con un consultorio operativo.
