# POC - Reliable Messaging with Outbox Pattern and Domain Events (.NET + EF Core)
**Contexto: Banco Digital**

Esta prova de conceito demonstra como implementar **mensageria confiável** em uma aplicação .NET simulando um **banco digital**, utilizando:

- **Eventos de domínio (Domain Events)**
- **Padrão Outbox**
- **Interceptors do Entity Framework Core**
- **Hosted Service (Background Worker)** para despacho assíncrono dos eventos

---

## Contexto

Em sistemas financeiros, é essencial garantir **consistência entre as transações do banco de dados e os eventos publicados** (por exemplo, notificações de transferência, integração com sistemas de liquidação ou auditoria).

O padrão **Outbox** resolve o problema de *mensagens perdidas ou duplicadas* garantindo que:

> Os eventos de domínio e os dados da aplicação são salvos **na mesma transação**, e o envio das mensagens é feito **posteriormente**, com segurança.

Este projeto demonstra isso em um **cenário de transferência entre contas bancárias**.

---

## Arquitetura 
```
BancoDigital
├── Application
│   ├── Interfaces
│   └── UseCases
├── Domain
│   ├── Entidades
│   ├── Eventos
│   └── Abstracoes
├── Infrastructure
│   ├── BancoDigitalDbContext (EF Core)
│   ├── Interceptors (Outbox)
│   ├── Outbox (OutboxDispatcher)
│   └── Serializers
└── BancoDigital.Api
    ├── DTOs
    └── Program.cs

> Aqui optei por adotar uma convenção de nomenclatura de abordagem mista inspirada nos princípios do Domain-Driven Design (DDD).
As camadas arquiteturais seguem a convenção tradicional em inglês (domain, application, infrastructure e interfaces) para manter compatibilidade com a literatura e frameworks amplamente utilizados.
Entretanto, todos os elementos do domínio (entidades, objetos de valor, serviços, eventos, casos de uso etc.) utilizam nomes em português, refletindo fielmente a linguagem ubíqua do negócio.    
```

---

## Conceitos-Chave

| Conceito | Descrição |
|-----------|------------|
| **Domain Event** | Representa um fato relevante ocorrido no domínio (ex: transferência concluída). |
| **Outbox Pattern** | Armazena eventos numa tabela (`OutboxMessages`) dentro da mesma transação do EF Core. |
| **Interceptor EF Core** | Intercepta o `SaveChanges` para capturar eventos e transformá-los em mensagens Outbox. |
| **Dispatcher (BackgroundService)** | Publica periodicamente as mensagens pendentes (simulado por logs). |

---

## Fluxo da Transferência Bancária

1. O UseCase `TransferenciaUseCase` executa a operação `EfetuarTransferencia` entre duas contas.
2. O agregado `Conta` gera um `TransferenciaRealizadaDomainEvent`.
3. O `OutboxSaveChangesInterceptor` intercepta o `SaveChangesAsync` e:
   - Serializa o evento.
   - Armazena-o na tabela `OutboxMessages`.
4. A transação é confirmada no banco.
5. O `OutboxDispatcher` (background worker) processa periodicamente os registros da Outbox e:
   - Publica o evento (nesta POC apenas simula via log).
   - Atualiza o campo `ProcessedOnUtc`.

---

## Configuração do Projeto

### 1. Requisitos

- .NET 8 SDK ou superior  
- SQLite / SQL Server (configurável) 
- Ferramenta HTTP (Postman, Insomnia, cURL)

### 2. Clonar o repositório

```bash
git clone https://github.com/josemarcosit/bancodigital-api.git
cd bancodigital-api
```

### 3. Configurar a Connection String

No arquivo appsettings.json:
```
{
  "ConnectionStrings": {
     "BancoDigital": "Data Source=banco_digital.db"
  }
}
```

### 4. Executar migrações
```bash
dotnet ef database update
```
Isso criará as tabelas:

* Contas
* OutboxMessages

### 5. Executar a aplicação
```bash
dotnet run
```

A API estará disponível em:
```
https://localhost:5001
```

## Testando

Criar contas
```
POST /conta
{
  "id": "11111111-1111-1111-1111-111111111111",
  "saldoInicial": 1000
}

POST /conta
{
  "id": "22222222-2222-2222-2222-222222222222",
  "saldoInicial": 500
}

```
Realizar uma transferência
```
POST /transferencia?origem=11111111-1111-1111-1111-111111111111&destino=22222222-2222-2222-2222-222222222222&valor=100
```
Resultado esperado:

As contas são atualizadas.
Um registro aparece na tabela OutboxMessages.
O OutboxDispatcher publica o evento (logado no console).

Exemplo de log:
```
[INF] Outbox message TransferenciaRealizadaDomainEvent armazenada 
[INF] Evento TransferenciaRealizada publicado: 11111111-1111-1111-1111-111111111111 -> 22222222-2222-2222-2222-222222222222 : Valor 100 
```