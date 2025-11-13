USE ERP_WMS_TMS_DB;
GO

-- 1. 系统角色表（SysRole）
CREATE TABLE SysRole (
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    RoleName NVARCHAR(20) NOT NULL,
    Description NVARCHAR(200) NULL,
    Permissions NVARCHAR(MAX) NULL DEFAULT '',
    CreateUserId INT NOT NULL DEFAULT 0,
    CreateTime DATETIME NOT NULL DEFAULT GETDATE(),
    UpdateUserId INT NULL,
    UpdateTime DATETIME NULL,
    IsDeleted BIT NOT NULL DEFAULT 0
);
GO

-- 2. 系统用户表（SysUser）
CREATE TABLE SysUser (
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    UserName NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(32) NOT NULL,  -- MD5加密存储
    RealName NVARCHAR(20) NOT NULL,
    Phone NVARCHAR(11) NULL,
    RoleId INT NOT NULL,
    Status INT NOT NULL DEFAULT 1,  -- 1=启用，0=禁用
    CreateUserId INT NOT NULL DEFAULT 0,
    CreateTime DATETIME NOT NULL DEFAULT GETDATE(),
    UpdateUserId INT NULL,
    UpdateTime DATETIME NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    -- 外键关联角色表
    CONSTRAINT FK_SysUser_SysRole FOREIGN KEY (RoleId) REFERENCES SysRole(Id)
);
GO

-- 3. 系统日志表（SystemLog）- 分表策略后续实现（方案6.1）
CREATE TABLE SystemLog (
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    RequestId NVARCHAR(32) NOT NULL,  -- 跨模块关联ID
    ModuleName NVARCHAR(20) NOT NULL,  -- WMS/TMS/ERP/Integration
    LogLevel INT NOT NULL,  -- 1=Info，2=Warn，3=Error，4=Fatal
    BusinessNo NVARCHAR(50) NULL DEFAULT '',  -- 业务编号
    Content NVARCHAR(MAX) NOT NULL,  -- 日志内容
    OperatorId INT NOT NULL DEFAULT 0,  -- 操作人ID
    CreateTime DATETIME NOT NULL DEFAULT GETDATE(),
    UpdateUserId INT NULL,
    UpdateTime DATETIME NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    -- 索引优化查询
    INDEX IX_SystemLog_RequestId (RequestId),
    INDEX IX_SystemLog_BusinessNo (BusinessNo),
    INDEX IX_SystemLog_CreateTime (CreateTime)
);
GO

-- 4. 初始化默认角色（适配方案1.1-1.4核心角色）
INSERT INTO SysRole (RoleName, Description, Permissions, CreateUserId, CreateTime)
VALUES 
('仓库操作员', '负责WMS入库、出库、盘点等现场操作', '{"WMS":{"Inbound":1,"Outbound":1,"Inventory":1}}', 0, GETDATE()),
('运输调度员', '负责TMS订单分配、在途监控', '{"TMS":{"OrderAssign":1,"Tracking":1,"Driver":1}}', 0, GETDATE()),
('财务人员', '负责ERP财务报表、审批、数据核对', '{"ERP":{"Financial":1,"Approval":1,"Check":1}}', 0, GETDATE()),
('管理层', '负责数据驾驶舱查看、决策', '{"Dashboard":1,"Report":1,"Setting":1}', 0, GETDATE()),
('系统管理员', '负责系统配置、用户管理', '{"SysAdmin":1,"User":1,"Role":1}', 0, GETDATE());
GO

-- 5. 初始化管理员用户（用户名：admin，密码：123456（MD5：e10adc3949ba59abbe56e057f20f883e））
INSERT INTO SysUser (UserName, Password, RealName, Phone, RoleId, Status, CreateUserId, CreateTime)
VALUES 
('admin', 'e10adc3949ba59abbe56e057f20f883e', '系统管理员', '13800138000', 5, 1, 0, GETDATE());
GO
