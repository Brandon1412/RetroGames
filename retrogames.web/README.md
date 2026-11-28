# RetroGames

Sistema web para gerenciamento e consulta de plataformas de videogame, desenvolvido como projeto acadêmico.

O RetroGames tem como objetivo oferecer uma aplicação para organização de informações sobre videogames, com interface web, API e banco de dados relacional.

## Tecnologias utilizadas

### Backend
- C# e .NET 10
- ASP.NET Core Web API
- Dapper
- MySqlConnector
- FluentValidation
- xUnit

### Frontend
- JavaScript
- React 19
- Vite
- React Router
- Axios
- Bootstrap 5

### Banco de dados
- MySQL 8

## Estrutura do projeto

A solução está organizada nos seguintes projetos:

| Projeto | Responsabilidade |
|---|---|
| `RetroGames.Model` | Classes e modelos de domínio |
| `RetroGames.Repository` | Acesso e comunicação com o banco de dados |
| `RetroGames.Services` | Regras e lógica de negócio |
| `RetroGames.WebApi.ApiModels` | Modelos de requisição e resposta da API |
| `RetroGames.WebApi` | API, controladores, autenticação e validações |
| `RetroGames.WebApi.UnitTests` | Testes automatizados |
| `retrogames.web` | Aplicação frontend em React |

Outros diretórios importantes:

- `database`: scripts SQL para criação, estrutura e dados do banco.
- `backup-*`: cópias de segurança realizadas durante o desenvolvimento.
- `AGENTS.md`: orientações do projeto para ferramentas de desenvolvimento.

## Funcionalidades

### Autenticação
- Login de administrador.
- Controle de sessão por cookie.
- Encerramento de sessão.
- Consulta dos dados do usuário autenticado.
- Proteção de operações administrativas.

### Gerenciamento de plataformas
- Cadastro de plataformas.
- Consulta de plataformas cadastradas.
- Edição de informações.
- Exclusão de registros.
- Controle de status.
- Validação dos campos.
- Exibição de informações opcionais como "Não informado".

Os dados das plataformas incluem:

- Nome.
- Fabricante.
- Ano de lançamento.
- Categoria.
- Outras informações.
- Status.

As categorias disponíveis são:
- Console de mesa.
- Console portátil.
- Arcade.
- Híbrido.
- Outro.

O nome é obrigatório. Os demais campos podem ser deixados sem preenchimento.

A validação do ano de lançamento considera o intervalo entre 1972 e o ano atual, calculado automaticamente. O campo também pode ficar vazio.

## Requisitos para execução

Antes de iniciar, instale:

- Windows 10 ou superior.
- .NET SDK 10.
- Node.js e npm.
- MySQL Server 8.
- Git.

Também é necessário ter um banco de dados MySQL configurado e em execução.

## Configuração do banco de dados

Os scripts SQL estão no diretório `database`.

| Arquivo | Finalidade |
|---|---|
| `01_create_tables.sql` | Criação das tabelas |
| `02_corrigir_estrutura.sql` | Ajustes na estrutura do banco |
| `03_dados_exemplo.sql` | Inserção de dados de exemplo |
| `04_consultas.sql` | Consultas SQL |
| `05_testes_integridade.sql` | Testes de integridade |
| `06_atualizar_plataformas.sql` | Atualização de informações das plataformas |

Para preparar um banco novo, execute os scripts de criação, correção e dados de exemplo, nessa ordem, utilizando o MySQL.

**Atenção:** o script `database/executar_banco.ps1` contém um comando `DROP DATABASE` e apaga o banco `retrogames` antes de recriá-lo. Portanto, ele não deve ser executado em um banco com dados que precisam ser preservados sem antes realizar um backup.

O arquivo `06_atualizar_plataformas.sql` é uma atualização específica dos registros existentes. Verifique seu conteúdo antes de executá-lo em outro banco.

## Configuração e execução do backend

Abra um terminal PowerShell na pasta raiz do projeto:

    cd RetroGames

Restaure as dependências:

    dotnet restore RetroGames.sln

Configure a conexão com o MySQL por meio da variável de ambiente:

    $env:ConnectionStrings__RetroGames = Read-Host "String de conexão do MySQL"

Informe a conexão diretamente no terminal. Não compartilhe a senha nem a armazene no repositório.

Configure os dados do administrador inicial:

    $env:AdminInicial__Email = Read-Host "Email do administrador"

    $senhaSegura = Read-Host "Senha do administrador" -AsSecureString

    $ponteiro = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($senhaSegura)

    try {
        $env:AdminInicial__Senha = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($ponteiro)
    } finally {
        [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($ponteiro)
    }

    $env:AdminInicial__Nome = "Administrador"

Inicie a API:

    dotnet run --project RetroGames.WebApi

A API estará disponível em:

    http://localhost:5267

As variáveis de ambiente precisam ser configuradas novamente quando uma nova sessão do PowerShell for aberta.

## Configuração e execução do frontend

Abra outro terminal PowerShell:

    cd retrogames.web

Instale as dependências:

    npm.cmd install

Inicie o servidor de desenvolvimento:

    npm.cmd run dev

O frontend estará disponível em:

    http://localhost:5173

Para gerar a versão de produção:

    npm.cmd run build

## Testes automatizados

Os testes utilizam xUnit.

Para executar todos os testes da solução, utilize, na pasta raiz:

    dotnet test RetroGames.sln

Os testes incluem verificações da validação do ano de lançamento das plataformas, contemplando:

- Ano inferior ao mínimo permitido.
- Ano mínimo permitido.
- Ano atual.
- Ano futuro.
- Ano não informado.

A última execução registrada apresentou seis testes aprovados, sem falhas.

## Endpoints principais

### Autenticação

| Método | Endpoint | Descrição |
|---|---|---|
| POST | `/api/Autenticacao/login` | Realiza login |
| POST | `/api/Autenticacao/logout` | Encerra a sessão |
| GET | `/api/Autenticacao/eu` | Consulta a sessão atual |

### Plataformas

| Método | Endpoint | Descrição |
|---|---|---|
| GET | `/api/Plataforma` | Lista as plataformas |
| GET | `/api/Plataforma/{id}` | Consulta uma plataforma |
| POST | `/api/Plataforma` | Cadastra uma plataforma |
| PUT | `/api/Plataforma/{id}` | Atualiza uma plataforma |
| DELETE | `/api/Plataforma/{id}` | Exclui uma plataforma |

As operações de alteração de plataformas exigem autenticação e autorização administrativa.

## Segurança

- Não publique senhas ou strings de conexão no GitHub.
- Utilize variáveis de ambiente para configurações sensíveis.
- Não versione arquivos que contenham credenciais.
- Faça backups antes de executar scripts que alterem ou excluam dados.
- Utilize credenciais próprias para cada ambiente.

## Desenvolvimento

O projeto utiliza uma arquitetura organizada em camadas, separando modelos, repositórios, serviços, modelos da API e controladores.

Essa organização facilita a manutenção, os testes e a evolução das funcionalidades.

---

**RetroGames**
Projeto acadêmico desenvolvido com .NET, React e MySQL.
