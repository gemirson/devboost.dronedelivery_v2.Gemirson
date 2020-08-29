﻿using DroneDelivery.Domain.Interfaces;
using System;
using System.Threading.Tasks;

namespace DroneDelivery.Data.Repositorios.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IPedidoRepository Pedidos { get; }
        IDroneRepository Drones { get; }
        IUserRepository Users { get; }
        Task SaveAsync();

    }
}
