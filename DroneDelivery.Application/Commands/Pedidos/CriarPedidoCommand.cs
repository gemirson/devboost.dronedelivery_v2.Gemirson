using DroneDelivery.Application.Mediatr.Request;
using DroneDelivery.Application.Response;
using Flunt.Validations;
using System;

namespace DroneDelivery.Application.Commands.Pedidos
{
    public class CriarPedidoCommand : Request<ResponseVal>
    {
        public double Peso { get; set; }
        public Guid ClienteId { get; set; }    

        public void Validate()
        {
            AddNotifications(new Contract()
                .Requires()
                .IsGreaterThan(Peso, 0, nameof(Peso), "O Peso tem que ser maior que zero"));

            AddNotifications(new Contract()
                .Requires()
                .IsLowerOrEqualsThan(Peso, Utility.Utils.CARGA_MAXIMA_GRAMAS, nameof(Peso), $"O Peso tem que ser menor ou igual a {Utility.Utils.CARGA_MAXIMA_GRAMAS / 1000} KGs"));

            AddNotifications(new Contract()
                .Requires()
                .AreNotEquals(ClienteId, 0, nameof(ClienteId), "O ClienteId não pode ser vazio"));

          

        }
    }
}
