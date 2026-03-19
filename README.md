# Backend Senior Payments

Solución backend para la gestión de pagos con evaluación de riesgo asíncrona utilizando .NET 8, PostgreSQL, Kafka y Docker.

## Arquitectura

La solución está separada en los siguientes componentes:

- **Payments.Api**: API REST para crear pagos y consultar su estado.
- **Payments.Application**: casos de uso, comandos, queries y contratos.
- **Payments.Domain**: entidades y reglas de dominio.
- **Payments.Infrastructure**: persistencia, configuración de Entity Framework y mensajería Kafka.
- **Risk.Worker**: consumidor Kafka que evalúa el riesgo y publica la respuesta.

### Flujo principal

1. El cliente invoca `POST /api/payments`.
2. La API registra el pago con estado inicial `evaluating`.
3. La API publica un mensaje en Kafka en el tópico `risk-evaluation-request`.
4. El `Risk.Worker` consume ese mensaje y evalúa el pago.
5. El `Risk.Worker` publica el resultado en `risk-evaluation-response`.
6. La API consume la respuesta y actualiza el estado a `accepted` o `denied`.
7. El cliente puede consultar el estado final mediante `GET /api/payments/{externalOperationId}`.

### Los endpoints pueden probarse desde Swagger:
http://localhost:8080/swagger

---

### Script para la Base de datos
No se necesita hacer ninguna migracion, ni se necesita script para la base de datos, yo desde la carpeta ubicada: docker/postgres/init/001-init-payments.sql
ya estoy pasandole un script para que cree todo la tabla cuando se inicia el contenedor de postgresSQL

## Tecnologías utilizadas

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- Apache Kafka
- Docker / Docker Compose
- Swagger

---

## Requisitos previos

- Docker Desktop (Importante que este en running para hacer las pruebas)
- .NET 8 SDK (opcional si se ejecuta todo por Docker)

---

## Cómo ejecutar el proyecto con Docker

### 1. Clonar el repositorio


git clone <URL_DEL_REPOSITORIO>


### 2. levantar los servicios
docker compose up -d --build

### 3. verificamos que esten arriba los contenedores
docker ps -a

### 4. luego podesmos abrir el Swagger para ver los endpoint
http://localhost:8080/swagger


### 5. Despues ya podriamos ingresar a un postman y ejecutar las consultas 
Post /api/payments
{
  "customerId": "cfe8b150-2f84-4a1a-bdf4-923b20e34973",
  "serviceProviderId": "5fa3ab5c-645f-4cd5-b29e-5c5c116d7ea4",
  "paymentMethodId": 2,
  "amount": 1500000.00
}

y para el Get /api/payments/{externalOperationId}
{
  "externalOperationId": "2a7fb0cd-4c1c-4e6e-b8f9-ef83bb14cf23",
  "createdAt": "2026-03-19T12:00:00Z",
  "status": "accepted"
}

### 6. Luego si se necesita verificar la base de datos desde la raiz se ejecuta el siguiente comando  
docker exec -it payments-postgres psql -U postgres -d paymentsdb -c "select * from payments order by created_at desc;"

Este comando muestra como ingresan las consultas a la Base de datos


```bash