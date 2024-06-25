using MediatR;
using Questao5.Application.Queries.Responses;

namespace Questao5.Application.Queries.Requests
{
    public class ConsultarSaldoQuery : IRequest<ConsultarSaldoResponse>
    {
        public string IdContaCorrente { get; set; }
    }
}
