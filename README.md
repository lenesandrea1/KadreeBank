# Kadree Bank

Caso de estudio. Banco con ahorros (persona natural) y corriente (empresa): consignar, retirar, ver saldo, movimientos recientes y extracto mensual. Hay dos reportes, el de transacciones por mes (de mayor a menor) y el de retiros fuera de la ciudad de origen cuando el total pasa de $1.000.000.

Backend en .NET 9, front en Angular, Postgres en Docker.

El UML del punto 4 está en `kadree-bank-diagrama-clases.drawio` (se abre con draw.io). `rotar-derecha.txt` es el otro ejercicio, el del arreglo.

## Cómo correrlo

Hace falta .NET 9, Node y Docker.

```powershell
docker compose up -d
dotnet run --project src/KadreeBank.Api/KadreeBank.Api.csproj --launch-profile http
```

En otra terminal:

```powershell
cd frontend
npm install
npx ng serve --port 4200
```

Swagger queda en http://localhost:5203/swagger y el front en http://localhost:4200.

La base usa el puerto 55432 (no 5432) porque en el equipo ya había un Postgres ocupando el default. Usuario `kadree`, clave `kadree`, base `kadreebank`.

Al primer arranque se crean dos cuentas:

- Ana Pérez, ahorros `AH-001`, Bogotá
- Acme SAS, corriente `CC-100`, Medellín

Ana tiene un retiro en Cali y Acme uno en Cartagena, así el reporte de fuera de ciudad no sale vacío.

## Endpoints

Base: `http://localhost:5203/api`

```
GET  /cuentas
GET  /cuentas/{id}/saldo
GET  /cuentas/{id}/movimientos?take=10
GET  /cuentas/{id}/extractos/{año}/{mes}
POST /cuentas/{id}/consignaciones     { "amount": 10000, "city": "Bogotá" }
POST /cuentas/{id}/retiros            igual
GET  /reportes/clientes-transacciones?year=2026&month=8
GET  /reportes/retiros-fuera-ciudad
```

Si el retiro deja el saldo en rojo, no pasa. Si dos operaciones llegan a la misma cuenta a la vez, se bloquea la fila (`FOR UPDATE` + transacción Serializable) para que el saldo no quede mal.

## Por qué está armado así

Clean Architecture, cuatro proyectos. La idea era no meter las reglas del banco en el controller ni en EF.

- `Domain`: saldo, tipo de titular, consignar/retirar. Si esto está mal, el resto da igual.
- `Application`: lo que pide el caso (saldo, movimientos, extracto, reportes). Los controllers solo llaman acá.
- `Infrastructure`: Postgres. El bloqueo de la cuenta va aquí porque es un tema de la base, no de negocio.
- `Api`: HTTP, Swagger y CORS al 4200.
- `frontend`: Angular, lo que pedían (saldo, consignar/retirar, movimientos). El extracto y los reportes también quedaron porque ya estaban en la API.

En código no hice `SavingsAccount` y `CheckingAccount`. Las dos cuentas mueven plata igual; lo único distinto es quién puede abrirlas, y eso se valida en `Open()`. Un enum era más simple. El diagrama sí va con subclases porque el enunciado habla de dos tipos de cuenta, no de cómo lo programé.

`EnsureCreated` + seed al arrancar. Para el caso alcanza; no hay migraciones.
