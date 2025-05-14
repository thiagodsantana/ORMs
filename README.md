```markdown
# 🧩 ORMs em C# – Comparativo de Frameworks de Acesso a Dados

Este repositório apresenta um estudo comparativo entre diferentes ORMs (Object-Relational Mappers) em C#, com foco em desempenho, facilidade de uso e flexibilidade. O objetivo é fornecer uma visão prática sobre como cada ORM se comporta em cenários reais de desenvolvimento.

## 🛠️ Tecnologias Utilizadas

- **ORMs Comparados:**
  - [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
  - [Dapper](https://dapper-tutorial.net/)
  - [NHibernate](https://nhibernate.info/)
  - [ServiceStack.OrmLite](https://github.com/ServiceStack/ServiceStack.OrmLite)

- **Banco de Dados:**
  - SQL Server (ou outro banco relacional compatível)

- **Framework:**
  - .NET 6.0 ou superior

## 📁 Estrutura do Projeto

```

ORMs/
├── CsharpORM.ApiService/           # API RESTful para expor operações CRUD
├── CsharpORM.AppHost/              # Inicialização e configuração da aplicação
├── CsharpORM.Benchmark/            # Scripts para benchmarks de desempenho
├── CsharpORM.Dapper/               # Implementação utilizando Dapper
├── CsharpORM.Data/                 # Repositórios e acesso a dados
├── CsharpORM.Domain/               # Modelos de domínio e entidades
├── CsharpORM.Performance/          # Ferramentas para análise de desempenho
├── CsharpORM.ServiceDefaults/      # Configurações padrão de serviços
├── CsharpORM.Web/                  # Interface web para interação com a API
└── CsharpORM.sln                   # Solução do Visual Studio

````

## 🚀 Como Executar o Projeto

1. **Clonar o repositório:**

   ```bash
   git clone https://github.com/thiagodsantana/ORMs.git
   cd ORMs
````

2. **Abrir a solução no Visual Studio:**

   * Abra o arquivo `CsharpORM.sln`.

3. **Restaurar pacotes NuGet:**

   * No Visual Studio, clique com o botão direito na solução e selecione "Restaurar pacotes NuGet".

4. **Executar a aplicação:**

   * Defina o projeto `CsharpORM.ApiService` como o projeto inicial.
   * Pressione `F5` ou clique em "Iniciar" para rodar a aplicação.

5. **Acessar a API:**

   * A API estará disponível em `https://localhost:5001` por padrão.

## 📊 Benchmarks

O diretório `CsharpORM.Benchmark` contém scripts para comparar o desempenho das operações CRUD utilizando os diferentes ORMs. Os benchmarks incluem:

* Tempo de execução para inserção de registros.
* Tempo de execução para leitura de registros.
* Tempo de execução para atualização de registros.
* Tempo de execução para exclusão de registros.

## 📄 Licença

Este projeto está licenciado sob a [MIT License](LICENSE).
