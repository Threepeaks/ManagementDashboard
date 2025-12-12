select 

dbt_ref as 'Client',
dbt_unpaid_datetime as 'Unpaid Date',
dbt_rbr as 'RBR',
dbt_amount as 'Amount',
dbt_accrejcode as 'Code',
hec_description as 'Reason'


from tbldebits 
	left join tblhyphen_errcodes on hec_code = dbt_accrejcode
where dbt_comref = 'THREE'
	
and dbt_pass_unpaid in (2,3)
and dbt_unpaid_datetime between '{{StartDate}}' and '{{EndDate}}'
order by dbt_ref