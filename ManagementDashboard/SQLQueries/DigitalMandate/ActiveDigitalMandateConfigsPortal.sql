select 
 clientReference as 'Client Reference',
 cpr_name as 'Client',
 initialActivationDate as 'Activation Date'
 
from tbl_service_emandate
	left join tbl_customer_profile on cpr_ref = clientReference

order by clientReference