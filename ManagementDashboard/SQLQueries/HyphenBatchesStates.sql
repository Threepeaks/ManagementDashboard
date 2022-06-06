

    select 
      hbn_no as 'Batch No', 
      case hbn_type when 0 then 'CDV' when 1 then 'Collection' when 2 then 'Payments' when 3 then 'y' end as 'Type', 
      hbn_datetime as 'Send', 
      case hbn_type when 0 then 'RBR' when 1 then 'RBR' when 2 then 'RECON' when 3 then 'CRNo' end as 'Ref/Type', 
      hbn_rbr as 'Reference', 
      case hbn_status when 0 then 'Not Send' when 1 then 'Sending' when 2 then 'Send' when 3 then 'Confirmed/Completed' else hbn_status end as 'State', 
      case hbn_type when 0 then 'RBR' when 1 then rbr.rbr_date when 2 then hbn_actiondate when 3 then 'CRNo' end as 'Action Date', 
      case hbn_type when 0 then '' when 1 then rbr.rbr_comref when 2 then '' when 3 then '' end as 'Customer Reference' 
    from 
      tblhyphen_batchno h 
      left join tblrbr rbr on rbr.rbr_id = hbn_rbr -- where hbn_type=1
      -- 0 - CDV
      -- 1 - Collection
      -- 2 - Payment
      -- 3 - Credit Sure
    order by 
      hbn_id desc 
    limit 
      100

