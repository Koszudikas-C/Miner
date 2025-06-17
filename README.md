## Version [0.0.1]

### Reconnect Strategy

- [x] Implementar reconexão automática com o servidor: ✅ 2025-06-17
  - [x] Máximo de 3 tentativas consecutivas. ✅ 2025-06-16
  - [x] Intervalo de 10 segundos entre as tentativas. ✅ 2025-06-16
  - [x] Após 3 falhas, aguardar 5 minutos antes de tentar novamente. ✅ 2025-06-16
  - [x] Geração de logs detalhados: ✅ 2025-06-13
    - [x] Indicando número da tentativa. ✅ 2025-06-13
    - [x] Tempo restante para próxima tentativa ou cooldown. ✅ 2025-06-13
    - [x] Motivo da falha (exceção, timeout, etc). ✅ 2025-06-13
  - [ ] Cobertura com testes de unidade:
    - [ ] Simular perda de conexão.
    - [ ] Verificar comportamento em falha sucessiva.
    - [ ] Garantir que o tempo de espera está correto.
    - [ ] Verificar persistência de estado após reconexão bem-sucedida.

