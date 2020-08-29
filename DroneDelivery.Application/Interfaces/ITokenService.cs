using DroneDelivery.Domain.Entidades;
using DroneDelivery.Infra.Security;

namespace DroneDelivery.Application.Interfaces
{
    public interface ITokenService
    {
        JsonWebToken CreateJWT(Cliente user);
    }
}
