using System;

namespace TarjetaSube
{
    public class Tarjeta
    {
        private static int contadorId = 0;
        public int Id { get; private set; }
        protected int saldo;
        private const int LIMITE_SALDO = 56000;
        private int saldoPendiente;
        private int[] CARGAS_VALIDAS = { 2000, 3000, 4000, 5000, 8000, 10000, 15000, 20000, 25000, 30000 };
        protected const int TARIFA_PASAJE = 1580;
  
        protected const int LIMITE_NEGATIVO = -1200;

        private int viajesDelMes = 0;
        private int mesActual = -1;

        // Nuevos campos para trasbordos
        private DateTime? ultimaPagoConMonto;
        private string ultimaLineaPago;

        public int Saldo => saldo;
        public int SaldoPendiente => saldoPendiente;

        public Tarjeta()
        {
            contadorId++;
            Id = contadorId;
            saldo = 0;
            saldoPendiente = 0;
            mesActual = -1;
            viajesDelMes = 0;
            ultimaPagoConMonto = null;
            ultimaLineaPago = null;
        }

        public int ObtenerSaldo()
        {
            return saldo;
        }

        public bool Cargar(int monto)
        {
            bool valido = false;
            foreach (var carga in CARGAS_VALIDAS)
            {
                if (monto == carga)
                {
                    valido = true;
                    break;
                }
            }

            if (!valido) return false;

            int nuevoSaldo = saldo + monto;

            if (nuevoSaldo > LIMITE_SALDO)
            {
                saldoPendiente += nuevoSaldo - LIMITE_SALDO;
                saldo = LIMITE_SALDO;
            }
            else
            {
                saldo = nuevoSaldo;
            }

            return true;
        }

        public void AcreditarCarga()
        {
            if (saldoPendiente > 0 && saldo < LIMITE_SALDO)
            {
                int espacioDisponible = LIMITE_SALDO - saldo;
                int montoAcreditado = Math.Min(espacioDisponible, saldoPendiente);

                saldo += montoAcreditado;
                saldoPendiente -= montoAcreditado;
            }
        }

        public virtual bool Descontar(int monto)
        {
            if (saldo - monto < LIMITE_NEGATIVO)
            {
                return false;
            }

            saldo -= monto;
            AcreditarCarga();
            return true;
        }

        private double ObtenerDescuento(Tiempo tiempo)
        {
            DateTime ahora = tiempo.Now();

            if (mesActual != ahora.Month)
            {
                mesActual = ahora.Month;
                viajesDelMes = 0;
            }

            int numeroDelViaje = viajesDelMes + 1;

            if (numeroDelViaje >= 30 && numeroDelViaje <= 59)
                return 0.20; 
            if (numeroDelViaje >= 60 && numeroDelViaje <= 80)
                return 0.25; 

            return 0.0; 
        }

        public virtual int CalcularTarifaConDescuento(int tarifaBase, Tiempo tiempo)
        {
            double descuento = ObtenerDescuento(tiempo);
            double tarifaConDescuento = tarifaBase * (1.0 - descuento);
            return (int)Math.Round(tarifaConDescuento);
        }

        public int ObtenerCantidadViajesMes()
        {
            return viajesDelMes;
        }

        public virtual bool PuedeViajar(Tiempo tiempo)
        {
            return true;
        }

        public virtual void RegistrarViaje(Tiempo tiempo)
        {
            DateTime ahora = tiempo.Now();

            if (mesActual != ahora.Month)
            {
                mesActual = ahora.Month;
                viajesDelMes = 0;
            }

            viajesDelMes++;
        }

        // Nuevos métodos para trasbordos

        public virtual bool PuedeHacerTrasbordo(string linea, Tiempo tiempo)
        {
            if (!ultimaPagoConMonto.HasValue)
                return false;

            DateTime ahora = tiempo.Now();

            // Solo lunes a sábado (domingo no), de 7:00 a 22:00
            if (ahora.DayOfWeek == DayOfWeek.Sunday)
                return false;
            var hora = ahora.TimeOfDay;
            if (hora < TimeSpan.FromHours(7) || hora >= TimeSpan.FromHours(22))
                return false;

            // Debe ser entre líneas distintas
            if (string.Equals(linea, ultimaLineaPago, StringComparison.Ordinal))
                return false;

            // Dentro de la ventana de 1 hora desde el último pago con monto
            if (ahora <= ultimaPagoConMonto.Value.AddHours(1))
                return true;

            return false;
        }

        public virtual void RegistrarPagoParaTrasbordo(string linea, Tiempo tiempo)
        {
            ultimaPagoConMonto = tiempo.Now();
            ultimaLineaPago = linea;
        }
    }

    public class MedioBoleto : Tarjeta
    {
        private DateTime? ultimoViaje;
        private DateTime? inicioDelDia;
        private int viajesHoy;
        private const int MINUTOS_ENTRE_VIAJES = 5;
        private const int MAX_VIAJES_CON_DESCUENTO = 2;

        public override bool Descontar(int monto)
        {
            if (saldo - monto < LIMITE_NEGATIVO) return false;
            saldo -= monto;
            AcreditarCarga();
            return true;
        }

        private bool EstaEnHorarioFranquicia(Tiempo tiempo)
        {
            DateTime ahora = tiempo.Now();
            if (ahora.DayOfWeek == DayOfWeek.Saturday || ahora.DayOfWeek == DayOfWeek.Sunday)
                return false;
            var hora = ahora.TimeOfDay;
            if (hora < TimeSpan.FromHours(6) || hora >= TimeSpan.FromHours(22))
                return false;
            return true;
        }

        public override bool PuedeViajar(Tiempo tiempo)
        {
            if (!EstaEnHorarioFranquicia(tiempo))
                return false;

            DateTime ahora = tiempo.Now();

            if (ultimoViaje.HasValue)
            {
                TimeSpan diferencia = ahora - ultimoViaje.Value;
                if (diferencia.TotalMinutes < MINUTOS_ENTRE_VIAJES)
                {
                    return false;
                }
            }

            if (!inicioDelDia.HasValue || ahora.Date != inicioDelDia.Value.Date)
            {
                inicioDelDia = ahora;
                viajesHoy = 0;
            }

            return true;
        }

        public override void RegistrarViaje(Tiempo tiempo)
        {
            DateTime ahora = tiempo.Now();
            ultimoViaje = ahora;

            if (!inicioDelDia.HasValue || ahora.Date != inicioDelDia.Value.Date)
            {
                inicioDelDia = ahora;
                viajesHoy = 0;
            }

            viajesHoy++;
        }

        public int ObtenerTarifaActual(int tarifaBase)
        {
            if (viajesHoy >= MAX_VIAJES_CON_DESCUENTO)
            {
                return tarifaBase;
            }
            return tarifaBase / 2;
        }

        public bool DescontarSegunViajes(int tarifaBase)
        {
            int tarifa = ObtenerTarifaActual(tarifaBase);
            return Descontar(tarifa);
        }

        public override int CalcularTarifaConDescuento(int tarifaBase, Tiempo tiempo)
        {
            return tarifaBase;
        }
    }

    public class BoletoGratuito : Tarjeta
    {
        private DateTime? inicioDelDia;
        private int viajesHoy;
        private const int MAX_VIAJES_GRATUITOS = 2;

        public override bool Descontar(int monto) => true;

        public override void RegistrarViaje(Tiempo tiempo)
        {
            DateTime ahora = tiempo.Now();

            if (!inicioDelDia.HasValue || ahora.Date != inicioDelDia.Value.Date)
            {
                inicioDelDia = ahora;
                viajesHoy = 0;
            }

            viajesHoy++;
            base.RegistrarViaje(tiempo);
        }

        private bool EstaEnHorarioFranquicia(Tiempo tiempo)
        {
            DateTime ahora = tiempo.Now();
            if (ahora.DayOfWeek == DayOfWeek.Saturday || ahora.DayOfWeek == DayOfWeek.Sunday)
                return false;
            var hora = ahora.TimeOfDay;
            if (hora < TimeSpan.FromHours(6) || hora >= TimeSpan.FromHours(22))
                return false;
            return true;
        }

        public override bool PuedeViajar(Tiempo tiempo)
        {
            return EstaEnHorarioFranquicia(tiempo);
        }

        public int ObtenerTarifaActual(int tarifaBase)
        {
            return viajesHoy >= MAX_VIAJES_GRATUITOS ? tarifaBase : 0;
        }

        public bool DescontarSegunViajes(int tarifaBase, Tiempo tiempo)
        {
            DateTime ahora = tiempo.Now();

            if (!inicioDelDia.HasValue || ahora.Date != inicioDelDia.Value.Date)
            {
                inicioDelDia = ahora;
                viajesHoy = 0;
            }

            int tarifa = ObtenerTarifaActual(tarifaBase);

            if (tarifa == 0)
                return true;

            if (saldo - tarifa < LIMITE_NEGATIVO)
                return false;

            saldo -= tarifa;
            AcreditarCarga();
            return true;
        }

        public bool DescontarSegunViajes(Tiempo tiempo)
        {
            return DescontarSegunViajes(TARIFA_PASAJE, tiempo);
        }

        public override int CalcularTarifaConDescuento(int tarifaBase, Tiempo tiempo)
        {
            return tarifaBase;
        }
    }

    public class FranquiciaCompleta : Tarjeta
    {
        private DateTime? inicioDelDia;
        private int viajesHoy;
        private const int MAX_VIAJES_GRATUITOS = 2;

        public override bool Descontar(int monto) => true;

        public override void RegistrarViaje(Tiempo tiempo)
        {
            DateTime ahora = tiempo.Now();

            if (!inicioDelDia.HasValue || ahora.Date != inicioDelDia.Value.Date)
            {
                inicioDelDia = ahora;
                viajesHoy = 0;
            }

            viajesHoy++;
            base.RegistrarViaje(tiempo);
        }

        private bool EstaEnHorarioFranquicia(Tiempo tiempo)
        {
            DateTime ahora = tiempo.Now();
            if (ahora.DayOfWeek == DayOfWeek.Saturday || ahora.DayOfWeek == DayOfWeek.Sunday)
                return false;
            var hora = ahora.TimeOfDay;
            if (hora < TimeSpan.FromHours(6) || hora >= TimeSpan.FromHours(22))
                return false;
            return true;
        }

        public override bool PuedeViajar(Tiempo tiempo)
        {
            return EstaEnHorarioFranquicia(tiempo);
        }

        public int ObtenerTarifaActual(int tarifaBase)
        {
            return viajesHoy >= MAX_VIAJES_GRATUITOS ? tarifaBase : 0;
        }
        public bool DescontarSegunViajes(int tarifaBase, Tiempo tiempo)
        {
            DateTime ahora = tiempo.Now();

            if (!inicioDelDia.HasValue || ahora.Date != inicioDelDia.Value.Date)
            {
                inicioDelDia = ahora;
                viajesHoy = 0;
            }

            int tarifa = ObtenerTarifaActual(tarifaBase);

            if (tarifa == 0)
                return true;

            if (saldo - tarifa < LIMITE_NEGATIVO)
                return false;

            saldo -= tarifa;
            AcreditarCarga();
            return true;
        }

        public override int CalcularTarifaConDescuento(int tarifaBase, Tiempo tiempo)
        {
            return tarifaBase;
        }
    }
}