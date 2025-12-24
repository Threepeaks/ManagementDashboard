select 
                rbr_comref as 'Customer', 
                rbr_id  as 'RBR',
                rbr_Date as 'Action Date',
                DATEDIFF(rbr_Date ,CURDATE()) as 'Action Date In',
                rbr_datetime_sub as 'Received',
               
                DATEDIFF(rbr_datetime_sub, rbr_Date ) as 'In Before Action Date',
                hbn_rbr as 'Batch Ref',
                hbn_datetime as 'Batch Created',
                case rbr_status
                                when 0 then 'Not Validated'
        when 1 then 'CDV Validated'
        when 2 then 'Processed to Bank'
        when 3 then 'Paid'
        when 4 then 'Rejected'
        when 99 then 'Cancelled or Recalled'
    end as 'Status'
    from tblrbr 
left join tblhyphen_batchno on hbn_rbr = rbr_id and hbn_type = 1

where rbr_date <= '{{endActionDate}}'
and rbr_status in (0,1)
order by rbr_date,rbr_datetime_sub

