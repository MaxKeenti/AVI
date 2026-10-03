# Revisión visual en Unity — 2 de octubre de 2026

Unity 6.4 (6000.4.6f1), macOS, Metal, perfil PC.

- 12 pruebas EditMode aprobadas y 9 pruebas PlayMode aprobadas. La prueba visual se repitió tras el último ajuste del espejo y también aprobó.
- Se conserva la revisión de recepción, guía de bienvenida, continuidad del mapeo tonal, recorridos, actividades y regreso desde los menús.
- Lavabo orientado hacia el espacio de uso, con la grifería junto al muro. Las sondas cubren por completo el espejo y el piso; el espejo refleja el interior sin el disco de las luces de relleno.
- Espera de entrada amueblada con tres sillas, mesa auxiliar y planta del paquete Furniture Kit ya acreditado. El paso central permanece libre. El protector sanitario recorre la pared derecha, se elimina el tramo aislado junto al lavabo y la pintura de la entrada coincide con los laterales.
- Seis números al ras del muro, junto a cada puerta y a la altura de la vista. No sobresalen sobre los nombres ni obstruyen el pasillo. Se revisaron capturas desde ambos sentidos.
- Contorno blanco en objetos pequeños cercanos de la misma sala y dorado al apuntarlos. La prueba compara los píxeles con y sin efecto; verifica que el halo desaparece en menús, pausa, al sostener o clasificar y al entrar a una sala contigua dentro del alcance. También comprueba los pasos de jeringa y gasas.
- El aviso muestra una pulsación de E, sin «Hold». Una entrada de teclado simulada comprueba la recogida del instrumento. En el ejecutor sin ventana enfocada, la prueba permite entrada sintética y después restaura los ajustes de foco.
- El efecto descarta el pase anterior al recrearse y no encola trabajo adicional cuando no hay instrumentos que resaltar.
- Escena regenerada sin errores de compilación, de shaders ni avisos de reducción del atlas de sombras. La definición Typst compila.

Claude realizó una segunda revisión de código y capturas. Se corrigieron sus hallazgos confirmados sobre señalética, cobertura de reflejos, comprobación de salas y ciclo de vida del efecto; además se ajustaron los detalles de recepción. Su revisión fue de lectura: la ejecución y las capturas de comprobación se realizaron después en Unity.

Las capturas interiores se obtuvieron en PlayMode con la interfaz oculta. `lavabo.png`, `espera-entrada.png` y `rotulos-salas.png` muestran las correcciones de distribución. `pasillo.png` y `pasillo-regreso.png` comprueban los nombres desde el eje del pasillo. `instrumentos-contorno.png` e `instrumentos-sin-contorno.png` comparan la misma cámara con y sin el efecto. Las vistas exteriores y la planta proceden del constructor en el Editor.

En catorce vistas a 1600 × 1000, después del calentamiento, la mediana del tiempo entre fotogramas fue de 2.8 a 7.5 ms y el percentil 95 de 3.7 a 8.0 ms en este Editor. Se tomaron 60 muestras por vista. Esto no mide el arranque de las sondas, no separa CPU/GPU y no garantiza el rendimiento de otros equipos o de Android. Quedan sin medir el coste del halo en dispositivos móviles y su legibilidad en 4K; el radio actual se conserva en dos píxeles.
