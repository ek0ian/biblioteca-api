# Princípios SOLID Aplicados

Este documento descreve como os cinco princípios SOLID foram aplicados no backend da Biblioteca API.

---

## S — Single Responsibility Principle (Princípio da Responsabilidade Única)

> *"Uma classe deve ter apenas um motivo para mudar."*

### Onde foi aplicado

Cada camada da aplicação possui uma única responsabilidade bem definida:

| Classe / Arquivo | Responsabilidade única |
|---|---|
| `LivroRepository.cs` | Exclusivamente acesso a dados (MongoDB) para livros |
| `LivroService.cs` | Exclusivamente regras de negócio dos livros |
| `LivrosController.cs` | Exclusivamente receber requisições HTTP e devolver respostas |
| `ErrorHandlingMiddleware.cs` | Exclusivamente capturar e formatar erros não tratados |
| `AuthService.cs` | Exclusivamente autenticação: hash de senha e geração de JWT |

### Justificativa

O `LivrosController` não acessa o banco diretamente, nem valida regras de negócio. O `LivroService` não sabe nada sobre HTTP. O `LivroRepository` não conhece regras de negócio. Cada classe muda por um único motivo: alterações no banco afetam apenas o repositório; alterações nas regras de negócio afetam apenas o serviço; alterações na API afetam apenas o controller.

---

## I — Interface Segregation Principle (Princípio da Segregação de Interfaces)

> *"Nenhum cliente deve ser forçado a depender de métodos que não usa."*

### Onde foi aplicado

As interfaces de repositório e serviço são segregadas por entidade:

```
Repositories/Interfaces/
├── ILivroRepository.cs       → métodos exclusivos de livros
├── IEmprestimoRepository.cs  → métodos exclusivos de empréstimos
└── IUsuarioRepository.cs     → métodos exclusivos de usuários

Services/Interfaces/
├── ILivroService.cs          → operações de livros
├── IEmprestimoService.cs     → operações de empréstimos
└── IAuthService.cs           → autenticação
```

### Justificativa

Ao invés de uma única interface `IRepository<T>` genérica com todos os métodos possíveis, cada contrato define apenas o que aquela entidade precisa. `IUsuarioRepository`, por exemplo, não expõe `DeletarAsync` pois usuários não são deletados na regra de negócio atual. O `EmprestimoService` depende de `ILivroRepository` apenas para `ObterPorIdAsync` e `AtualizarQuantidadeAsync`, sem ser exposto aos demais métodos.

---

## D — Dependency Inversion Principle (Princípio da Inversão de Dependência)

> *"Módulos de alto nível não devem depender de módulos de baixo nível. Ambos devem depender de abstrações."*

### Onde foi aplicado

**1. Controllers dependem de interfaces de serviço, não de implementações:**

```csharp
// LivrosController.cs
public class LivrosController : ControllerBase
{
    private readonly ILivroService _livroService; // ← interface, não LivroService

    public LivrosController(ILivroService livroService) { ... }
}
```

**2. Serviços dependem de interfaces de repositório, não de implementações:**

```csharp
// LivroService.cs
public class LivroService : ILivroService
{
    private readonly ILivroRepository _livroRepository; // ← interface, não LivroRepository

    public LivroService(ILivroRepository livroRepository) { ... }
}
```

**3. Injeção de dependência registrada no `Program.cs`:**

```csharp
// Program.cs
builder.Services.AddSingleton<ILivroRepository, LivroRepository>();
builder.Services.AddScoped<ILivroService, LivroService>();
```

**4. Configurações injetadas via `IOptions<T>`:**

```csharp
// LivroRepository.cs
public LivroRepository(IOptions<MongoDbSettings> settings) { ... }
```

### Justificativa

Toda a aplicação depende de abstrações (interfaces), não de classes concretas. Isso significa que é possível substituir `LivroRepository` por outra implementação (ex: outro banco NoSQL) sem alterar o `LivroService`. Nos testes unitários, essa inversão é aproveitada diretamente: `Moq` injeta mocks das interfaces sem precisar de banco de dados real.

---

## O — Open/Closed Principle (Princípio Aberto/Fechado)

> *"Entidades de software devem ser abertas para extensão, mas fechadas para modificação."*

### Onde foi aplicado

O `ErrorHandlingMiddleware` utiliza um `switch expression` para mapear tipos de exceção a status HTTP. Para adicionar um novo tipo de erro, basta acrescentar um novo caso sem modificar a lógica existente:

```csharp
// ErrorHandlingMiddleware.cs
var (statusCode, mensagem) = ex switch
{
    KeyNotFoundException       => (HttpStatusCode.NotFound,           ex.Message),
    InvalidOperationException  => (HttpStatusCode.BadRequest,         ex.Message),
    UnauthorizedAccessException=> (HttpStatusCode.Unauthorized,       ex.Message),
    ArgumentException          => (HttpStatusCode.BadRequest,         ex.Message),
    _                          => (HttpStatusCode.InternalServerError, "Erro interno.")
    // ↑ Novos tipos de exceção são adicionados aqui, sem modificar o restante
};
```

Adicionalmente, novos serviços ou repositórios podem ser registrados no contêiner de DI do `Program.cs` sem modificar as classes existentes — apenas adicionando novas implementações que atendem às interfaces já definidas.

### Justificativa

O sistema está "fechado" para modificação nas classes existentes e "aberto" para extensão: ao criar uma nova entidade (ex: `Autor`), basta adicionar `IAutorRepository`, `AutorRepository`, `IAutorService`, `AutorService` e `AutoresController` sem alterar nenhum arquivo existente.

---

## L — Liskov Substitution Principle (Princípio da Substituição de Liskov)

> *"Objetos de uma classe derivada devem poder substituir objetos da classe base sem alterar o comportamento correto do programa."*

### Onde foi aplicado

As implementações concretas de repositório (`LivroRepository`, `EmprestimoRepository`, `UsuarioRepository`) respeitam integralmente os contratos definidos pelas interfaces correspondentes:

```csharp
public class LivroRepository : ILivroRepository
{
    // Implementa todos os métodos com o comportamento esperado pelo contrato
    public async Task<List<Livro>> ObterTodosAsync() => ...
    public async Task<Livro?> ObterPorIdAsync(string id) => ...
    // ...
}
```

### Justificativa

Qualquer código que receba um `ILivroRepository` funcionará corretamente com `LivroRepository` ou com qualquer mock criado pelo Moq nos testes. Os testes unitários comprovam isso: o `LivroService` recebe um `Mock<ILivroRepository>` no lugar da implementação real e o comportamento esperado é mantido. A substituição é transparente — o serviço não percebe a diferença.

---

## Resumo

| Princípio | Sigla | Onde aparece |
|---|---|---|
| Single Responsibility | **S** | Controllers, Services, Repositories e Middleware com responsabilidades separadas |
| Open/Closed | **O** | `ErrorHandlingMiddleware` extensível via switch; arquitetura aberta a novas entidades |
| Liskov Substitution | **L** | Repositórios substituíveis por mocks nos testes sem alterar comportamento |
| Interface Segregation | **I** | Interfaces separadas por entidade (`ILivroRepository`, `IEmprestimoRepository`, etc.) |
| Dependency Inversion | **D** | Controllers e Services dependem de abstrações; DI configurada em `Program.cs` |
