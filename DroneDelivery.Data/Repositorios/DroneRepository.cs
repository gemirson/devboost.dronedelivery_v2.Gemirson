using DroneDelivery.Data.Data;
using DroneDelivery.Domain.Entidades;
using DroneDelivery.Domain.Enum;
using DroneDelivery.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace DroneDelivery.Data.Repositorios
{
    public class DroneRepository : IDroneRepository
    {
        private readonly DroneDbContext _context;

        public DroneRepository(DroneDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarAsync(Drone drone)
        {
            await _context.Drones.AddAsync(drone);
        }

        public async Task<IEnumerable<Drone>> ObterAsync()
        {
            return await _context.Drones.Include(x => x.Pedidos).ToListAsync();
        }

        public async Task<Drone> ObterAsync(Guid id)
        {
            return await _context.Drones.Include(x => x.Pedidos).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task RemoverAsync(Guid id)
        {
            var drone = await _context.Drones.FirstOrDefaultAsync(x => x.Id == id);
            _context.Remove(drone);
        }

        public void Remover(Drone drone)
        {
            _context.Remove(drone);
        }

        public async Task<IEnumerable<Drone>> ObterDronesParaEntregaAsync()
        {
            var drones = await _context.Drones.Include(x => x.Pedidos).ThenInclude(x => x.Cliente)
                .Where(x => x.Status == DroneStatus.Livre
                    && x.Pedidos.Count() > 0).ToListAsync();
            return drones;
        }
    }
}
