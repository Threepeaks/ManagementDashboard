select 
	com_ref as 'Reference',
    com_name as 'Client',
    case com_acc_cancel
     when 0 then 'Active'
     when 1 then 'In-cancellation'
     when 2 then 'Cancelled'
     end as 'State',
    com_depositamount as 'Deposit',
    (select sum(rbr_total_retention) from tblrbr where rbr_status = 3 and rbr_comref = com_Ref and rbr_ret_released = 0) as 'Retention',
    ifnull(collections.c,0) as 'Debits Count',
    ifnull(collections.sumValue,0) as 'Debits Value',
    ifnull(unpaids.c,0) as 'Unpaids Count',
    ifnull(unpaids.sumValue,0) as 'Unpaids Value',
        concat(round(ifnull(((unpaids.sumValue /collections.sumValue ) * 100),0),2),"%")  as 'UNP RATIO'
    
    
from tblcompany
left join (select dbt_comref as cref , count(*) as c,sum(dbt_amount) as sumValue  
			from tbldebits left join tblrbr on rbr_id = dbt_rbr where  rbr_status in (2,3) and  dbt_cdv = 3 and dbt_accrej = 3 and dbt_date between '{startDate}' and '{endDate}'
			group by dbt_comref)  collections
			on collections.cref = com_ref
left join  (select 
	dbt_comref as cref , count(*) as c,sum(dbt_amount) as sumValue from tbldebits 
    left join tblrbr on rbr_id = dbt_rbr 
    where  rbr_status in (2,3) and  dbt_cdv = 3 and dbt_accrej = 3 and 
dbt_pass_unpaid in (2,3) and
dbt_unpaid_datetime between '{startDate}' and '{endDate}'

group by dbt_comref) unpaids
on unpaids.cref = com_ref

order by round(ifnull(((unpaids.sumValue /collections.sumValue ) * 100),0),2) desc

