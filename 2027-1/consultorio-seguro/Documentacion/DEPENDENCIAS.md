# Dependencias y recursos distribuidos con Unity

Las 51 entradas de `catalogo-recursos.json` corresponden a los recursos de `Assets/Terceros`. Las dependencias de software y los recursos esenciales importados de Unity se documentan aquí por separado; no se suman a ese recuento.

| Dependencia | Versión del proyecto | Autor / distribuidor | Licencia del paquete |
| --- | --- | --- | --- |
| [Universal Render Pipeline](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@17.4/manual/index.html) | 17.4.0 | Unity Technologies ApS | Unity Companion License |
| [Input System](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.19/manual/index.html) | 1.19.0 | Unity Technologies | Unity Companion License |
| [uGUI y TextMesh Pro](https://docs.unity3d.com/Packages/com.unity.ugui@2.0/manual/index.html) | 2.0.0 | Unity Technologies ApS | Unity Companion License; fuente Liberation Sans bajo OFL 1.1 |
| [Unity glTFast](https://docs.unity3d.com/Packages/com.unity.cloud.gltfast@6.20/manual/index.html) | 6.20.0 | Unity Technologies y autores de Unity glTFast | Apache License 2.0 |
| [Unity Test Framework](https://docs.unity3d.com/Packages/com.unity.test-framework@1.6/manual/index.html) | 1.6.0 | Unity Technologies ApS | Unity Companion License |

Versiones declaradas en `Packages/manifest.json`; dependencias transitivas registradas en `Packages/packages-lock.json`. Paquetes originales conservados sin cambios. Unity 6000.4.6f1 aporta los componentes integrados del motor y la fuente de interfaz `LegacyRuntime.ttf`, utilizados mediante sus API.

## Recursos esenciales de TextMesh Pro

**Fuente:** `TMP Essential Resources.unitypackage` del paquete instalado `com.unity.ugui` 2.0.0, [documentación oficial](https://docs.unity3d.com/Packages/com.unity.ugui@2.0/manual/index.html). **Autor:** Unity Technologies ApS. **Licencia:** [Unity Companion License](https://unity.com/legal/licenses/unity-companion-license), conservada también en `Assets/TextMesh Pro/LICENSE-Unity.md`.

**Modificaciones:** importación de 33 recursos del paquete para ajustes, estilos, shaders y fuentes de respaldo; los 33 coinciden byte por byte con el archivo de distribución. La comparación SHA-256 y cada ruta están en `procedencia-dependencias.json`. El atlas de la señalización Noto Sans se genera por separado en `Assets/ConsultorioSeguro/Generados/Tipografía clínica.asset`.

## Liberation Sans

Incluida en el paquete de Unity, con su TTF, atlas SDF y materiales de respaldo. **Autoría según el aviso distribuido:** datos digitalizados © 2010 Google Corporation; © 2012 Red Hat, Inc. **Fuente original:** [Liberation Fonts](https://github.com/liberationfonts/liberation-fonts). **Licencia:** [SIL Open Font License 1.1](https://github.com/liberationfonts/liberation-fonts/blob/main/LICENSE), cuyo texto exacto acompaña al archivo en `Assets/TextMesh Pro/Fonts/LiberationSans - OFL.txt`.

**Modificaciones:** ninguna en la fuente ni en los atlas importados. La fuente se conserva como dependencia de respaldo; los rótulos de la clínica usan Noto Sans y la interfaz de menús usa la fuente integrada LegacyRuntime de Unity.
