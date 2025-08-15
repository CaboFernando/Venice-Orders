# Venice Orders API

## **Arquitetura**

O projeto segue **Clean Architecture + DDD + CQRS**:

* **API**: Camada de entrada, expõe endpoints REST.
* **Application**: Lógica de aplicação, comandos e queries.
* **Domain**: Entidades, agregados, regras de negócio.
* **Infrastructure**: Persistência (SQL Server, MongoDB), cache (Redis), integrações externas (Service Bus).

---

## **Pré-requisitos**

* Docker & Docker Compose
* .NET 9 SDK (para build local, opcional se usar Docker)
* Azure Subscription (para Service Bus, opcional para teste local com conexão fake)

---

## **Rodar local com Docker**

### **1. Configurar variáveis de ambiente**

```powershell
set SERVICEBUS_CONNECTION=Endpoint=sb://sb-veniceorders.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=G+24Ub4m3eHAvoabMvtTp1GbVxHrjul/E+ASbMECbLs=
set JWT_KEY=Fm8G+Xq2Y5k3Lw4oPz9nJ7y6aEa5Hw0oZ0I4bC3T+uR=
```

### **2. Rodar containers**

```powershell
docker compose up --build
```

O `docker-compose.yml` irá subir:

* **API**: [http://localhost:8080](http://localhost:8080)
* **SQL Server**: localhost:1433
* **MongoDB**: localhost:27017
* **Redis**: localhost:6379

---

## **Endpoints principais**

* **Login**

```http
POST /api/v1/auth/login
Body: { "username": "...", "password": "..." }
```

* **Criar Pedido**

```http
POST /api/v1/pedidos
Body: { "clienteId": "...", "itens": [...] }
```

* **Consultar Pedido**

```http
GET /api/v1/pedidos/{pedidoId}
```

---

## **Provisionar Azure Service Bus**

1. **Criar Grupo de Recursos**

   * Nome: `rg-veniceorders`
   * Região: Brazil South

2. **Criar Namespace**

   * Nome: `sb-veniceorders` *(único)*
   * Tipo de preço: Basic

3. **Criar Fila**

   * Nome: `pedido-criado`

4. **Obter Connection String**

   * No item 1 tem as informações de conexão do service bus que já está criada e pronta pra uso

---

## **Banco de dados**

### **SQL Server**

* Database: `VeniceOrders`
* Usuário: `sa`
* Senha: `sql123##`

### **MongoDB**

* Database: `venice_orders`

### **Redis**

* Host: `redis`
* Porta: 6379

---

## **Observações**

* Para teste local, é possível criar um **Service Bus fake** usando `FakeServiceBus` ou um mock para não gerar custos.
* JWT usado para autenticação dos endpoints.
* CQRS: comandos (writes) via `/api/v1/pedidos` e queries (reads) via `/api/v1/pedidos/{id}`.

---

