select 
rbr_id as 'RBR',
rbr_date as 'Action Date',
case rbr_status
 when 0 then 'Not Validated'
 when 1 then 'CDV Validated'
 when 2 then 'Processed To Bank'
 when 3 then 'Paid'
 when 4 then 'Complete Rejected'
 when 99 then 'Recalled or Cancelled'
 else rbr_status
 end as 'State'

from tblrbr 

--where rbr_id={{id}}