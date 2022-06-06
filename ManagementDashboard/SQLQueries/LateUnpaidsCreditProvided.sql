select 
rbr_comref as 'Customer',

sum(dbt_amount) as 'Debit Total',
(select sum(rbr_total_retention) from tblrbr        where rbr_comref=com_ref and rbr_ret_released = 0 and rbr_status = 3) as 'Total Holding',
'Retention' as 'Risk Type',
if (sum(dbt_amount) > (select sum(rbr_total_retention) from tblrbr         where rbr_comref=com_ref and rbr_ret_released = 0 and rbr_status = 3) , sum(dbt_amount) - (select sum(rbr_total_retention) from tblrbr             where rbr_comref=com_ref and rbr_ret_released = 0 and rbr_status = 3),0) as 'Credit Provided'
from tbldebits 


left join tblrbr on rbr_id  = dbt_rbr           
left join tblcompany on com_ref = rbr_comref

where dbt_pass_unpaid = 3 and dbt_lunpaid_remid = 0
and com_retterms= 1
group by rbr_comref

union

select 
rbr_comref,

sum(dbt_amount),
com_depositamount,
'Deposit' as 'Risk Type',

if (sum(dbt_amount) > com_depositamount, sum(dbt_amount) - com_depositamount,0) as 'Credit Provided'
from tbldebits 


left join tblrbr on rbr_id  = dbt_rbr           
left join tblcompany on com_ref = rbr_comref

where dbt_pass_unpaid = 3 and dbt_lunpaid_remid = 0
and com_retterms= 3


group by rbr_comref
