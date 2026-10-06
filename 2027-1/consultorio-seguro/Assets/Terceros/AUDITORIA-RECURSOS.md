# Auditoría de recursos para el nuevo consultorio

Los recursos descargados de Poly Haven tienen modelos o texturas de superficies reales, licencia CC0 y procedencia individual conservada. Las páginas de los recursos y la licencia se consultaron en el sitio original. No se usaron imágenes de ejemplo del sitio como texturas.

| Carpeta | Contenido | Autor | Fuente |
| --- | --- | --- | --- |
| `polyhaven-modern-arm-chair-01` | Sillón de espera con estructura de madera y cojines; 8 916 triángulos | Vibrant Nordic | https://polyhaven.com/a/modern_arm_chair_01 |
| `polyhaven-potted-plant-02` | Planta de recepción en maceta, hojas recortadas y tierra; 69 806 triángulos | Rico Cilliers | https://polyhaven.com/a/potted_plant_02 |
| `polyhaven-oak-veneer-01` | Mapas de chapa de roble; muestra de referencia de 1,8 m de ancho | Jenelle van Heerden | https://polyhaven.com/a/oak_veneer_01 |
| `polyhaven-beige-wall-001` | Mapas de enlucido pintado; muestra de referencia de 3 m de alto | Dimitrios Savva y Rico Cilliers | https://polyhaven.com/a/beige_wall_001 |

Cada carpeta contiene `FUENTE.md` y `procedencia.json` con autor, URL, licencia, cambios y sumas SHA-256. `materiales.json` documenta el color base, las normales OpenGL, la rugosidad original y los mapas derivados para URP. `limites.json` describe las dimensiones de los modelos en metros.

## Integración

Los OBJ conservan normales, coordenadas UV, grupos de material y traslaciones de los nodos originales. Se invirtió la coordenada V de glTF para Wavefront. No se simplificaron las mallas. Los modelos están en metros con el eje Y vertical. El sillón mide aproximadamente 0,82 × 1,02 × 0,99 m; la planta mide 0,70 × 0,84 × 0,66 m. El frente geométrico del sillón apunta a +Z antes de la importación; comprobar la orientación resultante dentro de Unity.

Los mapas `metallic_gloss` contienen metalicidad en rojo y suavidad en alfa. Los mapas `occlusion` contienen la oclusión ambiental en verde. Ambos deben importarse sin sRGB. Las normales deben importarse como mapas normales. El follaje usa el PNG RGBA derivado `potted_plant_02_leaves_base_alpha_2k.png`: activar recorte alfa a 0,5 y caras dobles.

Los procedimientos reproducibles están en `Tools/preparar_polyhaven.py` y `Tools/preparar_mapas_urp.py`. El primero conserva las descargas originales y el segundo solamente empaqueta sus canales para URP.

## Límites de los recursos clínicos existentes

Los modelos de 3D Assets tienen licencia CC0 declarada por el proveedor y metadatos de generación mediante IA. El sillón reclinado contiene 1 656 triángulos; la lámpara operatoria, 772; el brazo de radiografía, 488; el módulo de gabinete con lavabo, 1 392; y el autoclave con banco, 1 788. Ninguno de estos cinco GLB contiene imágenes de textura. Su forma es una representación esquemática y no permite afirmar detalle fotográfico de equipamiento especializado. Los materiales y acabados de la escena pueden mejorar su presentación, sin convertirlos en reproducciones exactas de dispositivos clínicos.

Las dimensiones, conservación de atributos y empaquetado de canales se comprobaron por lectura de archivos. La iluminación, importación visual, colisiones y orientación final requieren la revisión de la escena en Unity; esta auditoría de archivos por sí sola no las certifica.
