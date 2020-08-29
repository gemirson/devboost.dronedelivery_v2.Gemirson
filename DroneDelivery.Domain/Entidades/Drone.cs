using DroneDelivery.Domain.Core;
using DroneDelivery.Domain.Enum;
using DroneDelivery.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DroneDelivery.Domain.Entidades
{
    public class Drone : EntidadeBase<Guid>
    {
        public double Capacidade { get; private set; }

        public double Velocidade { get; private set; }

        public double Autonomia { get; private set; }

        public double Carga { get; private set; }

        public ICollection<Pedido> Pedidos { get; private set; } = new List<Pedido>();

        public DroneStatus Status { get; private set; }

        protected Drone() { }

        public Drone(Guid id, double capacidade, double velocidade, double autonomia, double carga, DroneStatus status)
        {
            Id = id;
            Capacidade = capacidade;
            Velocidade = velocidade;
            Autonomia = autonomia;
            Carga = carga;
            Status = status;
        }


        public bool ValidarCapacidadeSobra(double pesoPedido)
        {
            double capacidadeAtual = 0;
            foreach (var pedido in Pedidos.Where(x => x.Status == PedidoStatus.EmEntrega))
                capacidadeAtual += pedido.Peso;


            //adicioar o peso do novo pedido
            capacidadeAtual += pesoPedido;

            return (Capacidade - capacidadeAtual) >= 0;
        }

        public bool ValidarAutonomia(ITempoEntregaService tempoEntregaService, double latitudeInicial, double longitudeInicial, double latitudePedido, double longitudePedido)
        {
            var tempoEmMinutos = tempoEntregaService.ObterTempoEntregaEmMinutos(latitudeInicial, longitudeInicial, latitudePedido, longitudePedido, Velocidade);

            // nao entrega
            if (tempoEmMinutos == 0)
                return false;

            return tempoEmMinutos <= Autonomia;
        }


        public bool ValidarAutonomiaSobraPorPontoEntrega(ITempoEntregaService tempoEntregaService, double latitudeInicial, double longitudeInicial, double latitudePedido, double longitudePedido)
        {
            var ultimaLatitude = latitudeInicial;
            var ultimaLongitude = longitudeInicial;

            //obter tempo entrega dos pedidos que ja estao no drone
            double tempoEntregaAtual = 0;
            foreach (var pedido in Pedidos.Where(x => x.Status == PedidoStatus.EmEntrega).Select(x => x.Cliente))
            {
                tempoEntregaAtual += tempoEntregaService.ObterTempoEntregaEmMinutosIda(ultimaLatitude, ultimaLongitude, pedido.Latitude,pedido.Longitude, Velocidade);
                ultimaLatitude = pedido.Latitude;
                ultimaLongitude = pedido.Longitude;
            }

            //ida do ultimo pedido até o novo pedido
            var tempoIdaUltimoPedido = tempoEntregaService.ObterTempoEntregaEmMinutosIda(ultimaLatitude, ultimaLongitude, latitudePedido, longitudePedido, Velocidade);

            //volta pra base
            var tempoVoltaUltimoPedido = tempoEntregaService.ObterTempoEntregaEmMinutosIda(latitudePedido, longitudePedido, latitudeInicial, longitudeInicial, Velocidade);

            // somar com o tempo do novo pedido
            tempoEntregaAtual += tempoIdaUltimoPedido;
            tempoEntregaAtual += tempoVoltaUltimoPedido;

            //obter autonomia atual considerando a bateria do drone
            var autonomialAtual = Autonomia * Carga / 100;

            return (autonomialAtual - tempoEntregaAtual) >= 0;
        }



        public bool VerificarDroneAceitaOPesoPedido(double pesoPedido)
        {
            return Capacidade >= pesoPedido;
        }


        public void AtualizarStatus(DroneStatus status)
        {
            Status = status;
        }

        public void SepararPedidosParaEntrega()
        {
            Pedidos = Pedidos.Where(x => x.Status == PedidoStatus.EmEntrega).ToList();
        }

    }
}
