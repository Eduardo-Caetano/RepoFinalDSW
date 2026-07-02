# Observatório Parlamentar de Santa Catarina

Aplicação web acadêmica desenvolvida para consulta e visualização de dados públicos sobre deputados federais de Santa Catarina.

O projeto utiliza .NET 8, ASP.NET Core Web API, React, Vite e PostgreSQL para reunir informações sobre deputados, despesas parlamentares, atuação legislativa, presença em plenário e participação em comissões.

## Tecnologias utilizadas

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- React
- Vite
- Recharts
- Axios
- Swagger
- HtmlAgilityPack

## Funcionalidades

- Landing page institucional
- Listagem de deputados federais de Santa Catarina
- Página de detalhes do deputado
- CRUD da entidade Deputado
- Importação de deputados pela API pública da Câmara dos Deputados
- Importação de despesas parlamentares
- Dashboard financeiro com métricas e gráficos
- Scraping estruturado de dados de atuação parlamentar
- Persistência dos dados no PostgreSQL
- Dashboard de presença em plenário e comissões
- Filtro por ano nos dashboards e despesas

## Modelagem do banco

### Deputado

Representa um deputado federal cadastrado no sistema.

Principais campos:

- Id
- IdCamara
- Nome
- NomeEleitoral
- SiglaPartido
- SiglaUf
- Email
- UrlFoto

### Despesa

Representa uma despesa parlamentar vinculada a um deputado.

Principais campos:

- Id
- TipoDespesa
- Fornecedor
- Valor
- DataDoc
- DeputadoId

### PerfilAnalise

Representa uma estrutura reservada para análises textuais associadas a um deputado.

Principais campos:

- Id
- Resumo
- Observacoes
- DeputadoId

### AtuacaoParlamentar

Representa os indicadores de atuação parlamentar persistidos no banco.

Principais campos:

- Id
- DeputadoId
- Propostas
- Votacoes
- Discursos
- PresencasPlenario
- AusenciasJustificadasPlenario
- AusenciasNaoJustificadasPlenario
- PresencasComissoes
- AusenciasJustificadasComissoes
- AusenciasNaoJustificadasComissoes
- ComissoesTitular
- ComissoesSuplente
- DataAtualizacao

### Relacionamentos

- Deputado 1:N Despesas
- Deputado 1:1 PerfilAnalise
- Deputado 1:1 AtuacaoParlamentar

## Pré-requisitos

- .NET 8 SDK
- Node.js
- PostgreSQL
- Git
- Ferramenta `dotnet-ef` instalada para execução das migrations

Instalação do `dotnet-ef`, caso necessário:

```bash
dotnet tool install --global dotnet-ef
```

## Como executar localmente

Fluxo completo para executar o projeto após clonar o repositório:

1. Clonar o repositório:

```bash
git clone URL_DO_REPOSITORIO
cd NOME_DA_PASTA
```

2. Criar o banco no PostgreSQL:

Nome sugerido:

```text
observatorio_sc
```

3. Configurar a connection string no backend:

Arquivo:

```text
backend/ObservatorioApi/appsettings.json
```

Exemplo:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=observatorio_sc;Username=postgres;Password=SUA_SENHA"
  }
}
```

4. Rodar as migrations:

```bash
cd backend/ObservatorioApi
dotnet restore
dotnet ef database update
```

5. Rodar o backend:

```bash
dotnet run
```

6. Abrir o Swagger:

```text
http://localhost:5022/swagger
```

7. Popular o banco pelo Swagger, nesta ordem:

```text
POST /api/importacao/deputados
POST /api/importacao/despesas
POST /api/importacao/atuacao
```

8. Rodar o frontend em outro terminal:

```bash
cd frontend
npm install
npm run dev
```

9. Acessar o sistema:

```text
http://localhost:5173
```

Ou a porta exibida pelo Vite no terminal.

10. Observação:

O backend precisa estar rodando antes do frontend consumir os dados. Caso a porta do backend seja diferente, ajuste a `baseURL` dos arquivos em `frontend/src/api`.

## Como executar o backend

1. Entrar na pasta do backend:

```bash
cd backend/ObservatorioApi
```

2. Configurar o arquivo `appsettings.json` com a connection string do PostgreSQL.

Exemplo usado no projeto:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=observatorio_sc;Username=postgres;Password=admin"
  }
}
```

3. Restaurar os pacotes:

```bash
dotnet restore
```

4. Aplicar as migrations:

```bash
dotnet ef database update
```

5. Executar a API:

```bash
dotnet run
```

6. Acessar o Swagger:

```text
http://localhost:5022/swagger
```

## Como executar o frontend

1. Entrar na pasta do frontend:

```bash
cd frontend
```

2. Instalar as dependências:

```bash
npm install
```

3. Executar o servidor de desenvolvimento:

```bash
npm run dev
```

4. Acessar a URL exibida pelo Vite.

Exemplo:

```text
http://localhost:5173
```

Observação: o backend está configurado com CORS para `http://localhost:5174`. Caso o Vite execute em outra porta, ajuste a configuração de CORS em `backend/ObservatorioApi/Program.cs` ou execute o frontend na porta permitida.

## Endpoints principais

### Importação

- `POST /api/importacao/deputados`
- `POST /api/importacao/despesas`
- `POST /api/importacao/atuacao`

### Deputados

- `GET /api/deputados`
- `GET /api/deputados/{id}`
- `POST /api/deputados`
- `PUT /api/deputados/{id}`
- `DELETE /api/deputados/{id}`

### Despesas e dashboards

- `GET /api/deputados/{id}/despesas`
- `GET /api/deputados/{id}/dashboard-financeiro`
- `GET /api/dashboard/atuacao/{id}`

Alguns endpoints aceitam parâmetro opcional de ano:

```text
GET /api/deputados/{id}/despesas?ano=2026
GET /api/deputados/{id}/dashboard-financeiro?ano=2026
GET /api/dashboard/atuacao/{id}?ano=2026
```

## Ordem recomendada para popular o banco

1. `POST /api/importacao/deputados`
2. `POST /api/importacao/despesas`
3. `POST /api/importacao/atuacao`

## Fontes de dados

Os dados de deputados e despesas parlamentares são obtidos pela API pública Dados Abertos da Câmara dos Deputados.

Exemplos de fontes utilizadas:

- `https://dadosabertos.camara.leg.br/api/v2/deputados?siglaUf=SC`
- `https://dadosabertos.camara.leg.br/api/v2/deputados/{idCamara}/despesas`

Os dados de presença, ausência e parte da atuação parlamentar são obtidos a partir da página oficial da Câmara dos Deputados por scraping estruturado com HtmlAgilityPack, pois nem todos esses dados estão disponíveis diretamente na API pública em formato adequado para o projeto.

Exemplo de página usada no scraping:

```text
https://www.camara.leg.br/deputados/{idCamara}
```

## Estrutura de pastas

```text
backend/
└── ObservatorioApi/
    ├── Controllers/
    ├── Data/
    ├── DTOs/
    │   └── Atuacao/
    ├── Migrations/
    ├── Models/
    ├── Services/
    ├── appsettings.json
    └── Program.cs

frontend/
└── src/
    ├── api/
    ├── assets/
    ├── pages/
    ├── App.jsx
    ├── main.jsx
    └── styles.css
```

## Integração frontend e backend

O frontend consome a API pelo endereço:

```text
http://localhost:5022/api
```

Esse endereço está configurado nos arquivos:

- `frontend/src/api/deputadoApi.js`
- `frontend/src/api/dashboardApi.js`
- `frontend/src/api/atuacaoApi.js`


## Possíveis melhorias futuras

- Filtro por ano
- Ranking de deputados
- Comparação entre parlamentares
- Explicação simplificada de projetos de lei
- Integração com dados da ALESC e Senado
- Módulo de IA para linguagem cidadã

## Observações acadêmicas

Este projeto foi desenvolvido como protótipo acadêmico para organização, consulta e visualização de dados públicos. A proposta é facilitar o acesso a informações parlamentares e apoiar estudos sobre transparência pública, controle social e acompanhamento da atividade legislativa.
