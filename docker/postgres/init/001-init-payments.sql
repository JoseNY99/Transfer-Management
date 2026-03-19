CREATE TABLE IF NOT EXISTS payments (
    id uuid PRIMARY KEY,
    external_operation_id uuid NOT NULL UNIQUE,
    customer_id uuid NOT NULL,
    service_provider_id uuid NOT NULL,
    payment_method_id integer NOT NULL,
    amount numeric(18,2) NOT NULL,
    status varchar(20) NOT NULL,
    created_at timestamp without time zone NOT NULL,
    updated_at timestamp without time zone NULL
);

CREATE INDEX IF NOT EXISTS ix_payments_customer_created_at
ON payments (customer_id, created_at);