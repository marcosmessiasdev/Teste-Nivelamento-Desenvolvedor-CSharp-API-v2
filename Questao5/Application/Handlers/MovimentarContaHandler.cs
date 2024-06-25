using Dapper;
using MediatR;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Commands.Responses;
using Questao5.Domain.Entities;
using System.Data;

namespace Questao5.Application.Handlers
{
    public class MovimentarContaHandler : IRequestHandler<MovimentarContaCommand, MovimentarContaResponse>
    {
        private readonly IDbConnection _dbConnection;

        public MovimentarContaHandler(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<MovimentarContaResponse> Handle(MovimentarContaCommand request, CancellationToken cancellationToken)
        {
            var conta = await _dbConnection.QueryFirstOrDefaultAsync<ContaCorrente>(
                "SELECT * FROM contacorrente WHERE idcontacorrente = @IdContaCorrente",
                new { request.IdContaCorrente });

            if (conta == null)
                throw new Exception("Conta corrente inválida");

            if (!conta.Ativo)
                throw new Exception("Conta corrente inativa");

            if (request.Valor <= 0)
                throw new Exception("Valor inválido");

            if (request.TipoMovimento != "C" && request.TipoMovimento != "D")
                throw new Exception("Tipo de movimento inválido");

            var idempotencia = await _dbConnection.QueryFirstOrDefaultAsync<Idempotencia>(
                "SELECT * FROM idempotencia WHERE chave_idempotencia = @ChaveIdempotencia",
                new { request.ChaveIdempotencia });

            if (idempotencia != null)
                return Newtonsoft.Json.JsonConvert.DeserializeObject<MovimentarContaResponse>(idempotencia.Resultado);

            var idMovimento = Guid.NewGuid().ToString();
            var dataMovimento = DateTime.UtcNow.ToString("dd/MM/yyyy");
            var movimento = new Movimento
            {
                IdMovimento = idMovimento,
                IdContaCorrente = request.IdContaCorrente,
                DataMovimento = dataMovimento,
                TipoMovimento = request.TipoMovimento,
                Valor = request.Valor
            };

            await _dbConnection.ExecuteAsync(
                "INSERT INTO movimento (idmovimento, idcontacorrente, datamovimento, tipomovimento, valor) VALUES (@IdMovimento, @IdContaCorrente, @DataMovimento, @TipoMovimento, @Valor)",
                movimento);

            var resultado = new MovimentarContaResponse { IdMovimento = idMovimento };
            await _dbConnection.ExecuteAsync(
                "INSERT INTO idempotencia (chave_idempotencia, requisicao, resultado) VALUES (@ChaveIdempotencia, @Requisicao, @Resultado)",
                new
                {
                    request.ChaveIdempotencia,
                    Requisicao = Newtonsoft.Json.JsonConvert.SerializeObject(request),
                    Resultado = Newtonsoft.Json.JsonConvert.SerializeObject(resultado)
                });

            return resultado;
        }
    }
}
