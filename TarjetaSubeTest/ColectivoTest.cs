using NUnit.Framework;
using TarjetaSube;

namespace TarjetaSubeTest
{
    [TestFixture]
    public class ColectivoTest
    {
        [Test]
        public void TestCrearColectivo()
        {
            Colectivo colectivo = new Colectivo("120");
            Assert.AreEqual("120", colectivo.Linea);
        }

        [Test]
        public void TestPagarConSaldoSuficiente()
        {
            Colectivo colectivo = new Colectivo("120");
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(5000);

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.AreEqual(3420, tarjeta.ObtenerSaldo());
            Assert.AreEqual(1580, boleto.TotalAbonado);
            Assert.AreEqual(3420, boleto.SaldoRestante);
            Assert.AreEqual("120", boleto.LineaColectivo);
            Assert.AreEqual("Tarjeta", boleto.TipoTarjeta);
            Assert.AreEqual(tarjeta.Id, boleto.IdTarjeta);
        }

        [Test]
        public void TestPagarConTarjetaSinSaldo()
        {
            Colectivo colectivo = new Colectivo("102");
            Tarjeta tarjeta = new Tarjeta();

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNull(boleto);
            Assert.AreEqual(0, tarjeta.ObtenerSaldo());
        }

        [Test]
        public void TestMultiplesPagos()
        {
            Colectivo colectivo = new Colectivo("115");
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(10000);

            Boleto boleto1 = colectivo.PagarCon(tarjeta);
            Assert.IsNotNull(boleto1);
            Assert.AreEqual(8420, tarjeta.ObtenerSaldo());

            Boleto boleto2 = colectivo.PagarCon(tarjeta);
            Assert.IsNotNull(boleto2);
            Assert.AreEqual(6840, tarjeta.ObtenerSaldo());
        }

        [Test]
        public void TestPagarConMedioBoleto()
        {
            Colectivo colectivo = new Colectivo("200");
            MedioBoleto tarjeta = new MedioBoleto();
            tarjeta.Cargar(2000);

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.AreEqual(2000 - 1580 / 2, tarjeta.ObtenerSaldo());
            Assert.AreEqual(790, boleto.TotalAbonado);
            Assert.AreEqual(tarjeta.Id, boleto.IdTarjeta);
        }

        [Test]
        public void TestPagarConBoletoGratuito()
        {
            Colectivo colectivo = new Colectivo("201");
            BoletoGratuito tarjeta = new BoletoGratuito();

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.AreEqual(0, tarjeta.ObtenerSaldo());
            Assert.AreEqual(0, boleto.TotalAbonado);
            Assert.AreEqual(tarjeta.Id, boleto.IdTarjeta);
        }

        [Test]
        public void TestPagarConFranquiciaCompleta()
        {
            Colectivo colectivo = new Colectivo("202");
            FranquiciaCompleta tarjeta = new FranquiciaCompleta();

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.AreEqual(0, tarjeta.ObtenerSaldo());
            Assert.AreEqual(0, boleto.TotalAbonado);
            Assert.AreEqual(tarjeta.Id, boleto.IdTarjeta);
        }

        [Test]
        public void TestTipoTarjetaRegistradoEnBoleto()
        {
            Colectivo colectivo = new Colectivo("203");
            MedioBoleto tarjeta = new MedioBoleto();
            tarjeta.Cargar(2000);

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.AreEqual("MedioBoleto", boleto.TipoTarjeta);
        }

        [Test]
        public void TestBoletoRetornadoContieneTodosLosDatos()
        {
            Colectivo colectivo = new Colectivo("150");
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(5000);

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.AreEqual("Tarjeta", boleto.TipoTarjeta);
            Assert.AreEqual("150", boleto.LineaColectivo);
            Assert.AreEqual(1580, boleto.TotalAbonado);
            Assert.AreEqual(3420, boleto.SaldoRestante);
            Assert.AreEqual(tarjeta.Id, boleto.IdTarjeta);
            Assert.IsNotNull(boleto.Fecha);
        }
    }
}