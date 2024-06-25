using Dapper;
using MediatR;
using Questao5.Application.Queries.Requests;
using Questao5.Application.Queries.Responses;
using Questao5.Domain.Entities;
using System.Data;

namespace Questao5.Application.Handlers
{
    public class ConsultarSaldoHandler : IRequestHandler<ConsultarSaldoQuery, ConsultarSaldoResponse>
    {
        private readonly IDbConnection _dbConnection;

        public ConsultarSaldoHandler(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<ConsultarSaldoResponse> Handle(ConsultarSaldoQuery request, CancellationToken cancellationToken)
        {
            var conta = await _dbConnection.QueryFirstOrDefaultAsync<ContaCorrente>(
                "SELECT * FROM contacorrente WHERE idcontacorrente = @IdContaCorrente",
                new { IdContaCorrente = request.IdContaCorrente });

            if (conta == null)
                throw new Exception("Conta corrente inválida");

            if (!conta.Ativo)
                throw new Exception("Conta corrente inativa");

            var creditos = await _dbConnection.QueryFirstOrDefaultAsync<double>(
                "SELECT COALESCE(SUM(valor), 0) FROM movimento WHERE idcontacorrente = @IdContaCorrente AND tipomovimento = 'C'",
                new { IdContaCorrente = request.IdContaCorrente });

            var debitos = await _dbConnection.QueryFirstOrDefaultAsync<double>(
                "SELECT COALESCE(SUM(valor), 0) FROM movimento WHERE idcontacorrente = @IdContaCorrente AND tipomovimento = 'D'",
                new { IdContaCorrente = request.IdContaCorrente });

            var saldo = creditos - debitos;

            return new ConsultarSaldoResponse
            {
                Numero = conta.Numero,
                Nome = conta.Nome,
                DataHoraConsulta = DateTime.UtcNow,
                Saldo = saldo
            };
        }
    }
}
