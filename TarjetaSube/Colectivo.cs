using System;

namespace TarjetaSube
{
    public class Colectivo
    {
        private const int TARIFA_BASICA = 1580;
        private const int TARIFA_INTERURBANA = 3000;
        public string Linea { get; private set; }
        private Tiempo tiempo;
        private bool esInterurbano;

        public Colectivo(string linea) : this(linea, false, new Tiempo())
        {
        }

        public Colectivo(string linea, Tiempo tiempo) : this(linea, false, tiempo)
        {
        }

        public Colectivo(string linea, bool esInterurbano) : this(linea, esInterurbano, new Tiempo())
        {
        }

        public Colectivo(string linea, bool esInterurbano, Tiempo tiempo)
        {
            Linea = linea;
            this.esInterurbano = esInterurbano;
            this.tiempo = tiempo;
        }

        public Boleto? PagarCon(Tarjeta tarjeta)
        {
            if (tarjeta == null)
            {
                throw new ArgumentNullException(nameof(tarjeta));
            }

            if (!tarjeta.PuedeViajar(tiempo))
            {
                return null;
            }

            int tarifaBase = esInterurbano ? TARIFA_INTERURBANA : TARIFA_BASICA;

            int saldoAnterior = tarjeta.ObtenerSaldo();
            bool pagoExitoso = false;
            int tarifaAplicada = 0;
            bool esTrasbordo = false;

            // Verificar si corresponde trasbordo (gratuito) antes de aplicar tarifas
            if (tarjeta.PuedeHacerTrasbordo(Linea, tiempo))
            {
                esTrasbordo = true;
                tarifaAplicada = 0;
                pagoExitoso = true;
            }
            else
            {
                if (tarjeta is MedioBoleto medioBoleto)
                {
                    pagoExitoso = medioBoleto.DescontarSegunViajes(tarifaBase);
                    tarifaAplicada = medioBoleto.ObtenerTarifaActual(tarifaBase);
                }
                else if (tarjeta is BoletoGratuito boletoGratuito)
                {
                    pagoExitoso = boletoGratuito.DescontarSegunViajes(tarifaBase, tiempo);
                    tarifaAplicada = boletoGratuito.ObtenerTarifaActual(tarifaBase);
                }
                else if (tarjeta is FranquiciaCompleta franquiciaCompleta)
                {
                    pagoExitoso = franquiciaCompleta.DescontarSegunViajes(tarifaBase, tiempo);
                    tarifaAplicada = franquiciaCompleta.ObtenerTarifaActual(tarifaBase);
                }
                else
                {
                    tarifaAplicada = tarjeta.CalcularTarifaConDescuento(tarifaBase, tiempo);
                    pagoExitoso = tarjeta.Descontar(tarifaAplicada);
                }
            }

            if (!pagoExitoso)
            {
                return null;
            }

            // Si se pagó una tarifa mayor a 0 (no fue trasbordo y no fue gratuito por franquicia),
            // registrar el pago para iniciar/actualizar la ventana de trasbordos.
            if (!esTrasbordo && tarifaAplicada > 0)
            {
                tarjeta.RegistrarPagoParaTrasbordo(Linea, tiempo);
            }

            int saldoNuevo = tarjeta.ObtenerSaldo();

            int totalAbonado = tarifaAplicada;

            // Registrar el viaje en la tarjeta (para contadores internos)
            tarjeta.RegistrarViaje(tiempo);

            // Construir boleto indicando si fue trasbordo
            return new Boleto(tarjeta.GetType().Name, Linea, totalAbonado, saldoNuevo, tarjeta.Id, tiempo, esTrasbordo);
        }
    }
}