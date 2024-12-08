USE [PUPOPMSESTPUP]
GO

/****** Object:  StoredProcedure [dbo].[sp_TaxCP8D]    Script Date: 08/12/2024 12:26:43 PM ******/
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
	, fld_Kdrkyt
	, fld_ChildAbove18CertFull
	, fld_ChildAbove18CertHalf
	, fld_ChildAbove18HigherFull
	, fld_ChildAbove18HigherHalf
	, fld_ChildBelow18Full
	, fld_ChildBelow18Half
	, fld_DisabledChildFull
	, fld_DisabledChildHalf
	, fld_DisabledChildStudyFull
	, fld_DisabledChildStudyHalf
	, fld_ContractExpiryDate
	, fld_Trlhr
	, fld_TaxMaritalStatus
	FROM vw_TaxCP8D WITH (NOLOCK) WHERE fld_Year = @Year 

	SELECT fld_NoPkjPermanent, fld_CarumanPekerja, fld_KodCaruman, fld_Month, fld_Year FROM vw_CarumanTambahan WHERE fld_KodCaruman IN ('SBKP', 'SIP') AND fld_Year = @Year AND fld_CarumanPekerja > 0

	SELECT * FROM tbl_SpecialInsentif WHERE fld_Year = @Year
END
GO


