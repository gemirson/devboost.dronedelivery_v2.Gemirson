using AutoMapper;
using DroneDelivery.Application.Commands.Pedidos;
using DroneDelivery.Application.Response;
using DroneDelivery.Application.Validador;
using DroneDelivery.Data.Repositorios.IRepository;
using DroneDelivery.Domain.Entidades;
using DroneDelivery.Domain.Enum;
using DroneDelivery.Domain.Interfaces;
using DroneDelivery.Infra.BaseDrone;
using Flunt.Notifications;
using MediatR;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DroneDelivery.Application.Core;

namespace DroneDelivery.Application.Handlers.Drones
{
    public class CriarPedidoHandler : ValidatorResponse, IRequestHandler<CriarPedidoCommand, ResponseVal>
    {
        private readonly ITempoEntregaService _calcularDistancia;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IOptions<BaseDroneConfig> _config;
        private readonly AuthenticatedCliente _clienteHttpContext;

        public CriarPedidoHandler(IUnitOfWork unitOfWork, IMapper mapper, IOptions<BaseDroneConfig> config, ITempoEntregaService calcularDistancia, AuthenticatedCliente clienteHttpContext)
        {
            _calcularDistancia = calcularDistancia;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _config = config;
            _clienteHttpContext = clienteHttpContext;
        }


        public async Task<ResponseVal> Handle(CriarPedidoCommand request, CancellationToken cancellationToken)
        {
            request.Validate();
            if (request.Notifications.Any())
            {
                _response.AddNotifications(request.Notifications);
                return _response;
            }

          

           var cliente = _unitOfWork.Users.ObterPorIdAsync(_clienteHttpContext.ID);

            if (cliente == null) {
                _response.AddNotification(new Notification("ID","Cliente não encontrado"));
                return _response;
            }
            var pedido = new Pedido(request.Peso, _clienteHttpContext.ID,PedidoStatus.EmEntrega);

            if (!pedido.ValidarPesoPedido(Utility.Utils.CARGA_MAXIMA_GRAMAS))
                _response.AddNotification(new Notification("pedido", $"capacidade do pedido não pode ser maior que {Utility.Utils.CARGA_MAXIMA_GRAMAS / 1000} KGs"));

            // temos que procurar drones disponiveis
            Drone droneDisponivel = null;

            //temos que olhar TODOS os drones, e nao somente os disponiveis, pq se todos estiverem ocupados... 
            //ainda sim, precisamos validar se temos capacidade de entregar o pedido
            var drones = await _unitOfWork.Drones.ObterAsync();

            if (drones.Count() == 0)
                _response.AddNotification(new Notification("pedido", $"não existe drone cadastrado"));

            if (_response.HasFails)
                return _response;

            foreach (var drone in drones)
            {

                //valida se algum drone tem autonomia e aceita capacidade para entregar o pedido
                var droneTemAutonomia = drone.ValidarAutonomia(_calcularDistancia, _config.Value.Latitude, _config.Value.Longitude, cliente.Result.Latitude, cliente.Result.Longitude);
                var droneAceitaPeso = drone.VerificarDroneAceitaOPesoPedido(pedido.Peso);
                if (!droneTemAutonomia || !droneAceitaPeso)
                    continue;

                //verificar se tem algum drone disponivel
                if (drone.Status != DroneStatus.Livre)
                    continue;

                //verifica se o drone possui espaço para adicionar mais peso
                if (!drone.ValidarCapacidadeSobra(pedido.Peso))
                    continue;

                //verifica se o drone possui autonomia para enttregar o pedido
                if (!drone.ValidarAutonomiaSobraPorPontoEntrega(_calcularDistancia, _config.Value.Latitude, _config.Value.Longitude, cliente.Result.Latitude, cliente.Result.Longitude))
                    continue;

                droneDisponivel = drone;
                break;
            }

            if (droneDisponivel == null)
            {
                pedido.AtualizarStatusPedido(PedidoStatus.AguardandoEntrega);
            }
            else
            {
                pedido.AssociarDrone(droneDisponivel.Id);
                pedido.AtualizarStatusPedido(PedidoStatus.EmEntrega);

            }

            await _unitOfWork.Pedidos.AdicionarAsync(pedido);
            await _unitOfWork.SaveAsync();

            _response.AddValue(new
            {
                status = "ok"
            });

            return _response;
        }
    }
}
