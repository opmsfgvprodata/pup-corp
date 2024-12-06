USE [PUPOPMSESTPUP]
GO

/****** Object:  StoredProcedure [dbo].[sp_TaxCP8D]    Script Date: 12/6/2024 11:42:34 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_TaxCP8D] 
	-- Add the parameters for the stored procedure here
	@Year INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT fld_LadangID, fld_DivisionID, fld_Nama, fld_NoPkjPermanent, fld_Nokp, fld_Nopkj FROM tbl_Pkjmast WHERE fld_NoPkjPermanent IN (SELECT fld_NoPkjPermanent
	FROM tbl_Pkjmast where fld_Kdaktf = '1') order by fld_LadangID, fld_DivisionID, fld_Nama asc

	SELECT fld_WorkerTaxID
	, fld_NoPkjPermanent
	, fld_Nama
	, fld_KWSPPkj
	, fld_SocsoPkj
	, fld_CarumanPekerja AS fld_PCB
	, fld_Z AS fld_Zakat
	, ISNULL(fld_CP38Amount, 0) AS fld_CP38
	, fld_C * fld_Q AS fld_PelepasanAnak
	, fld_Y1 AS fld_SaraanKasar
	, fld_Month
	, fld_Year
	, fld_LadangID
	, fld_DivisionID
	, fld_TaxNo
	, fld_Nokp
	FROM vw_TaxCP8D WITH (NOLOCK) WHERE fld_Year = @Year 

	SELECT a.fld_UniqueID
	, a.fld_TaxNo
	, a.fld_NopkjPermanent
	, a.fld_TaxMaritalStatus
	, b.fld_Nokp
	, b.fld_Nama
	, b.fld_Kdrkyt
	, a.fld_ChildAbove18CertFull
	, a.fld_ChildAbove18CertHalf
	, a.fld_ChildAbove18HigherFull
	, a.fld_ChildAbove18HigherHalf
	, a.fld_ChildBelow18Full
	, a.fld_ChildBelow18Half
	, a.fld_DisabledChildFull
	, a.fld_DisabledChildHalf
	, a.fld_DisabledChildStudyFull
	, a.fld_DisabledChildStudyHalf
	, b.fld_ContractExpiryDate
	, b.fld_Trlhr
	, a.fld_DivisionID
	FROM tbl_TaxWorkerInfo a WITH (NOLOCK) 
	INNER JOIN tbl_Pkjmast b WITH (NOLOCK) ON a.fld_NopkjPermanent = b.fld_NoPkjPermanent
    WHERE fld_Year = @Year AND fld_Kdaktf = '1'

	SELECT fld_NoPkjPermanent, fld_CarumanPekerja, fld_KodCaruman, fld_Month, fld_Year FROM vw_CarumanTambahan WHERE fld_KodCaruman IN ('SBKP', 'SIP') AND fld_Year = @Year AND fld_CarumanPekerja > 0

	SELECT * FROM tbl_SpecialInsentif WHERE fld_Year = @Year
END
GO


