using NUnit.Framework;
using TarjetaSube;

namespace TarjetaSubeTest
{
    [TestFixture]
    public class TarjetaTest
    {
        [Test]
        public void TestTarjetaNuevaTieneSaldoCero()
        {
            Tarjeta tarjeta = new Tarjeta();
            Assert.AreEqual(0, tarjeta.ObtenerSaldo());
        }

        [Test]
        public void TestTarjetaTieneIdUnico()
        {
            Tarjeta tarjeta1 = new Tarjeta();
            Tarjeta tarjeta2 = new Tarjeta();

            Assert.AreNotEqual(tarjeta1.Id, tarjeta2.Id);
            Assert.Greater(tarjeta1.Id, 0);
            Assert.Greater(tarjeta2.Id, 0);
        }

        [Test]
        public void TestIdSeIncrementaAutomaticamente()
        {
            Tarjeta tarjeta1 = new Tarjeta();
            int id1 = tarjeta1.Id;

            Tarjeta tarjeta2 = new Tarjeta();
            int id2 = tarjeta2.Id;

            Assert.AreEqual(id1 + 1, id2);
        }

        [Test]
        [TestCase(2000)]
        [TestCase(3000)]
        [TestCase(4000)]
        [TestCase(5000)]
        [TestCase(8000)]
        [TestCase(10000)]
        [TestCase(15000)]
        [TestCase(20000)]
        [TestCase(25000)]
        [TestCase(30000)]
        public void TestCargasValidas(int monto)
        {
            Tarjeta tarjeta = new Tarjeta();
            bool resultado = tarjeta.Cargar(monto);
            Assert.IsTrue(resultado);
            Assert.AreEqual(monto, tarjeta.ObtenerSaldo());
        }

        [Test]
        public void TestCargaInvalida()
        {
            Tarjeta tarjeta = new Tarjeta();
            bool resultado = tarjeta.Cargar(1000);
            Assert.IsFalse(resultado);
            Assert.AreEqual(0, tarjeta.ObtenerSaldo());
        }

        [Test]
        public void TestCargasAcumuladas()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(5000);
            tarjeta.Cargar(3000);
            Assert.AreEqual(8000, tarjeta.ObtenerSaldo());
        }

        [Test]
        public void TestLimiteDeSaldo()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000); 

            Assert.AreEqual(56000, tarjeta.ObtenerSaldo());
            Assert.Greater(tarjeta.SaldoPendiente, 0); 
        }


        [Test]
        public void TestDescontarConSaldoSuficiente()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(5000);
            bool resultado = tarjeta.Descontar(1580);
            Assert.IsTrue(resultado);
            Assert.AreEqual(3420, tarjeta.ObtenerSaldo());
        }

        [Test]
        public void TestDescontarConSaldoInsuficientePeroDentroDelLimiteNegativo()
        {
            Tarjeta tarjeta = new Tarjeta();
            bool resultado = tarjeta.Descontar(1000);
            Assert.IsTrue(resultado);
            Assert.AreEqual(-1000, tarjeta.ObtenerSaldo());
        }

        [Test]
        public void TestNoPermitirPasarDelLimiteNegativo()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Descontar(1200);
            bool resultado = tarjeta.Descontar(1);
            Assert.IsFalse(resultado);
            Assert.AreEqual(-1200, tarjeta.ObtenerSaldo());
        }

        [Test]
        public void TestRecuperarSaldoNegativoAlCargar()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Descontar(1200);
            tarjeta.Cargar(2000);
            Assert.AreEqual(800, tarjeta.ObtenerSaldo());
        }

        [Test]
        public void TestPagarTarifaConColectivo()
        {
            TiempoFalso tiempo = new TiempoFalso();
            Tarjeta tarjeta = new Tarjeta();
            Colectivo colectivo = new Colectivo("152", tiempo);
            
            tarjeta.Cargar(2000);
            Boleto boleto = colectivo.PagarCon(tarjeta);
            
            Assert.IsNotNull(boleto);
            Assert.AreEqual(1580, boleto.TotalAbonado); 
            Assert.AreEqual(420, tarjeta.ObtenerSaldo());
        }

        [Test]
        public void TestMedioBoletoDescuentaMitad()
        {
            TiempoFalso tiempo = new TiempoFalso();
            tiempo.AgregarMinutos(360);
            MedioBoleto tarjeta = new MedioBoleto();
            Colectivo colectivo = new Colectivo("152", tiempo);
            
            tarjeta.Cargar(2000);
            Boleto boleto = colectivo.PagarCon(tarjeta);
            
            Assert.IsNotNull(boleto);
            Assert.AreEqual(790, boleto.TotalAbonado);
            Assert.AreEqual(2000 - 790, tarjeta.ObtenerSaldo());
        }

        [Test]
        public void TestMedioBoletoTieneIdUnico()
        {
            MedioBoleto tarjeta1 = new MedioBoleto();
            MedioBoleto tarjeta2 = new MedioBoleto();

            Assert.AreNotEqual(tarjeta1.Id, tarjeta2.Id);
        }

        [Test]
        public void TestBoletoGratuitoNoDescuenta()
        {
            TiempoFalso tiempoFalso = new TiempoFalso();
            tiempoFalso.AgregarMinutos(360); 
            BoletoGratuito tarjeta = new BoletoGratuito();
            tarjeta.Cargar(2000);

            bool resultado = tarjeta.DescontarSegunViajes(tiempoFalso);
            Assert.IsTrue(resultado);
            Assert.AreEqual(2000, tarjeta.ObtenerSaldo());
        }

        [Test]
        public void TestBoletoGratuitoTercerViajeDescuentaTarifaCompleta()
        {
            TiempoFalso tiempoFalso = new TiempoFalso();
            tiempoFalso.AgregarMinutos(360); 
            BoletoGratuito tarjeta = new BoletoGratuito();
            tarjeta.Cargar(5000);

            tarjeta.DescontarSegunViajes(tiempoFalso);
            tarjeta.RegistrarViaje(tiempoFalso);

            tarjeta.DescontarSegunViajes(tiempoFalso);
            tarjeta.RegistrarViaje(tiempoFalso);

            bool resultado = tarjeta.DescontarSegunViajes(tiempoFalso);
            Assert.IsTrue(resultado);
            Assert.AreEqual(3420, tarjeta.ObtenerSaldo());
        }

        [Test]
        public void TestBoletoGratuitoTieneIdUnico()
        {
            BoletoGratuito tarjeta1 = new BoletoGratuito();
            BoletoGratuito tarjeta2 = new BoletoGratuito();

            Assert.AreNotEqual(tarjeta1.Id, tarjeta2.Id);
        }

        [Test]
        public void TestFranquiciaCompletaSiemprePuedePagar()
        {
            TiempoFalso tiempo = new TiempoFalso();
            tiempo.AgregarMinutos(360); 
            FranquiciaCompleta tarjeta = new FranquiciaCompleta();
            Colectivo colectivo = new Colectivo("152", tiempo);
            
            tarjeta.Cargar(0);
            Boleto boleto = colectivo.PagarCon(tarjeta);
            
            Assert.IsNotNull(boleto);
            Assert.AreEqual(0, boleto.TotalAbonado); 
            Assert.AreEqual(0, tarjeta.ObtenerSaldo());
        }

        [Test]
        public void TestFranquiciaCompletaTieneIdUnico()
        {
            FranquiciaCompleta tarjeta1 = new FranquiciaCompleta();
            FranquiciaCompleta tarjeta2 = new FranquiciaCompleta();

            Assert.AreNotEqual(tarjeta1.Id, tarjeta2.Id);
        }

        [Test]
        public void TestCargaConExcesoGeneraSaldoPendiente()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000);
            Assert.AreEqual(56000, tarjeta.ObtenerSaldo());
            Assert.Greater(tarjeta.SaldoPendiente, 0);
        }

        [Test]
        public void TestAcreditarCargaCuandoSeDescuenta()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000);
            int saldoPendienteInicial = tarjeta.SaldoPendiente;

            tarjeta.Descontar(1580);

            Assert.Less(tarjeta.SaldoPendiente, saldoPendienteInicial);
            Assert.LessOrEqual(tarjeta.ObtenerSaldo(), 56000);
        }

        [Test]
        public void TestAcreditarCargaVariasVecesHastaLimite()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000);

            for (int i = 0; i < 10; i++)
                tarjeta.Descontar(1580);

            Assert.LessOrEqual(tarjeta.SaldoPendiente, 0);
            Assert.LessOrEqual(tarjeta.ObtenerSaldo(), 56000);
        }


        [Test]
        public void TestCargaQueSuperaElMaximoDejaSaldoPendiente()
        {
            Tarjeta tarjeta = new Tarjeta();

            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000); 

            Assert.AreEqual(56000, tarjeta.ObtenerSaldo());

            tarjeta.Descontar(2000);
            tarjeta.AcreditarCarga();

            Assert.AreEqual(56000, tarjeta.ObtenerSaldo());
        }

        [Test]
        public void TestSaldoPendienteSeAcreditaDespuesDeUnViaje()
        {
            Tarjeta tarjeta = new Tarjeta();

            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000); 

            tarjeta.Descontar(2000); 

            Assert.AreEqual(56000, tarjeta.ObtenerSaldo()); 
        }

        [Test]
        public void TestBoletoUsoFrecuente_NoDescuentoHasta29()
        {
            TiempoFalso tiempo = new TiempoFalso();
            Tarjeta tarjeta = new Tarjeta();
            Colectivo colectivo = new Colectivo("152", tiempo);

            for (int i = 0; i < 28; i++)
                tarjeta.RegistrarViaje(tiempo);

            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000); 

            Boleto boleto = colectivo.PagarCon(tarjeta);
            Assert.IsNotNull(boleto);
            Assert.AreEqual(1580, boleto.TotalAbonado); 
        }

        [Test]
        public void TestBoletoUsoFrecuente_30avoViaje_20PorCiento()
        {
            TiempoFalso tiempo = new TiempoFalso();
            Tarjeta tarjeta = new Tarjeta();
            Colectivo colectivo = new Colectivo("152", tiempo);

            for (int i = 0; i < 29; i++)
                tarjeta.RegistrarViaje(tiempo);

            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000);

            Boleto boleto = colectivo.PagarCon(tarjeta);
            Assert.IsNotNull(boleto);
            Assert.AreEqual(1264, boleto.TotalAbonado); 
        }

        [Test]
        public void TestBoletoUsoFrecuente_60avoViaje_25PorCiento()
        {
            TiempoFalso tiempo = new TiempoFalso();
            Tarjeta tarjeta = new Tarjeta();
            Colectivo colectivo = new Colectivo("152", tiempo);

            for (int i = 0; i < 59; i++)
                tarjeta.RegistrarViaje(tiempo);

            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000);

            Boleto boleto = colectivo.PagarCon(tarjeta);
            Assert.IsNotNull(boleto);
            Assert.AreEqual(1185, boleto.TotalAbonado); 
        }

        [Test]
        public void TestBoletoUsoFrecuente_81EnAdelante_NoDescuento()
        {
            TiempoFalso tiempo = new TiempoFalso();
            Tarjeta tarjeta = new Tarjeta();
            Colectivo colectivo = new Colectivo("152", tiempo);

            for (int i = 0; i < 80; i++)
                tarjeta.RegistrarViaje(tiempo);

            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000);

            Boleto boleto = colectivo.PagarCon(tarjeta);
            Assert.IsNotNull(boleto);
            Assert.AreEqual(1580, boleto.TotalAbonado); 
        }

        [Test]
        public void TestBoletoUsoFrecuente_SeReiniciaAlCambiarDeMes()
        {
            TiempoFalso tiempo = new TiempoFalso(); 
            Tarjeta tarjeta = new Tarjeta();
            Colectivo colectivo = new Colectivo("152", tiempo);

            for (int i = 0; i < 30; i++)
                tarjeta.RegistrarViaje(tiempo);

            tiempo.AgregarDias(20);

            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000);

            Boleto boleto = colectivo.PagarCon(tarjeta);
            Assert.IsNotNull(boleto);
            Assert.AreEqual(1580, boleto.TotalAbonado); 
        }

        [Test]
        public void TestObtenerCantidadViajesMes_ReflejaLosRegistros()
        {
            TiempoFalso tiempo = new TiempoFalso();
            Tarjeta tarjeta = new Tarjeta();

            for (int i = 0; i < 15; i++)
                tarjeta.RegistrarViaje(tiempo);

            Assert.AreEqual(15, tarjeta.ObtenerCantidadViajesMes());
        }

        [Test]
        public void TestAcreditarCarga_SinSaldoPendiente_NoHaceNada()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(2000);
            int saldoInicial = tarjeta.ObtenerSaldo();
            int saldoPendienteInicial = tarjeta.SaldoPendiente;

            tarjeta.AcreditarCarga();

            Assert.AreEqual(saldoInicial, tarjeta.ObtenerSaldo());
            Assert.AreEqual(saldoPendienteInicial, tarjeta.SaldoPendiente);
        }

        [Test]
        public void TestAcreditarCarga_SaldoEnLimite_NoAcredita()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000);
            int saldoPendienteInicial = tarjeta.SaldoPendiente;

            tarjeta.AcreditarCarga();

            Assert.AreEqual(56000, tarjeta.ObtenerSaldo());
            Assert.AreEqual(saldoPendienteInicial, tarjeta.SaldoPendiente);
        }

        [Test]
public void TestObtenerDescuento_Viaje29_NoDescuento()
{
    TiempoFalso tiempo = new TiempoFalso();
    Tarjeta tarjeta = new Tarjeta();
    
    // Registrar 28 viajes (el próximo será el #29)
    for (int i = 0; i < 28; i++)
        tarjeta.RegistrarViaje(tiempo);
    
    Colectivo colectivo = new Colectivo("152", tiempo);
    tarjeta.Cargar(5000);
    Boleto boleto = colectivo.PagarCon(tarjeta);
    
    Assert.AreEqual(1580, boleto.TotalAbonado); // Viaje #29 - sin descuento
}

        [Test]
public void TestObtenerDescuento_Viaje59_20PorCientoDescuento()
{
    TiempoFalso tiempo = new TiempoFalso();
    Tarjeta tarjeta = new Tarjeta();
    
    // Registrar 58 viajes (el próximo será el #59)
    for (int i = 0; i < 58; i++)
        tarjeta.RegistrarViaje(tiempo);
    
    Colectivo colectivo = new Colectivo("152", tiempo);
    tarjeta.Cargar(5000);
    Boleto boleto = colectivo.PagarCon(tarjeta);
    
    Assert.AreEqual(1264, boleto.TotalAbonado); // Viaje #59 - 20% descuento
}

        [Test]
public void TestObtenerDescuento_Viaje80_25PorCientoDescuento()
{
    TiempoFalso tiempo = new TiempoFalso();
    Tarjeta tarjeta = new Tarjeta();
    
    // Registrar 79 viajes (el próximo será el #80)
    for (int i = 0; i < 79; i++)
        tarjeta.RegistrarViaje(tiempo);
    
    Colectivo colectivo = new Colectivo("152", tiempo);
    tarjeta.Cargar(5000);
    Boleto boleto = colectivo.PagarCon(tarjeta);
    
    Assert.AreEqual(1185, boleto.TotalAbonado); // Viaje #80 - 25% descuento
}

        [Test]
        public void TestAcreditarCarga_ParcialConEspacioDisponible()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000); // 56000 + 4000 pendientes

            Assert.AreEqual(56000, tarjeta.ObtenerSaldo());
            Assert.AreEqual(4000, tarjeta.SaldoPendiente);

            tarjeta.Descontar(2000); // Libera 2000 de espacio
            tarjeta.AcreditarCarga();

            Assert.AreEqual(56000, tarjeta.ObtenerSaldo()); // Se mantiene en límite
            Assert.AreEqual(2000, tarjeta.SaldoPendiente); // 2000 acreditados, 2000 pendientes
        }
        
        [Test]
        public void TestCalcularTarifaConDescuento_EnClasesDerivadas_RetornaTarifaBase()
        {
            TiempoFalso tiempo = new TiempoFalso();
            tiempo.AgregarMinutos(360);
            
            MedioBoleto medio = new MedioBoleto();
            BoletoGratuito gratuito = new BoletoGratuito();
            FranquiciaCompleta franquicia = new FranquiciaCompleta();
            
            int tarifaBase = 1580;
            
            Assert.AreEqual(tarifaBase, medio.CalcularTarifaConDescuento(tarifaBase, tiempo));
            Assert.AreEqual(tarifaBase, gratuito.CalcularTarifaConDescuento(tarifaBase, tiempo));
            Assert.AreEqual(tarifaBase, franquicia.CalcularTarifaConDescuento(tarifaBase, tiempo));
        }
    }

}