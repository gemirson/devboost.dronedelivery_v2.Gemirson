using System;

namespace DroneDelivery.Application.Models
{
    public class DronePedidoModel
    {
        public Guid Id { get; set; }
        public ClienteModel Cliente { get; set; }

    }
}
