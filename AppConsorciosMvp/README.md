# API de Consórcios - MVP

API para transação de cartas de consórcio contempladas desenvolvida em ASP.NET Core 9.0.

## Tecnologias Utilizadas

- ASP.NET Core 9.0 Web API
- Entity Framework Core 9.0
- SQL Server
- JWT para autenticação
- Swagger para documentação
- Docker e Docker Compose para contêinerização

## Estrutura do Projeto

- **Models**: Entidades do sistema (Usuario, CartaConsorcio)
- **DTOs**: Objetos de transferência de dados
- **Controllers**: Controladores da API
- **Services**: Serviços para autenticação e operações
- **Data**: Contexto do Entity Framework e migrações
- **Extensions**: Classes de extensão para funcionalidades adicionais

## Configuração do Ambiente

### Pré-requisitos

- .NET 9.0 SDK
- SQL Server (ou SQL Server LocalDB para desenvolvimento local)
- Visual Studio 2022, VS Code ou outro editor de código
- Docker e Docker Compose (opcional, para execução em contêineres)

## Executando o Projeto

### Método 1: Localmente com Visual Studio ou VS Code

1. Clone o repositório
2. Verifique a string de conexão no arquivo `appsettings.json` ou `appsettings.Development.json`
3. Execute as migrações do banco de dados:

   **Com Visual Studio (Package Manager Console):**
   ```
   Add-Migration Initial
   Update-Database
   ```

   **Com CLI do .NET:**
   ```bash
   dotnet ef migrations add Initial
   dotnet ef database update
   ```

4. Execute o projeto:

   **Com Visual Studio:**
   - Abra a solução no Visual Studio
   - Pressione F5 para iniciar o projeto

   **Com CLI do .NET:**
   ```bash
   dotnet restore
   dotnet build
   dotnet run --project AppConsorciosMvp
   ```

5. Acesse a API:
   - Swagger UI: https://localhost:7090 (ou http://localhost:5210)
   - API diretamente: https://localhost:7090/api

### Método 2: Com Docker Compose

1. Clone o repositório
2. No diretório raiz do projeto, execute:

   ```bash
   docker-compose up -d
   ```

3. Acesse a API:
   - Swagger UI: http://localhost:8080
   - API diretamente: http://localhost:8080/api

## Configurações

### Banco de Dados

A aplicação está configurada para usar:

- **Local**: SQL Server LocalDB (desenvolvimento local)
- **Docker**: SQL Server em contêiner

As strings de conexão estão nos arquivos de configuração:

- `appsettings.json`: Configuração padrão
- `appsettings.Development.json`: Configuração para ambiente de desenvolvimento

### Autenticação JWT

A aplicação usa JWT (JSON Web Tokens) para autenticação. A configuração está em:

- Chave secreta: Definida em `appsettings.json` na seção `JWT:Secret`
- Tempo de validade: 24 horas por padrão

## Credenciais Iniciais

Ao iniciar pela primeira vez, o sistema cria um usuário administrador:

- **Email**: admin@consorcios.com
- **Senha**: Admin@123

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

Para acessar endpoints protegidos:

1. Faça login em `/api/usuarios/login` para obter um token
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

## Solução de Problemas

### Problemas com Banco de Dados

- Verifique se o SQL Server está em execução
- Verifique se as strings de conexão estão corretas
- Execute `dotnet ef database update` para aplicar migrações manualmente

### Problemas com Docker

- Execute `docker-compose down` e depois `docker-compose up -d` para reiniciar os contêineres
- Verifique os logs com `docker-compose logs`
