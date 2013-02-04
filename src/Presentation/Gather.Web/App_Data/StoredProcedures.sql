CREATE PROCEDURE [Location_Import]
	@p0 TEXT
AS
BEGIN
SET NOCOUNT ON;

    DECLARE @i INT
    DECLARE @TranStarted BIT ; SET @TranStarted = 0
    DECLARE @Result BIT ; SET @Result = 0
    	
	BEGIN TRY
		
		-- Create XML document
		EXEC sp_xml_preparedocument @i OUTPUT, @p0

	END TRY
	BEGIN CATCH
	
		GOTO Cleanup
		
	END CATCH
	
	BEGIN TRANSACTION ImportLocations
	SET @TranStarted = 1
	
	BEGIN TRY
	
		---- Drop constraints
		--ALTER TABLE [dbo].[Location] DROP CONSTRAINT [FK_Location_Location_ParentLocation_Id]
	
		SET IDENTITY_INSERT [dbo].[Location] ON
	
		-- Insert records			        
		INSERT INTO
			dbo.Location
			( 
				Id,
				Active ,	
				CreatedDate ,
				Deleted ,
				HashTag ,
				LastModifiedBy ,
				LastModifiedDate ,
				Name ,
				ParentLocation_Id ,
				SeoName ,
				IsRegion ,
				Latitude ,
				Longitude 
		    )
		SELECT  
			i.Id, -- Id - int
			1, -- Active - bit 
			GETDATE(), -- CreatedDate - datetime 
			0, -- Deleted - bit,
			'', -- HashTag - nvarchar(20)
			0, -- LastModifiedBy - int
			GETDATE(), -- LastModifiedDate - datetime,
			i.Name, -- Name - nvarchar(100) 
			NULLIF(i.ParentLocation_Id, ''), -- ParentLocation_Id - int
			i.SeoName, -- SeoName - nvarchar(max)
			i.IsRegion, -- IsRegion - bit
			i.Latitude, -- Latitude - decimal
			i.Longitude -- Longitude - decimal
		FROM
			OPENXML(@i, '/Locations/Location',2)
		WITH 	
			(
				Id INT,
				Name NVARCHAR(100),
				ParentLocation_Id INT,
				SeoName NVARCHAR(MAX),
				IsRegion BIT,
				Latitude DECIMAL(25,18),
				Longitude DECIMAL(25,18)
			) i
			
		SET IDENTITY_INSERT [dbo].[Location] OFF
		
		---- Add constraints
		--ALTER TABLE [dbo].[Location] ADD CONSTRAINT [FK_Location_Location_ParentLocation_Id] FOREIGN KEY ([ParentLocation_Id]) REFERENCES [dbo].[Location] ([Id])
	
	END TRY
	BEGIN CATCH
	
		GOTO Cleanup
	
	END CATCH	
	
	IF( @TranStarted = 1 )
	BEGIN
		SET @TranStarted = 0
		SET @Result = 1
		COMMIT TRANSACTION ImportLocations			
	END
	
Cleanup:

	IF( @TranStarted = 1 )
	BEGIN
		SET @TranStarted = 0
		SET @Result = 0
		ROLLBACK TRANSACTION ImportLocations   		
	END
	
	-- Remove XML document
	EXEC sp_xml_removedocument @i
	
	-- Return the result
	SELECT @Result
	
END
GO