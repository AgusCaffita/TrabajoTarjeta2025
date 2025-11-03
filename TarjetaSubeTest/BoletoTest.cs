using NUnit.Framework;
using TarjetaSube;

namespace TarjetaSubeTest
{
    [TestFixture]
    public class BoletoTest
    {
        [Test]
        public void TestCrearBoleto()
        {
            Boleto boleto = new Boleto("Normal", "120", 1580, 3420, 1);

            Assert.AreEqual("Normal", boleto.TipoTarjeta);
            Assert.AreEqual("120", boleto.LineaColectivo);
            Assert.AreEqual(1580, boleto.TotalAbonado);
            Assert.AreEqual(3420, boleto.SaldoRestante);
            Assert.AreEqual(1, boleto.IdTarjeta);
            Assert.IsNotNull(boleto.Fecha);
        }

        [Test]
        public void TestBoletoTieneFecha()
        {
            Boleto boleto = new Boleto("Normal", "115", 1580, 5000, 2);
            
            Assert.IsTrue(boleto.Fecha <= System.DateTime.Now);
            Assert.IsTrue(boleto.Fecha >= System.DateTime.Now.AddSeconds(-5));
        }

        [Test]
        public void TestPropiedadesBoletoSonCorrectas()
        {
            int totalAbonado = 1580;
            int saldoRestante = 8420;
            string linea = "133";
            string tipo = "Normal";
            int idTarjeta = 3;

            Boleto boleto = new Boleto(tipo, linea, totalAbonado, saldoRestante, idTarjeta);

            Assert.AreEqual(tipo, boleto.TipoTarjeta);
            Assert.AreEqual(linea, boleto.LineaColectivo);
            Assert.AreEqual(totalAbonado, boleto.TotalAbonado);
            Assert.AreEqual(saldoRestante, boleto.SaldoRestante);
            Assert.AreEqual(idTarjeta, boleto.IdTarjeta);
        }

        [Test]
        public void TestBoletoTieneIdTarjeta()
        {
            Tarjeta tarjeta = new Tarjeta();
            Boleto boleto = new Boleto("Tarjeta", "150", 1580, 0, tarjeta.Id);

            Assert.AreEqual(tarjeta.Id, boleto.IdTarjeta);
        }

        [Test]
        public void TestBoletoConTarjetaNormal()
        {
            Colectivo colectivo = new Colectivo("120");
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(5000);

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.AreEqual("Tarjeta", boleto.TipoTarjeta);
            Assert.AreEqual("120", boleto.LineaColectivo);
            Assert.AreEqual(1580, boleto.TotalAbonado);
            Assert.AreEqual(3420, boleto.SaldoRestante);
            Assert.AreEqual(tarjeta.Id, boleto.IdTarjeta);
        }

        [Test]
        public void TestBoletoConMedioBoleto()
        {
            Colectivo colectivo = new Colectivo("200");
            MedioBoleto tarjeta = new MedioBoleto();
            tarjeta.Cargar(2000);

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.AreEqual("MedioBoleto", boleto.TipoTarjeta);
            Assert.AreEqual("200", boleto.LineaColectivo);
            Assert.AreEqual(790, boleto.TotalAbonado); 
            Assert.AreEqual(1210, boleto.SaldoRestante);
            Assert.AreEqual(tarjeta.Id, boleto.IdTarjeta);
        }

        [Test]
        public void TestBoletoConBoletoGratuito()
        {
            Colectivo colectivo = new Colectivo("201");
            BoletoGratuito tarjeta = new BoletoGratuito();

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.AreEqual("BoletoGratuito", boleto.TipoTarjeta);
            Assert.AreEqual("201", boleto.LineaColectivo);
            Assert.AreEqual(0, boleto.TotalAbonado); 
            Assert.AreEqual(0, boleto.SaldoRestante);
            Assert.AreEqual(tarjeta.Id, boleto.IdTarjeta);
        }

        [Test]
        public void TestBoletoConFranquiciaCompleta()
        {
            Colectivo colectivo = new Colectivo("202");
            FranquiciaCompleta tarjeta = new FranquiciaCompleta();

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.AreEqual("FranquiciaCompleta", boleto.TipoTarjeta);
            Assert.AreEqual("202", boleto.LineaColectivo);
            Assert.AreEqual(0, boleto.TotalAbonado); 
            Assert.AreEqual(0, boleto.SaldoRestante);
            Assert.AreEqual(tarjeta.Id, boleto.IdTarjeta);
        }

        [Test]
        public void TestBoletoConSaldoNegativoPagaMas()
        {
            Colectivo colectivo = new Colectivo("150");
            Tarjeta tarjeta = new Tarjeta();
            
            tarjeta.Descontar(800);
            
            tarjeta.Cargar(2000);
            
            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.AreEqual(1580, boleto.TotalAbonado); 
            Assert.AreEqual(-380, boleto.SaldoRestante); 
        }

        [Test]
        public void TestBoletoConSaldoMuyNegativoPagaMasDeLaTarifa()
        {
            Colectivo colectivo = new Colectivo("160");
            Tarjeta tarjeta = new Tarjeta();

            tarjeta.Descontar(1200);

            tarjeta.Cargar(2000);
            
            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.AreEqual(1580, boleto.TotalAbonado);
            Assert.AreEqual(-780, boleto.SaldoRestante);
        }
    }
}