# API de Consórcios - MVP

API para transação de cartas de consórcio contempladas desenvolvida em ASP.NET Core 9.0.

## Tecnologias Utilizadas

- ASP.NET Core 9.0 Web API
- Entity Framework Core 9.0
- SQL Server
- JWT para autenticação
- Swagger para documentação

## Estrutura do Projeto

- **Models**: Entidades do sistema (Usuario, CartaConsorcio)
- **DTOs**: Objetos de transferência de dados
- **Controllers**: Controladores da API
- **Services**: Serviços para autenticação e operações
- **Data**: Contexto do Entity Framework e migrações

## Configuração do Ambiente

### Pré-requisitos

- .NET 9.0 SDK
- SQL Server (ou SQL Server LocalDB)
- Visual Studio 2022 ou outro editor de código

### Configuração do Banco de Dados

1. Verifique a string de conexão no arquivo `appsettings.json`
2. Abra o Package Manager Console e execute:

```
Add-Migration Initial
Update-Database
```

Ou, usando o CLI do .NET:

```bash
dotnet ef migrations add Initial
dotnet ef database update
```

## Executando o Projeto

### Usando Visual Studio

1. Abra a solução no Visual Studio
2. Pressione F5 para iniciar o projeto

### Usando CLI do .NET

```bash
dotnet restore
dotnet build
dotnet run --project AppConsorciosMvp
```

## Endpoints da API

A API segue o padrão RESTful e possui os seguintes endpoints principais:

### Usuários

- `POST /api/usuarios/registrar` - Registrar novo usuário
- `POST /api/usuarios/login` - Autenticar usuário
- `GET /api/usuarios/{id}` - Obter detalhes de um usuário
- `POST /api/usuarios/{id}/verificar` - Verificar um usuário (apenas admin)

### Cartas de Consórcio

- `POST /api/cartas` - Criar nova carta (vendedor ou admin)
- `GET /api/cartas` - Listar cartas disponíveis
- `GET /api/cartas/{id}` - Obter detalhes de uma carta
- `GET /api/cartas/pesquisar` - Pesquisar cartas com filtros
- `PUT /api/cartas/{id}/status` - Atualizar status de uma carta
- `POST /api/cartas/{id}/verificar` - Verificar uma carta (apenas admin)

## Autenticação

A API utiliza JWT (JSON Web Tokens) para autenticação. Para acessar endpoints protegidos:

1. Faça login para obter um token
2. Inclua o token no header de autorização das requisições:
   `Authorization: Bearer {seu-token}`

## Papéis de Usuário

- **admin**: Acesso total ao sistema, pode verificar usuários e cartas
- **vendedor**: Pode criar e gerenciar suas próprias cartas
- **comprador**: Acesso básico para visualização de cartas

## Regras de Negócio

- Apenas usuários com papel "vendedor" ou "admin" podem criar cartas
- Apenas o próprio vendedor ou um admin pode alterar o status de uma carta
- Uma carta só pode ser vendida se estiver com status "negociando" ou "disponivel"
- Apenas usuários "admin" podem verificar usuários e cartas
