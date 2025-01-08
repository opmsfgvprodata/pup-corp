USE [PUPOPMSESTPUP]
GO

/****** Object:  View [dbo].[vw_TaxCP8D]    Script Date: 08/12/2024 12:25:58 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vw_TaxCP8D]
AS
SELECT dbo.tbl_GajiBulanan.fld_ID, dbo.tbl_Pkjmast.fld_Psptno, dbo.tbl_Pkjmast.fld_Nama, dbo.tbl_Pkjmast.fld_Nokp, dbo.tbl_Pkjmast.fld_Kdjnt, dbo.tbl_Pkjmast.fld_Kdrkyt, dbo.tbl_Pkjmast.fld_Kdkwn, dbo.tbl_Pkjmast.fld_NoPkjPermanent, dbo.tbl_Pkjmast.fld_NegaraID, 
             dbo.tbl_Pkjmast.fld_SyarikatID, dbo.tbl_Pkjmast.fld_WilayahID, dbo.tbl_Pkjmast.fld_LadangID, dbo.tbl_Pkjmast.fld_DivisionID, dbo.tbl_GajiBulanan.fld_Year, dbo.tbl_GajiBulanan.fld_Month, dbo.tbl_ByrCarumanTambahan.fld_KodCaruman, 
             dbo.tbl_ByrCarumanTambahan.fld_KodSubCaruman, dbo.tbl_ByrCarumanTambahan.fld_CarumanPekerja, dbo.tbl_TaxWorkerInfo.fld_TaxNo, dbo.tbl_TaxPCB2Form.fld_CP38Amount, dbo.tbl_TaxWorkerInfo.fld_TaxMaritalStatus, dbo.tbl_Pkjmast.fld_ContractExpiryDate, 
             dbo.tbl_Pkjmast.fld_Trlhr, dbo.tbl_GajiBulanan.fld_KWSPPkj, dbo.tbl_GajiBulanan.fld_SocsoPkj, dbo.tbl_ByrCarumanTambahan.fld_C, dbo.tbl_ByrCarumanTambahan.fld_Q, dbo.tbl_TaxWorkerInfo.fld_UniqueID AS fld_WorkerTaxID, dbo.tbl_ByrCarumanTambahan.fld_Z, 
             dbo.tbl_Pkjmast.fld_Kdaktf, dbo.tbl_ByrCarumanTambahan.fld_Y1, dbo.tbl_TaxWorkerInfo.fld_ChildBelow18Full, dbo.tbl_TaxWorkerInfo.fld_ChildBelow18Half, dbo.tbl_TaxWorkerInfo.fld_ChildAbove18CertFull, dbo.tbl_TaxWorkerInfo.fld_ChildAbove18CertHalf, 
             dbo.tbl_TaxWorkerInfo.fld_ChildAbove18HigherFull, dbo.tbl_TaxWorkerInfo.fld_ChildAbove18HigherHalf, dbo.tbl_TaxWorkerInfo.fld_DisabledChildFull, dbo.tbl_TaxWorkerInfo.fld_DisabledChildHalf, dbo.tbl_TaxWorkerInfo.fld_DisabledChildStudyFull, 
             dbo.tbl_TaxWorkerInfo.fld_DisabledChildStudyHalf
FROM   dbo.tbl_TaxPCB2Form RIGHT OUTER JOIN
             dbo.tbl_Pkjmast LEFT OUTER JOIN
             dbo.tbl_ByrCarumanTambahan INNER JOIN
             dbo.tbl_GajiBulanan ON dbo.tbl_ByrCarumanTambahan.fld_GajiID = dbo.tbl_GajiBulanan.fld_ID AND dbo.tbl_ByrCarumanTambahan.fld_Year = dbo.tbl_GajiBulanan.fld_Year AND dbo.tbl_ByrCarumanTambahan.fld_Month = dbo.tbl_GajiBulanan.fld_Month INNER JOIN
             dbo.tbl_TaxWorkerInfo ON dbo.tbl_GajiBulanan.fld_Year = dbo.tbl_TaxWorkerInfo.fld_Year ON dbo.tbl_Pkjmast.fld_LadangID = dbo.tbl_TaxWorkerInfo.fld_LadangID AND dbo.tbl_Pkjmast.fld_NoPkjPermanent = dbo.tbl_TaxWorkerInfo.fld_NopkjPermanent AND 
             dbo.tbl_Pkjmast.fld_NoPkjPermanent = dbo.tbl_GajiBulanan.fld_NoPkjPermanent ON dbo.tbl_TaxPCB2Form.fld_Year = dbo.tbl_GajiBulanan.fld_Year AND dbo.tbl_TaxPCB2Form.fld_Month = dbo.tbl_GajiBulanan.fld_Month AND 
             dbo.tbl_TaxPCB2Form.fld_GajiID = dbo.tbl_GajiBulanan.fld_ID
WHERE (dbo.tbl_ByrCarumanTambahan.fld_CarumanPekerja > 0) AND (dbo.tbl_ByrCarumanTambahan.fld_KodSubCaruman = N'PCB02') AND (dbo.tbl_ByrCarumanTambahan.fld_KodCaruman = N'PCB') AND (dbo.tbl_Pkjmast.fld_Kdaktf = N'1')
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "tbl_Pkjmast"
            Begin Extent = 
               Top = 0
               Left = 0
               Bottom = 247
               Right = 431
            End
            DisplayFlags = 280
            TopColumn = 44
         End
         Begin Table = "tbl_TaxPCB2Form"
            Begin Extent = 
               Top = 277
               Left = 1311
               Bottom = 524
               Right = 1648
            End
            DisplayFlags = 280
            TopColumn = 1
         End
         Begin Table = "tbl_ByrCarumanTambahan"
            Begin Extent = 
               Top = 0
               Left = 1355
               Bottom = 247
               Right = 1773
            End
            DisplayFlags = 280
            TopColumn = 11
         End
         Begin Table = "tbl_GajiBulanan"
            Begin Extent = 
               Top = 9
               Left = 879
               Bottom = 256
               Right = 1245
            End
            DisplayFlags = 280
            TopColumn = 19
         End
         Begin Table = "tbl_TaxWorkerInfo"
            Begin Extent = 
               Top = 32
               Left = 506
               Bottom = 279
               Right = 924
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 2750
         Alias = 1190
         Table = 2480
         Output = 1390
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
     ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vw_TaxCP8D'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'    Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vw_TaxCP8D'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vw_TaxCP8D'
GO


