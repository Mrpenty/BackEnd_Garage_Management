# Multi-Branch Garage Management — Design Spec

**Date:** 2026-04-19
**Author:** KhanhDV
**Status:** Draft for review

## 1. Background

Yêu cầu từ hội đồng đồ án: hệ thống gara hiện là đơn chi nhánh, cần mở rộng thành chuỗi nhiều chi nhánh với các yêu cầu:
- CRUD chi nhánh
- Gán nhân viên theo chi nhánh
- Gán kho (phụ tùng) theo chi nhánh
- Theo dõi doanh thu theo chi nhánh
- Theo dõi JobCard theo chi nhánh

Hệ thống hiện tại (ASP.NET Core 8 + EF Core + SQL Server) chưa có entity `Branch` nào. Tất cả dữ liệu vận hành (Employee, Inventory, JobCard, Invoice, WorkBay, Appointment, StockTransaction) đều phẳng, không có khái niệm chi nhánh.

## 2. Goals

- Thêm entity `Branch` và CRUD API
- Gán mỗi nhân viên vào đúng 1 chi nhánh
- Tách tồn kho phụ tùng theo chi nhánh
- Mỗi JobCard/Appointment/Invoice/WorkBay/StockTransaction thuộc về 1 chi nhánh
- Phân quyền: nhân viên chi nhánh chỉ xem/sửa dữ liệu chi nhánh mình; `Admin` xem toàn hệ thống
- API report doanh thu & job card theo chi nhánh

## 3. Non-goals

- Khách hàng / Xe / Dịch vụ (catalog) / Hãng xe / Hãng phụ tùng / Nhà cung cấp KHÔNG scope theo chi nhánh (vẫn dùng chung)
- Không thêm role mới (giữ nguyên 6 role: `Customer`, `Receptionist`, `Supervisor`, `Mechanic`, `Stocker`, `Admin`)
- Không làm multi-tenant đầy đủ (không có quản lý chuỗi/franchise)
- Không share tồn kho giữa chi nhánh (không có chức năng "điều chuyển phụ tùng")
- Không làm dashboard/biểu đồ chi tiết (time-series, top mechanic, avg job time)
- Unit test cho logic mới sẽ làm ở pha sau (ngoài scope spec này)

## 4. High-level architecture

Giữ nguyên kiến trúc 4 tầng:
- **API**: thêm `BranchesController`, `ReportsController`; endpoint hiện có không đổi URL
- **Application**: thêm `IBranchService`/`BranchService`, `IBranchRepository`/`BranchRepository`, `ICurrentUserService`/`CurrentUserService`; các service hiện có được inject `ICurrentUserService` để filter theo `BranchId`
- **Base**: thêm entity `Branch`, cấu hình Fluent API, 2 migration; thêm `BranchId` vào 7 entity hiện có
- **UnitTest**: cập nhật sau (out of scope)

## 5. Data model

### 5.1 Entity mới: `Branch`

Đặt tại `Garage_Management.Base/Entities/Branches/Branch.cs`, kế thừa `AuditableEntity`.

| Trường | Kiểu | Ràng buộc |
|---|---|---|
| `BranchId` | int (PK, identity) | — |
| `BranchCode` | string | Required, unique index, max 20 |
| `Name` | string | Required, max 200 |
| `Address` | string | Required, max 500 |
| `Phone` | string? | Optional, max 20 |
| `Email` | string? | Optional, max 100 |
| `ManagerEmployeeId` | int? | Optional FK → `Employee.EmployeeId`, `DeleteBehavior.NoAction` |
| `IsActive` | bool | Default true |

**Navigation collections:**
- `ICollection<Employee>` Employees
- `ICollection<Inventory>` Inventories
- `ICollection<JobCard>` JobCards
- `ICollection<Invoice>` Invoices
- `ICollection<Appointment>` Appointments
- `ICollection<WorkBay>` WorkBays
- `ICollection<StockTransaction>` StockTransactions

**Fluent API config:** `Garage_Management.Base/Data/Configurations/BranchConfiguration.cs`
- Unique index trên `BranchCode`
- Required fields: `Name`, `Address`, `BranchCode`
- FK `ManagerEmployeeId` → `Employee` với `OnDelete(NoAction)` tránh cycle

**DbSet:** `public DbSet<Branch> Branches => Set<Branch>();` trong `AppDbContext`.

### 5.2 Thay đổi entity hiện có

Thêm `public int BranchId { get; set; }` + `public Branch Branch { get; set; } = null!;` vào:

| Entity | File | Ý nghĩa BranchId |
|---|---|---|
| `Employee` | `Entities/Accounts/Employee.cs` | Nhân viên thuộc 1 chi nhánh |
| `Inventory` | `Entities/Inventories/Inventory.cs` | Phụ tùng thuộc 1 chi nhánh |
| `JobCard` | `Entities/JobCards/JobCard.cs` | Chi nhánh xử lý phiếu sửa |
| `Invoice` | `Entities/Invoice.cs` | Denormalize từ JobCard (query report nhanh) |
| `Appointment` | `Entities/Accounts/Appointment.cs` | Chi nhánh khách đặt lịch |
| `WorkBay` | `Entities/JobCards/WorkBay.cs` | Khoang sửa thuộc chi nhánh |
| `StockTransaction` | `Entities/Inventories/StockTransaction.cs` | Giao dịch nhập/xuất kho theo chi nhánh |

Tất cả FK: `OnDelete(DeleteBehavior.Restrict)`. Soft delete qua `AuditableEntity.DeletedAt`.

### 5.3 Không thay đổi (giữ global)

`Customer`, `Vehicle`, `VehicleBrand`, `VehicleModel`, `VehicleType`, `Service`, `ServiceTask`, `ServiceVehicleType`, `SparePartBrand`, `SparePartCategory`, `Supplier`, `RepairEstimate` (kế thừa branch qua `JobCard`), các policy bảo hành.

## 6. Migration strategy

Chia làm 2 migration EF Core riêng biệt:

### 6.1 Migration 1: `AddBranchEntity`

- Tạo bảng `Branches`
- Seed 1 chi nhánh mặc định (`BranchId=1`, `BranchCode="HQ-01"`, `Name="Chi nhánh chính"`) trong `InitialSeed.cs`
- Gán tất cả bản ghi hiện có về `BranchId=1` bằng `migrationBuilder.Sql("UPDATE ... SET BranchId = 1")` trong `Up()`

### 6.2 Migration 2: `AddBranchIdToEntities`

- Thêm cột `BranchId` (default value = 1) vào 7 bảng: `Employees`, `Inventories`, `JobCards`, `Invoices`, `Appointments`, `WorkBay`, `StockTransactions`
- Sau khi backfill, alter cột thành NOT NULL
- Tạo FK `OnDelete(Restrict)` tới `Branches`

Tách 2 migration để rollback từng bước khi sự cố.

## 7. Authorization

### 7.1 JWT claims

Sửa `GenerateToken.cs`:
- Staff login (`StaffLoginRequest`) thêm claim `branch_id = Employee.BranchId`
- Admin vẫn có claim `branch_id` của mình (nếu có), nhưng logic bypass filter dựa trên role
- Customer login không có claim này

`LoginResponse` DTO thêm `BranchId`, `BranchName` để frontend hiển thị.

### 7.2 `ICurrentUserService`

Interface mới tại `Garage_Management.Application/Interfaces/Auth/ICurrentUserService.cs`:

```csharp
int? GetCurrentUserId();
int? GetCurrentEmployeeId();
int? GetCurrentBranchId();   // null nếu không có claim
bool IsAdmin();
```

Implementation `CurrentUserService` dùng `IHttpContextAccessor` đọc claims từ `HttpContext.User`. Đăng ký `Scoped` trong DI (tự động qua Scrutor hoặc thêm tay trong `ApplicationDependencyInjection.cs`).

### 7.3 Enforce ở service layer

Mỗi service xử lý entity branch-scoped:

1. Inject `ICurrentUserService`
2. Với query **List/Get**: nếu không phải Admin → append `.Where(x => x.BranchId == currentBranchId)`
3. Với **Create**: `BranchId` trong request bị bỏ qua với non-admin → luôn set từ token. Admin có thể chỉ định `BranchId` tùy ý.
4. Với **Update/Delete**: kiểm tra `entity.BranchId == currentBranchId` trước, sai thì trả `ApiResponse.Failure(403, "Không có quyền truy cập chi nhánh khác")`

### 7.4 Services bị ảnh hưởng

- `EmployeeService`, `InventoryService`, `JobCardService`, `AppointmentService`, `InvoiceService`, `WorkBayService`, `StockTransactionService`
- Service phái sinh:
  - `JobCardSparepartService` — kiểm tra `Inventory.BranchId == JobCard.BranchId` (không cho dùng phụ tùng khác chi nhánh)
  - `PaymentService` — scope qua `Invoice.BranchId`
  - `RepairEstimateService` — scope qua `JobCard.BranchId`

## 8. API design

### 8.1 `BranchesController`

| Method | Route | Role | Mô tả |
|---|---|---|---|
| `POST` | `/api/branches` | Admin | Tạo chi nhánh |
| `PUT` | `/api/branches/{id}` | Admin | Sửa chi nhánh (không đổi BranchCode) |
| `DELETE` | `/api/branches/{id}` | Admin | Soft delete (reject nếu còn Employee/JobCard active) |
| `GET` | `/api/branches` | Admin, Supervisor, Receptionist | List (non-admin chỉ thấy branch mình) |
| `GET` | `/api/branches/{id}` | Admin, Supervisor, Receptionist | Chi tiết |
| `PATCH` | `/api/branches/{id}/status` | Admin | Bật/tắt `IsActive` |

**DTOs** tại `Garage_Management.Application/DTOs/Branches/`:
- `BranchCreateRequest` — Name, BranchCode, Address, Phone?, Email?, ManagerEmployeeId?
- `BranchUpdateRequest` — Name, Address, Phone?, Email?, ManagerEmployeeId? (không có BranchCode)
- `BranchResponse` — tất cả field của Branch + `EmployeeCount`, `ActiveJobCardCount`
- `BranchStatusUpdateRequest` — IsActive

Validation: Data Annotations trên DTO (`[Required]`, `[MaxLength]`) + check trùng `BranchCode` trong service.

### 8.2 `ReportsController`

| Method | Route | Role | Mô tả |
|---|---|---|---|
| `GET` | `/api/reports/branches/{branchId}/revenue?from=&to=` | Admin, Supervisor | Tổng doanh thu 1 chi nhánh (ServiceTotal, SparePartTotal, GrandTotal) |
| `GET` | `/api/reports/branches/{branchId}/jobcards?status=&from=&to=` | Admin, Supervisor | Count JobCard theo trạng thái |
| `GET` | `/api/reports/revenue-by-branch?from=&to=` | Admin | So sánh doanh thu tất cả chi nhánh (breakdown) |

Non-admin gọi endpoint với `branchId` khác chi nhánh mình → HTTP 403.

### 8.3 Thay đổi hành vi endpoint hiện có

Các endpoint `GET` list của Employee / Inventory / JobCard / Invoice / Appointment / WorkBay / StockTransaction:
- Ngầm filter theo `BranchId` từ token (non-admin)
- Admin có query param optional `?branchId=` để filter riêng

Các endpoint `POST` create: `BranchId` trong body bị bỏ qua với non-admin (luôn dùng từ token). Admin được chỉ định.

Các endpoint `PUT/PATCH/DELETE`: validate `entity.BranchId == currentBranchId` trước thao tác (non-admin).

URL và tên endpoint **không đổi** — frontend chỉ cần cập nhật để hiển thị BranchId/BranchName từ response.

## 9. Thứ tự triển khai

1. **Entity + DbContext + Seed** — `Branch.cs`, `BranchConfiguration.cs`, DbSet, update `InitialSeed.cs`
2. **Thêm `BranchId` vào 7 entity hiện có** + update Fluent config tương ứng
3. **Chạy 2 migration** (`AddBranchEntity`, `AddBranchIdToEntities`) — verify trên local DB
4. **`ICurrentUserService` + `CurrentUserService`** — DI đăng ký Scoped
5. **`GenerateToken.cs` + `LoginResponse` DTO** — thêm claim `branch_id` và trường response
6. **`BranchRepository` + `BranchService` + `BranchesController` + DTOs Branches** — CRUD Branch hoàn chỉnh
7. **Cập nhật service hiện có** — inject `ICurrentUserService`, thêm filter + enforcement ở 10 service (7 trực tiếp + 3 phái sinh)
8. **`ReportsController` + `ReportsService`** — 3 endpoint report

Unit tests sẽ làm ở pha riêng sau khi merge.

## 10. Rủi ro

- **Unit tests hiện có** (InventoryService, JobCardService, AppointmentService, EmployeeService, VehicleService, SupplierService, SparePartBrandService) sẽ hỏng vì service đổi chữ ký / thêm dependency. Giải quyết ở pha test sau.
- **`VehicleService` hiện đang đọc claim trực tiếp** thay vì qua abstraction — không refactor trong scope này để giữ thay đổi gọn, nhưng nên làm nice-to-have về sau.
- **Seed data hiện có** (Employee user 1-5, Customer) sẽ đồng loạt về `BranchId=1`. Nhất quán với migration 1.
- **Performance** của report endpoint: nếu dữ liệu lớn cần đánh index trên `(BranchId, CreatedAt)` của `Invoices` và `JobCards`. Trong phạm vi đồ án chưa cần.

## 11. Open questions

Không còn. Tất cả quyết định đã chốt qua brainstorming:
- Phương án branch-scoped (câu 1): A
- Employee 1-1 Branch (câu 2): A
- Inventory tách hoàn toàn (câu 3): A
- Enforce phân quyền qua JWT (câu 4): A
- Appointment bắt buộc BranchId (câu 5): A
- Report đơn giản (câu 6): A
- Không thêm role mới, giữ Supervisor (theo feedback): A
- Không dùng FluentValidation validator riêng (theo feedback): dùng Data Annotations
