using DroneDelivery.Application.Commands.Users;
using DroneDelivery.Application.Interfaces;
using DroneDelivery.Application.Response;
using DroneDelivery.Application.Validador;
using DroneDelivery.Data.Repositorios.IRepository;
using DroneDelivery.Domain.Entidades;
using Flunt.Notifications;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DroneDelivery.Application.Handlers.Users
{
    public class CriarUsuarioHandler : ValidatorResponse, IRequestHandler<CriarUsuarioCommand, ResponseVal>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher<Cliente> _passwordHasher;

        public CriarUsuarioHandler(IUnitOfWork unitOfWork, IPasswordHasher<Cliente> passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }

        public async Task<ResponseVal> Handle(CriarUsuarioCommand request, CancellationToken cancellationToken)
        {
            request.Validate();
            if (request.Notifications.Any())
            {
                _response.AddNotifications(request.Notifications);
                return _response;
            }

            var user = await _unitOfWork.Users.ObterPorEmailAsync(request.Email);

            // Verifica se o usuário existe
            if (user != null)
            {
                _response.AddNotification(new Notification("user", "Email já foi utilizado"));
                return _response;
            }

            user = new Cliente
            {
                Email = request.Email,
                Nome = request.Username,
                Role = request.Role,
                Latitude = request.Latitude,
                Longitude = request.Longitude,

            };

            //hash password
            var passwordHash = _passwordHasher.HashPassword(user, request.Password);
            user.Senha = passwordHash;

            await _unitOfWork.Users.AdicionarAsync(user);
            await _unitOfWork.SaveAsync();

            return _response;
        }
    }
}
