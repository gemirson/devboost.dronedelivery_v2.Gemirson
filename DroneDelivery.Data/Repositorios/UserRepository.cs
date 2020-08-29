using DroneDelivery.Data.Data;
using DroneDelivery.Domain.Entidades;
using DroneDelivery.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DroneDelivery.Data.Repositorios
{
    public class UserRepository : IUserRepository
    {
        private readonly DroneDbContext _context;

        public UserRepository(DroneDbContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<Cliente>> ObterAsync()
        {
            return await _context.Clientes.ToListAsync();
        }

        public async Task<Cliente> ObterPorIdAsync(Guid id)
        {
            return await _context.Clientes.FirstOrDefaultAsync(x => x.Id == id);
        }


        public async Task AdicionarAsync(Cliente user)
        {
            await _context.Clientes.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<Cliente> ObterPorEmailAsync(string email)
        {
            return await _context.Clientes.FirstOrDefaultAsync(x => x.Email == email);
        }
    }
}
