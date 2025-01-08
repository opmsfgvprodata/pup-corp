USE [PUPOPMSESTPUP]
GO

/****** Object:  View [dbo].[vw_CarumanTambahan]    Script Date: 12/6/2024 11:57:05 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vw_CarumanTambahan]
AS
SELECT TOP (100) PERCENT dbo.tbl_GajiBulanan.fld_Nopkj, dbo.tbl_Pkjmast.fld_Nokp, dbo.tbl_Pkjmast.fld_Nama, dbo.tbl_Pkjmast.fld_Kdrkyt, dbo.tbl_Pkjmast.fld_Kdaktf, dbo.tbl_Pkjmast.fld_Ktgpkj, dbo.tbl_Pkjmast.fld_Jenispekerja, dbo.tbl_Pkjmast.fld_KodSocso, 
             dbo.tbl_Pkjmast.fld_Noperkeso, dbo.tbl_Pkjmast.fld_KodKWSP, dbo.tbl_Pkjmast.fld_Nokwsp, dbo.tbl_Pkjmast.fld_NegaraID, dbo.tbl_Pkjmast.fld_SyarikatID, dbo.tbl_Pkjmast.fld_WilayahID, dbo.tbl_Pkjmast.fld_LadangID, dbo.tbl_Pkjmast.fld_DivisionID, 
             dbo.tbl_Pkjmast.fld_StatusKwspSocso, dbo.tbl_GajiBulanan.fld_KWSPPkj, dbo.tbl_GajiBulanan.fld_KWSPMjk, dbo.tbl_GajiBulanan.fld_SocsoPkj, dbo.tbl_GajiBulanan.fld_SocsoMjk, dbo.tbl_GajiBulanan.fld_GajiKasar, dbo.tbl_GajiBulanan.fld_GajiBersih, dbo.tbl_GajiBulanan.fld_Year, 
             dbo.tbl_GajiBulanan.fld_Month, dbo.tbl_ByrCarumanTambahan.fld_KodCaruman, dbo.tbl_ByrCarumanTambahan.fld_KodSubCaruman, dbo.tbl_ByrCarumanTambahan.fld_CarumanPekerja, dbo.tbl_ByrCarumanTambahan.fld_CarumanMajikan, 
             dbo.tbl_Pkjmast.fld_NoPkjPermanent
FROM   dbo.tbl_GajiBulanan INNER JOIN
             dbo.tbl_ByrCarumanTambahan ON dbo.tbl_GajiBulanan.fld_ID = dbo.tbl_ByrCarumanTambahan.fld_GajiID INNER JOIN
             dbo.tbl_Pkjmast ON dbo.tbl_GajiBulanan.fld_Nopkj = dbo.tbl_Pkjmast.fld_Nopkj AND dbo.tbl_GajiBulanan.fld_NegaraID = dbo.tbl_Pkjmast.fld_NegaraID AND dbo.tbl_GajiBulanan.fld_SyarikatID = dbo.tbl_Pkjmast.fld_SyarikatID AND 
             dbo.tbl_GajiBulanan.fld_WilayahID = dbo.tbl_Pkjmast.fld_WilayahID AND dbo.tbl_GajiBulanan.fld_LadangID = dbo.tbl_Pkjmast.fld_LadangID
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
         Begin Table = "tbl_GajiBulanan"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 193
               Right = 240
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "tbl_ByrCarumanTambahan"
            Begin Extent = 
               Top = 36
               Left = 467
               Bottom = 214
               Right = 707
            End
            DisplayFlags = 280
            TopColumn = 1
         End
         Begin Table = "tbl_Pkjmast"
            Begin Extent = 
               Top = 17
               Left = 771
               Bottom = 185
               Right = 1016
            End
            DisplayFlags = 280
            TopColumn = 72
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
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vw_CarumanTambahan'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vw_CarumanTambahan'
GO


