using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace ConsultorioSeguro.Editor
{
    // Todo recurso descargado va en su propia carpeta dentro de Assets/Terceros/
    // junto con una Atribucion (Create > Consultorio Seguro > Atribución).
    // Estas herramientas mantienen el catálogo de créditos al día, avisan de las
    // carpetas sin fuente y exportan los créditos para la documentación.
    public static class HerramientasAtribucion
    {
        public const string CarpetaTerceros = "Assets/Terceros";
        public const string RutaCatalogo = "Assets/Datos/CatalogoAtribuciones.asset";

        [MenuItem("Consultorio Seguro/Atribuciones/Actualizar catálogo de créditos")]
        public static CatalogoAtribuciones ActualizarCatalogo()
        {
            CatalogoAtribuciones catalogo = AssetDatabase.LoadAssetAtPath<CatalogoAtribuciones>(RutaCatalogo);
            if (catalogo == null)
            {
                ConstructorConsultorio.AsegurarCarpeta(Path.GetDirectoryName(RutaCatalogo));
                catalogo = ScriptableObject.CreateInstance<CatalogoAtribuciones>();
                AssetDatabase.CreateAsset(catalogo, RutaCatalogo);
            }

            List<Atribucion> todas = BuscarAtribuciones();
            if (!catalogo.atribuciones.SequenceEqual(todas))
            {
                catalogo.atribuciones = todas;
                EditorUtility.SetDirty(catalogo);
                AssetDatabase.SaveAssetIfDirty(catalogo);
            }

            return catalogo;
        }

        [MenuItem("Consultorio Seguro/Atribuciones/Revisar recursos sin atribución")]
        static void RevisarDesdeMenu()
        {
            List<string> problemas = Problemas();
            if (problemas.Count == 0)
                Debug.Log("Todos los recursos de Assets/Terceros tienen su atribución completa.");
            foreach (string problema in problemas)
                Debug.LogWarning(problema);
        }

        // Carpetas sin Atribucion, archivos sueltos y atribuciones con campos vacíos.
        public static List<string> Problemas()
        {
            List<string> problemas = new();
            if (!AssetDatabase.IsValidFolder(CarpetaTerceros))
                return problemas;

            foreach (string carpeta in AssetDatabase.GetSubFolders(CarpetaTerceros))
            {
                if (AssetDatabase.FindAssets("t:" + nameof(Atribucion), new[] { carpeta }).Length == 0)
                    problemas.Add($"{carpeta} no tiene una Atribucion. Créala con Create > Consultorio Seguro > Atribución.");
            }

            foreach (string archivo in Directory.GetFiles(CarpetaTerceros))
            {
                string nombre = Path.GetFileName(archivo);
                if (!nombre.EndsWith(".meta") && !nombre.StartsWith(".") && nombre != "LEEME.md")
                    problemas.Add($"{CarpetaTerceros}/{nombre} está suelto: muévelo a una carpeta propia con su Atribucion.");
            }

            foreach (Atribucion atribucion in BuscarAtribuciones())
            {
                if (string.IsNullOrWhiteSpace(atribucion.titulo) || string.IsNullOrWhiteSpace(atribucion.autor) ||
                    string.IsNullOrWhiteSpace(atribucion.url) || string.IsNullOrWhiteSpace(atribucion.licencia))
                    problemas.Add($"{AssetDatabase.GetAssetPath(atribucion)} tiene campos vacíos (título, autor, URL o licencia).");
            }

            return problemas;
        }

        [MenuItem("Consultorio Seguro/Atribuciones/Exportar créditos (CREDITOS.md y creditos.bib)")]
        public static void Exportar()
        {
            List<Atribucion> atribuciones = BuscarAtribuciones();
            File.WriteAllText("CREDITOS.md", Markdown(atribuciones));
            File.WriteAllText("creditos.bib", BibTex(atribuciones));
            Debug.Log($"Créditos exportados: {atribuciones.Count} recursos en CREDITOS.md y creditos.bib.");
        }

        static List<Atribucion> BuscarAtribuciones() =>
            AssetDatabase.FindAssets("t:" + nameof(Atribucion))
                .Select(guid => AssetDatabase.LoadAssetAtPath<Atribucion>(AssetDatabase.GUIDToAssetPath(guid)))
                .Where(atribucion => atribucion != null)
                .OrderBy(atribucion => atribucion.titulo, System.StringComparer.CurrentCultureIgnoreCase)
                .ToList();

        static string Markdown(List<Atribucion> atribuciones)
        {
            StringBuilder texto = new();
            texto.AppendLine("# Créditos de recursos de terceros");
            texto.AppendLine();
            texto.AppendLine("Archivo generado desde Unity con *Consultorio Seguro > Atribuciones > Exportar créditos*. No lo edites a mano: edita la Atribucion de cada recurso y vuelve a exportar.");
            texto.AppendLine();

            if (atribuciones.Count == 0)
            {
                texto.AppendLine("Todavía no se han agregado recursos de terceros.");
                return texto.ToString();
            }

            texto.AppendLine("| Recurso | Autor | Licencia | Fuente | Consultado | Modificaciones |");
            texto.AppendLine("| --- | --- | --- | --- | --- | --- |");
            foreach (Atribucion a in atribuciones)
                texto.AppendLine($"| {Celda(a.titulo)} | {Celda(a.autor)} | {Celda(a.licencia)} | <{a.url}> | {Celda(a.fechaConsulta)} | {Celda(a.modificaciones)} |");

            return texto.ToString();
        }

        static string Celda(string valor) =>
            string.IsNullOrWhiteSpace(valor) ? "—" : valor.Replace("|", "\\|").Replace("\n", " ");

        static string BibTex(List<Atribucion> atribuciones)
        {
            StringBuilder texto = new();
            HashSet<string> claves = new();

            foreach (Atribucion a in atribuciones)
            {
                string clave = Clave(a);
                for (int i = 2; !claves.Add(clave); i++)
                    clave = Clave(a) + i;

                texto.AppendLine($"@misc{{{clave},");
                texto.AppendLine($"  author = {{{a.autor}}},");
                texto.AppendLine($"  title = {{{a.titulo}}},");
                texto.AppendLine($"  url = {{{a.url}}},");
                if (!string.IsNullOrWhiteSpace(a.fechaConsulta))
                    texto.AppendLine($"  urldate = {{{a.fechaConsulta}}},");
                texto.AppendLine($"  note = {{Licencia: {a.licencia}}},");
                texto.AppendLine("}");
                texto.AppendLine();
            }

            return texto.ToString();
        }

        // Sigue el formato autorAñoPalabra de las referencias del repositorio, sin año
        // porque los sitios de modelos casi nunca lo publican.
        static string Clave(Atribucion a)
        {
            string autor = Palabras(a.autor).FirstOrDefault() ?? "anonimo";
            string titulo = Palabras(a.titulo).FirstOrDefault() ?? "recurso";
            return autor.ToLowerInvariant() + char.ToUpperInvariant(titulo[0]) + titulo[1..].ToLowerInvariant();
        }

        static IEnumerable<string> Palabras(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                yield break;

            string sinAcentos = new(texto.Normalize(NormalizationForm.FormD)
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray());

            foreach (string palabra in sinAcentos.Split(' ', '-', '_', '.', ','))
            {
                string limpia = new(palabra.Where(char.IsLetterOrDigit).ToArray());
                if (limpia.Length > 0)
                    yield return limpia;
            }
        }
    }

    class ProcesadorAtribuciones : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importados, string[] borrados, string[] movidos, string[] origenes)
        {
            bool relevante = importados.Concat(borrados).Concat(movidos).Concat(origenes)
                .Any(ruta => ruta.StartsWith(HerramientasAtribucion.CarpetaTerceros + "/") ||
                             AssetDatabase.GetMainAssetTypeAtPath(ruta) == typeof(Atribucion));
            if (!relevante)
                return;

            // Fuera del import para no modificar assets mientras se importan.
            EditorApplication.delayCall -= Revisar;
            EditorApplication.delayCall += Revisar;
        }

        static void Revisar()
        {
            HerramientasAtribucion.ActualizarCatalogo();
            foreach (string problema in HerramientasAtribucion.Problemas())
                Debug.LogWarning(problema);
        }
    }

    // El profesor pide citar la fuente de cada modelo: sin atribución no se compila.
    class VerificacionAtribucionesAlCompilar : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport reporte)
        {
            HerramientasAtribucion.ActualizarCatalogo();
            List<string> problemas = HerramientasAtribucion.Problemas();
            if (problemas.Count > 0)
                throw new BuildFailedException("Hay recursos sin atribución:\n" + string.Join("\n", problemas));

            HerramientasAtribucion.Exportar();
        }
    }
}
