# Payments API — Microsservico de Pagamentos (FCG)

Microsservico de **Pagamentos** da FIAP Cloud Games (Tech Challenge). Servico event-driven que processa pagamentos de pedidos.

## Finalidade

- **Consome `OrderPlacedEvent`** via SQS — recebe pedidos do Catalog API.
- **Processa pagamento** — simula aprovacao/rejeicao (mock).
- **Persiste pagamento** no PostgreSQL.
- **Publica `PaymentProcessedEvent`** via SNS — consumido pelo Catalog API (biblioteca) e Notifications API (email).

## Tecnologias / Dependencias

| Recurso | Local (dev) | AWS (producao) |
|---------|------------|----------------|
| Runtime | .NET 10 / C# | .NET 10 / C# |
| Banco | PostgreSQL 16 | RDS PostgreSQL 16 |
| Mensageria (sub) | SQS (localstack opcional) | SQS |
| Mensageria (pub) | SNS (localstack opcional) | SNS |
| Logs | Console / arquivo | CloudWatch Logs |
| Metricas | `/metrics` (Prometheus) | `/metrics` (Prometheus) |

Pacotes NuGet principais: `AWSSDK.SimpleNotificationService`, `AWSSDK.SQS`, `Npgsql.EntityFrameworkCore.PostgreSQL`, `prometheus-net.AspNetCore`, `Serilog.AspNetCore`, `ErrorOr`.

> **Nota:** Este e um servico puramente event-driven (worker). Nao possui endpoints REST alem de health check e metricas.

## Como rodar localmente

```bash
# 1. Subir PostgreSQL
cd ../postech-orchestration/docker
docker compose up -d postgresql

# 2. Rodar a API
cd ../../postech-payments-api/src/Postech.Payments.Api
dotnet run
```

- `http://localhost:{port}/health` — health check
- `http://localhost:{port}/metrics` — metricas Prometheus

## Variaveis de ambiente

| Variavel | Descricao | Default (local) |
|----------|-----------|-----------------|
| `ConnectionStrings__DefaultConnection` | PostgreSQL | `Host=localhost;Port=5432;Database=postech_payments;Username=postgres;Password=postgres` |
| `AWS__Region` | Regiao AWS | `us-east-1` |
| `AWS__ServiceURL` | LocalStack (opcional) | — |
| `AWS__SnsTopicArn` | ARN do topico SNS para PaymentProcessedEvent | — |
| `AWS__SqsQueueUrl` | URL da fila SQS para OrderPlacedEvent | — |

## Endpoints

| Metodo | Rota | Descricao |
|--------|------|-----------|
| `GET` | `/health` | Health check |
| `GET` | `/health/alive` | Liveness probe |
| `GET` | `/metrics` | Metricas Prometheus |

## Eventos

- **Consome:** `OrderPlacedEvent` (OrderId, UserId, GameId, Price) via SQS.
- **Publica:** `PaymentProcessedEvent` (OrderId, UserId, GameId, IsSuccessful, FailureReason) via SNS.

## Estrutura do projeto

```
src/Postech.Payments.Api/
  Application/            # DTOs, Services (PaymentService)
    Utils/                # CorrelationContext
  Domain/                 # Entities (Payment), Enums (PaymentStatus)
  Extensions/             # DI registration, pipeline
  Infrastructure/
    Data/                 # PaymentsDbContext (EF Core / Postgres)
    Messaging/            # SnsEventPublisher, SqsOrderPlacedConsumer
    Repositories/         # IPaymentRepository, PaymentRepository
  Migrations/             # EF Core migrations
```

## Como atualizar imagem no ECR

```bash
ACCOUNT=$(aws sts get-caller-identity --query Account --output text)
ECR="${ACCOUNT}.dkr.ecr.us-east-1.amazonaws.com/tf-postech-postech-payments-api"

aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin "${ACCOUNT}.dkr.ecr.us-east-1.amazonaws.com"

docker build -t "${ECR}:latest" -f Dockerfile .
docker push "${ECR}:latest"
```
