# Consultorio Seguro

Prototipo del simulador definido en `2027-1/docs/definicion-proyecto`: un consultorio en 3D donde el usuario clasifica Residuos Peligrosos Biológico-Infecciosos según la NOM-087-ECOL-SSA1-2002.

- **Unity:** 6000.4.6f1, Universal 3D (URP), Input System.
- **Abrir:** `unity open 2027-1/consultorio-seguro` o desde Unity Hub. La escena es `Assets/Escenas/Consultorio.unity`.
- **Controles:** WASD caminar, ratón mirar, Shift correr, E tomar / depositar / interactuar, Esc pausa.

## Qué hay

La escena usa **geometría sencilla con acabados arquitectónicos** (cubos, cilindros, cápsulas, materiales y detalles de mobiliario). Todo lo que hace funcionar el simulador vive en componentes, así que se puede cambiar el aspecto sin tocar código.

### Recorrido

1. **La calle.** El usuario empieza en la banqueta de enfrente, viendo la clínica al otro lado del paso peatonal. Alrededor hay edificios vecinos, árboles, faroles, autos estacionados, una banca y un bote de basura municipal que explica que los RPBI nunca van con la basura de la calle. Unos límites invisibles en los extremos impiden salir de la cuadra.
2. **La fachada.** Letrero de *Clínica dental*, lona institucional (UPIICSA · IPN · Equipo 7), marquesina y ventana.
3. **La recepción.** Mostrador, sala de espera, lavabo con su letrero de higiene de manos y carteles sobre el manejo de RPBI y el código de colores. Al entrar aparece un mensaje de bienvenida.
4. **Las salas de práctica**, al fondo. Un vestíbulo con directorio lleva al pasillo central y a cinco salas numeradas: clasificación, esterilización, materiales, procedimientos y radiografía. Hay una sala adicional de descanso.

Los mensajes de bienvenida salen de componentes `ZonaMensaje` y se muestran una sola vez por sesión.

No hay que elegir la acción en un menú: el menú principal ofrece *Manual de controles e interacción*, *Entrar al consultorio*, *Créditos* y *Salir*. Cada acción ocurre en su propia área y todo está en su lugar desde el inicio:

| Acción | Área | Qué hace el usuario |
| --- | --- | --- |
| 1. Clasificación por el tipo de desecho | Sala 01, carrito y recipientes | Clasifica 6 residuos en los recipientes |
| 2. Esterilización de instrumentos | Sala 02, mostrador de esterilización | Lleva lo reutilizable a la autoclave y desecha lo de un solo uso |
| 3. Muestra de materiales | Sala 03, estante de materiales | Decide a dónde irá cada material después de usarse |
| 4. Retiro de una pieza dental | Sala 04, unidad dental y mesa de instrumental | 3 pasos en orden y luego clasifica los residuos que generan |
| 5. Manejo del equipo de rayos X | Sala 05, equipo y sillón independientes | Enciende, coloca barreras, posiciona el brazo, toma y clasifica las barreras |

- **Al entrar a un área** por primera vez aparecen sus instrucciones y el marcador de la esquina cambia a esa acción. También cambia al tomar un residuo o hacer un paso, así que puedes cruzar el consultorio con un residuo en la mano sin perder el marcador.
- **Cada área tiene una hoja de práctica** (un tablero en la pared o en un atril) con su avance, aciertos, errores, puntos y mejor puntuación. Al interactuar muestra las instrucciones o, si la práctica ya se terminó, la reinicia para repetirla.
- Se puede trabajar en varias áreas a la vez; cada una lleva su propio marcador.
- Un error regresa el residuo a su lugar y explica por qué, para volver a intentarlo.
- En la pausa (Esc) está *Reiniciar la práctica actual*, que reinicia el área del marcador.

### Componentes

- `Residuo`: objeto que se toma. Apunta a un `DatosResiduo` (nombre, destino correcto, explicación).
- `Contenedor`: recipiente o estación con un `Destino` (bolsa roja, amarilla, punzocortantes, basura común, esterilización…). Puede haber varios del mismo tipo, como el segundo recipiente de punzocortantes del área de esterilización.
- `Escenario`: una acción. Los `Residuo` hijos son los que hay que clasificar.
- `ZonaEscenario`: volumen (Box Collider en modo trigger) hijo del escenario que detecta cuando el usuario entra al área. El objeto `Zona` está donde se para el usuario; el volumen cubre el área completa.
- `TableroPractica`: la hoja de práctica del área.
- `SecuenciaPasos` y `Paso`: pasos en orden antes de clasificar. Sus eventos `alCompletar` / `alReiniciar` se configuran en el Inspector (encender una luz, mover el brazo con `TransicionTransform`, etc.).
- `ZonaMensaje`: volumen que muestra un mensaje la primera vez que el usuario entra (bienvenida a la clínica y al consultorio).
- `ObjetoInformativo`: muebles que muestran su nombre y una descripción.
- `GestorSimulacion`, `InterfazSimulador`, `SonidosRetroalimentacion`: flujo, interfaz y sonidos. Los sonidos son tonos generados; asignen clips reales en el Inspector.

## Acabados visuales

La escena incluye piso de porcelanato con juntas, muros cálidos, acentos verde salvia, zoclos y cornisas, listones en recepción y marquesina, marcos de ventana y frentes de cajones con tiradores. La iluminación combina sol cálido y luces interiores con sombras suaves. Los colores de clasificación de residuos se conservan.

- Los materiales están en `Assets/Materiales/Acabados/`. Las texturas se generan de forma determinista en el proyecto; no requieren recursos de terceros.
- **Consultorio Seguro → Actualizar acabados visuales** regenera la escena desde el constructor y aplica acabados, modelos y distribución por salas. Guarda la escena; los datos existentes de las prácticas se conservan. Los cambios manuales a objetos de la escena se reemplazan.
- **Construir consultorio** también incluye estos acabados. Su implementación está en `ConstructorConsultorio.Acabados.cs`.
- `CapturarAcabados` genera vistas del editor y una planta en `Capturas/`. Para revisar el aspecto durante el juego, `RevisionVisualTests` captura los tableros con su contenido real y espera a que terminen los reflejos. Las capturas interiores de la revisión actual proceden de esta prueba; las vistas exteriores y la planta proceden del constructor.
- Las sombras interiores deben evaluarse en el dispositivo objetivo antes de exportar a Android; esta revisión se verificó en el editor de escritorio.

Los objetos pequeños cercanos de la misma sala tienen un **contorno blanco**; el objetivo apuntado cambia a **dorado**. La silueta se ve sobre la charola, conserva el tamaño real del instrumento y no revela la clasificación correcta. Se oculta en menús, en pausa y al sostener o clasificar el objeto. El efecto usa una máscara y un halo de dos píxeles en URP, registrados en los perfiles PC y Mobile; la comprobación visual se realiza en PC.

## Cambiar las primitivas por modelos

1. Importa el modelo en su propia carpeta dentro de `Assets/Terceros/` (ver abajo).
2. Arrastra el modelo como **hijo** del objeto que va a reemplazar (por ejemplo, `Escenarios/Escenario 1-Clasificacion/Gasa empapada de sangre`).
3. En el objeto original desactiva el `Mesh Renderer`. **No borres** el objeto ni sus componentes (`Residuo`, `Contenedor`, `Paso`, `TableroPractica`, `Box Collider`); son los que hacen funcionar la interacción.
4. Si mueves un mueble, mueve también la `Zona` de su escenario y ajusta el tamaño de su volumen.
5. Ajusta el `Box Collider` si el modelo es más grande que la primitiva.

## Recursos de terceros y atribuciones

Se utilizan modelos de **Furniture Kit, de Kenney**: `chairModernFrameCushion` (asientos de espera, recepción y descanso), `pottedPlant`, `computerScreen`, `bathroomSink`, `bench`, `trashcan` y `sideTable`. Fuente: https://kenney.nl/assets/furniture-kit; licencia CC0 1.0. Los archivos originales, la licencia y el registro de procedencia están en `Assets/Terceros/kenney-furniture-kit/`. La sección «Modelos 3D de terceros y sus fuentes» de `2027-1/docs/definicion-proyecto/definicion-proyecto.typ` documenta su uso.

**Consultorio Seguro → Actualizar modelos de terceros** regenera y guarda la escena, sustituye las representaciones visuales, distribuye las cinco salas y actualiza los créditos. Recrea sus colisionadores e interacciones a partir del constructor. La reconstrucción completa también incorpora estos modelos. Los materiales adaptados a URP están en `Assets/Materiales/Acabados/Modelo_*.mat`.

El profesor pide citar la fuente de cada modelo, así que el proyecto lo exige:

1. Cada recurso descargado (modelo, textura, sonido) va en **su propia carpeta** dentro de `Assets/Terceros/`, por ejemplo `Assets/Terceros/sillon-dental/`.
2. En esa carpeta: clic derecho → *Create → Consultorio Seguro → Atribución*. Llena título, autor, URL de la página, licencia y fecha de consulta (AAAA-MM-DD). Si modificaron el recurso, anótenlo en *Modificaciones* (las licencias CC BY lo piden).
3. Las atribuciones aparecen solas en la pantalla de **Créditos** del simulador, con su enlace.
4. *Consultorio Seguro → Atribuciones → Exportar créditos* genera `CREDITOS.md` y `creditos.bib`. El `.bib` se puede copiar a `media/referencias.bib` de los documentos Typst para citar en APA.

Si una carpeta de `Assets/Terceros/` no tiene atribución o le faltan campos, Unity lo avisa en la consola, la prueba `TodosLosRecursosDeTercerosTienenAtribucion` falla y **la compilación del juego se detiene**.

## Editar el contenido

- Los textos de residuos y escenarios están en `Assets/Datos/` y se editan desde el Inspector.
- Para agregar un residuo: duplica uno de `Assets/Datos/Residuos/`, cámbialo y ponlo en un objeto con el componente `Residuo` dentro del `Escenario` correspondiente.
- *Consultorio Seguro → Construir consultorio* vuelve a generar la maqueta desde cero (calle, edificio y consultorio). El código está en `ConstructorConsultorio.cs` (consultorio e interfaz) y `ConstructorConsultorio.Entorno.cs` (edificio, recepción y calle). **Reemplaza la escena** (pide confirmación), pero no toca `Assets/Datos/` ni los materiales.

## Clasificaciones por validar con el especialista

Estas asignaciones siguen la lectura de la NOM-087, pero dependen de criterios que conviene confirmar con el profesional de referencia. Se corrigen en su asset de `Assets/Datos/Residuos/`.

| Residuo | Destino asignado | Duda |
| --- | --- | --- |
| Pieza dental extraída | Bolsa amarilla (patológico) | Confirmar que el consultorio la maneja como residuo patológico |
| Guantes sin sangre visible | Basura común | La norma solo incluye material empapado, saturado o goteando sangre |
| Guantes empapados de sangre | Bolsa roja | Confirmar criterio del consultorio |
| Vaso desechable / funda del sensor con saliva | Basura común | La saliva sin sangre no está en el listado de la norma |
| Lima endodóntica desechable | Punzocortantes | Hay limas reutilizables un número limitado de veces |

## Pruebas

Desde la carpeta del proyecto, con el editor cerrado:

```sh
UNITY=/Applications/Unity/Hub/Editor/6000.4.6f1/Unity.app/Contents/MacOS/Unity
$UNITY -batchmode -projectPath . -runTests -testPlatform EditMode -testResults editmode.xml
$UNITY -batchmode -projectPath . -runTests -testPlatform PlayMode -testResults playmode.xml
```

También desde *Window → General → Test Runner*. Las nueve pruebas de PlayMode cubren menús y regreso, modelos interactivos, las cinco actividades, recorridos por la calle y todas las salas, el acceso al mostrador y la guía de bienvenida. La prueba del contorno compara imágenes con y sin halo, comprueba el borde dorado y verifica que se oculta al pausar, sostener o clasificar objetos y al cambiar a una sala contigua dentro del alcance. También simula una pulsación de E para tomar un instrumento y comprueba que solo se resalta el paso disponible. La prueba visual comprueba la cobertura de reflejos en el espejo y el piso, la continuidad del mapeo tonal al entrar, los números al ras del muro y la ausencia de desbordamiento en los textos de actividad.

Para guardar capturas de PlayMode y tiempos orientativos del Editor, define `AVI_CAPTURAS_REVISION` con una carpeta de salida y ejecuta las pruebas de PlayMode con gráficos habilitados:

```sh
AVI_CAPTURAS_REVISION="$PWD/Capturas" "$UNITY" -projectPath . -runTests -testPlatform PlayMode -testResults visual.xml
```

Los reflejos en tiempo real están habilitados en el perfil PC. Los tiempos de la prueba se miden tras calentar cada vista, a 1600 × 1000; no sustituyen la medición de una compilación en el equipo de destino. Véase `Capturas/REVISION.md`.

## Pendiente

- Modelos detallados de equipo dental y paciente, y sonidos definitivos.
- Realidad Aumentada para Android (AR Foundation): el módulo de Android no está instalado en este editor.
- Mostrar recipientes herméticos (líquidos) si se agregan residuos líquidos; el `Destino` ya existe.

### Sustitución ampliada de modelos

La escena incorpora equipos odontológicos, paciente adaptado, carritos, gabinetes, recipientes, puerta, consumibles y entorno urbano de terceros. Las fuentes, licencias y modificaciones están en `Assets/Terceros/*/FUENTE.md`, en `CREDITOS.md` y en la definición del proyecto. Los modelos de 3D Assets tienen generación mediante IA declarada por su proveedor.

`Consultorio Seguro > Actualizar modelos de terceros` regenera la escena con las sustituciones y la distribución por salas. glTFast 6.20.0 importa los originales GLB; `python3 Tools/preparar_modelos.py` regenera las piezas OBJ adaptadas desde esos originales. Los modelos de residuos son hijos de su objeto interactivo y el brazo de rayos X conserva su pivote animado.

Pendientes de una fuente adecuada: cubrebocas, fórceps de extracción, lima endodóntica y aguja de sutura. La jeringa y los guantes son adaptaciones esquemáticas, documentadas como tales. La arquitectura, los controles y la señalización son propios. Algunos modelos de la biblioteca se conservan para evaluación y aún no se instancian.

### Avenida y edificios altos

La avenida tiene 89 m de longitud y edificios vecinos de 16 a 32 m de altura. El consultorio ocupa la planta baja de una torre de aproximadamente 21 m; sus pisos superiores y los edificios vecinos son exteriores sin interiores visitables. Se reutilizan los modelos building-a, building-b y building-c de Kenney City Kit Commercial.

El menú `Consultorio Seguro > Actualizar entorno urbano` regenera la escena completa con entorno, acabados, modelos y distribución interior, conservando los datos de las prácticas. Las capturas `Capturas/barrio.png` y `Capturas/avenida.png` muestran el conjunto y el recorrido a nivel de calle.

### Salas de práctica

La ampliación posterior mide 15 × 19.5 m, con un pasillo central de 2.5 m y cinco salas independientes más descanso. `ConstructorConsultorio.Salas.cs` distribuye el equipamiento, las actividades y sus zonas después de importar los modelos. Los recipientes locales permiten clasificar los desechables dentro de cada sala; los reutilizables se llevan a la autoclave de la sala 02.

Los menús de actualización regeneran la escena desde los constructores para evitar aplicar dos veces los desplazamientos. Conservan los assets de datos, pero reemplazan las modificaciones manuales a objetos de escena. Las pruebas recorren todas las puertas con el controlador real y comprueban que se activa la actividad correcta.
