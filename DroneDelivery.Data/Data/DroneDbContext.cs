using DroneDelivery.Domain.Entidades;
using Microsoft.EntityFrameworkCore;

namespace DroneDelivery.Data.Data
{
    public class DroneDbContext : DbContext
    {
        public DroneDbContext(DbContextOptions<DroneDbContext> options) : base(options)
        {

        }

        public DbSet<Cliente> Clientes { get; set; }

        public DbSet<Drone> Drones { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<HistoricoPedido> HistoricoPedidos { get; set; }

    }
}
