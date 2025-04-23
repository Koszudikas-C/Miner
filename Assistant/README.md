# Assistant

Este módulo implementa o cliente/trabalhador (`WorkClientBlockChain`), responsável por receber comandos remotos, executar tarefas e comunicar resultados à API.

## Estrutura
- **WorkClientBlockChain/**: Lógica principal do cliente, gerenciamento de contexto, conexão e execução de tarefas.
- **Assistant.sln**: Solução Visual Studio para build e organização do módulo.

## Principais responsabilidades
- Gerenciar conexão segura (SSL/Sockets)
- Executar operações solicitadas pela API
- Reportar status e resultados
