# Resumen

## Objetivo
Implementar un `POST /api/orders` transaccional para registrar pedidos con validación externa, auditoría y manejo robusto de errores.

## Decisiones clave (versión corta)
1. **Arquitectura por capas** (`Api`, `Application`, `Domain`, `Infrastructure`) para separar responsabilidades y facilitar mantenimiento.
2. **MediatR (Command/Handler)** para concentrar la lógica del caso de uso fuera del controller.
3. **FluentValidation + pipeline** para rechazar entradas inválidas antes de tocar BD o servicios externos.
4. **Transacción (Unit of Work)** para asegurar atomicidad: si algo falla, rollback completo.
5. **Validación externa resiliente (Polly)** con retry, timeout y circuit breaker para fallos del proveedor.
6. **Manejo de errores en capas** (middleware global + errores de dominio) con respuestas consistentes.
7. **Auditoría y logging** para registrar inicio, errores y éxito del proceso.
8. **Idempotencia + RequestId** para evitar duplicados y mejorar trazabilidad.
9. **Persistencia con EF Core** con relaciones e índices relevantes (incluyendo `IdempotencyKey` único).

## Valor de la solución
- Cumple requisitos funcionales y no funcionales de la prueba.
- Prioriza consistencia de datos, trazabilidad y robustez operativa.
- Mantiene un diseño claro para evolucionar en contexto empresarial.

## Mejoras futuras sugeridas
- Códigos HTTP más específicos por tipo de error.
- Más pruebas unitarias/integración del flujo transaccional.
- Métricas y observabilidad avanzada.
