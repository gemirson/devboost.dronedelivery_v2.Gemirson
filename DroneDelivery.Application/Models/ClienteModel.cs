using System;
using System.Collections.Generic;
using System.Text;

namespace DroneDelivery.Application.Models
{
    public class ClienteModel
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

    }
}
