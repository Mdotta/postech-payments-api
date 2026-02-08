# FIAP Cloud Games - Payments API

Microsserviço responsável pelo processamento de pagamentos da plataforma FIAP Cloud Games.

## Tecnologias

- .NET 10
- PostgreSQL (via EF Core)
- RabbitMQ (via MassTransit)
- Docker

## Eventos

### Consome
- `OrderPlacedEvent`: Recebido do CatalogAPI quando um pedido é criado

### Publica
- `PaymentProcessedEvent`: Enviado após processar o pagamento (Approved/Rejected)