select 

com_ref as 'Reference',
com_name as 'Customer' 

from tblcompany where allowBankProcessing = 0
and com_acc_cancel in (0,1)
