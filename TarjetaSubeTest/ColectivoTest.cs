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
            TiempoFalso tiempoFalso = new TiempoFalso();
            tiempoFalso.AgregarMinutos(360);
            Colectivo colectivo = new Colectivo("200", tiempoFalso);
            MedioBoleto tarjeta = new MedioBoleto();
            tarjeta.Cargar(2000);

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.AreEqual(2000 - 1580 / 2, tarjeta.ObtenerSaldo());
            Assert.AreEqual(790, boleto.TotalAbonado);
            Assert.AreEqual(tarjeta.Id, boleto.IdTarjeta);
        }

        [Test]
        public void TestMedioBoletoNoPuedeViajarAntesDe5Minutos()
        {
            TiempoFalso tiempoFalso = new TiempoFalso();
            tiempoFalso.AgregarMinutos(360); 
            Colectivo colectivo = new Colectivo("200", tiempoFalso);
            MedioBoleto tarjeta = new MedioBoleto();
            tarjeta.Cargar(5000);

            Boleto boleto1 = colectivo.PagarCon(tarjeta);
            Assert.IsNotNull(boleto1);
            Assert.AreEqual(790, boleto1.TotalAbonado);

            Boleto boleto2 = colectivo.PagarCon(tarjeta);
            Assert.IsNull(boleto2);

            tiempoFalso.AgregarMinutos(3);
            Boleto boleto3 = colectivo.PagarCon(tarjeta);
            Assert.IsNull(boleto3);

            tiempoFalso.AgregarMinutos(2);
            Boleto boleto4 = colectivo.PagarCon(tarjeta);
            Assert.IsNotNull(boleto4);
            Assert.AreEqual(790, boleto4.TotalAbonado);
        }

        [Test]
        public void TestMedioBoletoNoPermiteMasDeDosViajesPorDia()
        {
            TiempoFalso tiempoFalso = new TiempoFalso();
            tiempoFalso.AgregarMinutos(360); 
            Colectivo colectivo = new Colectivo("200", tiempoFalso);
            MedioBoleto tarjeta = new MedioBoleto();
            tarjeta.Cargar(10000);

            Boleto boleto1 = colectivo.PagarCon(tarjeta);
            Assert.IsNotNull(boleto1);
            Assert.AreEqual(790, boleto1.TotalAbonado);
            int saldoDespuesViaje1 = tarjeta.ObtenerSaldo();

            tiempoFalso.AgregarMinutos(5);

            Boleto boleto2 = colectivo.PagarCon(tarjeta);
            Assert.IsNotNull(boleto2);
            Assert.AreEqual(790, boleto2.TotalAbonado);
            int saldoDespuesViaje2 = tarjeta.ObtenerSaldo();

            tiempoFalso.AgregarMinutos(5);

            Boleto boleto3 = colectivo.PagarCon(tarjeta);
            Assert.IsNotNull(boleto3);
            Assert.AreEqual(1580, boleto3.TotalAbonado);
            int saldoDespuesViaje3 = tarjeta.ObtenerSaldo();

            Assert.AreEqual(saldoDespuesViaje1 - 790, saldoDespuesViaje2);
            Assert.AreEqual(saldoDespuesViaje2 - 1580, saldoDespuesViaje3);
        }

        [Test]
        public void TestMedioBoletoReiniciaContadorAlDiaSiguiente()
        {
            TiempoFalso tiempoFalso = new TiempoFalso();
            tiempoFalso.AgregarMinutos(360); 
            Colectivo colectivo = new Colectivo("200", tiempoFalso);
            MedioBoleto tarjeta = new MedioBoleto();
            tarjeta.Cargar(10000);

            Boleto boleto1 = colectivo.PagarCon(tarjeta);
            Assert.AreEqual(790, boleto1.TotalAbonado);

            tiempoFalso.AgregarMinutos(5);
            Boleto boleto2 = colectivo.PagarCon(tarjeta);
            Assert.AreEqual(790, boleto2.TotalAbonado);

            tiempoFalso.AgregarMinutos(5);
            Boleto boleto3 = colectivo.PagarCon(tarjeta);
            Assert.AreEqual(1580, boleto3.TotalAbonado);

            tiempoFalso.AgregarDias(1);

            Boleto boleto4 = colectivo.PagarCon(tarjeta);
            Assert.IsNotNull(boleto4);
            Assert.AreEqual(790, boleto4.TotalAbonado);
        }

        [Test]
        public void TestPagarConBoletoGratuito()
        {
            TiempoFalso tiempoFalso = new TiempoFalso();
            tiempoFalso.AgregarMinutos(360); 
            Colectivo colectivo = new Colectivo("201", tiempoFalso);
            BoletoGratuito tarjeta = new BoletoGratuito();

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.AreEqual(0, tarjeta.ObtenerSaldo());
            Assert.AreEqual(0, boleto.TotalAbonado);
            Assert.AreEqual(tarjeta.Id, boleto.IdTarjeta);
        }

        [Test]
        public void TestBoletoGratuitoNoPermiteMasDeDosViajesGratuitos()
        {
            TiempoFalso tiempoFalso = new TiempoFalso();
            tiempoFalso.AgregarMinutos(360);
            Colectivo colectivo = new Colectivo("201", tiempoFalso);
            BoletoGratuito tarjeta = new BoletoGratuito();
            tarjeta.Cargar(5000);

            Boleto boleto1 = colectivo.PagarCon(tarjeta);
            Assert.IsNotNull(boleto1);
            Assert.AreEqual(0, boleto1.TotalAbonado);
            Assert.AreEqual(5000, tarjeta.ObtenerSaldo());

            Boleto boleto2 = colectivo.PagarCon(tarjeta);
            Assert.IsNotNull(boleto2);
            Assert.AreEqual(0, boleto2.TotalAbonado);
            Assert.AreEqual(5000, tarjeta.ObtenerSaldo());

            Boleto boleto3 = colectivo.PagarCon(tarjeta);
            Assert.IsNotNull(boleto3);
            Assert.AreEqual(1580, boleto3.TotalAbonado);
            Assert.AreEqual(3420, tarjeta.ObtenerSaldo());
        }

        [Test]
        public void TestBoletoGratuitoViajesPosterioresAlSegundoSeCobran()
        {
            TiempoFalso tiempoFalso = new TiempoFalso();
            tiempoFalso.AgregarMinutos(360); 
            Colectivo colectivo = new Colectivo("201", tiempoFalso);
            BoletoGratuito tarjeta = new BoletoGratuito();
            tarjeta.Cargar(10000);

            int saldoInicial = tarjeta.ObtenerSaldo();

            Boleto boleto1 = colectivo.PagarCon(tarjeta);
            Assert.AreEqual(0, boleto1.TotalAbonado);
            Assert.AreEqual(saldoInicial, tarjeta.ObtenerSaldo());

            Boleto boleto2 = colectivo.PagarCon(tarjeta);
            Assert.AreEqual(0, boleto2.TotalAbonado);
            Assert.AreEqual(saldoInicial, tarjeta.ObtenerSaldo());

            Boleto boleto3 = colectivo.PagarCon(tarjeta);
            Assert.AreEqual(1580, boleto3.TotalAbonado);
            Assert.AreEqual(saldoInicial - 1580, tarjeta.ObtenerSaldo());

            Boleto boleto4 = colectivo.PagarCon(tarjeta);
            Assert.AreEqual(1580, boleto4.TotalAbonado);
            Assert.AreEqual(saldoInicial - 1580 - 1580, tarjeta.ObtenerSaldo());
        }

        [Test]
        public void TestBoletoGratuitoReiniciaContadorAlDiaSiguiente()
        {
            TiempoFalso tiempoFalso = new TiempoFalso();
            tiempoFalso.AgregarMinutos(360); 
            Colectivo colectivo = new Colectivo("201", tiempoFalso);
            BoletoGratuito tarjeta = new BoletoGratuito();
            tarjeta.Cargar(10000);

            Boleto boleto1 = colectivo.PagarCon(tarjeta);
            Assert.AreEqual(0, boleto1.TotalAbonado);

            Boleto boleto2 = colectivo.PagarCon(tarjeta);
            Assert.AreEqual(0, boleto2.TotalAbonado);

            Boleto boleto3 = colectivo.PagarCon(tarjeta);
            Assert.AreEqual(1580, boleto3.TotalAbonado);
            int saldoDespuesViaje3 = tarjeta.ObtenerSaldo();

            tiempoFalso.AgregarDias(1);

            Boleto boleto4 = colectivo.PagarCon(tarjeta);
            Assert.IsNotNull(boleto4);
            Assert.AreEqual(0, boleto4.TotalAbonado);
            Assert.AreEqual(saldoDespuesViaje3, tarjeta.ObtenerSaldo());
        }

        [Test]
        public void TestBoletoGratuitoSinSaldoNoPermiteTercerViaje()
        {
            TiempoFalso tiempoFalso = new TiempoFalso();
            tiempoFalso.AgregarMinutos(360); 
            Colectivo colectivo = new Colectivo("201", tiempoFalso);
            BoletoGratuito tarjeta = new BoletoGratuito();

            Boleto boleto1 = colectivo.PagarCon(tarjeta);
            Assert.IsNotNull(boleto1);
            Assert.AreEqual(0, boleto1.TotalAbonado);

            Boleto boleto2 = colectivo.PagarCon(tarjeta);
            Assert.IsNotNull(boleto2);
            Assert.AreEqual(0, boleto2.TotalAbonado);

            Boleto boleto3 = colectivo.PagarCon(tarjeta);
            Assert.IsNull(boleto3);
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
            TiempoFalso tiempoFalso = new TiempoFalso();
            tiempoFalso.AgregarMinutos(360); 
            Colectivo colectivo = new Colectivo("203", tiempoFalso);
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

        [Test]
        public void TestFranquiciaNoPermiteFueraDeFranjaHorario()
        {
            TiempoFalso tiempoFalso = new TiempoFalso();

            tiempoFalso.AgregarDias(5);
            tiempoFalso.AgregarMinutos(600);
            Colectivo colectivo = new Colectivo("300", tiempoFalso);
            FranquiciaCompleta tarjeta = new FranquiciaCompleta();

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNull(boleto);
        }

        [Test]
        public void TestLineaInterurbanaTarifaNormal()
        {
            Colectivo colectivo = new Colectivo("Rosario-Galvez", true);
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(5000);

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.AreEqual(3000, boleto.TotalAbonado);
            Assert.AreEqual(2000, boleto.SaldoRestante);
            Assert.AreEqual("Rosario-Galvez", boleto.LineaColectivo);
        }

        [Test]
        public void TestLineaInterurbanaMedioBoleto()
        {
            TiempoFalso tiempoFalso = new TiempoFalso();
            tiempoFalso.AgregarMinutos(360); // asegura horario de franquicia

            Colectivo colectivo = new Colectivo("Rosario-Baigorria", true, tiempoFalso);
            MedioBoleto tarjeta = new MedioBoleto();
            tarjeta.Cargar(5000);

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.AreEqual(1500, boleto.TotalAbonado);
            Assert.AreEqual(3500, boleto.SaldoRestante);
            Assert.AreEqual("MedioBoleto", boleto.TipoTarjeta);
        }

        [Test]
        public void TestLineaInterurbanaFranquiciaCompleta()
        {
            TiempoFalso tiempoFalso = new TiempoFalso();
            tiempoFalso.AgregarMinutos(360);

            Colectivo colectivo = new Colectivo("Rosario-Galvez", true, tiempoFalso);
            FranquiciaCompleta tarjeta = new FranquiciaCompleta();

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.AreEqual(0, boleto.TotalAbonado);
            Assert.AreEqual(0, boleto.SaldoRestante);
        }

        [Test]
        public void TestLineaInterurbanaBoletoGratuito()
        {
            TiempoFalso tiempoFalso = new TiempoFalso();
            tiempoFalso.AgregarMinutos(360);

            Colectivo colectivo = new Colectivo("Rosario-Baigorria", true, tiempoFalso);
            BoletoGratuito tarjeta = new BoletoGratuito();

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.AreEqual(0, boleto.TotalAbonado);
            Assert.AreEqual(0, boleto.SaldoRestante);
        }

        [Test]
        public void TestTrasbordoValido()
        {
            TiempoFalso tiempo = new TiempoFalso();
            tiempo.AgregarMinutos(480); // 08:00, lunes
            Colectivo lineaA = new Colectivo("A", tiempo);
            Colectivo lineaB = new Colectivo("B", tiempo);
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(5000);

            Boleto primerBoleto = lineaA.PagarCon(tarjeta);
            Assert.IsNotNull(primerBoleto);
            Assert.AreEqual(1580, primerBoleto.TotalAbonado);

            tiempo.AgregarMinutos(30); // dentro de la ventana de 1 hora
            Boleto segundoBoleto = lineaB.PagarCon(tarjeta);
            Assert.IsNotNull(segundoBoleto);
            Assert.IsTrue(segundoBoleto.EsTrasbordo);
            Assert.AreEqual(0, segundoBoleto.TotalAbonado);
            Assert.AreEqual(5000 - 1580, tarjeta.ObtenerSaldo()); // saldo no cambia por trasbordo
        }

        [Test]
        public void TestTrasbordoMismaLineaNoValido()
        {
            TiempoFalso tiempo = new TiempoFalso();
            tiempo.AgregarMinutos(480); // 08:00
            Colectivo lineaA1 = new Colectivo("A", tiempo);
            Colectivo lineaA2 = new Colectivo("A", tiempo);
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(5000);

            Boleto b1 = lineaA1.PagarCon(tarjeta);
            Assert.IsNotNull(b1);
            Assert.AreEqual(1580, b1.TotalAbonado);

            tiempo.AgregarMinutos(30);
            Boleto b2 = lineaA2.PagarCon(tarjeta);
            Assert.IsNotNull(b2);
            Assert.IsFalse(b2.EsTrasbordo);
            Assert.AreEqual(1580, b2.TotalAbonado);
            Assert.AreEqual(5000 - 1580 - 1580, tarjeta.ObtenerSaldo());
        }

        [Test]
        public void TestTrasbordoFueraDeHorarioNoValido()
        {
            TiempoFalso tiempo = new TiempoFalso();
            tiempo.AgregarMinutos(480); // 08:00
            Colectivo lineaA = new Colectivo("A", tiempo);
            Colectivo lineaB = new Colectivo("B", tiempo);
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(5000);

            Boleto b1 = lineaA.PagarCon(tarjeta);
            Assert.IsNotNull(b1);
            Assert.AreEqual(1580, b1.TotalAbonado);

            tiempo.AgregarMinutos(900); // ahora 23:00, fuera de la franja 7:00-22:00
            Boleto b2 = lineaB.PagarCon(tarjeta);
            Assert.IsNotNull(b2);
            Assert.IsFalse(b2.EsTrasbordo);
            Assert.AreEqual(1580, b2.TotalAbonado);
            Assert.AreEqual(5000 - 1580 - 1580, tarjeta.ObtenerSaldo());
        }

        [Test]
        public void TestTrasbordoDespuesDeUnaHoraNoValido()
        {
            TiempoFalso tiempo = new TiempoFalso();
            tiempo.AgregarMinutos(480); // 08:00
            Colectivo lineaA = new Colectivo("A", tiempo);
            Colectivo lineaB = new Colectivo("B", tiempo);
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(5000);

            Boleto b1 = lineaA.PagarCon(tarjeta);
            Assert.IsNotNull(b1);

            tiempo.AgregarMinutos(61); // 1 hora y 1 minuto después
            Boleto b2 = lineaB.PagarCon(tarjeta);
            Assert.IsNotNull(b2);
            Assert.IsFalse(b2.EsTrasbordo);
            Assert.AreEqual(1580, b2.TotalAbonado);
            Assert.AreEqual(5000 - 1580 - 1580, tarjeta.ObtenerSaldo());
        }

        [Test]
        public void TestTrasbordoConMedioBoleto()
        {
            TiempoFalso tiempo = new TiempoFalso();
            tiempo.AgregarMinutos(480); // 08:00
            Colectivo lineaA = new Colectivo("A", tiempo);
            Colectivo lineaB = new Colectivo("B", tiempo);
            MedioBoleto medio = new MedioBoleto();
            medio.Cargar(2000);

            Boleto b1 = lineaA.PagarCon(medio);
            Assert.IsNotNull(b1);
            Assert.AreEqual(790, b1.TotalAbonado);

            tiempo.AgregarMinutos(30);
            Boleto b2 = lineaB.PagarCon(medio);
            Assert.IsNotNull(b2);
            Assert.IsTrue(b2.EsTrasbordo);
            Assert.AreEqual(0, b2.TotalAbonado);
            Assert.AreEqual(2000 - 790, medio.ObtenerSaldo());
        }
    }
}
