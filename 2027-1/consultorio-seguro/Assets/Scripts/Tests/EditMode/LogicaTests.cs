using NUnit.Framework;
using UnityEngine;

namespace ConsultorioSeguro.Tests
{
    public class PuntuacionTests
    {
        [Test]
        public void SumaAciertosYRestaErrores()
        {
            Puntuacion puntuacion = new(10, 5);
            puntuacion.Registrar(true);
            puntuacion.Registrar(true);
            puntuacion.Registrar(false);

            Assert.AreEqual(2, puntuacion.Aciertos);
            Assert.AreEqual(1, puntuacion.Errores);
            Assert.AreEqual(15, puntuacion.Puntos);
        }

        [Test]
        public void NuncaEsNegativa()
        {
            Puntuacion puntuacion = new(10, 5);
            puntuacion.Registrar(false);
            puntuacion.Registrar(false);

            Assert.AreEqual(0, puntuacion.Puntos);
        }

        [Test]
        public void ReiniciarBorraElMarcador()
        {
            Puntuacion puntuacion = new(10, 5);
            puntuacion.Registrar(true);
            puntuacion.Registrar(false);
            puntuacion.Reiniciar();

            Assert.AreEqual(0, puntuacion.Aciertos);
            Assert.AreEqual(0, puntuacion.Errores);
        }

        [Test]
        public void PuntosMaximosDependenDelTotal() => Assert.AreEqual(60, new Puntuacion(10, 5).PuntosMaximos(6));
    }

    public class ClasificadorTests
    {
        DatosResiduo gasa;

        [SetUp]
        public void Preparar()
        {
            gasa = ScriptableObject.CreateInstance<DatosResiduo>();
            gasa.nombre = "Gasa empapada de sangre";
            gasa.destinoCorrecto = Destino.BolsaRoja;
            gasa.explicacion = "Es residuo no anatómico.";
        }

        [TearDown]
        public void Limpiar() => Object.DestroyImmediate(gasa);

        [Test]
        public void AciertaConElDestinoCorrecto()
        {
            ResultadoClasificacion resultado = Clasificador.Evaluar(gasa, Destino.BolsaRoja);

            Assert.IsTrue(resultado.Correcto);
            StringAssert.Contains("Bolsa roja", resultado.Mensaje);
            StringAssert.Contains(gasa.explicacion, resultado.Mensaje);
        }

        [Test]
        public void ExplicaElErrorConElDestinoElegido()
        {
            ResultadoClasificacion resultado = Clasificador.Evaluar(gasa, Destino.BasuraComun);

            Assert.IsFalse(resultado.Correcto);
            StringAssert.Contains("no va en: Basura común", resultado.Mensaje);
            StringAssert.Contains(gasa.explicacion, resultado.Mensaje);
        }

        [Test]
        public void SoloBasuraComunYEsterilizacionNoSonRpbi()
        {
            foreach (Destino destino in System.Enum.GetValues(typeof(Destino)))
            {
                bool esperado = destino is not (Destino.BasuraComun or Destino.Esterilizacion);
                Assert.AreEqual(esperado, destino.EsRpbi(), destino.ToString());
                Assert.IsNotEmpty(destino.Descripcion(), destino.ToString());
            }
        }
    }
}
