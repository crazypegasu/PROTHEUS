# PROTHEUS - Visualizador de Câmeras IP

PROTHEUS é um software gratuito desenvolvido em C# com Avalonia UI para visualizar imagens de até 8 câmeras de segurança IP simultaneamente. O foco inicial são câmeras simples com suporte ao protocolo ONVIF, especialmente modelos da Intelbras.

---

## Funcionalidades Atuais

- Interface gráfica simples e responsiva, com tela dividida em até 8 quadrantes para visualização.
- Menu superior com botões: Configurações, Atualizar e Sair.
- Tela de configurações para cadastro dos dados (IP, usuário, senha) de até 8 câmeras.
- Salvamento automático das configurações em arquivo JSON (`config.json`).
- Carregamento automático das configurações na abertura do programa.

---

## Tecnologias Utilizadas

- C# (.NET 7 ou superior)
- Avalonia UI (framework multiplataforma para interfaces gráficas)
- System.Text.Json para manipulação de JSON

---

## Como Rodar

1. Clone o repositório:
   ```bash
   git clone https://github.com/seuusuario/protheus.git
   cd protheus