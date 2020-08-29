using AutoMapper;
using DroneDelivery.Application.Commands.Drones;
using DroneDelivery.Application.Commands.Pedidos;
using DroneDelivery.Application.Models;
using DroneDelivery.Domain.Entidades;
using DroneDelivery.Domain.Enum;
using System;
using System.Linq;

namespace DroneDelivery.Application.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Commands 
            CreateMap<CriarDroneCommand, Drone>()
                .ForMember(d => d.Status, o => o.MapFrom(x => DroneStatus.Livre));
            CreateMap<CriarPedidoCommand, Pedido>()
                .ForMember(d => d.DataPedido, o => o.MapFrom(x => DateTime.Now));

   
            //Domain to Model
            CreateMap<Drone, DroneModel>()
                .ForMember(d => d.Status, opts => opts.MapFrom(x => x.Status.ToString()));
            CreateMap<Pedido, PedidoModel>();

            CreateMap<Cliente, ClienteModel>();

            CreateMap<Pedido, DronePedidoModel>();

            CreateMap<Drone, DroneSituacaoModel>()
                .ForMember(d => d.Situacao, opts => opts.MapFrom(x => x.Status.ToString()))
                .ForMember(d => d.Pedidos, opts => opts.MapFrom(x => x.Pedidos));
        }

    }
}
