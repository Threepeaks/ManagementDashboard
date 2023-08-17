select 

com_ref as 'Reference',
com_name as 'Customer' ,

case concat(com_ac_pending,com_acc_cancel )
	when '00' then 'Active'
    when '10' then 'Pending'
    when '01' then 'Cancelling'
    when '11' then 'Cancelling'
    when '02' then 'Cancelled'
    when '12' then 'Cancelled'
    
    else  concat(com_ac_pending,com_acc_cancel )
end   
as 'Status'

from tblcompany where allowBankProcessing = 0
and com_acc_cancel in (0,1)