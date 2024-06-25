using System.Globalization;

namespace Questao1
{
    public class ContaBancaria
    {
        public int Numero { get; private set; }
        public string Titular { get; set; }
        private double _saldo;

        public ContaBancaria(int numero, string titular, double depositoInicial = 0)
        {
            Numero = numero;
            Titular = titular;
            _saldo = depositoInicial;
        }

        public double Saldo
        {
            get { return _saldo; }
        }

        public void Depositar(double quantia)
        {
            if (quantia > 0)
            {
                _saldo += quantia;
            }
        }

        public void Sacar(double quantia)
        {
            double taxaSaque = 3.50;
            _saldo -= (quantia + taxaSaque);
        }

        public override string ToString()
        {
            return $"Conta {Numero}, Titular: {Titular}, Saldo: $ {_saldo:F2}";
        }
    }
}
