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
    public class LoginUsuarioHandler : ValidatorResponse, IRequestHandler<LoginUsuarioCommand, ResponseVal>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher<Cliente> _passwordHasher;

        public LoginUsuarioHandler(IUnitOfWork unitOfWork, ITokenService tokenService, IPasswordHasher<Cliente> passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
        }

        public async Task<ResponseVal> Handle(LoginUsuarioCommand request, CancellationToken cancellationToken)
        {
            request.Validate();
            if (request.Notifications.Any())
            {
                _response.AddNotifications(request.Notifications);
                return _response;
            }


            // Recupera o usuário
            var user = await _unitOfWork.Users.ObterPorEmailAsync(request.Email);

            // Verifica se o usuário existe
            if (user == null)
                _response.AddNotification(new Notification("user", "Usuário ou senha inválidos"));


            var passwordResult = _passwordHasher.VerifyHashedPassword(user, user.Senha, request.Password);

            if (passwordResult == PasswordVerificationResult.Failed)
                _response.AddNotification(new Notification("user", "Usuário ou senha inválidos"));


            if (_response.HasFails)
                return _response;

            var jwt = _tokenService.CreateJWT(user);
            _response.AddValue(new
            {
                request.Email,
                jwt
            });

            return _response;
        }
    }
}
