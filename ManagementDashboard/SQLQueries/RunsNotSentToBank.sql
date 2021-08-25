select 
                rbr_comref as 'Customer', 
                rbr_id  as 'RBR',
                rbr_Date as 'Action Date',
                hbn_rbr as 'Batch Ref',
                case rbr_status
                                when 0 then 'Not Validated'
        when 1 then 'CDV Validated'
        when 2 then 'Processed to Bank'
        when 3 then 'Paid'
        when 4 then 'Rejected'
        when 99 then 'Cancelled or Recalled'
    end as 'Status'
    from tblrbr 
left join tblhyphen_batchno on hbn_rbr = rbr_id

where rbr_date <= CURDATE()
and rbr_status in (0,1)
and hbn_rbr is null
