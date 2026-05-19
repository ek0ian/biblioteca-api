# Biblioteca API

API REST para gerenciamento de biblioteca, desenvolvida como trabalho prático semestral da disciplina **Arquitetura de Aplicações Web — 2026.1**.

O sistema permite cadastrar livros, registrar empréstimos e devoluções, com autenticação JWT e controle de acesso por perfis (Admin / Usuário).

---

## Domínio

| Entidade | Descrição |
|---|---|
| **Livros** | Cadastro do acervo com controle de estoque por exemplares |
| **Empréstimos** | Registro de retirada e devolução de livros por usuários autenticados |
| **Usuários** | Registro e login com perfis Admin e Usuário |

---

## Stack Tecnológica

| Camada | Tecnologia |
|---|---|
| Backend | .NET 10 (C#) / ASP.NET Core Web API |
| Banco de dados | MongoDB 7 |
| Autenticação | JWT (JSON Web Token) + BCrypt |
| Documentação | Swagger / OpenAPI (Swashbuckle) |
| Testes | xUnit + Moq |
| Infraestrutura | Docker Compose |

---

## Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (para subir o MongoDB)

---

## Como executar localmente

### 1. Clonar o repositório

```bash
git clone https://github.com/ek0ian/biblioteca-api.git
cd biblioteca-api
```

### 2. Subir o MongoDB com Docker

```bash
docker-compose up -d
```

Isso sobe um container MongoDB na porta `27017` com usuário `admin` e senha `senha123`.

### 3. Configurar variáveis de ambiente

Copie o arquivo de exemplo e ajuste os valores:

```bash
cp .env.example .env
```

As variáveis utilizadas pela aplicação são lidas automaticamente via `appsettings.Development.json` em desenvolvimento. Para produção, defina:

| Variável | Descrição | Exemplo |
|---|---|---|
| `MONGODB_CONNECTION_STRING` | String de conexão do MongoDB | `mongodb://admin:senha@localhost:27017` |
| `JWT_SECRET` | Chave secreta para assinar tokens JWT (mín. 32 chars) | `minha-chave-secreta-longa` |

### 4. Executar a API

```bash
cd src/BibliotecaApi
dotnet run
```

A API estará disponível em `http://localhost:5000`.

---

## Documentação Swagger

Acesse a interface interativa em:

```
http://localhost:5000/swagger
```

Para testar endpoints protegidos:
1. Use `POST /api/auth/registrar` ou `POST /api/auth/login` para obter um token
2. Clique em **Authorize** (cadeado) no Swagger e cole o token no formato `Bearer <token>`

---

## Endpoints principais

### Autenticação
| Método | Rota | Descrição | Autenticação |
|---|---|---|---|
| POST | `/api/auth/registrar` | Registra novo usuário | Pública |
| POST | `/api/auth/login` | Login e retorno do JWT | Pública |

### Livros
| Método | Rota | Descrição | Autenticação |
|---|---|---|---|
| GET | `/api/livros` | Lista todos os livros | Pública |
| GET | `/api/livros/{id}` | Busca livro por ID | Pública |
| POST | `/api/livros` | Cadastra novo livro | Admin |
| PATCH | `/api/livros/{id}` | Atualiza dados do livro | Admin |
| DELETE | `/api/livros/{id}` | Remove livro | Admin |

### Empréstimos
| Método | Rota | Descrição | Autenticação |
|---|---|---|---|
| GET | `/api/emprestimos` | Lista todos os empréstimos | Admin |
| GET | `/api/emprestimos/meus` | Lista empréstimos do usuário logado | Usuário |
| GET | `/api/emprestimos/{id}` | Busca empréstimo por ID | Autenticado |
| POST | `/api/emprestimos` | Registra novo empréstimo | Autenticado |
| PATCH | `/api/emprestimos/{id}/devolver` | Registra devolução | Autenticado |
| DELETE | `/api/emprestimos/{id}` | Remove empréstimo | Admin |

---

## Frontend

Abra o arquivo `frontend/index.html` diretamente no navegador. O frontend consome a API via `fetch` com navegação assíncrona (sem recarregar a página).

> Certifique-se de que a API esteja rodando em `http://localhost:5000` antes de abrir o frontend.

---

## Testes unitários

```bash
dotnet test
```

Os testes cobrem a camada de serviços (`LivroService` e `EmprestimoService`) com cenários de sucesso e erro usando xUnit e Moq.

---

## Variáveis de ambiente

| Variável | Obrigatória | Descrição |
|---|---|---|
| `MONGODB_CONNECTION_STRING` | Sim | String de conexão MongoDB |
| `JWT_SECRET` | Sim | Chave secreta JWT (mínimo 32 caracteres) |
| `JWT_EXPIRATION_HOURS` | Não (padrão: 8) | Tempo de expiração do token em horas |
| `MONGO_USERNAME` | Não (docker) | Usuário root do MongoDB (docker-compose) |
| `MONGO_PASSWORD` | Não (docker) | Senha root do MongoDB (docker-compose) |

> Nunca comite valores reais de senhas ou chaves no repositório. Use `.env` (já ignorado pelo `.gitignore`).

---

## Estrutura do projeto

```
biblioteca-api/
├── src/
│   └── BibliotecaApi/
│       ├── Controllers/      # Endpoints REST
│       ├── DTOs/             # Objetos de transferência de dados
│       ├── Middleware/       # Tratamento global de erros
│       ├── Models/           # Entidades do domínio
│       ├── Repositories/     # Acesso ao MongoDB
│       ├── Services/         # Regras de negócio
│       └── Settings/         # Configurações tipadas
├── tests/
│   └── BibliotecaApi.Tests/  # Testes unitários (xUnit + Moq)
├── frontend/
│   └── index.html            # Frontend SPA assíncrono
├── docker-compose.yml
├── .env.example
└── SOLID.md
```
