# 🧾 Sistoque

Sistoque é um projeto experimental criado com o objetivo de praticar **testes unitários** em um ambiente realista e controlado, utilizando C# e .NET.

> Este projeto não visa produção — é um laboratório de aprendizado.


![.NET](https://img.shields.io/badge/.NET-9.0-blueviolet)
![License](https://img.shields.io/github/license/Hugo-Oliveira-RD11/sistoque)
![Last Commit](https://img.shields.io/github/last-commit/Hugo-Oliveira-RD11/sistoque)
![Open Issues](https://img.shields.io/github/issues/Hugo-Oliveira-RD11/sistoque)

---

## 🚀 Como Rodar

Antes de tudo, certifique-se de ter o SDK do [.NET 9.0](https://dotnet.microsoft.com/en-us/download) instalado.

### 🧱 1. Atualizar o banco de dados:

```bash
dotnet ef database update
```

> Obs: O projeto já vem com as migrations configuradas.

### ▶️ 2. Executar a aplicação:

```bash
dotnet run
```

---

## 🎯 Motivo do Projeto

A motivação principal do Sistoque é **aprender na prática o desenvolvimento orientado a testes unitários**.

O foco foi simular um sistema simples e aplicar testes que garantissem o comportamento esperado das regras de negócio desde o início do desenvolvimento.

---

## 📚 Aprendizados

Durante o desenvolvimento, vários pontos ficaram claros:

- Criar testes é importante — mas **escrever código testável** é o que realmente importa.
- **Arquitetura fraca impacta diretamente a testabilidade.**
- Uma estrutura de testes muito separada e segmentada dificulta a manutenção: a cada refatoração, era necessário revisar dezenas de arquivos.

E o mais importante:

> Depois de estudar Clean Architecture, ficou cada vez mais difícil **não querer aplicá-la**.  
> A separação clara de responsabilidades diminui drasticamente o impacto de mudanças, facilita testes e reduz dores de refatoração com o tempo.

---

## 🛠️ Tecnologias

- C# (.NET 9)
- Entity Framework Core
- xUnit
- Moq
- FluentValidation
- Scalar
- SQLite (so para testes da aplicacao)

---

