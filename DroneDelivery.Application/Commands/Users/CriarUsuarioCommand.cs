using DroneDelivery.Application.Mediatr.Request;
using DroneDelivery.Application.Response;
using Flunt.Notifications;
using Flunt.Validations;

namespace DroneDelivery.Application.Commands.Users
{
    public class CriarUsuarioCommand : Request<ResponseVal>
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string Role { get; set; }

        public string Email { get; set; }
        public double Latitude { get;  set; }
        public double Longitude { get; set; }

        public void Validate()
        {
            AddNotifications(new Contract()
                .Requires()
                .IsNotNullOrEmpty(Username, nameof(Username), "O Username não pode ser vazio"));

            AddNotifications(new Contract()
                .Requires()
                .IsNotNullOrEmpty(Password, nameof(Password), "O Password não pode ser vazio"));

            AddNotifications(new Contract()
                .Requires()
                .IsNotNullOrEmpty(Role, nameof(Role), "A Role não pode ser vazia"));

            AddNotifications(new Contract()
                .Requires()
                .IsNotNullOrEmpty(Email, nameof(Email), "o Email não pode ser vazio"));
        }
    }
}
