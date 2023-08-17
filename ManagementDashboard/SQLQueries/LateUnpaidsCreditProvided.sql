drop temporary table if exists tmpCreditProvided;
Create temporary table if not exists tmpCreditProvided

-- INSERT INTO tmpCreditProvided
select 
	rbr_comref as 'Customer',
	sum(dbt_amount) as 'DebitTotal',
	(select sum(rbr_total_retention) from tblrbr where rbr_comref=com_ref and rbr_ret_released = 0 and rbr_status = 3) as 'TotalHolding',
	'Retention' as 'RiskType',
	if (sum(dbt_amount) > (select sum(rbr_total_retention) from tblrbr where rbr_comref=com_ref and rbr_ret_released = 0 and rbr_status = 3) , 
	sum(dbt_amount) - (select sum(rbr_total_retention) from tblrbr where rbr_comref=com_ref and rbr_ret_released = 0 and rbr_status = 3),0) as 'CreditProvided',
    IFNULL((select sum(dta_amount) from tbldebtors_account where dta_cref = dbt_comref and dta_status = 0),0) as 'AjustmentAccount',
    IFNULL((select sum(rbr_h_amnt_ok) from tblrbr where rbr_comref =dbt_comref and rbr_status in (2)),0) as 'SubsNotSettled',
    IFNULL((select sum(rbr_cdv_amnt_rej) from tblrbr where rbr_comref =dbt_comref and rbr_status in (0,1)),0) as 'SubsNotProcessed'
from tbldebits 
	left join tblrbr on rbr_id  = dbt_rbr           
	left join tblcompany on com_ref = rbr_comref
where 
	dbt_pass_unpaid = 3 and 
    dbt_lunpaid_remid = 0 and 
    com_retterms= 1
group by rbr_comref
union
select 
rbr_comref,

sum(dbt_amount),
com_depositamount,
'Deposit' as 'Risk Type',

if (sum(dbt_amount) > com_depositamount, sum(dbt_amount) - com_depositamount,0) as 'Credit Provided',
ifnull((select sum(dta_amount) from tbldebtors_account where dta_cref = dbt_comref and dta_status = 0),0) as 'AjustmentAccount',
ifnull((select sum(rbr_h_amnt_ok) from tblrbr where rbr_comref =dbt_comref and rbr_status in (2)),0),
ifnull((select sum(rbr_cdv_amnt_rej) from tblrbr where rbr_comref =dbt_comref and rbr_status in (0,1)),0) as 'SubsNotProcessed'
from tbldebits 


left join tblrbr on rbr_id  = dbt_rbr           
left join tblcompany on com_ref = rbr_comref

where dbt_pass_unpaid = 3 and dbt_lunpaid_remid = 0
and com_retterms= 3


group by rbr_comref;
select 

Customer,
DebitTotal as 'Total Debits' ,
TotalHolding as 'Total Holding',
RiskType as 'Risk Type',

-- CreditProvided,
AjustmentAccount as 'Adjustment Account',
SubsNotSettled as 'Subs not Settled',
SubsNotProcessed as 'Upcoming Subs',
IF ((SubsNotSettled + TotalHolding- DebitTotal - AjustmentAccount ) < 0 , -1*(SubsNotSettled + TotalHolding- DebitTotal - AjustmentAccount  ),0)
AS 'Credit Provided'





from tmpCreditProvided 

where CreditProvided > 0
order by Customer
