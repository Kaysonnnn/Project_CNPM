create database QuanLyTraSua
go

use QuanLyTraSua
go

-- Food
-- Table
-- FoodCategory
-- Account
-- Bill
-- BillInfo

create table TableFood
(
	id int identity primary key,
	name nvarchar(100) not null default N'Bàn chưa có tên',
	status nvarchar(100) not null default N'Trống'		-- Trống || Có người
)
go

create table Account
(
	Username nvarchar(100) primary key,
	DisplayName nvarchar(100) not null default N'Chưa đặt tên',
	PassWord nvarchar(1000) not null default N'2251022057731868917119086224872421513662',
	Type int not null default 0		-- 1: admin && 0: staff
)
go

create table FoodCategory
(
	id int identity primary key,
	name nvarchar(100) not null default N'Chưa đặt tên'
)
go

create table Food
(
	id int identity primary key,
	name nvarchar(100) not null default N'Chưa đặt tên',
	idCategory int not null,
	price float not null default 0

	foreign key (idCategory) references dbo.FoodCategory(id)
)
go

create table Bill
(
	id int identity primary key,
	DateCheckIn date not null default getDate(),
	DateCheckOut date,
	idTable int not null,
	status int not null default 0		-- 1: đã thanh toán && 0: chưa thanh toán

	foreign key (idTable) references dbo.TableFood(id)
)
go

create table BillInfo
(
	id int identity primary key,
	idBill int not null,
	idFood int not null,
	count int not null default 0

	foreign key (idBill) references dbo.Bill(id),
	foreign key (idFood) references dbo.Food(id)
)
go

-------------------- CREATE --------------------

insert into dbo.Account
values (N'Hung', N'Hứa Thịnh Hưng', N'2251022057731868917119086224872421513662', 1),
	   (N'Lan', N'Bùi Sĩ Thế Lân', N'2251022057731868917119086224872421513662', 0),
	   (N'TranKiet', N'Trần Gia Kiệt', N'2251022057731868917119086224872421513662', 1)
go

declare @i int = 1
while @i <= 20
begin
	insert dbo.TableFood (name) values (N'Bàn ' + cast(@i as nvarchar(100)))
	set @i = @i + 1
end
go

insert dbo.FoodCategory (name)
values (N'Trà Sữa'),
	   (N'Cà Phê'),
	   (N'Freeze'),
	   (N'Phindi'),
	   (N'Bánh Ngọt')
go

insert dbo.Food (name, idCategory, price)
values (N'Trà Sữa Truyền thống', 1, 120000),
	   (N'Trà Sữa Hoa Nhài', 1, 50000),
	   (N'Cà Phê Đen', 2, 60000),
	   (N'Cà Phê Sữa Đá', 2, 85000),
	   (N'Freeze Trà Xanh', 3, 75000),
	   (N'Freeze Chocolate', 3, 15000),
	   (N'Phidin Hạnh Nhân', 4, 12000),
	   (N'Phidin Kem Sữa', 4, 45000),
	   (N'Bánh Su Kem', 5, 29000),
	   (N'Socolate', 5, 19000)
go

/*
insert dbo.Bill (DateCheckIn, DateCheckOut, idTable, status)
values (getDate(), null, 1, 0),
			 (getDate(), null, 2, 0),
			 (getDate(), getDate(), 2, 1)
go

insert dbo.BillInfo (idBill, idFood, count)
values (1, 1, 2),
			 (1, 3, 4),
			 (1, 5, 1),
			 (2, 1, 2),
			 (2, 6, 2),
			 (3, 5, 2)
go
*/


-------------------- READ --------------------

/*
select * from dbo.Account
select * from dbo.TableFood
select * from dbo.FoodCategory
select * from dbo.Food
select * from dbo.Bill
select * from dbo.BillInfo

SELECT f.name, bi.count, f.price, f.price * bi.count AS totalPrice
FROM dbo.BillInfo AS bi, dbo.Bill AS b, dbo.Food AS f
WHERE bi.idBill = b.id AND bi.idFood = f.id AND b.status = 0 AND b.idTable = 2

select top 4 * from dbo.Bill
except
select top 2 * from dbo.Bill
*/

-------------------- UPDATE --------------------

alter table dbo.Bill add discount int default 0
go
update dbo.Bill set discount = 0
GO

alter table dbo.Bill add totalPrice float
go


-------------------- DELETE --------------------

/*
delete dbo.BillInfo
delete dbo.Bill
delete dbo.TableFood
GO
*/

-------------------- PROCEDURE --------------------

create proc USP_GetAccountByUsername
@username nvarchar(100)
as
begin
	select * from dbo.Account where Username = @username
end
go
-- exec dbo.USP_GetAccountByUsername @username = N'nkduy'
-- go

create proc USP_Login
@username nvarchar(100), @password nvarchar(100)
as
begin
	select * from dbo.Account where Username = @username and PassWord = @password
end
go
-- exec dbo.USP_Login @username = N'nkduy', @password = N'123456'
-- go

create proc USP_GetTableList
as select * from dbo.TableFood
go
-- update dbo.TableFood set status = N'Có người' where id = 9
-- exec dbo.USP_GetTableList
-- go

create proc USP_InsertBill
@idTable int
as
begin
	insert dbo.Bill (DateCheckIn, DateCheckOut, idTable, status, discount)
	values (getDate(), null, @idTable, 0, 0)
end
go

create proc USP_InsertBillInfo
@idBill int, @idFood int, @count int
as
begin
	declare @isExistBillInfo int;
	declare @foodCount int = 1;

	select @isExistBillInfo = id, @foodCount = b.count
	from dbo.BillInfo as b
	where idBill = @idBill and idFood = @idFood

	if (@isExistBillInfo > 0)
	begin
		declare @newCount int = @foodCount + @count
		if (@newCount > 0)
			update dbo.BillInfo set count = @foodCount + @count where idBill = @idBill and idFood = @idFood
		else
			delete dbo.BillInfo where idBill = @idBill and idFood = @idFood
	end
	else
	BEGIN
		insert dbo.BillInfo (idBill, idFood, count)
		values (@idBill, @idFood, @count)
	end
end
go

create PROC USP_SwitchTable
@idTable1 INT, @idTable2 INT
AS
BEGIN
	DECLARE @idFirstBill INT
	DECLARE @idSecondBill INT

	declare @isFirstTableEmpty int = 1
	declare @isSecondTableEmpty int = 1

	SELECT @idSecondBill = id FROM dbo.Bill WHERE idTable = @idTable2 AND status = 0
	SELECT @idFirstBill = id FROM dbo.Bill WHERE idTable = @idTable1 AND status = 0

	IF (@idFirstBill IS NULL)
	BEGIN
		INSERT dbo.Bill (DateCheckIn, DateCheckOut, idTable, status)
		VALUES (GETDATE(), NULL, @idTable1, 0)
		SELECT @idFirstBill = MAX(id) FROM dbo.Bill WHERE idTable = @idTable1 AND status = 0
	END

	select @isFirstTableEmpty = COUNT(*) from dbo.BillInfo where idBill = @idFirstBill

	IF (@idSecondBill IS NULL)
	BEGIN
		INSERT dbo.Bill (DateCheckIn, DateCheckOut, idTable, status)
		VALUES (GETDATE(), NULL, @idTable2, 0)
		SELECT @idSecondBill = MAX(id) FROM dbo.Bill WHERE idTable = @idTable2 AND status = 0
	END

	select @isSecondTableEmpty = COUNT(*) from dbo.BillInfo where idBill = @idSecondBill

	SELECT id INTO IDBillInfoTable FROM dbo.BillInfo WHERE idBill = @idSecondBill

	UPDATE dbo.BillInfo SET idBill = @idSecondBill WHERE idBill = @idFirstBill
	UPDATE dbo.BillInfo SET idBill = @idFirstBill WHERE id IN (SELECT * FROM IDBillInfoTable)

	DROP TABLE IDBillInfoTable

	IF (@isFirstTableEmpty = 0)
		update dbo.TableFood set status = N'Trống' where id = @idTable2
	
	IF (@isSecondTableEmpty = 0)
		update dbo.TableFood set status = N'Trống' where id = @idTable1
END
GO

create proc USP_MergeTable
@idTable1 int, @idTable2 int
as
begin
	declare @idFirstBill int
	declare @idSecondBill int

	select @idFirstBill = id from dbo.Bill where idTable = @idTable1 and status = 0
	select @idSecondBill = id from dbo.Bill where idTable = @idTable2 and status = 0

	if (@idFirstBill is null)
	begin
		insert dbo.Bill (DateCheckIn, DateCheckOut, idTable, status)
		values (getDate(), null, @idTable1, 0)
		select @idFirstBill = max(id) from dbo.Bill where idTable = @idTable1 and status = 0
	end

	if (@idSecondBill is null)
	begin
		insert dbo.Bill (DateCheckIn, DateCheckOut, idTable, status)
		values (getDate(), null, @idTable2, 0)
		select @idSecondBill = max(id) from dbo.Bill where idTable = @idTable2 and status = 0
	end

	update dbo.BillInfo set idBill = @idSecondBill where idBill = @idFirstBill

	delete dbo.Bill where id = @idFirstBill

	update dbo.TableFood set status = N'Trống' where id = @idTable1
end
go

create proc USP_GetListBillByDate
@checkIn date, @checkOut date
as
begin
	select t.name as [Tên bàn], b.totalPrice as [Tổng tiền], DateCheckIn as [Ngày vào], DateCheckOut as [Ngày ra], discount as [Giảm giá]
	from dbo.Bill as b, dbo.TableFood as t
	where t.id = b.idTable and DateCheckIn >= @checkIn and DateCheckIn <= @checkOut and b.status = 1
end
go

create proc USP_UpdateAccount
@username nvarchar(100), @displayName nvarchar(100), @password nvarchar(100), @newPassword nvarchar(100)
AS
BEGIN
	declare @isRightPass int = 0

	select @isRightPass = count(*) from dbo.Account where Username = @username and PassWord = @password

	if (@isRightPass = 1)
	begin
		if (@newPassword = null or @newPassword = '')
		begin
			update dbo.Account set DisplayName = @displayName where Username = @username
		END
		ELSE
			update dbo.Account set DisplayName = @displayName, PassWord = @newPassword where Username = @username
	end
END
GO

create proc USP_GetListBillByDateAndPage
@checkIn date, @checkOut date, @page int
as
begin
	declare @pageRows int = 10
	declare @selectRows int = @pageRows
	declare @exceptRows int = (@page - 1) * @pageRows

  -- sắp xếp id giảm dần
	select b.id as [Mã đơn], t.name as [Tên bàn], b.totalPrice as [Tổng tiền], DateCheckIn as [Ngày vào], DateCheckOut as [Ngày ra], discount as [Giảm giá]
	from dbo.Bill as b, dbo.TableFood as t
	where t.id = b.idTable and DateCheckIn >= @checkIn and DateCheckIn <= @checkOut and b.status = 1
	order by [Mã đơn] desc
	offset @exceptRows rows fetch next @selectRows rows only
end
go
-- EXEC dbo.USP_GetListBillByDateAndPage @checkIn = '2023-06-01', @checkOut = '2023-07-31', @page = 9
-- go

create proc USP_GetNumBillByDate
@checkIn date, @checkOut date
as
begin
	select count(*)
	from dbo.Bill as b, dbo.TableFood as t
	where t.id = b.idTable and DateCheckIn >= @checkIn and DateCheckIn <= @checkOut and b.status = 1
end
go


-------------------- TRIGGER --------------------

create trigger UTG_UpdateBillInfo
on dbo.BillInfo for insert, update
as
begin
	declare @idBill int

	select @idBill = idBill from inserted

	declare @idTable int

	select @idTable = idTable from dbo.Bill where id = @idBill and status = 0

	declare @count INT
	select @count = count(*) from dbo.BillInfo where idBill = @idBill

	if (@count > 0)
		update dbo.TableFood set status = N'Có người' where id = @idTable
	ELSE
		update dbo.TableFood set status = N'Trống' where id = @idTable
end
go

create trigger UTG_UpdateBill
on dbo.Bill for update
as
begin
	declare @idBill int

	select @idBill = id from inserted

	declare @idTable int

	select @idTable = idTable from dbo.Bill where id = @idBill

	declare @count int = 0

	select @count = count(*) from dbo.Bill where idTable = @idTable and status = 0

	if (@count = 0)
		update dbo.TableFood set status = N'Trống' where id = @idTable
end
go

create TRIGGER UTG_DeteleBillInfo
ON dbo.BillInfo FOR DELETE
AS
BEGIN
	DECLARE @idBill INT
	DECLARE @idTable INT

	SELECT @idBill = idBill FROM deleted

	SELECT @idTable = idTable FROM dbo.Bill WHERE id = @idBill AND status = 0

	DECLARE @count INT
	SELECT @count = COUNT(*) FROM dbo.BillInfo WHERE idBill = @idBill

	IF (@count > 0)
		UPDATE dbo.TableFood SET status = N'Có người' WHERE id = @idTable
	ELSE
		UPDATE dbo.TableFood SET status = N'Trống' WHERE id = @idTable
END
GO


-------------------- FUNCTION --------------------

CREATE FUNCTION [dbo].[fuConvertToUnsign1] ( @strInput NVARCHAR(4000) ) RETURNS NVARCHAR(4000) AS BEGIN IF @strInput IS NULL RETURN @strInput IF @strInput = '' RETURN @strInput DECLARE @RT NVARCHAR(4000) DECLARE @SIGN_CHARS NCHAR(136) DECLARE @UNSIGN_CHARS NCHAR (136) SET @SIGN_CHARS = N'ăâđêôơưàảãạáằẳẵặắầẩẫậấèẻẽẹéềểễệế ìỉĩịíòỏõọóồổỗộốờởỡợớùủũụúừửữựứỳỷỹỵý ĂÂĐÊÔƠƯÀẢÃẠÁẰẲẴẶẮẦẨẪẬẤÈẺẼẸÉỀỂỄỆẾÌỈĨỊÍ ÒỎÕỌÓỒỔỖỘỐỜỞỠỢỚÙỦŨỤÚỪỬỮỰỨỲỶỸỴÝ' +NCHAR(272)+ NCHAR(208) SET @UNSIGN_CHARS = N'aadeoouaaaaaaaaaaaaaaaeeeeeeeeee iiiiiooooooooooooooouuuuuuuuuuyyyyy AADEOOUAAAAAAAAAAAAAAAEEEEEEEEEEIIIII OOOOOOOOOOOOOOOUUUUUUUUUUYYYYYDD' DECLARE @COUNTER int DECLARE @COUNTER1 int SET @COUNTER = 1 WHILE (@COUNTER <=LEN(@strInput)) BEGIN SET @COUNTER1 = 1 WHILE (@COUNTER1 <=LEN(@SIGN_CHARS)+1) BEGIN IF UNICODE(SUBSTRING(@SIGN_CHARS, @COUNTER1,1)) = UNICODE(SUBSTRING(@strInput,@COUNTER ,1) ) BEGIN IF @COUNTER=1 SET @strInput = SUBSTRING(@UNSIGN_CHARS, @COUNTER1,1) + SUBSTRING(@strInput, @COUNTER+1,LEN(@strInput)-1) ELSE SET @strInput = SUBSTRING(@strInput, 1, @COUNTER-1) +SUBSTRING(@UNSIGN_CHARS, @COUNTER1,1) + SUBSTRING(@strInput, @COUNTER+1,LEN(@strInput)- @COUNTER) BREAK END SET @COUNTER1 = @COUNTER1 +1 END SET @COUNTER = @COUNTER +1 END SET @strInput = replace(@strInput,' ','-') RETURN @strInput END
