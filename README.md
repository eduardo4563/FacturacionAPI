# 🧾 FacturacionAPI

API REST para gestión de facturación construida con **ASP.NET Core 8**, **SQLite** y autenticación **JWT**.

## 🚀 Demo en vivo

> **Swagger UI:** [https://tu-app.onrender.com](https://tu-app.onrender.com)

> ⏱️ _Si la API tarda en responder la primera vez, espera ~30 segundos (el servidor gratuito se "duerme" por inactividad)._

---

## 🛠️ Tecnologías

- **Framework:** ASP.NET Core 8
- **Base de datos:** SQLite + Entity Framework Core
- **Autenticación:** JWT Bearer Token
- **Documentación:** Swagger / OpenAPI
- **Deploy:** Render (gratis)

---

## 📋 Endpoints

### 🔐 Auth
| Método | Ruta | Descripción |
|--------|------|-------------|
| POST | `/api/auth/register` | Registrar nuevo usuario |
| POST | `/api/auth/login` | Iniciar sesión y obtener token JWT |

### 👥 Customers _(requiere JWT)_
| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/customers` | Listar todos los clientes |
| GET | `/api/customers/{id}` | Obtener cliente por ID |
| POST | `/api/customers` | Crear cliente |
| PUT | `/api/customers/{id}` | Actualizar cliente |
| DELETE | `/api/customers/{id}` | Eliminar cliente |

### 📦 Products _(requiere JWT)_
| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/products` | Listar todos los productos |
| GET | `/api/products/{id}` | Obtener producto por ID |
| POST | `/api/products` | Crear producto |
| PUT | `/api/products/{id}` | Actualizar producto |
| DELETE | `/api/products/{id}` | Eliminar producto |

### 🧾 Invoices _(requiere JWT)_
| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/invoices?page=1&pageSize=10` | Listar facturas (paginado) |
| GET | `/api/invoices/{id}` | Obtener factura por ID |
| POST | `/api/invoices` | Crear factura |
| PUT | `/api/invoices/{id}` | Actualizar factura |
| PUT | `/api/invoices/{id}/cancel` | Anular factura |
| PUT | `/api/invoices/{id}/pay` | Marcar como pagada |
| DELETE | `/api/invoices/{id}` | Eliminar factura |

---

## ⚡ Cómo usar

### 1. Registrar usuario
```http
POST /api/auth/register
Content-Type: application/json

{
  "email": "usuario@email.com",
  "password": "tu-password",
  "name": "Tu Nombre"
}
```

### 2. Iniciar sesión
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "usuario@email.com",
  "password": "tu-password"
}
```
Respuesta: `{ "token": "eyJ..." }`

### 3. Usar el token en las demás peticiones
```http
Authorization: Bearer eyJ...
```

---

## 💻 Ejecutar localmente

### Requisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Pasos
```bash
# Clonar el repositorio
git clone https://github.com/tu-usuario/FacturacionAPI.git
cd FacturacionAPI

# Ejecutar
dotnet run

# La API estará en: http://localhost:8080
# Swagger en:       http://localhost:8080
```

---

## 🌐 Deploy en Render

### Variables de entorno (opcionales)
| Variable | Descripción | Default |
|----------|-------------|---------|
| `JWT_SECRET` | Clave secreta para firmar tokens JWT | `MiClaveSecreta12345678901234567890` |
| `DB_PATH` | Ruta del archivo SQLite | `facturacion.db` |
| `PORT` | Puerto del servidor | `8080` |

---

## 📁 Estructura del proyecto

```
FacturacionAPI/
├── Controllers/         # Endpoints de la API
│   ├── AuthController.cs
│   ├── CustomersController.cs
│   ├── InvoicesController.cs
│   └── ProductsController.cs
├── Data/
│   └── ApplicationDbContext.cs   # Contexto de EF Core
├── DTOs/
│   └── DTOs.cs                   # Objetos de transferencia de datos
├── Entities/                     # Modelos de la base de datos
│   ├── Customer.cs
│   ├── Invoice.cs
│   ├── InvoiceDetail.cs
│   ├── Product.cs
│   └── User.cs
├── Repositories/                 # Patrón repositorio
├── Services/                     # Lógica de negocio
├── Program.cs                    # Configuración y arranque
└── FacturacionAPI.csproj
```

---

## 📄 Licencia

MIT
