using DroneDelivery.Domain.Core;
using DroneDelivery.Domain.Enum;
using System;

namespace DroneDelivery.Domain.Entidades
{
    public class Pedido : EntidadeBase<Guid>
    {
        public double Peso { get; private set; }

        public DateTime DataPedido { get; private set; }

       public PedidoStatus Status { get; private set; }

        public Guid? DroneId { get; private set; }
        public Drone Drone { get; private set; }

        public Guid? ClienteId { get; private set; } 
        public Cliente Cliente { get; private set; }

        protected Pedido() { }

        public Pedido(double peso, Guid clienteId, PedidoStatus status)
        {
            Peso = peso;
            Status = status;
            ClienteId = clienteId;
        }

        public void AtualizarStatusPedido(PedidoStatus status)
        {
            Status = status;
        }


        public bool ValidarPesoPedido(double peso)
        {
            return peso >= Peso;
        }

        public void AssociarDrone(Guid droneId)
        {
            DroneId = droneId;
        }





    }
}
