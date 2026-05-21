using Prometheus;

namespace Postech.Payments.Api.Infrastructure.Metrics;

public static class PaymentsMetrics
{
    public static readonly Counter PaymentsProcessed = Prometheus.Metrics.CreateCounter(
        "payments_processed_total", "Payments processed (approved or rejected)",
        new CounterConfiguration { LabelNames = ["status"] });

    public static readonly Counter OrdersReceived = Prometheus.Metrics.CreateCounter(
        "orders_received_total", "Total orders received from SQS");
}
