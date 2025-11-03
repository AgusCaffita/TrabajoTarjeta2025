using System;

namespace TarjetaSube
{
    public class Colectivo
    {
        private const int TARIFA_BASICA = 1580;
        public string Linea { get; private set; }

        public Colectivo(string linea)
        {
            Linea = linea;
        }

        public Boleto PagarCon(Tarjeta tarjeta)
        {
            if (tarjeta == null)
            {
                throw new ArgumentNullException(nameof(tarjeta));
            }

            int saldoAnterior = tarjeta.ObtenerSaldo();
            bool pagoExitoso = tarjeta.Descontar(TARIFA_BASICA);

            if (!pagoExitoso)
            {
                return null;
            }

            int saldoNuevo = tarjeta.ObtenerSaldo();
            
            int totalAbonado = saldoAnterior - saldoNuevo;

            Boleto boleto = new Boleto(
                tipoTarjeta: tarjeta.GetType().Name,
                lineaColectivo: Linea,
                totalAbonado: totalAbonado,
                saldoRestante: saldoNuevo,
                idTarjeta: tarjeta.Id
            );

            return boleto;
        }
    }
}