using ERP_WMS_TMS.Common.Entities;
using ERP_WMS_TMS.Common.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ERP_WMS_TMS.Infrastructure.Data
{
    /// <summary>
    /// 系统数据库上下文（EF Core）
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // 公共表
        public DbSet<SystemLog> SystemLogs { get; set; }
        public DbSet<SysUser> SysUsers { get; set; }
        public DbSet<SysRole> SysRoles { get; set; }

        // WMS/TMS/ERP业务表后续模块逐步添加
        // public DbSet<WmsMaterial> WmsMaterials { get; set; }
        // public DbSet<TmsTransportOrder> TmsTransportOrders { get; set; }
        // public DbSet<ErpFinancialBill> ErpFinancialBills { get; set; }

        /// <summary>
        /// 模型构建（配置实体映射）
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 应用所有IEntityTypeConfiguration配置（后续按模块拆分）
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // 全局配置：软删除筛选（IsDeleted=false）
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseAuditEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var property = Expression.Property(parameter, "IsDeleted");
                    var constant = Expression.Constant(false);
                    var filter = Expression.Lambda(
                        Expression.Equal(property, constant), 
                        parameter);

                    modelBuilder.Entity(entityType.ClrType)
                        .HasQueryFilter(filter);
                }
            }

            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// 保存更改前拦截（自动填充审计字段）
        /// </summary>
        public override int SaveChanges()
        {
            FillAuditFields();
            return base.SaveChanges();
        }

        /// <summary>
        /// 异步保存更改前拦截
        /// </summary>
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            FillAuditFields();
            return base.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// 填充审计字段（CreateTime/UpdateTime等）
        /// </summary>
        private void FillAuditFields()
        {
            var now = DateTime.Now;
            var entries = ChangeTracker.Entries<BaseAuditEntity>();

            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreateTime = now;
                        entry.Entity.IsDeleted = false;
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdateTime = now;
                        // 禁止修改创建时间和创建人
                        entry.Property(e => e.CreateTime).IsModified = false;
                        entry.Property(e => e.CreateUserId).IsModified = false;
                        break;
                    case EntityState.Deleted:
                        // 软删除：改为修改IsDeleted字段
                        entry.State = EntityState.Modified;
                        entry.Entity.IsDeleted = true;
                        entry.Entity.UpdateTime = now;
                        break;
                }
            }
        }
    }

    /// <summary>
    /// 通用仓储接口（EF Core数据访问封装）
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    public interface IRepository<TEntity> where TEntity : BaseAuditEntity
    {
        Task AddAsync(TEntity entity);
        Task<TEntity?> GetByIdAsync(int id);
        Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? predicate = null);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(int id);
        Task<int> SaveChangesAsync();
    }

    /// <summary>
    /// 通用仓储实现
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseAuditEntity
    {
        private readonly AppDbContext _dbContext;
        private readonly DbSet<TEntity> _dbSet;

        public Repository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<TEntity>();
        }

        public async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task<TEntity?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? predicate = null)
        {
            return predicate == null 
                ? await _dbSet.ToListAsync() 
                : await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task UpdateAsync(TEntity entity)
        {
            _dbSet.Update(entity);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}